// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrinterConfigurationParser.cs" company="ID Business Solutions Ltd.">
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
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Idbs.Ewb.Printing.Api;
using Idbs.Ewb.Printing.Engine;

namespace Idbs.Ewb.Printing.Configuration
{
    /// <summary>
    /// A configuration file parser, used for constructing document pipelines
    /// </summary>
    public class PrinterConfigurationParser
    {
        /// <summary> The processor factories used to create new processor instances </summary>
        private List<IDocumentProcessorFactory> processors;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrinterConfigurationParser"/> class.
        /// </summary>
        /// <param name="processors">The provided factories to use</param>
        public PrinterConfigurationParser(IEnumerable<IDocumentProcessorFactory> processors)
        {
            this.processors = new List<IDocumentProcessorFactory>(processors);
        }

        /// <summary>
        /// Gets the variable provider
        /// </summary>
        public IVariableProvider Variables { get; private set; }

        /// <summary>
        /// Based on a supplied input stream, this method parses the data and constructs
        /// a new pipeline.
        /// </summary>
        /// <param name="sourceStream">The source stream to read from</param>
        /// <returns>The new pipeline of processors</returns>
        public IDocumentProcessorPipeline Parse(Stream sourceStream)
        {
            XDocument document = XDocument.Load(sourceStream);

            XElement root = document.Root;
            
            Precondition.Assert(() => root != null, "No root element found in XML data");

            // ReSharper disable once PossibleNullReferenceException
            XElement processorsRootElement = root.Element("processors");
            var pipeline = new DocumentProcessorPipeline();

            if (processorsRootElement != null)
            {
                this.Variables = new VariableProvider(root.Element("variables"));
                var processorElements = processorsRootElement.Elements("processor");

                foreach (var processorElement in processorElements)
                {
                    IDocumentProcessor processor = LoadProcessorFromConfig(this.Variables, processorElement);
                    if (processor != null) 
                        pipeline.AddProcessor(processor);
                }
            }

            return pipeline;
        }

        /// <summary>
        /// Creates and initializes a new processor based on the supplied configuration
        /// </summary>
        /// <param name="provider">The variable provider, used for specializing
        /// any variables found within the initialization parameters</param>
        /// <param name="element">The processor element to use</param>
        /// <returns>The created processor</returns>
        private IDocumentProcessor LoadProcessorFromConfig(IVariableProvider provider, XElement element)
        {
            const StringComparison Comp = StringComparison.InvariantCultureIgnoreCase;

            XAttribute nameAttribute = element.Attribute("name");
            
            if (nameAttribute == null) return null;
            
            // determine the name of the processor to map to
            string processorName = nameAttribute.Value;
            
            // locate the factory by its name
            var factory = processors.FirstOrDefault(p => string.Compare(p.ProcessorName, processorName, Comp) == 0);

            // load and specialize any initialisation parameters for this factory
            IDictionary<string, string> parameters = LoadParameters(provider, element);
            return factory != null ? factory.Create(parameters) : null;
        }

        /// <summary>
        /// Loads the initialization parameters from the parent element
        /// </summary>
        /// <param name="provider">The variable provider to use when specializing
        /// initialization parameters</param>
        /// <param name="element">The parent element to read from</param>
        /// <returns>The dictionary of named parameters</returns>
        private IDictionary<string, string> LoadParameters(IVariableProvider provider, XElement element)
        {
            var parameters = new Dictionary<string, string>();
            XElement initElement = element.Element("init");

            if (initElement != null)
            {
                foreach (var child in initElement.Elements("param"))
                {
                    XAttribute nameAttr = child.Attribute("name");
                    XAttribute valueAttr = child.Attribute("value");

                    if (nameAttr == null) continue;
                    if (valueAttr == null) continue;

                    string name = nameAttr.Value;
                    string value = valueAttr.Value;
                    string formatted = TemplateEngine.FormatText(value, provider);

                    parameters.Add(name, formatted);
                }
            }

            return parameters;
        }
    }
}
