// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VirtualPrinter.cs" company="ID Business Solutions Ltd.">
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
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Management;

using Idbs.Ewb.Printing.Api;

namespace Idbs.Ewb.Printing
{
    /// <summary>
    /// Implementation class for the virtual printer interface.
    /// </summary>
    [Export(typeof(IVirtualPrinter))]
    public class VirtualPrinter : IVirtualPrinter
    {
        /// <summary> The default port to reset to </summary>
        private const string DefaultPort = "FILE:";

        /// <summary>The processor used by the printer</summary>
        private const string WinProcessor = "winprint";

        /// <summary> The printer configuration to use </summary>
        private readonly VirtualPrinterConfiguration configuration;

        /// <summary> The printer support implementation </summary>
        private readonly IPrinterSupport printerSupport;

        /// <summary> The current port used by the printer instance </summary>
        private string currentPort;

        /// <summary> Keeps track of whether this instance was responsible for the creation of the port used by this printer. </summary>
        private bool portCreated;

        /// <summary> Keeps track of whether this instance was responsible for the creation of the printer. </summary>
        private bool printerCreated;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualPrinter"/> class.
        /// </summary>
        /// <param name="systemConfiguration">The configuration to use to construct the printer</param>
        /// <param name="printerSupport">The printer support implementation</param>
        [ImportingConstructor]
        public VirtualPrinter(ISystemConfiguration systemConfiguration, IPrinterSupport printerSupport)
        {
            Precondition.NotNull(systemConfiguration, "systemConfiguration");
            Precondition.NotNull(printerSupport, "printerSupport");

            this.configuration = systemConfiguration.PrinterConfiguration;
            this.printerSupport = printerSupport;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="VirtualPrinter"/> class.
        /// </summary>
        ~VirtualPrinter()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the port used by this instance
        /// </summary>
        public string Port
        {
            get { return this.currentPort; }
        }

        /// <summary>
        /// Called to create the virtual printer
        /// </summary>
        public void Create()
        {
            this.portCreated = false;

            DriverConfiguration driverConfig = this.configuration.DriverConfiguration;

            if (!this.printerSupport.PrintDriverExists(driverConfig.Name))
            {
                this.printerSupport.AddPrintDriver(
                    driverConfig.ConfigFileName,
                    driverConfig.DataFileName,
                    driverConfig.DependentFileNames,
                    driverConfig.DriverFileName,
                    driverConfig.Name,
                    driverConfig.Environment,
                    driverConfig.HelpFileName);
            }

            bool create = !this.printerSupport.PortExists(this.configuration.DefaultPort);

            if (create)
            {
                this.printerSupport.AddPort(this.configuration.DefaultPort);
                this.portCreated = true;
            }

            this.currentPort = configuration.DefaultPort;

            create = !this.printerSupport.PrinterExists(this.configuration.PrinterName);

            if (create)
            {
                this.printerSupport.CreatePrinter(this.configuration.PrinterName,
                                   this.configuration.DefaultPort,
                                   driverConfig.Name,
                                   this.configuration.PrintProcessor ?? WinProcessor);
            }
            else
            {
                this.portCreated = this.printerSupport.UpdatePort(
                    this.currentPort,
                    this.configuration.DefaultPort,
                    this.portCreated,
                    this.configuration.PrinterName);

                this.currentPort = this.configuration.DefaultPort;
            }

            // Flag the printer as being created so it gets removed when this instance shuts down
            this.printerCreated = true;
        }

        /// <summary>
        /// Called to update the port used by this printer instance
        /// </summary>
        /// <param name="newPort">The new port to use</param>
        public void UpdatePort(string newPort)
        {
            if (this.printerSupport.UpdatePort(
                this.currentPort,
                newPort, 
                this.portCreated, 
                this.configuration.PrinterName))
            {
                this.currentPort = newPort;
                this.portCreated = true;
            }
        }

        /// <summary>
        /// Disposes this instance
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of this instance
        /// </summary>
        /// <param name="disposing">True if called from the Dispose method, false if called from the instance destructor</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }

            bool printerDeleted = false;

            if (this.printerCreated && !string.IsNullOrEmpty(this.configuration.PrinterName))
            {
                try
                {
                    this.printerSupport.DeletePrinter(this.configuration.PrinterName, this.portCreated);
                    this.currentPort = null;

                    printerDeleted = true;
                    printerCreated = false;
                }
                catch (Win32Exception)
                {
                }
            }

            if (!this.portCreated || string.IsNullOrEmpty(this.currentPort))
            {
                return;
            }

            if (!printerDeleted && this.printerSupport.PrinterExists(this.configuration.PrinterName))
            {
                var searcher =
                    new ManagementObjectSearcher(
                        string.Format(
                            "SELECT * FROM Win32_Printer WHERE Name='{0}'",
                            this.configuration.PrinterName));

                bool deletePort = false;

                foreach (ManagementBaseObject printer in searcher.Get())
                {
                    if (string.Equals(this.currentPort,
                        printer["PortName"] as string,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        printer["PortName"] = DefaultPort;
                        deletePort = true;
                        var printerObject = printer as ManagementObject;
                        if (printerObject != null)
                            printerObject.Put();
                    }
                }

                try
                {
                    if (deletePort)
                        this.printerSupport.DeletePort(this.currentPort);

                    this.currentPort = null;
                    this.portCreated = false;
                }
                catch (Win32Exception)
                {
                }
                catch (VirtualPrinterException)
                {
                }
            }
        }
    }
}
