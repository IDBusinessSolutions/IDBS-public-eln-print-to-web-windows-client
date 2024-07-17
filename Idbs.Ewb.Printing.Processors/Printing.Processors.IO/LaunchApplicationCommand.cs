// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LaunchApplicationCommand.cs" company="ID Business Solutions Ltd.">
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

using Idbs.Ewb.Printing.Api;
using Idbs.Ewb.Printing.Configuration;

using log4net;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace Idbs.Ewb.Printing.Processors.IO
{
    /// <summary>
    /// This processor is used to shell out to an application with
    /// a single command line argument passed containing the path
    /// to the input file.
    /// </summary>
    [Export(typeof(IDocumentProcessor))]
    internal class LaunchApplicationCommand : IDocumentProcessor
    {
        /// <summary> The logger used by this class </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LaunchApplicationCommand));

        /// <summary> The path to the application to launch </summary>
        private readonly string applicationPath;

        /// <summary> The registry key used to store values to determine the next action of the application.</summary>
        private const string printToEwbWebKey = "Software\\IDBS\\EWorkbook\\PrintToEwbWeb";

        /// <summary> The specific value to look for in the registry key to determine the resulted PDF file should be save to a specific location.</summary>
        private const string saveToDisk = "SaveToDisk";

        /// <summary>
        /// Initializes a new instance of the <see cref="LaunchApplicationCommand"/> class.
        /// </summary>
        /// <param name="applicationPath">The path to the application to launch</param>
        public LaunchApplicationCommand(string applicationPath)
        {
            Precondition.NotNullOrEmpty(applicationPath, "applicationPath");

            Logger.InfoFormat("Initialized with an application path of '{0}'", applicationPath);

            this.applicationPath = applicationPath;
        }

        /// <summary>
        /// Called to process the input file.
        /// In this case, the input file is passed on the command line to the application
        /// that this instance is configured for.
        /// </summary>
        /// <param name="inputFile">The input file to process</param>
        /// <param name="variables">The variable provider</param>
        /// <param name="next">The next action to call</param>
        public void Process(string inputFile, IVariableProvider variables, Action<string> next)
        {
            string fileName = variables.Get("filename");

            Boolean startClientAsUsual = true;

            Logger.InfoFormat("Process called, original file name is '{0}'", fileName);

            // Check if a specific registry key is available. If it is, copy the resulted PDF to the
            // specific location instead of launch the application.
            RegistryKey key = Registry.CurrentUser.OpenSubKey(printToEwbWebKey);

            if (key != null)
            {
                string destFile = (string)key.GetValue(saveToDisk);

                if (destFile != null)
                {
                    Logger.InfoFormat("{0} key is found. Saving resulted PDF to specific location.", saveToDisk);
                    try
                    {
                        Logger.InfoFormat("Copying resulted PDF file from {0} to {1}", inputFile, destFile);
                        File.Copy(inputFile, destFile);

                        if (File.Exists(destFile))
                        {
                            // Do not start client.
                            startClientAsUsual = false;
                            Logger.InfoFormat("Copy success. Deleting registry key value.");
                            Registry.CurrentUser.OpenSubKey(printToEwbWebKey, true).DeleteValue("SaveToDisk");
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.ErrorFormat(e.ToString());
                        Logger.InfoFormat("An error has occurred when saving resulted PDF into specific location. Deleting registry key value.");
                        Registry.CurrentUser.OpenSubKey(printToEwbWebKey, true).DeleteValue("SaveToDisk");
                        Logger.InfoFormat("Launching Print to IDBS Cloud Web instead.");
                    }
                }
            }

            if (startClientAsUsual)
            {
                StartClient(inputFile, fileName, next);
            }
        }

        private void StartClient(string inputFile, string fileName, Action<string> next)
        {
            // To start the electron app front.
            var proc = new Process
            {
                StartInfo =
                    {                   
                        // run via executable
                        FileName = this.applicationPath,
                        UseShellExecute = true,
                        CreateNoWindow = false,
                        Arguments = "\"" + inputFile + "\" \"" + fileName + "\""
                    }
            };

            proc.Start();

            /* This processor does not provide a different output file to the input one (it is really
             * intended as an end of line processor, so just return the input file as the output */
            next(inputFile);
        }
    }
}
