// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoOpProcessor.cs" company="ID Business Solutions Ltd.">
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

using Idbs.Ewb.Printing.Api;
using Idbs.Ewb.Printing.Configuration;

namespace Idbs.Ewb.Printing.Processors
{
    /// <summary>
    /// Represents a null processor, which does nothing and does not call the next processor
    /// </summary>
    public class NoOpProcessor : IDocumentProcessor
    {
        /// <summary> The single instance referenced statically </summary>
        private static readonly NoOpProcessor Singleton = new NoOpProcessor();
        
        /// <summary>
        /// Prevents a default instance of the <see cref="NoOpProcessor"/> class from being created.
        /// </summary>
        private NoOpProcessor()
        {
        }

        /// <summary>
        /// Gets a singleton instance of this class.
        /// </summary>
        public static IDocumentProcessor Instance
        {
            get { return Singleton; }
        }

        /// <summary>
        /// Processes the input file.  This instance does nothing of use other than ensuring 
        /// a non null instance can be supplied as the next argument to the final processor
        /// in a processor chain.
        /// </summary>
        /// <param name="inputFile">The input file to process</param>
        /// <param name="variables">The variable provider</param>
        /// <param name="next">The next processor to call</param>
        public void Process(string inputFile, IVariableProvider variables, Action<string> next)
        {
        }
    }
}
