using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using IniParser.Exceptions;

namespace IniParser.Extensions
{
    static class PropertyExtensions
    {
        public static KeyInfo GetKeyInfo(this PropertyInfo property)
        {
            var dataMember = property.GetCustomAttribute<DataMemberAttribute>();
            if (dataMember != null)
            {
                return KeyInfo.Create(property, dataMember);
            }

            var configurationProperty = property.GetCustomAttribute<ConfigurationPropertyAttribute>();
            if (configurationProperty != null)
            {
                return KeyInfo.Create(configurationProperty);
            }

            return null;
        }

        public static PropertyInfo GetKeyProperty(this Type type)
        {
            return type
                .GetProperties()
                .Select(property => new
                {
                    property,
                    keyAttribute = property.GetCustomAttribute<KeyAttribute>()
                })
                .Where(temp => temp.keyAttribute != null)
                .Select(temp => temp.property)
                .FirstOrDefault();
        }

        public static string GetSectionName(this Type type)
        {
            var sectionAttr = type.GetCustomAttribute<SettingsGroupNameAttribute>();
            return sectionAttr != null ? sectionAttr.GroupName : null;
        }

        public static string GetSectionName(this object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            var type = obj.GetType();
            var keyProperty = type
                .GetProperties()
                .Select(property => new
                {
                    property,
                    keyAttribute = property.GetCustomAttribute<KeyAttribute>()
                })
                .Where(temp => temp.keyAttribute != null)
                .Select(temp => temp.property)
                .FirstOrDefault();

            var typeSectionName = type.GetSectionName();
            if (keyProperty == null)
            {
                return typeSectionName;
            }

            var propertyValue = keyProperty.GetValue(obj);
            if (propertyValue == null)
            {
                return typeSectionName;
            }

            return propertyValue.ToString();
        }

        public static void SetValueExt(this PropertyInfo property, object obj, string value)
        {
            var keyInfo = property.GetKeyInfo();
            if (keyInfo == null)
            {
                return;
            }

            try
            {
                var propertyValue = Parser.Parse(property.PropertyType, value);
                property.SetValue(obj, propertyValue);
            }
            catch (Exception ex)
            {
                if (keyInfo.IsRequired)
                {
                    throw new ParsingException("required field fetch error: " + keyInfo.Name, ex);
                }
                else if (keyInfo.DefaultValue != null && typeof(object) != keyInfo.DefaultValue.GetType())
                {
                    property.SetValue(obj, keyInfo.DefaultValue);
                }
            }

        }
    }
}
