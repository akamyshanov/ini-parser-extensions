using System;
using System.Configuration;
using IniParser.Extensions;
using IniParser.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace INIFileParser.Extensions.Test
{
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
    }
}
