// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IVariableProvider.cs" company="ID Business Solutions Ltd.">
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
    /// An interface to provide specialized versions of the anchored text
    /// </summary>
    public interface IVariableProvider
    {
        /// <summary>
        /// Adds a global variable to the provider
        /// </summary>
        /// <param name="name">The name of the variable</param>
        /// <param name="value">The variable value</param>
        void AddGlobal(string name, string value);

        /// <summary>
        /// Returns a formatted string representing the actual value represented by the
        /// given anchorText
        /// </summary>
        /// <param name="anchorText">The text of the anchored item to retrieve the real 
        /// value for</param>
        /// <returns>The specialized value formatted based on the variables definition</returns>
        string Get(string anchorText);
    }
}
