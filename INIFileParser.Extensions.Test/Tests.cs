using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using IniParser.Extensions;
using IniParser.Model;
using IniParser.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace INIFileParser.Extensions.Test
{
    class SectionWithKey
    {
        public const string SampleIni = @"
[section1]
othervalue = dddd
";

        [Key]
        public string SectionName { get; set; }

        [DataMember, DefaultValue("aasd")]
        public string OtherValue { get; set; }
    }

    enum TestEnum
    {
        One,
        Two,
        Three
    }

    class ArrayContainer
    {
        public const string SampleIni =
@"
enums = One,two
enums2 = *
strings = hello, world
ints = 1,2,3,asd,4
";

        [ConfigurationProperty("enums")]
        public TestEnum[] Enums { get; set; }

        [ConfigurationProperty("enums2")]
        public TestEnum[] Enums2 { get; set; }

        [ConfigurationProperty("strings")]
        public string[] Strings { get; set; }

        [ConfigurationProperty("ints")]
        public int[] Ints { get; set; }
    }

    [TestClass]
    public class Tests
    {


        static readonly IniDataParser Parser = new IniDataParser();

        [TestMethod]
        public void TestArrays()
        {
            var config = Parser.Parse(ArrayContainer.SampleIni).CreateObject<ArrayContainer>();
        }

        [TestMethod]
        public void TestSectionName()
        {
            var iniData = Parser.Parse(SectionWithKey.SampleIni);
            var sections = iniData.Sections;

            var parsed = sections.Select(s => s.CreateObject<SectionWithKey>()).ToList();
        }
    }
}
