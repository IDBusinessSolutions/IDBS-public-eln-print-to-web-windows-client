// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplateEngine.cs" company="ID Business Solutions Ltd.">
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
using System.IO;
using System.Text;

namespace Idbs.Ewb.Printing.Configuration
{
    /// <summary>
    /// A static class for formatting strings that contain placeholder text of the form ${name} or
    /// environmental variables of the form %NAME%
    /// </summary>
    public static class TemplateEngine
    {
        /// <summary> The character offset to the text of a variable i.e. ${data} </summary>
        private const int OffSet = 3;

        /// <summary>
        /// Returns a list of all tokens in the source string
        /// </summary>
        /// <param name="sourceText">The source text to tokenize</param>
        /// <returns>The list of found variable regions</returns>
        public static IList<Region> GetTextRegions(string sourceText)
        {
            return GetTextRegions(new StringReader(sourceText));
        }

        /// <summary>
        /// Returns a list of all tokens in the source string
        /// </summary>
        /// <param name="reader">The Text Reader to use for lexical analysis</param>
        /// <returns>The list of found variable regions</returns>
        public static IList<Region> GetTextRegions(TextReader reader)
        {
            return new RegionLexer().Lex(reader);
        }

        /// <summary>
        /// Formats the source text based on the given provider
        /// </summary>
        /// <param name="sourceText">The text to format</param>
        /// <param name="provider">The provider to use when replacing variables</param>
        /// <returns>The formatted text</returns>
        public static string FormatText(string sourceText, IVariableProvider provider)
        {
            if (string.IsNullOrEmpty(sourceText))
                return string.Empty;

            IList<Region> regions = GetTextRegions(sourceText);

            // Format any variables as well
            return FormatText(sourceText, regions, provider);
        }

        /// <summary>
        /// Formats the source text based on the given provider
        /// </summary>
        /// <param name="sourceText">The text to format</param>
        /// <param name="regions">The regions containing text</param>
        /// <param name="provider">The provider to use when replacing variables</param>
        /// <returns>The formatted text</returns>
        public static string FormatText(string sourceText, IList<Region> regions, IVariableProvider provider)
        {
            if (string.IsNullOrEmpty(sourceText))
                return string.Empty;

            if (regions.Count == 0)
                return sourceText;

            if (provider == null)
                return sourceText;

            StringBuilder builder = new StringBuilder();
            
            int startIndex = 0;
            int endIndex = 0;

            // go through each region and replace this area in the text with the actual
            // value for the variable
            foreach (Region region in regions)
            {
                int length = region.AbsolutePosition - startIndex;

                // check for text before the first anchor
                if (region.AbsolutePosition > 0)
                    builder.Append(sourceText.Substring(startIndex, length));
                
                builder.Append(provider.Get(region.RegionText));
                
                startIndex += region.RegionText.Length + OffSet + length;
                endIndex = startIndex;
            }

            // check for text after the last anchor
            if (endIndex < sourceText.Length)
                builder.Append(sourceText.Substring(endIndex));
         
            return builder.ToString();
        }
    }
}
