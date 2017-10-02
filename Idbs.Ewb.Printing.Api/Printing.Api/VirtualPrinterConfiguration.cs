// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VirtualPrinterConfiguration.cs" company="ID Business Solutions Ltd.">
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

namespace Idbs.Ewb.Printing.Api
{
    /// <summary>
    /// Represents the configuration used when creating a new printer instance
    /// </summary>
    public sealed class VirtualPrinterConfiguration
    {
        /// <summary>
        /// Gets or sets the name of the driver to use
        /// </summary>
        public DriverConfiguration DriverConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the name of the driver to use
        /// </summary>
        public string PrintProcessor { get; set; }        

        /// <summary>
        /// Gets or sets the port to listen on
        /// </summary>
        public string DefaultPort { get; set; }

        /// <summary>
        /// Gets or sets the name of the printer
        /// </summary>
        public string PrinterName { get; set; }
    }
}
