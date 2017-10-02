// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrintMonitor.cs" company="ID Business Solutions Ltd.">
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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Printing;
using System.Threading;

using Idbs.Ewb.Printing.Api;
using Idbs.Ewb.Printing.IApi;

namespace Idbs.Ewb.Printing
{
    /// <summary>
    /// Print monitor implementation, using very basic polling
    /// </summary>
    [Export(typeof(IPrintMonitor))]
    internal class PrintMonitor : IPrintMonitor
    {
        /// <summary> Wait time in between checks on the print job </summary>
        private const int SleepDuration = 250;

        /// <summary> The worker thread name </summary>
        private const string WorkerThreadName = "PrintMonitor";

        /// <summary> The printer name </summary>
        private readonly string printerName;

        /// <summary> Cancellation flag, used to gracefully terminate the background thread </summary>
        private volatile bool cancelled;

        /// <summary> The background polling thread </summary>
        private Thread workerThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintMonitor"/> class.
        /// </summary>
        /// <param name="systemConfiguration"> The system configuration </param>
        [ImportingConstructor]
        public PrintMonitor(ISystemConfiguration systemConfiguration)
        {
            this.printerName = systemConfiguration.PrinterConfiguration.PrinterName;
        }

        /// <summary>
        /// Raised when the print job changes
        /// </summary>
        public event EventHandler<JobChangedEventArgs> NewPrintJobReceived;

        /// <summary>
        /// Called to start the monitor
        /// </summary>
        public void Start()
        {
            cancelled = false;
            workerThread = new Thread(Poll) { IsBackground = true, Name = WorkerThreadName };
            workerThread.Start();
        }

        /// <summary>
        /// Stops the monitor
        /// </summary>
        public void Stop()
        {
            cancelled = true;
            if (workerThread != null)
                workerThread.Join();

            workerThread = null;
        }

        /// <summary>
        /// Disposes this instance
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Stop();
        }

        /// <summary>
        /// The main method used to poll the print queue
        /// </summary>
        private void Poll()
        {
            PrintQueue spooler = new PrintQueue(new PrintServer(), printerName);

            string lastJobReported = null;

            while (!cancelled)
            {
                List<PrintSystemJobInfo> jobs = spooler.GetPrintJobInfoCollection().ToList();
                if (jobs.Count > 0)
                {
                    if (jobs[0].Name != lastJobReported)
                    {
                        OnJobsChanged(new JobChangedEventArgs(jobs[0]));
                        lastJobReported = jobs[0].Name;
                    }
                }

                Thread.Sleep(SleepDuration);
            }
        }

        /// <summary>
        /// Raises the <see cref="NewPrintJobReceived"/> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        private void OnJobsChanged(JobChangedEventArgs e)
        {
            EventHandler<JobChangedEventArgs> handlers = NewPrintJobReceived;
            if (handlers != null) handlers(this, e);
        }
    }
}
