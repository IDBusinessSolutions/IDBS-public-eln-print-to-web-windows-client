// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemConfiguration.cs" company="ID Business Solutions Ltd.">
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
using System.IO;

using Idbs.Ewb.Printing.Api;
using Idbs.Ewb.Printing.Configuration;
using Idbs.Ewb.Printing.IApi;

using log4net;

namespace Idbs.Ewb.Printing.Engine.Configuration
{
    /// <summary>
    /// The configuration class is used to provide configuration data for the system
    /// </summary>
    [Export(typeof(ISystemConfiguration))]
    public class SystemConfiguration : ISystemConfiguration
    {
        /// <summary> The logger used by this class </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SystemConfiguration));

        /// <summary> The processor pipeline</summary>
        private readonly IDocumentProcessorPipeline processorPipeline;

        /// <summary> The printer configuration </summary>
        private readonly VirtualPrinterConfiguration printerConfiguration;

        /// <summary> The printer configuration data </summary>
        private readonly PrinterConfigurationParser parser;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemConfiguration"/> class.
        /// </summary>
        /// <param name="settingsReader">The settings reader</param>
        /// <param name="factories">The sequence of factories to use when creating the
        /// pipeline</param>
        [ImportingConstructor]
        public SystemConfiguration(ISettingsReader settingsReader, [ImportMany] IEnumerable<IDocumentProcessorFactory> factories)
        {
            string fileName = settingsReader.GetSetting("printerConfigFile", string.Empty);

            Precondition.Assert(
                () => !string.IsNullOrEmpty(fileName) && File.Exists(fileName),
                "Invalid configuration provided - missing configuration file");

            using (Stream inputStream = File.OpenRead(fileName))
            {
                parser = new PrinterConfigurationParser(factories);
                processorPipeline = parser.Parse(inputStream);
            }

            string printerDir = parser.Variables.Get("printer-dir");
            string printerFileExt = parser.Variables.Get("printer-file-extension");
            string printerName = parser.Variables.Get("printer-name");
            string driverName = parser.Variables.Get("printer-driver");

            string port = Path.Combine(new DirectoryInfo(printerDir).FullName, Guid.NewGuid().ToString()) + printerFileExt;

            var driverConfiguration = new DriverConfiguration
            {
                ConfigFileName = parser.Variables.Get("driver-config-file"),
                DataFileName = parser.Variables.Get("driver-data-file"),
                DependentFileNames = parser.Variables.Get("driver-dependent-file").Split(","[0]),
                DriverFileName = parser.Variables.Get("driver-path"),
                Environment = parser.Variables.Get("driver-environment"),
                HelpFileName = parser.Variables.Get("driver-help-file"),
                Name = parser.Variables.Get("driver-name")
            };

            printerConfiguration = new VirtualPrinterConfiguration
            {
                DefaultPort = port,
                PrinterName = printerName,
                DriverConfiguration = driverConfiguration
            };

            Logger.Info("Using the following configuration:");
            Logger.Info(string.Empty);
            Logger.InfoFormat("  Print Name:      {0}", printerName);
            Logger.InfoFormat("  Print Driver:    {0}", driverName);
            Logger.InfoFormat("  Print Directory: {0}", printerDir);
            Logger.InfoFormat("  File Extension:  {0}", printerFileExt);
            Logger.Info(string.Empty);
        }

        /// <summary>
        /// Gets the virtual printer configuration
        /// </summary>
        public VirtualPrinterConfiguration PrinterConfiguration
        {
            get
            {
                return printerConfiguration;
            }
        }

        /// <summary>
        /// Gets the variable provider
        /// </summary>
        public IVariableProvider Variables
        {
            get
            {
                return parser.Variables;
            }
        }

        /// <summary>
        /// Gets the list of document processors
        /// </summary>
        public IDocumentProcessorPipeline DocumentProcessorPipeline
        {
            get
            {
                return processorPipeline;
            }
        }
    }
}
