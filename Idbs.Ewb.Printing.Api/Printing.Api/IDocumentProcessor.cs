// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDocumentProcessor.cs" company="ID Business Solutions Ltd.">
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

using Idbs.Ewb.Printing.Configuration;

namespace Idbs.Ewb.Printing.Api
{
    /// <summary>
    /// A processor interface used to process an input file and pass along the results
    /// to the next operation
    /// </summary>
    public interface IDocumentProcessor
    {
        /// <summary>
        /// Called to process the given file.
        /// </summary>
        /// <param name="inputFile">The input file to process</param>
        /// <param name="variables">The variable provider</param>
        /// <param name="next">The next processor to call in the chain.
        /// The method should be called with the pass to the new file to process,
        /// which may or may not be the same as the input.</param>
        void Process(string inputFile, IVariableProvider variables, Action<string> next);
    }
}
