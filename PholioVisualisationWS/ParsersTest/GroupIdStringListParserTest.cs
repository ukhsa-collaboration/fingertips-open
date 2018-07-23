using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Parsers;

namespace PholioVisualisation.ParsersTest
{
    [TestClass]
    public class GroupIdStringListParserTest
    {
        [TestMethod]
        public void TestParseList()
        {
            var ids = new GroupIdStringListParser(new List<string> { "1", "2,3"});
            Assert.AreEqual(3, ids.IntList.Count);
        }

        [TestMethod]
        public void TestAllIdsAreUnique()
        {
            var ids = new GroupIdStringListParser(new List<string> { "2", "2" });
            Assert.AreEqual(1, ids.IntList.Count);
        }

        [TestMethod]
        public void TestEmptyStringsTolerated()
        {
            var ids = new GroupIdStringListParser(new List<string> { "", "2" });
            Assert.AreEqual(1, ids.IntList.Count);
        }
    }
}
