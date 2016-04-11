using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;

namespace PholioVisualisation.DataAccessTest
{
    [TestClass]
    public class StringListParserTest
    {
        [TestMethod]
        public void TestTwoValues()
        {
            Assert.AreEqual(2, ParseList("a,b").Count);
        }

        [TestMethod]
        public void TestSpacesTolerated()
        {
            var list = ParseList("a , b");
            Assert.AreEqual("a", list[0]);
            Assert.AreEqual("b", list[1]);
        }

        [TestMethod]
        public void TestEmptyValueIgnored()
        {
            var list = ParseList(",b");
            Assert.AreEqual("b", list[0]);
            Assert.AreEqual(1, list.Count);
        }

        [TestMethod]
        public void TestNullStringProducesEmptyList()
        {
            var list = ParseList(null);
            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Count);
        }

        private static IList<string> ParseList(string ins)
        {
            var list = new StringListParser(ins).StringList;
            return list;
        }
    }
}
