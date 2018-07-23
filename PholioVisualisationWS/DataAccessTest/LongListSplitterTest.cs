using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;

namespace PholioVisualisation.DataAccessTest
{
    [TestClass]
    public class LongListSplitterTest
    {
        public const int NumberOfCodesToTakeAtOnce = LongListSplitter<string>.NumberOfCodesToTakeAtOnce;

        [TestMethod]
        public void Test_2_And_Half_Lots_Of_codes()
        {
            var splitter = GetSplitterWithNumberOfItems((int)(NumberOfCodesToTakeAtOnce * 2.5));

            TakeNextSetOfCodes(splitter);
            TakeNextSetOfCodes(splitter);

            Assert.AreEqual(NumberOfCodesToTakeAtOnce * 0.5, splitter.NextItems().Count());
            Assert.IsFalse(splitter.AnyLeft());
        }

        [TestMethod]
        public void Test_2_Lots_Of_codes()
        {
            var splitter = GetSplitterWithNumberOfItems(NumberOfCodesToTakeAtOnce * 2);

            TakeNextSetOfCodes(splitter);
            TakeNextSetOfCodes(splitter);

            Assert.IsFalse(splitter.AnyLeft());
        }

        private static LongListSplitter<string> GetSplitterWithNumberOfItems(int itemCount)
        {
            List<string> list = new List<string>();
            list.AddRange(Enumerable.Repeat("a", itemCount));

            var splitter = new LongListSplitter<string>(list);
            return splitter;
        }

        private static void TakeNextSetOfCodes(LongListSplitter<string> splitter)
        {
            Assert.IsTrue(splitter.AnyLeft());
            Assert.AreEqual(NumberOfCodesToTakeAtOnce, splitter.NextItems().Count());
        }
    }
}
