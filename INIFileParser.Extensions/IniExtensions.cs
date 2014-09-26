using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using IniParser.Exceptions;
using IniParser.Model;
namespace IniParser.Extensions
{
    /// <summary>
    /// Extensions for IniParser
    /// </summary>
    public static class IniExtensions
    {
        private static void FillObject<T>(T obj, Func<string, string, string> valueExtractor)
            where T : class
        {
            if (obj == null) throw new ArgumentNullException("obj");

            var sectionAttribute = typeof(T).GetCustomAttribute<SettingsGroupNameAttribute>() ?? new SettingsGroupNameAttribute(null);
            foreach (var property in typeof(T).GetProperties())
            {
                var propertyAttribute = property.GetCustomAttribute<ConfigurationPropertyAttribute>() ?? new ConfigurationPropertyAttribute(property.Name);
                try
                {
                    var value = valueExtractor(sectionAttribute.GroupName, propertyAttribute.Name);
                    property.SetValue(obj, Convert.ChangeType(value, property.PropertyType));
                }
                catch (Exception ex)
                {
                    if (propertyAttribute.IsRequired)
                    {
                        throw new ParsingException("required field fetch error: " + propertyAttribute.Name, ex);
                    }
                    else if (propertyAttribute.DefaultValue != null)
                    {
                        property.SetValue(obj, propertyAttribute.DefaultValue);
                    }
                }
            }
        }

        /// <summary>
        /// Fills an object from section data
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="section">Section data</param>
        /// <param name="obj">Object</param>
        public static void FillObject<T>(this KeyDataCollection section, T obj)
            where T : class
        {
            FillObject(obj, (_, key) => section.GetValue(key));
        }

        /// <summary>
        /// Fills an object from INI data
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="data">INI data</param>
        /// <param name="obj">Object</param>
        public static void FillObject<T>(this IniData data, T obj)
            where T : class
        {
            FillObject(obj, data.GetValue);
        }

        /// <summary>
        /// Creates an object from INI file
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="data">INI data</param>
        /// <returns>Converted object</returns>
        public static T CreateObject<T>(this IniData data)
            where T : class, new()
        {
            var t = new T();
            data.FillObject(t);
            return t;
        }

        /// <summary>
        /// Creates an object from a section
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="data">Section data</param>
        /// <returns>Converted object</returns>
        public static T CreateObject<T>(this KeyDataCollection data)
            where T : class, new()
        {
            var t = new T();
            data.FillObject(t);
            return t;
        }

        /// <summary>
        /// Creates IniData from any object.
        /// Object must be decorated with SettingsGroupNameAttribute and ConfigurationPropertyAttribute.
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <returns>IniData</returns>
        public static IniData CreateIniData(object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            var type = obj.GetType();
            var sectionAttr = type.GetCustomAttribute<SettingsGroupNameAttribute>();
            var data = new IniData();
            foreach (var propertyInfo in type.GetProperties())
            {
                KeyDataCollection keyData;
                if (sectionAttr == null)
                {
                    keyData = data.Global;
                }
                else
                {
                    data.Sections.AddSection(sectionAttr.GroupName);
                    keyData = data.Sections[sectionAttr.GroupName];
                }

                var nameAttr = propertyInfo.GetCustomAttribute<ConfigurationPropertyAttribute>();
                if (nameAttr == null) continue;
                keyData[nameAttr.Name] = (propertyInfo.GetValue(obj) ?? "").ToString();
            }

            return data;
        }

        /// <summary>
        /// Tries to fetch a value by key from a section
        /// </summary>
        /// <param name="section">Section object</param>
        /// <param name="key">Key</param>
        /// <param name="value">out Value</param>
        /// <returns>Whether the fetch succeeded</returns>
        public static bool TryGetValue(this KeyDataCollection section, string key, out string value)
        {
            value = section != null
                ? section[key]
                : null;
            return value != null;
        }

        /// <summary>
        /// Fetches the value by key from a section
        /// </summary>
        /// <param name="section">Section object</param>
        /// <param name="key">Key</param>
        /// <returns>Value</returns>
        /// <exception cref="KeyNotFoundException">If fetch failed</exception>
        public static string GetValue(this KeyDataCollection section, string key)
        {
            string value;
            if (section.TryGetValue(key, out value))
            {
                return value;
            }
            else
            {
                throw new KeyNotFoundException("Key not found: " + key);
            }
        }

        /// <summary>
        /// Tries to fetch the value by section and key
        /// </summary>
        /// <param name="data">IniData</param>
        /// <param name="sectionName">Section name</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <returns>Whether the fetch succeeded</returns>
        public static bool TryGetValue(this IniData data, string sectionName, string key, out string value)
        {
            var section = sectionName == null
                ? data.Global
                : data[sectionName];
            return section.TryGetValue(key, out value);
        }

        /// <summary>
        /// Fetches the value by section and key
        /// </summary>
        /// <param name="file">IniData</param>
        /// <param name="sectionName">Section name</param>
        /// <param name="key">Key</param>
        /// <returns>Value</returns>
        public static string GetValue(this IniData file, string sectionName, string key)
        {
            string value;
            if (TryGetValue(file, sectionName, key, out value))
            {
                return value;
            }
            else
            {
                throw new KeyNotFoundException("Section/Key not found: " + sectionName + "/" + key);
            }
        }

    }
}
