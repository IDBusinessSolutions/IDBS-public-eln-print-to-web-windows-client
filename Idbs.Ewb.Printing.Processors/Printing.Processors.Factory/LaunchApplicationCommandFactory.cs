// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LaunchApplicationCommandFactory.cs" company="ID Business Solutions Ltd.">
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
using System.ComponentModel.Composition;

using Idbs.Ewb.Printing.Api;
using Idbs.Ewb.Printing.Processors.IO;

namespace Idbs.Ewb.Printing.Processors.Factory
{
    /// <summary>
    /// A factory for the <see cref="LaunchApplicationCommand"/> processor
    /// </summary>
    [Export(typeof(IDocumentProcessorFactory))]
    public class LaunchApplicationCommandFactory : DocumentProcessorFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LaunchApplicationCommandFactory"/> class.
        /// </summary>
        public LaunchApplicationCommandFactory()
            : base("LaunchApplicationCommand")
        {
        }

        /// <summary>
        /// Called to create a new document processor from the given initialization parameters
        /// </summary>
        /// <param name="parameters">The parameters used to configure the processor</param>
        /// <returns>The created processor </returns>
        public override IDocumentProcessor Create(IDictionary<string, string> parameters)
        {
            Precondition.NotNull(parameters, "parameters");

            string applicationPath = this.GetParameterOrThrow(parameters, "applicationPath");
            
            return new LaunchApplicationCommand(applicationPath);
        }
    }
}
