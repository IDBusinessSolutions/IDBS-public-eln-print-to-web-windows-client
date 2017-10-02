// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VariableProvider.cs" company="ID Business Solutions Ltd.">
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
using System.Linq;
using System.Xml.Linq;

namespace Idbs.Ewb.Printing.Configuration
{
    /// <summary>
    /// A variable provider whose backing store is defined in XML
    /// </summary>
    public class VariableProvider : IVariableProvider
    {
        /// <summary> The variable map </summary>
        private IDictionary<string, string> variables;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableProvider"/> class.
        /// </summary>
        /// <param name="variableElement">The element to read from</param>
        public VariableProvider(XElement variableElement)
        {
            variables = new Dictionary<string, string>();

            if (variableElement != null)
            {
                foreach (var kv in this.Variables(variableElement)) variables[kv.Item1] = kv.Item2;
            }
        }

        /// <summary>
        /// Adds a global variable to the provider
        /// </summary>
        /// <param name="name">The name of the variable</param>
        /// <param name="value">The variable value</param>
        public void AddGlobal(string name, string value)
        {
            Precondition.NotNullOrEmpty(name, "name");
            Precondition.NotNullOrEmpty(value, "value");

            variables[name] = value;
        }

        /// <summary>
        /// Returns a formatted string representing the actual value represented by the
        /// given anchorText
        /// </summary>
        /// <param name="anchorText">The text of the anchored item to retrieve the real 
        /// value for</param>
        /// <returns>The specialized value formatted based on the variables definition</returns>
        public string Get(string anchorText)
        {
            string value = variables.ContainsKey(anchorText) ? variables[anchorText] : string.Empty;

            // Expand and format variable
            if (!string.IsNullOrEmpty(value))
            {
                value = Environment.ExpandEnvironmentVariables(value);
                value = TemplateEngine.FormatText(value, this);
            }

            return value;
        }

        /// <summary>
        /// Reads and returns the variables as a sequence of tuple pairs.
        /// </summary>
        /// <param name="el">The parent element to read from</param>
        /// <returns>The tuple sequence</returns>
        private IEnumerable<Tuple<string, string>> Variables(XElement el)
        {
            return from x in el.Elements("variable")
                   select Tuple.Create(
                       x.Attribute("name").Value,
                       x.Attribute("value").Value);
        }
    }
}
