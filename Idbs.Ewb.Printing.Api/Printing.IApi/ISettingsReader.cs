// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISettingsReader.cs" company="ID Business Solutions Ltd.">
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

namespace Idbs.Ewb.Printing.IApi
{
    /// <summary>
    /// Implementers of this interface are able to provide the application with
    /// key \ value pair settings.
    /// </summary>
    public interface ISettingsReader
    {
        /// <summary>
        /// Returns the value of a setting from a configuration file.  If the setting is null, or the key is not found,
        /// the default value will be returned instead.
        /// </summary>
        /// <param name="key">The key of the setting to get</param>
        /// <param name="defaultValue">The default value to return if the key is not found, or the value is null or empty</param>
        /// <returns>The value for this key, or the default value</returns>
        string GetSetting(string key, string defaultValue);

        /// <summary>
        /// Attempts to get a typed setting based on the given key
        /// </summary>
        /// <typeparam name="T">The typed value for the result</typeparam>
        /// <param name="key">The key of the item to get</param>
        /// <param name="defaultValue">The default value to return when the key does not exist, or the value is empty</param>
        /// <returns>The setting value</returns>
        /// <remarks>
        /// If the key is present and the value is not null or empty, then an attempt will be made to convert the
        /// string value into a value of type T.  This may raise an exception if no converter can be found.
        /// </remarks>
        T GetSetting<T>(string key, T defaultValue);
    }
}
