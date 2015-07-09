using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser.Model;

namespace IniParser.Extensions
{
    public static class KeyDataCollectionExtensions
    {
        /// <summary>
        /// Fills an object from section data
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="keyData">Section data</param>
        /// <param name="obj">Object</param>
        public static void FillObject<T>(this KeyDataCollection keyData, T obj)
            where T : class
        {
            if (obj == null) throw new ArgumentNullException("obj");
            var type = obj.GetType();
            foreach (var property in type.GetProperties())
            {
                var keyInfo = property.GetKeyInfo();
                if (keyInfo == null)
                {
                    continue;
                }

                property.SetValueExt(obj, keyData[keyInfo.Name]);
            }
        }

        public static KeyDataCollection FillKeyDataCollection(this KeyDataCollection collection, object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            var type = obj.GetType();
            var keyData = new KeyDataCollection();
            foreach (var propertyInfo in type.GetProperties())
            {
                var keyInfo = propertyInfo.GetKeyInfo();
                if (keyInfo != null)
                {
                    keyData[keyInfo.Name] = (propertyInfo.GetValue(obj) ?? "").ToString();
                }
            }

            return keyData;
        }

        public static KeyDataCollection CreateKeyDataCollection(object obj)
        {
            var coll = new KeyDataCollection();
            return coll.FillKeyDataCollection(obj);
        }

        /// <summary>
        /// Creates an object from KeyDataCollection
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="data">Section data</param>
        /// <returns>Converted object</returns>
        [Pure]
        public static T CreateObject<T>(this KeyDataCollection data)
            where T : class, new()
        {
            var t = new T();
            data.FillObject(t);
            return t;
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
    }
}
