using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Parsers;

namespace PholioVisualisation.ParsersTest
{
    [TestClass]
    public class KeyValuePairListParserTest
    {
        [TestMethod]
        public void Where_Nothing_Defined()
        {
            Assert.AreEqual(0, KeyValuePairListParser.Parse(null).Count);
            Assert.AreEqual(0, KeyValuePairListParser.Parse("").Count);
            Assert.AreEqual(0, KeyValuePairListParser.Parse(" ").Count);
        }

        [TestMethod]
        public void Where_Two_Pairs_Defined()
        {
            var map = KeyValuePairListParser.Parse("a:1,b:2");
            Assert.AreEqual(2, map.Count);
            Assert.AreEqual("1", map["a"]);
        }
    }
}
