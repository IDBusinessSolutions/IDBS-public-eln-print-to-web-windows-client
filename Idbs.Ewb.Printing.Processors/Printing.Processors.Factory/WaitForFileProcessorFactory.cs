// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitForFileProcessorFactory.cs" company="ID Business Solutions Ltd.">
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
using System.ComponentModel.Composition;

using Idbs.Ewb.Printing.Api;
using Idbs.Ewb.Printing.Processors.IO;

namespace Idbs.Ewb.Printing.Processors.Factory
{
    /// <summary>
    /// A factory for the <see cref="WaitForFileProcessor"/> processor
    /// </summary>
    [Export(typeof(IDocumentProcessorFactory))]
    public class WaitForFileProcessorFactory : DocumentProcessorFactory
    {
        /// <summary> The default timeout in seconds when no configuration is provided </summary>
        private const int DefaultTimeoutInSecondsInt = 20;

        /// <summary> The default retry delay in seconds when no configuration is provided </summary>
        private const int DefaultRetryDelayInMilliSecondsInt = 2000;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitForFileProcessorFactory"/> class.
        /// </summary>
        public WaitForFileProcessorFactory() : base("WaitForFileProcessor")
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

            string timeoutInSeconds;
            string retryDelayInMilliSeconds;

            parameters.TryGetValue("timeoutInSeconds", out timeoutInSeconds);
            parameters.TryGetValue("retryDelayInMilliSeconds", out retryDelayInMilliSeconds);

            int timeoutInSecondsInt = DefaultTimeoutInSecondsInt;
            if (!string.IsNullOrEmpty(timeoutInSeconds))
            {
                if (!int.TryParse(timeoutInSeconds, out timeoutInSecondsInt))
                {
                    timeoutInSecondsInt = DefaultTimeoutInSecondsInt;
                }
                else
                {
                    throw new ArgumentException(
                        string.Format("timeoutInSeconds parameter value '{0}' is not a valid number", timeoutInSeconds));
                }
            }

            int retryDelayInMilliSecondsInt = DefaultRetryDelayInMilliSecondsInt;
            if (!string.IsNullOrEmpty(retryDelayInMilliSeconds))
            {
                if (!int.TryParse(retryDelayInMilliSeconds, out retryDelayInMilliSecondsInt))
                {
                    retryDelayInMilliSecondsInt = DefaultRetryDelayInMilliSecondsInt;
                }
                else
                {
                    throw new ArgumentException(
                        string.Format("retryDelayInMilliSeconds parameter value '{0}' is not a valid number", timeoutInSeconds));
                }
            }

            // Verify values are reasonable
            Precondition.Assert(() => timeoutInSecondsInt > 0, "timeout must be > 0");
            Precondition.Assert(() => retryDelayInMilliSecondsInt > 0, "retryDelayInMilliSeconds must be > 0");

            return new WaitForFileProcessor(timeoutInSecondsInt, retryDelayInMilliSecondsInt);
        }
    }
}
