// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitForFileProcessor.cs" company="ID Business Solutions Ltd.">
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
using System.Threading;

using Idbs.Ewb.Printing.Api;
using Idbs.Ewb.Printing.Configuration;

namespace Idbs.Ewb.Printing.Processors.IO
{
    /// <summary>
    /// This processor waits for a file to become available
    /// </summary>
    [Export(typeof(IDocumentProcessor))]
    internal class WaitForFileProcessor : IDocumentProcessor
    {
        /// <summary> The amount of time to wait for a file to become available </summary>
        private readonly int timeout;

        /// <summary> The time to wait between attempts </summary>
        private readonly int retryDelay;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitForFileProcessor" /> class.
        /// </summary>
        /// <param name="timeoutInSeconds">The amount of time to wait for a file to become available before timing out</param>
        /// <param name="retryDelayInMilliSeconds">The amount of time in seconds to delay until
        /// the next retry.  Note that this delay will sleep the current thread</param>
        public WaitForFileProcessor(int timeoutInSeconds = 20, int retryDelayInMilliSeconds = 2000)
        {
            this.timeout = timeoutInSeconds;
            this.retryDelay = retryDelayInMilliSeconds;
        }

        /// <summary>
        /// When called, this method wait for the input file to be made available (no file lock).
        /// The wait is configurable based on an input timeout provided by the caller.
        /// </summary>
        /// <param name="inputFile">The input file to wait on</param>
        /// <param name="variables">The variable provider</param>
        /// <param name="next">The next processor to call</param>
        public void Process(string inputFile, IVariableProvider variables, Action<string> next)
        {
            DateTime startingTime = DateTime.UtcNow;

            while (true)
            {
                try
                {
                    using (File.Open(inputFile, FileMode.Open, FileAccess.Read))
                    {
                    }

                    break;
                }
                catch (IOException)
                {
                    Thread.Sleep(retryDelay);

                    // check for a timeout
                    DateTime currentTime = DateTime.UtcNow;
                    TimeSpan span = currentTime - startingTime;
                    if (span.TotalSeconds > timeout)
                        break;
                }
            }

            next(inputFile);
        }
    }
}
