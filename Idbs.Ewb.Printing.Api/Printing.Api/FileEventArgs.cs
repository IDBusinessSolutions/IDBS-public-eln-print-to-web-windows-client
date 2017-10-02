// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileEventArgs.cs" company="ID Business Solutions Ltd.">
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
//    along with this program.  If not, see http://www.gnu.org/licenses/
//
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;

namespace Idbs.Ewb.Printing.Api
{
    /// <summary>
    /// Represents the file based event arguments
    /// </summary>
    public class FileEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileEventArgs"/> class.
        /// </summary>
        /// <param name="file">The PDF file</param>
        /// /// <param name="sourceFileName">The original file name being printed</param>
        public FileEventArgs(FileInfo file, string sourceFileName)
        {
            this.File = file;
            this.SourceFileName = sourceFileName;
        }

        /// <summary>
        /// Gets the associated file
        /// </summary>
        public FileInfo File
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the source file name that is associated to this print job
        /// </summary>
        public string SourceFileName
        {
            get; 
            private set;
        }
    }
}
