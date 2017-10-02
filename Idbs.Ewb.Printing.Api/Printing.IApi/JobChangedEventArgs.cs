// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobChangedEventArgs.cs" company="ID Business Solutions Ltd.">
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
using System.Printing;

namespace Idbs.Ewb.Printing.IApi
{
    /// <summary>
    /// Represents the event arguments passed when a print job changes
    /// </summary>
    public class JobChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobChangedEventArgs"/> class.
        /// </summary>
        /// <param name="jobInfo">The job information</param>
        public JobChangedEventArgs(PrintSystemJobInfo jobInfo)
        {
            this.JobInfo = jobInfo;
        }

        /// <summary>
        /// Gets the job information
        /// </summary>
        public PrintSystemJobInfo JobInfo { get; private set; }
    }
}
