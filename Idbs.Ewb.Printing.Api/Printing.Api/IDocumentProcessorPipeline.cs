// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDocumentProcessorPipeline.cs" company="ID Business Solutions Ltd.">
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

using Idbs.Ewb.Printing.Configuration;

namespace Idbs.Ewb.Printing.Api
{
    /// <summary>
    /// Represents a pipeline of document processors, which when run, will perform
    /// a workflow on a printed file
    /// </summary>
    public interface IDocumentProcessorPipeline
    {
        /// <summary>
        /// Gets the number of processors in the pipeline
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the processors as a new immutable sequence
        /// </summary>
        IEnumerable<IDocumentProcessor> Processors { get; }

        /// <summary>
        /// Adds a new processor to the end of the list
        /// </summary>
        /// <param name="processor">The processor to add</param>
        void AddProcessor(IDocumentProcessor processor);

        /// <summary>
        /// Runs the pipeline of processors, starting at the initial file.  
        /// The end result is returned as the response to this method
        /// </summary>
        /// <param name="initialFile">The starting file</param>
        /// <param name="variables">The variable provider</param>
        /// <returns>The final file output by the final processor</returns>
        string Run(string initialFile, IVariableProvider variables);
    }
}
