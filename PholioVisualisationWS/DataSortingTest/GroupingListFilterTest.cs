using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataSortingTest
{
    [TestClass]
    public class GroupingListFilterTest
    {
        [TestMethod]
        public void TestSelectDistinctComparatorIds()
        {
            var groupings = new List<Grouping> {
                new Grouping{
                    ComparatorId = 1
                },   
                new Grouping{
                    ComparatorId = 4
                },
                new Grouping{
                    ComparatorId = 4
                },
            };

            var comparatorIds = new GroupingListFilter(groupings)
                .SelectDistinctComparatorIds()
                .ToList();
            comparatorIds.Sort();

            Assert.AreEqual(2, comparatorIds.Count());
            Assert.AreEqual(1, comparatorIds[0]);
            Assert.AreEqual(4, comparatorIds[1]);
        }

        [TestMethod]
        public void TestSelectForAreaTypeIds()
        {
            var areaTypeToKeep = AreaTypeIds.County;
            var areaTypeIds = new List<int> { areaTypeToKeep };

            var groupings = new List<Grouping> {
                new Grouping{
                    AreaTypeId = areaTypeToKeep
                },   
                new Grouping{
                    AreaTypeId = (int) AreaTypeIds.Pct
                },
            };

            var filteredGroupings = new GroupingListFilter(groupings).SelectForAreaTypeIds(areaTypeIds);
            Assert.AreEqual(1, filteredGroupings.Count());
            Assert.AreEqual(areaTypeToKeep, filteredGroupings.First().AreaTypeId);
        }

        [TestMethod]
        public void TestSelectForAreaTypeIdsReturnsOriginalListIfAreaTypeIdListIsNull()
        {
            var groupings = new List<Grouping> {  
                new Grouping{
                    AreaTypeId = (int) AreaTypeIds.Pct
                },
            };

            var filteredGroupings = new GroupingListFilter(groupings).SelectForAreaTypeIds(null);
            Assert.AreEqual(1, filteredGroupings.Count());
        }

        [TestMethod]
        public void TestSelectForBaseLineTimePeriods()
        {
            var groupings = new List<Grouping>
            {
                new Grouping {BaselineYear = 2010, BaselineQuarter = -1, BaselineMonth = -1},
                new Grouping {BaselineYear = 2010, BaselineQuarter = -1, BaselineMonth = -1},
                new Grouping {BaselineYear = 2001, BaselineQuarter = -1, BaselineMonth = -1},
                new Grouping {BaselineYear = 2001, BaselineQuarter = -1, BaselineMonth = -1},
                new Grouping {BaselineYear = 2010, BaselineQuarter = -1, BaselineMonth = -1},
                new Grouping {BaselineYear = 2010, BaselineQuarter = -1, BaselineMonth = -1}
            };

            var grouping = new Grouping() { BaselineYear = 2010, BaselineQuarter = -1, BaselineMonth =  -1 };

            var filteredGroupings = new GroupingListFilter(groupings).SelectForBaseLineTimePeriods(grouping);
            Assert.AreEqual(4, filteredGroupings.Count());
            Assert.AreEqual(2010, filteredGroupings.FirstOrDefault().BaselineYear);
        }
    }
}
