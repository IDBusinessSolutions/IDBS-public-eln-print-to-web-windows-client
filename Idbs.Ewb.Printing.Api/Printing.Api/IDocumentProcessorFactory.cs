// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDocumentProcessorFactory.cs" company="ID Business Solutions Ltd.">
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

namespace Idbs.Ewb.Printing.Api
{
    /// <summary>
    /// This interface must be implemented by each instance responsible for constructing a valid
    /// <see cref="IDocumentProcessor"/>.  The main intent is to enable processors to be
    /// constructed from configuration files, where the processor implementation mandates it's
    /// configuration via constructor parameters. The factory must therefore provide a bridge 
    /// where the parameters are read from a dictionary of parameters and passed to the processor
    /// after validation has been applied to ensure correctness.
    /// </summary>
    public interface IDocumentProcessorFactory
    {
        /// <summary>
        /// Gets the name of the processor that this factory can create.  This
        /// name must match the name found within the configuration file
        /// </summary>
        string ProcessorName { get; }

        /// <summary>
        /// Called to create a new document processor from the given initialization parameters
        /// </summary>
        /// <param name="parameters">The parameters used to configure the processor</param>
        /// <returns>The created processor </returns>
        IDocumentProcessor Create(IDictionary<string, string> parameters);
    }
}
