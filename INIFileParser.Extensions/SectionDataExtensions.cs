using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser.Model;

namespace IniParser.Extensions
{
    public static class SectionDataExtensions
    {
        /// <summary>
        /// Creates objects from sections
        /// </summary>
        /// <typeparam name="T">Type to create</typeparam>
        /// <param name="section">Section data</param>
        /// <returns>Created object</returns>
        public static T CreateObject<T>(this SectionData section)
            where T : class, new()
        {
            section = new SectionData(section, StringComparer.OrdinalIgnoreCase);
            var obj = section.Keys.CreateObject<T>();
            var keyProperty = typeof(T).GetKeyProperty();
            if (keyProperty != null)
            {
                keyProperty.SetValue(obj, Parser.Parse(keyProperty.PropertyType, section.SectionName));
            }
            return obj;
        }

        public static SectionData CreateSectionData(object obj)
        {
            var keyDataCollection = KeyDataCollectionExtensions.CreateKeyDataCollection(obj);
            var sectionData = new SectionData(obj.GetSectionName())
            {
                Keys = keyDataCollection
            };
            return sectionData;
        }
    }
}
