// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentProcessorFactory.cs" company="ID Business Solutions Ltd.">
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
using System.Collections.Generic;

using Idbs.Ewb.Printing.Api;

namespace Idbs.Ewb.Printing.Processors.Factory
{
    /// <summary>
    /// An abstract factory providing common support used by each concrete factory example.
    /// </summary>
    public abstract class DocumentProcessorFactory : IDocumentProcessorFactory
    {
        /// <summary> The name of the processor that the instance is responsible for constructing. </summary>
        private readonly string processorName;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentProcessorFactory"/> class.
        /// </summary>
        /// <param name="processorName">The processor name that the factory instance provides</param>
        protected DocumentProcessorFactory(string processorName)
        {
            Precondition.NotNullOrEmpty(processorName, "processorName");

            this.processorName = processorName;
        }

        /// <summary>
        /// Gets the processor name for this factory instance
        /// </summary>
        public string ProcessorName
        {
            get
            {
                return processorName;
            }
        }

        /// <summary>
        /// Called to create a new document processor from the given initialization parameters
        /// </summary>
        /// <param name="parameters">The parameters used to configure the processor</param>
        /// <returns>The created processor </returns>
        public abstract IDocumentProcessor Create(IDictionary<string, string> parameters);

        /// <summary>
        /// Returns the value of a parameter within the given parameters dictionary if it exists. 
        /// If it does not, an <see cref="ArgumentException"/> will be raised to indicate this.
        /// </summary>
        /// <param name="parameters">The dictionary or parameters to look up</param>
        /// <param name="parameterName">The name of the parameter to look for</param>
        /// <returns>The parameter value if present in the dictionary</returns>
        /// <exception cref="ArgumentException">Raised when the parameter dictionary does not contain 
        /// an entry for the given parameter</exception>
        protected string GetParameterOrThrow(IDictionary<string, string> parameters, string parameterName)
        {
            string parameterValue;
            if (!parameters.TryGetValue(parameterName, out parameterValue))
            {
                throw new ArgumentException(
                    "parameter dictionary does not contain the required parameter",
                    parameterName);
            }

            return parameterValue;
        }
    }
}
