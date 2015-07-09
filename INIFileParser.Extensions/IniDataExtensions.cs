using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IniParser.Model;

namespace IniParser.Extensions
{
    public static class IniDataExtensions
    {
        /// <summary>
        /// Fills an object from INI data
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="data">INI data</param>
        /// <param name="obj">Object</param>
        public static void FillObject<T>(this IniData data, T obj)
            where T : class
        {
            var section = typeof(T).GetSectionName();
            var kdColl = section != null ? data.Sections[section] : data.Global;
            if (kdColl == null)
            {
                return;
            }

            kdColl.FillObject(obj);
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
