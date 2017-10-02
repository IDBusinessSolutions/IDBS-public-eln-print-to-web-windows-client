// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrinterSupport.cs" company="ID Business Solutions Ltd.">
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
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using Idbs.Ewb.Printing.Api;
using Idbs.Ewb.Printing.WinApi;

using log4net;

namespace Idbs.Ewb.Printing
{
    /// <summary>
    /// Implements the <see cref="IPrinterSupport"/> interface.
    /// </summary>
    [Export(typeof(IPrinterSupport))]
    internal class PrinterSupport : IPrinterSupport
    {
        /// <summary>The number of times to retry when attempting to delete an existing printer </summary>
        private const int MaxPrinterDeleteAttempts = 10;

        /// <summary> The number of milliseconds to wait before trying to delete the printer again </summary>
        private const int PrinterDeleteRetryDelayMs = 100;

        /// <summary> The number of times to retry when adding a new print driver </summary>
        private const int AddPrintDriverRetryLimit = 10;

        /// <summary> The number of milliseconds to wait before trying to add a print driver again </summary>
        private const int AddPrintDriverRetryInterval = 1000;

        /// <summary> Default port monitor </summary>
        private const string PortMonitor = "Local Port";

        /// <summary> The default port to reset to </summary>
        private const string DefaultPort = "FILE:";

        /// <summary> The logger instance </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(PrinterSupport));

        /// <summary>
        /// Returns a value indicating whether a given port is already present
        /// </summary>
        /// <param name="portName">The name of the port to look for</param>
        /// <returns>True if the port exists, otherwise false</returns>
        public bool PortExists(string portName)
        {
            Precondition.NotNullOrEmpty(portName, "portName");

            return
                GetInstalledPorts()
                    .Any(port => string.Compare(port, portName, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        /// <summary>
        /// Adds a new port with the given name
        /// </summary>
        /// <param name="portName">The name of the port to add</param>
        /// <exception cref="Win32Exception">Raised when an underlying call fails</exception>
        public void AddPort(string portName)
        {
            Precondition.NotNullOrEmpty(portName, "portName");

            SafeNativeMethods.PORT_INFO_1 pInfo = new SafeNativeMethods.PORT_INFO_1 { szPortName = portName };

            int result = SafeNativeMethods.AddPortEx(null, 1, ref pInfo, PortMonitor);

            if (result != 0)
            {
                Logger.InfoFormat("Port created with name of {0}", portName);
                return;
            }

            result = Marshal.GetLastWin32Error();
            if (result != 87) throw new Win32Exception(result);

            // Double check that the port does, in fact, exist
            if (this.PortExists(portName)) throw new VirtualPrinterException(string.Format("Port {0} already exists", portName));

            throw new Win32Exception(result);
        }

        /// <summary>
        /// Adds a new print driver in the system
        /// </summary>
        /// <param name="configFile"> The config file. </param>
        /// <param name="dataFile"> The data file. </param>
        /// <param name="dependentFiles"> The dependent files. </param>
        /// <param name="driverPath"> The driver path. </param>
        /// <param name="driverName"> The driver name. </param>
        /// <param name="environment"> The environment. </param>
        /// <param name="helpFile"> The help file. </param>
        /// <param name="monitorName"> The monitor name. </param>
        /// <param name="defaultDataType"> The default data type. </param>
        /// <exception cref="Win32Exception">Raised when the operation errored for whatever reason</exception>
        public void AddPrintDriver(
            string configFile,
            string dataFile,
            string[] dependentFiles,
            string driverPath,
            string driverName,
            string environment,
            string helpFile = null,
            string monitorName = null,
            string defaultDataType = "RAW")
        {
            // DRIVER_INFO_6 structure type
            const int DriverInfoVersion6 = 6;

            // Create the full path to the print directory where these files *should* be installed to (we will find out
            // if that is the case when attempting to add the driver as the call will fail
            string printDirectory = GetPrinterDriverDirectory(environment);
            Logger.Info("Print directory is " + printDirectory);

            Func<string, string> installedPath = fileName => Path.Combine(printDirectory, fileName);

            var di6 = new SafeNativeMethods.DRIVER_INFO_6();
            di6.cVersion = 3;
            di6.pName = driverName;
            di6.pConfigFile = installedPath(configFile);
            di6.pDataFile = installedPath(dataFile);
            di6.pDependentFiles = dependentFiles.Length == 1 ? installedPath(dependentFiles[0]) : null;
            di6.pDriverPath = installedPath(driverPath);
            di6.pEnvironment = environment;
            di6.pHelpFile = installedPath(helpFile);
            di6.pMonitorName = monitorName;
            di6.pDefaultDataType = defaultDataType;

            int tryCount = 1;
            
            while (true)
            {
                try
                {
                    IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SafeNativeMethods.DRIVER_INFO_6)));

                    try
                    {
                        Marshal.StructureToPtr(di6, intPtr, false);

                        if (!SafeNativeMethods.AddPrinterDriverExW(
                                null,
                                DriverInfoVersion6,
                                intPtr,
                                SafeNativeMethods.APD_COPY_NEW_FILES))
                        {
                            int w32Error = Marshal.GetLastWin32Error();
                            throw new Win32Exception(w32Error);
                        }

                        Logger.Info("Print driver installed successfully with name of " + driverName + " after " + tryCount  + " attempts");
                        break;
                    }
                    finally
                    {
                        if (IntPtr.Zero != intPtr) Marshal.FreeHGlobal(intPtr);
                    }
                }
                catch (Win32Exception win32Ex)
                {
                    // YUCK - on some machines, driver registration will fail the first time it is called (when run as an administrator).
                    // This retry loop is added to try and force the issue to see if it is a timing problem (simulates several attempts by the
                    // user to register the driver).
                    Logger.Warn("Error caught trying to create print driver : " + win32Ex);
                    Thread.Sleep(AddPrintDriverRetryInterval);

                    if (tryCount > AddPrintDriverRetryLimit)
                    {
                        Logger.Fatal("Retry count exceeded, unable to create print driver");
                        throw;
                    }

                    tryCount++;
                }
            }
        }

