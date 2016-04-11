
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;

namespace PholioVisualisation.DataAccessTest
{
    [TestClass]
    public class IntListStringParserTest
    {
        [TestMethod]
        public void TestEmptyString()
        {
            Assert.AreEqual(0, new IntListStringParser("").IntList.Count);
            Assert.AreEqual(0, new IntListStringParser(null).IntList.Count);
            Assert.AreEqual(0, new IntListStringParser(",").IntList.Count);
        }

        [TestMethod]
        public void TestParseOk()
        {
            List<int> list = new IntListStringParser("1,24").IntList;

            Assert.AreEqual(2, list.Count);
            Assert.IsTrue(list.Contains(1));
            Assert.IsTrue(list.Contains(24));
        }
    }
}
