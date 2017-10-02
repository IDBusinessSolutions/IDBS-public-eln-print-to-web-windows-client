// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DriverConfiguration.cs" company="ID Business Solutions Ltd.">
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
    /// Represents the configuration for the print driver
    /// </summary>
    public class DriverConfiguration
    {
        /// <summary>
        /// Gets or sets the name of the driver
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the configuration file name
        /// </summary>
        public string ConfigFileName { get; set; }

        /// <summary>
        /// Gets or sets the data file name
        /// </summary>
        public string DataFileName { get; set; }

        /// <summary>
        /// Gets or sets the help file name
        /// </summary>
        public string HelpFileName { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the driver file
        /// </summary>
        public string DriverFileName { get; set; }

        /// <summary>
        /// Gets or sets an array of dependent file names
        /// </summary>
        public string[] DependentFileNames { get; set; }

        /// <summary>
        /// Gets or sets the environment name (based on whether the system is 32 or 64 bit)
        /// </summary>
        public string Environment { get; set; }
    }
}
