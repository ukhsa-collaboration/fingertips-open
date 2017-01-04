using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;

namespace PholioVisualisation.DataAccessTest
{
    [TestClass]
    public class AreaCodeListSplitterTest
    {
        public const int NumberOfCodesToTakeAtOnce = AreaCodeListSplitter.NumberOfCodesToTakeAtOnce;

        [TestMethod]
        public void Test_2500_codes()
        {
            List<string> list = new List<string>(2500);
            list.AddRange(Enumerable.Repeat("a", 2500));

            var splitter = new AreaCodeListSplitter(list);
            
            Assert.IsTrue(splitter.AnyLeft());
            Assert.AreEqual(NumberOfCodesToTakeAtOnce, splitter.NextCodes().Count());
            Assert.IsTrue(splitter.AnyLeft());
            Assert.AreEqual(NumberOfCodesToTakeAtOnce, splitter.NextCodes().Count());
            Assert.IsTrue(splitter.AnyLeft());
            Assert.AreEqual(500, splitter.NextCodes().Count());
            Assert.IsFalse(splitter.AnyLeft());
        }

        [TestMethod]
        public void Test_2000_codes()
        {
            List<string> list = new List<string>(2000);
            list.AddRange(Enumerable.Repeat("a", 2000));

            var splitter = new AreaCodeListSplitter(list);

            Assert.IsTrue(splitter.AnyLeft());
            Assert.AreEqual(NumberOfCodesToTakeAtOnce, splitter.NextCodes().Count());
            Assert.IsTrue(splitter.AnyLeft());
            Assert.AreEqual(NumberOfCodesToTakeAtOnce, splitter.NextCodes().Count());
            Assert.IsFalse(splitter.AnyLeft());
        }
    }
}