        /// <summary>
        /// Returns a value indicating whether the given printer exists
        /// </summary>
        /// <param name="printerName">The name of the printer to check</param>
        /// <returns>True if the printer already exists, false if not</returns>
        public bool PrinterExists(string printerName)
        {
            Precondition.NotNullOrEmpty(printerName, "printerName");

            var defaults = new SafeNativeMethods.PRINTER_DEFAULTS
                                {
                                    DesiredAccess = SafeNativeMethods.PRINTER_ALL_ACCESS,
                                    pDatatype = IntPtr.Zero,
                                    pDevMode = IntPtr.Zero
                                };

            IntPtr hPrinter;
            if (SafeNativeMethods.OpenPrinter(printerName, out hPrinter, ref defaults))
            {
                SafeNativeMethods.ClosePrinter(hPrinter);
                return hPrinter != IntPtr.Zero;
            }

            return false;
        }

        /// <summary>
        /// Creates a new printer on the system
        /// </summary>
        /// <param name="printerName">The name of the printer</param>
        /// <param name="portName">The port to listen on</param>
        /// <param name="driverName">The name of the print driver to use</param>
        /// <param name="printProcessor">The processor to use</param>
        public void CreatePrinter(string printerName, string portName, string driverName, string printProcessor)
        {
            Precondition.NotNullOrEmpty(printerName, "printerName");
            Precondition.NotNullOrEmpty(portName, "portName");
            Precondition.NotNullOrEmpty(driverName, "driverName");
            Precondition.NotNullOrEmpty(printProcessor, "printProcessor");

            SafeNativeMethods.PRINTER_INFO_2 _pInfo = new SafeNativeMethods.PRINTER_INFO_2
                                                          {
                                                              pPrinterName = printerName,
                                                              pPortName = portName,
                                                              pDriverName = driverName,
                                                              pPrintProcessor =
                                                                  printProcessor
                                                          };

            IntPtr hPrinter = SafeNativeMethods.AddPrinter(null, 2, ref _pInfo);

            if (hPrinter != IntPtr.Zero)
            {
                SafeNativeMethods.ClosePrinter(hPrinter);

                Logger.InfoFormat("Printer created with name {0} using port: {1} and driver {2}", printerName, portName, driverName);
            }
            else
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        /// <summary>
        /// Deletes the given printer from the system
        /// </summary>
        /// <param name="printerName">The name of the printer to delete</param>
        /// <param name="portCreated">If true, the port is deleted as part of this operation</param>
        public void DeletePrinter(string printerName, bool portCreated)
        {
            Precondition.NotNullOrEmpty(printerName, "printerName");

            var pDefaults = new SafeNativeMethods.PRINTER_DEFAULTS
                                {
                                    DesiredAccess = SafeNativeMethods.PRINTER_ALL_ACCESS,
                                    pDatatype = IntPtr.Zero,
                                    pDevMode = IntPtr.Zero
                                };

            Exception failure = null;

            /*
             * Set the port to the default port before deleting the printer. Otherwise the delete may fail because the port is in use
             */
            try
            {
                this.UpdatePort(string.Empty, DefaultPort, portCreated, printerName);
            }
            catch (Win32Exception)
            {
            }
            catch (VirtualPrinterException)
            {
            }

            for (int retries = 0; ; retries++)
            {
                IntPtr hPrinter;
                if (SafeNativeMethods.OpenPrinter(printerName, out hPrinter, ref pDefaults))
                {
                    try
                    {
                        /*
                         * Retry the deletion of the printer for a second or so, just in case
                         * the spooler has not had time to clean up its last print job
                         */
                        if (!SafeNativeMethods.DeletePrinter(hPrinter))
                        {
                            if (retries < MaxPrinterDeleteAttempts)
                            {
                                Thread.Sleep(PrinterDeleteRetryDelayMs);
                                continue;
                            }

                            failure = new Win32Exception(Marshal.GetLastWin32Error());
                        }

                        break;
                    }
                    finally
                    {
                        SafeNativeMethods.ClosePrinter(hPrinter);
                    }
                }

                failure = new Win32Exception(Marshal.GetLastWin32Error());
                break;
            }

            if (failure != null && this.PrinterExists(printerName))
            {
                throw failure;
            }
        }

        /// <summary>
        /// Updates the printer instance to use a new port
        /// </summary>
        /// <param name="currentPort">The name of the current port in use</param>
        /// <param name="newPort">The new port to listen on</param>
        /// <param name="portCreated">True if the port has already been created</param>
        /// <param name="printerName">The name of the printer</param>
        /// <returns>True if the port was created</returns>
        public bool UpdatePort(string currentPort, string newPort, bool portCreated, string printerName)
        {
            Precondition.NotNull(currentPort, "currentPort");
            Precondition.NotNullOrEmpty(newPort, "newPort");
            Precondition.NotNullOrEmpty(printerName, "printerName");

            bool created = false;

            if (!this.PortExists(newPort))
            {
                this.AddPort(newPort);
                created = true;
            }

            ManagementObjectSearcher searcher =
                new ManagementObjectSearcher(string.Format("SELECT * FROM Win32_Printer WHERE Name='{0}'", printerName));

            string priorPort = string.Empty;

            foreach (ManagementBaseObject printer in searcher.Get())
            {
                var portNameObj = printer["PortName"];
                priorPort = portNameObj == null ? string.Empty : portNameObj.ToString();
                printer["PortName"] = newPort;

                ManagementObject printerObject = printer as ManagementObject;
                if (printerObject != null) printerObject.Put(); // Call put to save the settings.
            }

            /* If this application instance created the port, and the new one is different,
             * delete the old one */
            if (priorPort != newPort && !string.IsNullOrEmpty(priorPort))
            {
                this.DeletePort(priorPort);
            }

            return created;
        }

        /// <summary>
        /// Deletes the given port from the system
        /// </summary>
        /// <param name="portName">The name of the port to delete</param>
        public void DeletePort(string portName)
        {
            if (this.PortExists(portName))
            {
                int result = SafeNativeMethods.DeletePort(null, IntPtr.Zero, portName);
                if (result == 0)
                {
                    result = Marshal.GetLastWin32Error();
                    throw new Win32Exception(result);
                }
            }
            else
            {
                throw new VirtualPrinterException(string.Format("Local Port '{0}' not found", portName));
            }
        }

        /// <summary>
        /// Returns an array of the installed ports for the system
        /// </summary>
        /// <returns>An array of port names</returns>
        /// <exception cref="Win32Exception">Thrown when any Win API call fails</exception>
        public IEnumerable<string> GetInstalledPorts()
        {
            uint pcbNeeded = 0;
            uint pcReturned = 0;

            if (SafeNativeMethods.EnumPorts(null, 1, IntPtr.Zero, 0, ref pcbNeeded, ref pcReturned))
            {
                // succeeds, but must not, because buffer is zero (too small)!
                return new string[0];
            }

            int lastWin32Error = Marshal.GetLastWin32Error();

            // ERROR_INSUFFICIENT_BUFFER expected, if not -> Exception
            if (lastWin32Error != SafeNativeMethods.ERROR_INSUFFICIENT_BUFFER)
            {
                throw new Win32Exception(lastWin32Error);
            }

            IntPtr pPorts = Marshal.AllocHGlobal((int)pcbNeeded);

            if (SafeNativeMethods.EnumPorts(null, 1, pPorts, pcbNeeded, ref pcbNeeded, ref pcReturned))
            {
                IntPtr currentPort = pPorts;
                string[] ports = new string[pcReturned];
                for (int i = 0; i < pcReturned; i++)
                {
                    var pinfo = (SafeNativeMethods.PORT_INFO_1)Marshal.PtrToStructure(currentPort, typeof(SafeNativeMethods.PORT_INFO_1));
                    ports[i] = pinfo.szPortName;
                    currentPort = (IntPtr)(currentPort.ToInt32() + Marshal.SizeOf(typeof(SafeNativeMethods.PORT_INFO_1)));
                }

                Marshal.FreeHGlobal(pPorts);

                return ports;
            }

            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// Gets a value indicating whether the given print driver exists
        /// </summary>
        /// <param name="driverName">The name of the driver to look up</param>
        /// <returns>True if it exists, false if it does not</returns>
        public bool PrintDriverExists(string driverName)
        {
            return GetInstalledPrinterDrivers().Any(info2 => info2.pName == driverName);
        }

        /// <summary>
        /// Determines the location of the print driver directory, depending on the given environment
        /// </summary>
        /// <param name="environment">The environment</param>
        /// <returns>The location of the print driver directory</returns>
        private string GetPrinterDriverDirectory(string environment)
        {
            uint pcbNeeded;
            const int MaxPath = 2048;
            StringBuilder sb = new StringBuilder(MaxPath);
            if (SafeNativeMethods.GetPrinterDriverDirectory(null, environment, 1, sb, MaxPath, out pcbNeeded))
            {
                return sb.ToString();
            }

            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// Returns a an array of installed drivers
        /// </summary>
        /// <returns>
        /// The <see cref="SafeNativeMethods.DRIVER_INFO_2"/>. array
        /// </returns>
        /// <exception cref="Win32Exception">Raised whenever an API call fails </exception>
        private IEnumerable<SafeNativeMethods.DRIVER_INFO_2> GetInstalledPrinterDrivers()
        {
            /*
             * To determine the required buffer size, call EnumPrinterDrivers with cbBuffer set
             * to zero. The call will fails specifying ERROR_INSUFFICIENT_BUFFER and filling in
             * cbRequired with the required size, in bytes, of the buffer required to hold the array
             * of structures and data.
            */

            uint cbNeeded = 0;
            uint cReturned = 0;
            if (SafeNativeMethods.EnumPrinterDrivers(null, null, 2, IntPtr.Zero, 0, ref cbNeeded, ref cReturned))
            {
                // succeeds, but shouldn't, because buffer is zero (too small)!
                throw new Win32Exception(Marshal.GetLastWin32Error(), "EnumPrinters should fail!");
            }

            int lastWin32Error = Marshal.GetLastWin32Error();

            // ERROR_INSUFFICIENT_BUFFER = 122 expected, if not -> Exception
            if (lastWin32Error != SafeNativeMethods.ERROR_INSUFFICIENT_BUFFER)
            {
                throw new Win32Exception(lastWin32Error);
            }

            IntPtr addr = Marshal.AllocHGlobal((int)cbNeeded);
            if (SafeNativeMethods.EnumPrinterDrivers(null, null, 2, addr, cbNeeded, ref cbNeeded, ref cReturned))
            {
                var printerInfo2 = new SafeNativeMethods.DRIVER_INFO_2[cReturned];
                int offset = addr.ToInt32();
                Type type = typeof(SafeNativeMethods.DRIVER_INFO_2);
                int increment = Marshal.SizeOf(type);

                for (int i = 0; i < cReturned; i++)
                {
                    printerInfo2[i] = (SafeNativeMethods.DRIVER_INFO_2)Marshal.PtrToStructure(new IntPtr(offset), type);
                    offset += increment;
                }

                Marshal.FreeHGlobal(addr);
                return printerInfo2;
            }

            throw new Win32Exception();
        }
    }
}
