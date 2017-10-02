// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IVirtualPrinter.cs" company="ID Business Solutions Ltd.">
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
    /// Represents a virtual printer
    /// </summary>
    public interface IVirtualPrinter : IDisposable
    {
        /// <summary>
        /// Gets the port that the printer is listening on.  This in reality is a file path to spool the
        /// output of the print job to
        /// </summary>
        string Port { get; }

        /// <summary>
        /// Called when the printer needs to be created
        /// </summary>
        void Create();

        /// <summary>
        /// Updates the printer to use a new port
        /// </summary>
        /// <param name="newPort">The new port to listen on</param>
        void UpdatePort(string newPort);
    }
}
