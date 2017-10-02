// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPrintMonitor.cs" company="ID Business Solutions Ltd.">
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

namespace Idbs.Ewb.Printing.IApi
{
    /// <summary>
    /// This interface defines the behaviour of a component responsible for monitoring a print queue
    /// for new jobs, and raising an appropriate event when a new job is found.
    /// </summary>
    public interface IPrintMonitor : IDisposable
    {
        /// <summary>
        /// Raised when a print job changes
        /// </summary>
        event EventHandler<JobChangedEventArgs> NewPrintJobReceived;

        /// <summary>
        /// Called to start monitoring of the print queue
        /// </summary>
        void Start();

        /// <summary>
        /// Called to stop monitoring of the print queue.
        /// </summary>
        void Stop();
    }
}
