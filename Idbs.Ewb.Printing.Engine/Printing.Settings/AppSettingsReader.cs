// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppSettingsReader.cs" company="ID Business Solutions Ltd.">
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
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Idbs.Ewb.Printing.IApi;

namespace Idbs.Ewb.Printing.Settings
{
    /// <summary>
    /// An implementation of the <see cref="ISettingsReader"/> interface, using the application 
    /// configuration file as a source of settings
    /// </summary>}
    [Export(typeof(ISettingsReader))]
    public class AppSettingsReader : ISettingsReader
    {
        /// <summary> The config file to use </summary>
        private readonly NameValueCollection appConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingsReader"/> class.
        /// </summary>
        public AppSettingsReader()
        {
            appConfig = new NameValueCollection();
            LoadSettingsFromFile(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        }

        /// <summary>
        /// Returns the value of a setting from a configuration file.  If the setting is null, or the key 
        /// is not found, the default value will be returned instead.
        /// </summary>
        /// <param name="key">The key of the setting to get</param>
        /// <param name="defaultValue">The default value to return if the key is not found, or the value
        /// is null or empty</param>
        /// <returns>The value for this key, or the default value</returns>
        public string GetSetting(string key, string defaultValue)
        {
            return appConfig.Get(key) ?? defaultValue;
        }

        /// <summary>
        /// Attempts to get a typed setting based on the given key
        /// </summary>
        /// <typeparam name="T">The typed value for the result</typeparam>
        /// <param name="key">The key of the item to get</param>
        /// <param name="defaultValue">The default value to return when the key does not exist, or the 
        /// value is empty</param>
        /// <returns>The setting value</returns>
        /// <remarks>
        /// If the key is present and the value is not null or empty, then an attempt will be made to 
        /// convert the string value into a value of type T.  This may raise an exception if no converter 
        /// can be found.
        /// </remarks>
        public T GetSetting<T>(string key, T defaultValue)
        {
            string setting = GetSetting(key, null);

            if (setting != null) 
                return (T)Convert.ChangeType(setting, typeof(T));

            return defaultValue;
        }

        /// <summary>
        /// Load the application settings from the given file.
        /// </summary>
        /// <param name="filePath"> The file path. </param>
        private void LoadSettingsFromFile(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            {
                // Use the configuration file that the AppDomain was set up with
                XDocument doc = XDocument.Load(stream);

                if (doc.Root != null)
                {
                    XElement settingsEl = doc.Root.Element("appSettings");

                    if (settingsEl != null)
                    {
                        var pairs = from el in settingsEl.Elements("add")
                                    let xKeyAttr = el.Attribute("key")
                                    let xValueAttr = el.Attribute("value")
                                    where xKeyAttr != null 
                                    where xValueAttr != null
                                    select new { key = xKeyAttr.Value, value = xValueAttr.Value };

                        foreach (var pair in pairs)
                            appConfig.Add(pair.key, pair.value);
                    }
                }   
            }
        }
    }
}
