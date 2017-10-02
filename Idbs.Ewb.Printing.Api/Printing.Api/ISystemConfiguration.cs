// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISystemConfiguration.cs" company="ID Business Solutions Ltd.">
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

using Idbs.Ewb.Printing.Configuration;

namespace Idbs.Ewb.Printing.Api
{
    /// <summary>
    /// Represents the configuration of the system, including virtual printer
    /// set up, pipeline processing and general application defaults
    /// </summary>
    public interface ISystemConfiguration
    {
        /// <summary>
        /// Gets the configuration data used for the creation of the virtual printer
        /// </summary>
        VirtualPrinterConfiguration PrinterConfiguration { get; }

        /// <summary>
        /// Gets the variable provider
        /// </summary>
        IVariableProvider Variables { get; }

        /// <summary>
        /// Gets the processor pipeline, used for performing workflows on a printed file
        /// </summary>
        IDocumentProcessorPipeline DocumentProcessorPipeline { get; }
    }
}
