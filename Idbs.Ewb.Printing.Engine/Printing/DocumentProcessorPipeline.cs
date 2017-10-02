// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentProcessorPipeline.cs" company="ID Business Solutions Ltd.">
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
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

using Idbs.Ewb.Printing.Api;
using Idbs.Ewb.Printing.Configuration;

namespace Idbs.Ewb.Printing.Engine
{
    /// <summary>
    /// Represents a pipeline of document processors
    /// </summary>
    [Export(typeof(IDocumentProcessorPipeline))]
    public class DocumentProcessorPipeline : IDocumentProcessorPipeline
    {
        /// <summary> The internal list of processors </summary>
        private readonly IList<IDocumentProcessor> processors = new List<IDocumentProcessor>();

        /// <summary> The readonly view of the processors </summary>
        private readonly ReadOnlyCollection<IDocumentProcessor> readonlyView;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentProcessorPipeline"/> class.
        /// </summary>
        public DocumentProcessorPipeline()
        {
            readonlyView = new ReadOnlyCollection<IDocumentProcessor>(processors);
        }

        /// <summary>
        /// Gets the number of processors in the pipeline
        /// </summary>
        public int Count
        {
            get { return processors.Count; }
        }

        /// <summary>
        /// Gets the processors as a new immutable sequence
        /// </summary>
        public IEnumerable<IDocumentProcessor> Processors
        {
            get { return readonlyView; }
        }

        /// <summary>
        /// Adds a new processor to the end of the list
        /// </summary>
        /// <param name="processor">The processor to add</param>
        public void AddProcessor(IDocumentProcessor processor)
        {
            processors.Add(processor);
        }

        /// <summary>
        /// Runs the pipeline of processors, starting at the initial file. The end result is returned
        /// as the response to this method
        /// </summary>
        /// <param name="initialFile">The starting file</param>
        /// <param name="variables">The variable provider</param>
        /// <returns>The final file output by the final processor</returns>
        public string Run(string initialFile, IVariableProvider variables)
        {
            var processorArray = processors.ToArray();
            
            string currentFile = initialFile;

            foreach (var processor in processorArray)
            {
                // assumes a synchronous operation
                processor.Process(currentFile, variables, nextFile => { currentFile = nextFile; });
            }

            return currentFile;
        }
    }
}
