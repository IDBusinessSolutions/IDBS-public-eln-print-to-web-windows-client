// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrinterService.cs" company="ID Business Solutions Ltd.">
//
//    Copyright (C) 2014
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as
//    published by the Free Software Foundation, either version 3 of the
//    License, or (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/.
//
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.IO;
using System.Printing;

using Microsoft.Win32;

using Idbs.Ewb.Printing.Api;
using Idbs.Ewb.Printing.IApi;

namespace Idbs.Ewb.Printing.Engine
{
    using System.Linq;

    /// <summary>
    /// Represents the printer service, responsible for the orchestration of the printer setup, port
    /// configuration, file monitoring and file conversion.
    /// </summary>
    [Export(typeof(IPrinterService))]
    public class PrinterService : IPrinterService
    {
        /// <summary> The name of the variable to add to the global scope during pipeline execution </summary>
        private const string FileNameVariable = "filename";

        /// <summary> PDF file extension - used when renaming the print job </summary>
        private const string PdfFileExtension = "pdf";

        /// <summary> The pipe line of processors </summary>
        private readonly IDocumentProcessorPipeline processorPipeline;

        /// <summary> The system configuration </summary>
        private readonly ISystemConfiguration systemConfiguration;

        /// <summary> The print monitor, used to marry up print documents
        /// with print job names </summary>
        private readonly IPrintMonitor printMonitor;

        /// <summary> Represents a queue of print jobs </summary>
        private readonly ConcurrentStack<PrintSystemJobInfo> printJobStack;

        /// <summary> The virtual printer reference </summary>
        private IVirtualPrinter printer;

        /// <summary> The file watcher used for monitoring the print directory for new files </summary>
        private FileSystemWatcher fileWatcher;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="PrinterService"/> class.
        /// </summary>
        /// <param name="systemConfiguration">The system configuration data to use </param>
        /// <param name="printer">The instance printer to use </param>
        /// <param name="printMonitor">The print monitor</param>
        [ImportingConstructor]
        public PrinterService(ISystemConfiguration systemConfiguration,
            IVirtualPrinter printer, 
            IPrintMonitor printMonitor)
        {
            this.systemConfiguration = systemConfiguration;
            this.processorPipeline = systemConfiguration.DocumentProcessorPipeline;
            this.printer = printer;
            this.printMonitor = printMonitor;
            this.printJobStack = new ConcurrentStack<PrintSystemJobInfo>();      
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="PrinterService"/> class.
        /// </summary>
        ~PrinterService()
        {
            this.Stop();
        }

        /// <summary> Raised when the new PDF file is ready </summary>
        public event EventHandler<FileEventArgs> PdfFileReady;

        /// <summary>
        /// Called to start the printer service
        /// </summary>
        public void Start()
        {
            Stop();

            EnsureDirectories();
            printer.Create();

            StartWatching();
            StartMonitoring();

            /* Added handler to Windows shutdown / logout events */
            SystemEvents.SessionEnding += new SessionEndingEventHandler(SystemEvents_SessionEnding);
        }

        /// <summary>
        /// Handler of session ending event to ensure the any active printer services to be closed properly
        /// before Windows shutdone / user logout.
        /// </summary>
        private void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            Stop();
        }

        /// <summary>
        /// Stops the printer service
        /// </summary>
        public void Stop()
        {
            if (printer != null)
            {
                printer.Dispose();
            }

            if (fileWatcher != null)
            {
                fileWatcher.Dispose();
            }

            if (printMonitor != null)
            {
                printMonitor.Dispose();
            }
        }

        /// <summary>
        /// Disposes of unmanaged resources held by this instance
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Stop();
        }

        /// <summary>
        /// Cleans up the job name to remove any illegal file name characters from the string
        /// </summary>
        /// <param name="jobName"> The job name to clean up. </param>
        /// <returns>
        /// The <see cref="string"/> containing the cleaned string. </returns>
        private static string CleanJobName(string jobName)
        {
            const char ReplacementChar = '_';

            if (string.IsNullOrEmpty(jobName)) return string.Empty;

            try
            {
                string fileName = Path.GetFileName(jobName);
                jobName = fileName;
            }
            catch
            {
                // Do nothing, the job is not a valid file path, so it does not need extracting
            }

            char[] illegalChars = Path.GetInvalidFileNameChars();
            char[] cleanedName = jobName.ToCharArray();

            for (int charIndex = 0; charIndex < cleanedName.Length; charIndex++)
            {
                char currentChar = cleanedName[charIndex];
                if (illegalChars.Contains(currentChar)) cleanedName[charIndex] = ReplacementChar;
            }

            return new string(cleanedName);
        }

        /// <summary>
        /// Ensures each directory is created
        /// </summary>
        private void EnsureDirectories()
        {
            string port = printer.Port ?? systemConfiguration.PrinterConfiguration.DefaultPort;

            var printerPortFile = new FileInfo(port);
            if (printerPortFile.Directory != null && !printerPortFile.Directory.Exists)
                printerPortFile.Directory.Create();
        }

        /// <summary>
        /// Starts watching the printer directory for new files
        /// </summary>
        private void StartWatching()
        {
            var watchedFile = new FileInfo(printer.Port);
            string extension = Path.GetExtension(printer.Port);

            if (watchedFile.Directory != null)
                fileWatcher = new FileSystemWatcher(watchedFile.Directory.FullName);

            fileWatcher.Created += OnNewFileCreated;
            fileWatcher.EnableRaisingEvents = true;
            fileWatcher.Filter = "*" + extension;
        }

        /// <summary>
        /// Starts the print monitor
        /// </summary>
        private void StartMonitoring()
        {
            printMonitor.NewPrintJobReceived += OnNewJobFound;
            printMonitor.Start();
        }

        /// <summary>
        /// Handles the NewJobFound event
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event arguments</param>
        private void OnNewJobFound(object sender, JobChangedEventArgs e)
        {
            printJobStack.Push(e.JobInfo);
        }

        /// <summary>
        /// Called when the printer directory changes
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event arguments</param>
        private void OnNewFileCreated(object sender, FileSystemEventArgs e)
        {
            // Check the file extension of the file that the virtual printer is writing to and use
            // this to compare against the new file created in the directory. If they do not match, the
            // file was not generated from the printer
            string extension = Path.GetExtension(systemConfiguration.PrinterConfiguration.DefaultPort);
            if (Path.GetExtension(e.FullPath) != extension)
                return;

            if (!File.Exists(e.FullPath))
                return;

            string sourceFileName = string.Empty;

            PrintSystemJobInfo headJob;
            if (printJobStack.TryPop(out headJob))
            {
                string jobName = headJob.Name;
                string cleanedJobName = CleanJobName(jobName);
                sourceFileName = Path.GetFileName(cleanedJobName);
            }

            if (!string.IsNullOrEmpty(sourceFileName))
                systemConfiguration.Variables.AddGlobal(FileNameVariable, Path.ChangeExtension(sourceFileName, PdfFileExtension));
            
            string finalFile = processorPipeline.Run(e.FullPath, systemConfiguration.Variables);
            if (string.IsNullOrEmpty(finalFile))
                return;

            if (File.Exists(finalFile))
                OnPdfFileReady(new FileEventArgs(new FileInfo(finalFile), sourceFileName));
        }

        /// <summary>
        /// Raises the PdfFileReady event
        /// </summary>
        /// <param name="e">The event arguments</param>
        private void OnPdfFileReady(FileEventArgs e)
        {
            EventHandler<FileEventArgs> fileReadyEvent = this.PdfFileReady;
            if (fileReadyEvent != null)
                fileReadyEvent(this, e);
        }
    }
}
