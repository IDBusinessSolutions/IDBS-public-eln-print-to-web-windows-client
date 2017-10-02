// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MoveFileProcessor.cs" company="ID Business Solutions Ltd.">
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
using System.ComponentModel.Composition;
using System.IO;

using Idbs.Ewb.Printing.Api;
using Idbs.Ewb.Printing.Configuration;

using log4net;

namespace Idbs.Ewb.Printing.Processors.IO
{
    /// <summary>
    /// Used to move a file from one location to another
    /// </summary>
    [Export(typeof(IDocumentProcessor))]
    internal class MoveFileProcessor : IDocumentProcessor
    {
        /// <summary> The logger used by this class </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MoveFileProcessor));

        /// <summary> The destination directory to move to </summary>
        private readonly DirectoryInfo destinationDirectoryInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveFileProcessor"/> class.
        /// </summary>
        /// <param name="destinationDirectory">The destination directory to move the file to</param>
        public MoveFileProcessor(string destinationDirectory)
        {
            Precondition.NotNullOrEmpty(destinationDirectory, "destinationDirectory");

            Logger.InfoFormat("Initialized with a destination directory of '{0}'", destinationDirectory);

            this.destinationDirectoryInfo = new DirectoryInfo(destinationDirectory);
        }

        /// <summary>
        /// When this method is called, the input file will be moved to the destination file provided
        /// in the constructor for this instance.
        /// When the next processor is called, it will be called with the location of the destination file,
        /// not the inputFile to this method.
        /// </summary>
        /// <param name="inputFile">The input file to process</param>
        /// <param name="variables">The variable provider</param>
        /// <param name="nextCaller">The next processor to call</param>
        public void Process(string inputFile, IVariableProvider variables, Action<string> nextCaller)
        {
            FileInfo inputFileInfo = new FileInfo(inputFile);
            string newFileName = Guid.NewGuid() + Path.GetExtension(inputFile);

            FileInfo destinationFileInfo = new FileInfo(Path.Combine(destinationDirectoryInfo.FullName, newFileName));
            
            if (!destinationDirectoryInfo.Exists)
                destinationDirectoryInfo.Create();
            
            Logger.InfoFormat("Moving input file '{0}", inputFile);

            inputFileInfo.MoveTo(destinationFileInfo.FullName);

            // pass the new file name to the next caller
            nextCaller(destinationFileInfo.FullName);
        }
    }
}
