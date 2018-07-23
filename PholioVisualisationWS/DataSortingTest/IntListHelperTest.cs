using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataSorting;

namespace PholioVisualisation.DataSortingTest
{
    [TestClass]
    public class IntListHelperTest
    {
        [TestMethod]
        public void TestOrdered()
        {
            Assert.AreEqual(2, IntListHelper.FindMostFrequentValue(
                new List<int> {1,2,2,3}));
        }

        [TestMethod]
        public void TestUnordered()
        {
            Assert.AreEqual(2, IntListHelper.FindMostFrequentValue(
                new List<int> { 2, 1, 2, 3 }));
        }
    }
}
