using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataSortingTest
{
    [TestClass]
    public class GroupRootFilterTest
    {
        private Mock<IGroupDataReader> _groupDataReader;

        [TestInitialize]
        public void TestInitialize()
        {
            _groupDataReader = new Mock<IGroupDataReader>();
        }

        [TestMethod]
        public void TestRemoveRootsWithoutChildAreaData_OneRootWithDataOneWithout()
        {
            List<GroupRoot> roots = new List<GroupRoot>{
                Root(),Root()
            };

            _groupDataReader.SetupSequence(x => x.GetCoreDataCountAtDataPoint(It.IsAny<Grouping>()))
                .Returns(0)
                .Returns(1);

            Assert.AreEqual(1, RemoveRootsWithoutChildAreaData(_groupDataReader.Object, roots).Count);
        }

        [TestMethod]
        public void TestRemoveRootsWithoutChildAreaData_TwoRootsWithNoData()
        {
            List<GroupRoot> roots = new List<GroupRoot>{
                Root(),Root()
            };

            _groupDataReader.Setup(x => x.GetCoreDataCountAtDataPoint(It.IsAny<Grouping>())).Returns(0);

            Assert.AreEqual(0, RemoveRootsWithoutChildAreaData(_groupDataReader.Object, roots).Count);
        }

        [TestMethod]
        public void TestRemoveRootsWithoutChildAreaData_TwoRootsWithData()
        {
            List<GroupRoot> roots = new List<GroupRoot>{
                Root(), Root()
            };

            _groupDataReader.Setup(x => x.GetCoreDataCountAtDataPoint(It.IsAny<Grouping>())).Returns(1);

            Assert.AreEqual(2, RemoveRootsWithoutChildAreaData(_groupDataReader.Object, roots).Count);
        }

        [TestMethod]
        public void TestKeepRootsWithIndicatorIds()
        {
            List<GroupRoot> roots = new List<GroupRoot>{
                new GroupRoot {IndicatorId = 1},
                 new GroupRoot {IndicatorId = 2}
            };

            var indicatorIds = new List<int> { 2 };

            var filteredRoots = new GroupRootFilter(null).KeepRootsWithIndicatorIds(roots, indicatorIds);
            Assert.AreEqual(1, filteredRoots.Count);
            Assert.AreEqual(2, filteredRoots.First().IndicatorId);
        }

        private static IList<GroupRoot> RemoveRootsWithoutChildAreaData(IGroupDataReader groupDataReader, 
            List<GroupRoot> roots)
        {
            var filteredRoots = new GroupRootFilter(groupDataReader).RemoveRootsWithoutChildAreaData(roots);
            return filteredRoots;
        }

        private static GroupRoot Root()
        {
            return new GroupRoot
            {
                Grouping = new List<Grouping> { new Grouping() }
            };
        }
    }
}
