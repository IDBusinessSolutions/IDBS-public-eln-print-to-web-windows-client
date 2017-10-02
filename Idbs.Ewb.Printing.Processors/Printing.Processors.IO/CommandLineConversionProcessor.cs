// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandLineConversionProcessor.cs" company="ID Business Solutions Ltd.">
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
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;

using Idbs.Ewb.Printing.Api;
using Idbs.Ewb.Printing.Configuration;

using log4net;

namespace Idbs.Ewb.Printing.Processors.IO
{
    /// <summary>
    /// Represents a document processor that can be used to call an external process
    /// to perform a document conversion.
    /// </summary>
    [Export(typeof(IDocumentProcessor))]
    internal class CommandLineConversionProcessor : IDocumentProcessor
    {
        /// <summary> The logger used by this class </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CommandLineConversionProcessor));

        /// <summary> The output folder to write to </summary>
        private readonly DirectoryInfo outputDirectoryInfo;

        /// <summary> The file extension of the converted file </summary>
        private readonly string outputFileExtension;

        /// <summary> The command line template </summary>
        private readonly string commandLineTemplate;

        /// <summary> The path to the executable to call </summary>
        private readonly string executablePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineConversionProcessor"/> class.
        /// </summary>
        /// <param name="executablePath">The path to the executable to call</param>
        /// <param name="outputFolderName">The destination folder used to generate the output file name</param>
        /// <param name="outputFileExtension">The extension to use when renaming the output file</param>
        /// <param name="commandLineTemplate">The command line template to use.  This should contain 2 variables {0} and {1}.
        /// Where {0} represents the input file, and {1} will be replaced with the output file</param>
        public CommandLineConversionProcessor(
            string executablePath, 
            string outputFolderName, 
            string outputFileExtension,
            string commandLineTemplate)
        {
            Precondition.NotNullOrEmpty(executablePath, "executablePath");
            Precondition.NotNullOrEmpty(outputFolderName, "outputFolderName");
            Precondition.NotNullOrEmpty(outputFileExtension, "outputFileExtension");
            Precondition.NotNullOrEmpty(commandLineTemplate, "commandLineTemplate");

            Precondition.Assert(() => File.Exists(executablePath), "Invalid executablePath");

            Logger.InfoFormat("Executable Path:       '{0}'", executablePath);
            Logger.InfoFormat("Output Folder Name:    '{0}'", outputFolderName);
            Logger.InfoFormat("Output File Extension: '{0}'", outputFileExtension);
            Logger.InfoFormat("Command Line       :   '{0}'", commandLineTemplate);

            this.outputDirectoryInfo = new DirectoryInfo(outputFolderName);
            this.commandLineTemplate = commandLineTemplate;
            this.executablePath = executablePath;
            this.outputFileExtension = outputFileExtension.StartsWith(".") 
                                            ? outputFileExtension 
                                            : "." + outputFileExtension;

            if (!this.outputDirectoryInfo.Exists)
                this.outputDirectoryInfo.Create();
        }

        /// <summary>
        /// Called to execute a command line process to perform a document conversion.
        /// </summary>
        /// <param name="inputFile">The input file to process</param>
        /// <param name="variables">The variable provider</param>
        /// <param name="next">The next processor to call</param>
        public void Process(string inputFile, IVariableProvider variables, Action<string> next)
        {
            string outputFile = GenerateOutputFileName(outputDirectoryInfo, outputFileExtension);
            string fullCommandLine = string.Format(commandLineTemplate, inputFile, outputFile);

            Logger.InfoFormat("Process called with input file of '{0}'", inputFile);
            Logger.InfoFormat("Output file is '{0}'", outputFile);
            Logger.InfoFormat("Full command line: '{0}'", fullCommandLine);

            var proc = new Process
            {
                StartInfo =
                {
                    FileName = this.executablePath,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = fullCommandLine
                }
            };

            proc.Start();

            // To avoid deadlocks, always read the output stream first and then wait.
            // Reference: https://msdn.microsoft.com/en-us/library/system.diagnostics.processstartinfo.redirectstandardoutput(v=vs.110).aspx
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            if (File.Exists(inputFile))
            {
                File.Delete(inputFile);
            }

            next(outputFile);
        }

        /// <summary>
        /// Given an input file name, output directory and output file extension, this method will construct
        /// a new output file name
        /// </summary>
        /// <param name="outputDirectoryInfo">The output directory</param>
        /// <param name="outputFileExtension">The new extension</param>
        /// <returns>A new unique file name</returns>
        private string GenerateOutputFileName(DirectoryInfo outputDirectoryInfo, string outputFileExtension)
        {
            string newFileName = Guid.NewGuid() + outputFileExtension;
            return Path.Combine(outputDirectoryInfo.FullName, newFileName);
        }
    }
}
