// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegionLexer.cs" company="ID Business Solutions Ltd.">
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
    /// A lexical analyzer to extract text regions from a source string
    /// </summary>
    public class RegionLexer
    {
        #region class data

        /// <summary> The starting anchor for a text block </summary>
        private const char StartAnchor = '$';

        /// <summary> The expected open region of a text block </summary>
        private const char OpenRegion = '{';

        /// <summary> The expected closing block for a text region </summary>
        private const char CloseRegion = '}';

        /// <summary> The number of additional characters in an anchored region i.e. ${} </summary>
        private const int AnchorOffset = 3;

        /// <summary> The end of file indicator </summary>
        private const int EndOfFile = -1;

        /// <summary> The current absolute position within the source string </summary>
        private int currentPosition;

        /// <summary> The current line in the source string </summary>
        private int currentRow;

        /// <summary> The current column in the source string </summary>
        private int currentColumn;

        /// <summary> The current character from the source string </summary>
        private int current;

        #endregion

        #region public methods

        /// <summary>
        /// The main lexer method to parse the region elements within a string
        /// </summary>
        /// <param name="data">The source data to lex</param>
        /// <returns>The list of found variable anchors</returns>
        public IList<Region> Lex(string data)
        {
            return Lex(new StringReader(data));
        }

        /// <summary>
        /// The main lexer method to parse the region elements within a string reader
        /// </summary>
        /// <param name="reader">The reader to read from</param>
        /// <returns>The list of found variable anchors</returns>
        public IList<Region> Lex(TextReader reader)
        {
            List<Region> regions = new List<Region>();

            Reset();
            Advance(reader);

            while (current != EndOfFile)
            {
                switch (current)
                {
                    case '\r':

                        // move on to the next character and check for a newline character
                        current = reader.Read();
                        currentPosition++;
                        if (current == '\n')
                        {
                            // move on again to the next row
                            current = reader.Read();
                            currentPosition++;
                        }

                        // increment the current row and reset the column position back to the start
                        currentRow++;
                        currentColumn = 0;
                        break;

                    case '\n':

                        // move onto the next char
                        current = reader.Read();

                        // increment the position and move onto the next row
                        currentPosition++;
                        currentColumn = 0;
                        currentRow++;
                        break;

                    case StartAnchor:

                        // If a start anchor has been located, check the next char
                        // for the open bracket
                        int next = reader.Peek();

                        if (next == OpenRegion)
                        {
                            // bracket found so attempt to read in the variable name
                            ReadRegion(reader, regions);
                        }
                        else
                        {
                            // Not an expected token, so move on
                            Advance(reader);
                        }

                        break;
                    default:

                        // move on to the next character
                        Advance(reader);
                        break;
                }
            }

            return regions;
        }

        #endregion

        #region private methods

        /// <summary>
        /// Reads from the current position up until the closing bracket
        /// </summary>
        /// <param name="reader">The reader to read from</param>
        /// <param name="regions">The collection to add to</param>
        private void ReadRegion(TextReader reader, ICollection<Region> regions)
        {
            // move past the peeked open bracket token
            Advance(reader);

            bool foundClosingBracket = false;
            StringBuilder regionBuilder = new StringBuilder();

            while (current != EndOfFile && current != '\r' && current != '\n')
            {
                Advance(reader);
                if (current == CloseRegion)
                {
                    foundClosingBracket = true;
                    break;
                }

                regionBuilder.Append((char)current);
            }

            if (current == CloseRegion)
            {
                Advance(reader);
                foundClosingBracket = true;
            }

            if (foundClosingBracket)
                Push(regions, regionBuilder.ToString());
        }

        /// <summary>
        /// Advances to the next character in the source text
        /// </summary>
        /// <param name="reader">The reader to advance</param>
        private void Advance(TextReader reader)
        {
            current = reader.Read();
            currentPosition++;
            currentColumn++;
        }

        /// <summary>
        /// Pushes a new region onto the head of the collection
        /// </summary>
        /// <param name="regions">The regions to add to</param>
        /// <param name="regionText">The text to add to the collection</param>
        private void Push(ICollection<Region> regions, string regionText)
        {
            if (string.IsNullOrEmpty(regionText))
                return;

            Region region = new Region();

            region.AbsolutePosition = currentPosition - (regionText.Length + AnchorOffset);
            region.Column = currentColumn - (regionText.Length + AnchorOffset);
            region.Line = currentRow;
            region.RegionText = regionText;
            regions.Add(region);
        }

        /// <summary>
        /// Resets the internal counters
        /// </summary>
        private void Reset()
        {
            currentPosition = -1;
            currentRow = 0;
            currentColumn = -1;
            current = -1;
        }

        #endregion
    }
}
