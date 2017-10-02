// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPrinterSupport.cs" company="ID Business Solutions Ltd.">
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

using System.Collections.Generic;
using System.ComponentModel;

namespace Idbs.Ewb.Printing.Api
{
    /// <summary>
    /// Provides the Win32 API support for managing system printers and ports.
    /// </summary>
    public interface IPrinterSupport
    {
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
        void AddPrintDriver(
            string configFile,
            string dataFile,
            string[] dependentFiles,
            string driverPath,
            string driverName,
            string environment,
            string helpFile = null,
            string monitorName = null,
            string defaultDataType = "RAW");

        /// <summary>
        /// Returns a value indicating whether the given printer exists
        /// </summary>
        /// <param name="printerName">The name of the printer to check</param>
        /// <returns>True if the printer already exists, false if not</returns>
        bool PrinterExists(string printerName);

        /// <summary>
        /// Returns a value indicating whether a given port is already present
        /// </summary>
        /// <param name="portName">The name of the port to look for</param>
        /// <returns>True if the port exists, otherwise false</returns>
        bool PortExists(string portName);

        /// <summary>
        /// Adds a new port with the given name
        /// </summary>
        /// <param name="portName">The name of the port to add</param>
        /// <exception cref="Win32Exception">Raised when an underlying call fails</exception>
        void AddPort(string portName);

        /// <summary>
        /// Deletes the given printer from the system
        /// </summary>
        /// <param name="printerName">The name of the printer to delete</param>
        /// <param name="portCreated">If true, the port is deleted as part of this operation</param>
        void DeletePrinter(string printerName, bool portCreated);

        /// <summary>
        /// Creates a new printer on the system
        /// </summary>
        /// <param name="printerName">The name of the printer</param>
        /// <param name="portName">The port to listen on</param>
        /// <param name="driverName">The name of the print driver to use</param>
        /// <param name="printProcessor">The processor to use</param>
        void CreatePrinter(string printerName, string portName, string driverName, string printProcessor);

        /// <summary>
        /// Updates the printer instance to use a new port
        /// </summary>
        /// <param name="currentPort">The name of the current port in use</param>
        /// <param name="newPort">The new port to listen on</param>
        /// <param name="portCreated">True if the port has already been created</param>
        /// <param name="printerName">The name of the printer</param>
        /// <returns>True if the port was created</returns>
        bool UpdatePort(string currentPort, string newPort, bool portCreated, string printerName);

        /// <summary>
        /// Deletes the given port from the system
        /// </summary>
        /// <param name="portName">The name of the port to delete</param>
        void DeletePort(string portName);

        /// <summary>
        /// Returns an array of the installed ports for the system
        /// </summary>
        /// <returns>An array of port names</returns>
        /// <exception cref="Win32Exception">Thrown when any Win API call fails</exception>
        IEnumerable<string> GetInstalledPorts();

        /// <summary>
        /// Gets a value indicating whether the given print driver exists
        /// </summary>
        /// <param name="driverName">The name of the driver to look up</param>
        /// <returns>True if it exists, false if it does not</returns>
        bool PrintDriverExists(string driverName);
    }
}
