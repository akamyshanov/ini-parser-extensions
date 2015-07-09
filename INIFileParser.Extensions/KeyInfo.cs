using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IniParser.Extensions
{
    class KeyInfo
    {
        public string Name { get; set; }
        public bool IsRequired { get; set; }
        public object DefaultValue { get; set; }

        public static KeyInfo Create(PropertyInfo propertyInfo, DataMemberAttribute dataMember)
        {
            return new KeyInfo
            {
                Name = dataMember.Name ?? propertyInfo.Name,
                IsRequired = dataMember.IsRequired
            };
        }

        public static KeyInfo Create(ConfigurationPropertyAttribute prop)
        {
            return new KeyInfo
            {
                Name = prop.Name,
                IsRequired = prop.IsRequired,
                DefaultValue = prop.DefaultValue
            };
        }
    }
}
