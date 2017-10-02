// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VirtualPrinterException.cs" company="ID Business Solutions Ltd.">
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

namespace Idbs.Ewb.Printing.Api
{
    /// <summary>
    /// Represents a general printer related exception.
    /// </summary>
    [Serializable]
    public class VirtualPrinterException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the VirtualPrinterException class.
        /// </summary>
        public VirtualPrinterException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualPrinterException"/> class.
        /// </summary>
        /// <param name="message">The exception message</param>
        public VirtualPrinterException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualPrinterException"/> class.
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public VirtualPrinterException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
