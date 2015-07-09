using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using IniParser.Exceptions;
using IniParser.Model;
namespace IniParser.Extensions
{
    [Obsolete("Use IniDataExtensions, SectionDataExtensions, KeyDataCollectionExtensions")]
    public static class IniExtensions
    {
        public static IniData CreateIniData(object obj)
        {
            return IniDataExtensions.CreateIniData(obj);
        }

        public static KeyDataCollection CreateKeyDataCollection(object obj)
        {
            return KeyDataCollectionExtensions.CreateKeyDataCollection(obj);
        }
    }
}
