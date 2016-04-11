using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class GroupRootFilterTest
    {
        [TestMethod]
        public void TestRemoveRootsWithoutChildAreaData_OneRootWithDataOneWithout()
        {
            List<GroupRoot> roots = new List<GroupRoot>{
                RootWithNoData(),
                RootWithData()
            };

            Assert.AreEqual(1, RemoveRootsWithoutChildAreaData(roots).Count);
        }

        [TestMethod]
        public void TestRemoveRootsWithoutChildAreaData_TwoRootsWithNoData()
        {
            List<GroupRoot> roots = new List<GroupRoot>{
                RootWithNoData(),
                RootWithNoData()
            };

            Assert.AreEqual(0, RemoveRootsWithoutChildAreaData(roots).Count);
        }

        [TestMethod]
        public void TestRemoveRootsWithoutChildAreaData_TwoRootsWithData()
        {
            List<GroupRoot> roots = new List<GroupRoot>{
                RootWithData(),
                RootWithData()
            };

            Assert.AreEqual(2, RemoveRootsWithoutChildAreaData(roots).Count);
        }

        [TestMethod]
        public void TestKeepRootsWithIndicatorIds()
        {
            List<GroupRoot> roots = new List<GroupRoot>{
                new GroupRoot {IndicatorId = 1},
                 new GroupRoot {IndicatorId = 2}
            };

            var indicatorIds = new List<int> {2};

            var filteredRoots = new GroupRootFilter(roots).KeepRootsWithIndicatorIds(indicatorIds);
            Assert.AreEqual(1, filteredRoots.Count);
            Assert.AreEqual(2, filteredRoots.First().IndicatorId);
        }

        private static IList<GroupRoot> RemoveRootsWithoutChildAreaData(List<GroupRoot> roots)
        {
            var filteredRoots = new GroupRootFilter(roots).RemoveRootsWithoutChildAreaData();
            return filteredRoots;
        }

        private static GroupRoot RootWithNoData()
        {
            return new GroupRoot { Data = new List<CoreDataSet>() };
        }

        private static GroupRoot RootWithData()
        {
            return new GroupRoot
            {
                Data = new List<CoreDataSet> { 
                    new CoreDataSet()
                }
            };
        }
    }
}
