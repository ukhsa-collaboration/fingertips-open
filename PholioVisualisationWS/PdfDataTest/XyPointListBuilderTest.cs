using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PdfData;
using PholioVisualisation.PholioObjects;

namespace PdfDataTest
{
    [TestClass]
    public class XyPointListBuilderTest
    {
        [TestMethod]
        public void TestEmptyCoreDataSetListsProduceEmptyPointsList()
        {
            var points = new XyPointListBuilder(new List<CoreDataSet>(),
                new List<CoreDataSet>()).XyPoints;
            Assert.AreEqual(0, points.Count);
        }

        [TestMethod]
        public void TestEmptyCoreDataSetAndIntListsProduceEmptyPointsList()
        {
            var points = new XyPointListBuilder(new List<int>(),
                new List<CoreDataSet>()).XyPoints;
            Assert.AreEqual(0, points.Count);
        }

        [TestMethod]
        public void TestUnequalSizedCoreDataSetListsProduceEmptyPointsList()
        {
            var points = new XyPointListBuilder(new List<CoreDataSet>(),
                new List<CoreDataSet> { new CoreDataSet() }).XyPoints;
            Assert.AreEqual(0, points.Count);
        }

        [TestMethod]
        public void TestEqualSizedCoreDataSetListsProduceXyListWithCorrectlyAssignedValues()
        {
            var points = new XyPointListBuilder(
                new List<CoreDataSet> { new CoreDataSet { Value = 1 } },
                new List<CoreDataSet> { new CoreDataSet { Value = 2 } }
                ).XyPoints;
            var point = points.First();
            Assert.AreEqual(1, point.X);
            Assert.AreEqual(2, point.Y);
        }

        [TestMethod]
        public void TestUnequalSizedCoreDataSetAndIntListsProduceEmptyPointsList()
        {
            var points = new XyPointListBuilder(new List<int>(),
                new List<CoreDataSet> { new CoreDataSet() }).XyPoints;
            Assert.AreEqual(0, points.Count);
        }

        [TestMethod]
        public void TestEqualSizedCoreDataSetAndIntListsProduceXyListWithCorrectlyAssignedValues()
        {
            var points = new XyPointListBuilder(
                new List<int> { 1 },
                new List<CoreDataSet> { new CoreDataSet { Value = 2 } }
                ).XyPoints;
            var point = points.First();
            Assert.AreEqual(1, point.X);
            Assert.AreEqual(2, point.Y);
        }
    }
}
