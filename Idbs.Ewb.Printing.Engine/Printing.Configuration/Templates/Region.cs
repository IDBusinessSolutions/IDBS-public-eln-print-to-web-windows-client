// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Region.cs" company="ID Business Solutions Ltd.">
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

namespace Idbs.Ewb.Printing.Configuration
{
    /// <summary>
    /// A class to represent a region within a source string which represents a variable
    /// </summary>
    public class Region
    {
        /// <summary>
        /// Gets or sets the line number in the source text where the token starts
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Gets or sets the column number in the source text where the token starts
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Gets or sets the absolute position of the first character in the token
        /// </summary>
        public int AbsolutePosition { get; set; }

        /// <summary>
        /// Gets or sets the index of the text element in the corresponding data list
        /// </summary>
        public string RegionText { get; set; }
    }
}
