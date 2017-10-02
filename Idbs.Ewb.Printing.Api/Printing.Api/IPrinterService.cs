// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPrinterService.cs" company="ID Business Solutions Ltd.">
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
    /// Represents the printer service, responsible for the orchestration of the printer setup, port
    /// configuration, file monitoring and file conversion.
    /// </summary>
    public interface IPrinterService : IDisposable
    {
        /// <summary>
        /// Raised when a new PDF file is ready for consumption
        /// </summary>
        event EventHandler<FileEventArgs> PdfFileReady;

        /// <summary>
        /// Starts the printer service, setting up the virtual printer and starting the monitor
        /// of the printer
        /// </summary>
        void Start();

        /// <summary>
        /// Stops monitoring, shuts down the printer and removes it from the system
        /// </summary>
        void Stop();
    }
}
