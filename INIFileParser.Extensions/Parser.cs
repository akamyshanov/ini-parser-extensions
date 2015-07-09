using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniParser.Extensions
{
    static class Parser
    {
        public static object Parse(Type propertyType, string value)
        {
            if (propertyType.IsArray)
            {
                return ParseArray(propertyType.GetElementType(), value);
            }
            else if (propertyType.IsEnum)
            {
                return Enum.Parse(propertyType, value, true);
            }
            else
            {
                return Convert.ChangeType(value, propertyType);
            }
        }

        private static object ParseArray(Type elementType, string value)
        {
            var parts = value
                .Split(',')
                .Where(s => !String.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .ToArray();

            if (elementType.IsEnum && value.Trim() == "*")
            {
                return Enum.GetValues(elementType);
            }

            var list = new List<object>(parts.Length);
            foreach (var part in parts)
            {
                try
                {
                    list.Add(elementType.IsEnum
                        ? Enum.Parse(elementType, part, true)
                        : Convert.ChangeType(part, elementType));
                }
                catch
                {
                    // ignore invalid values
                }
            }

            var array = Array.CreateInstance(elementType, list.Count);
            for (var i = 0; i < list.Count; ++i)
            {
                array.SetValue(list[i], i);
            }
            return array;
        }
    }
}
