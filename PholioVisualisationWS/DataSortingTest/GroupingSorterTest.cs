using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataSortingTest
{
    [TestClass]
    public class GroupingSorterTest
    {
        [TestMethod]
        public void TestOrderByYear()
        {
            var groupings = new List<Grouping>
            {
                GetGrouping(2014, 1, 1),
                GetGrouping(2011, 1, 1),
                GetGrouping(2013, 1, 1),
                GetGrouping(2012, 1, 1)
            };

            var sorted = new GroupingSorter(groupings).SortByDataPointTimePeriodMostRecentFirst();

            Assert.AreEqual(2014, sorted[0].DataPointYear);
            Assert.AreEqual(2013, sorted[1].DataPointYear);
            Assert.AreEqual(2012, sorted[2].DataPointYear);
            Assert.AreEqual(2011, sorted[3].DataPointYear);
        }

        [TestMethod]
        public void TestBaseLineOrderByYear()
        {
            var groupings = new List<Grouping>
            {
                GetBaseLineGrouping(2014, 1, 1),
                GetBaseLineGrouping(2011, 1, 1),
                GetBaseLineGrouping(2013, 1, 1),
                GetBaseLineGrouping(2012, 1, 1)
            };

            var sorted = new GroupingSorter(groupings).SortByBaseLineTimePeriodEarliestFirst();

            Assert.AreEqual(2011, sorted[0].BaselineYear);
            Assert.AreEqual(2012, sorted[1].BaselineYear);
            Assert.AreEqual(2013, sorted[2].BaselineYear);
            Assert.AreEqual(2014, sorted[3].BaselineYear);
        }

        [TestMethod]
        public void TestOrderByYearAndMonth()
        {
            var groupings = new List<Grouping>
            {
                GetGrouping(2014, 1, 1),
                GetGrouping(2014, 1, 2),
                GetGrouping(2013, 1, 2),
                GetGrouping(2013, 1, 1)
            };

            var sorted = new GroupingSorter(groupings).SortByDataPointTimePeriodMostRecentFirst();

            // Assert years 
            Assert.AreEqual(2014, sorted[0].DataPointYear);
            Assert.AreEqual(2014, sorted[1].DataPointYear);
            Assert.AreEqual(2013, sorted[2].DataPointYear);
            Assert.AreEqual(2013, sorted[3].DataPointYear);

            // Assert months 
            Assert.AreEqual(2, sorted[0].DataPointMonth);
            Assert.AreEqual(1, sorted[1].DataPointMonth);
            Assert.AreEqual(2, sorted[2].DataPointMonth);
            Assert.AreEqual(1, sorted[3].DataPointMonth);
        }

        [TestMethod]
        public void TestBaseLineOrderByYearAndMonth()
        {
            var groupings = new List<Grouping>
            {
                GetBaseLineGrouping(2014, 1, 1),
                GetBaseLineGrouping(2014, 1, 2),
                GetBaseLineGrouping(2013, 1, 2),
                GetBaseLineGrouping(2013, 1, 1)
            };

            var sorted = new GroupingSorter(groupings).SortByBaseLineTimePeriodEarliestFirst();

            // Assert years 
            Assert.AreEqual(2013, sorted[0].BaselineYear);
            Assert.AreEqual(2013, sorted[1].BaselineYear);
            Assert.AreEqual(2014, sorted[2].BaselineYear);
            Assert.AreEqual(2014, sorted[3].BaselineYear);

            // Assert months 
            Assert.AreEqual(1, sorted[0].BaselineMonth);
            Assert.AreEqual(2, sorted[1].BaselineMonth);
            Assert.AreEqual(1, sorted[2].BaselineMonth);
            Assert.AreEqual(2, sorted[3].BaselineMonth);
        }

        [TestMethod]
        public void TestOrderByYearAndQuarter()
        {
            var groupings = new List<Grouping>
            {
                GetGrouping(2014, 1, 1),
                GetGrouping(2014, 2, 1),
                GetGrouping(2013, 2, 1),
                GetGrouping(2013, 1, 1)
            };

            var sorted = new GroupingSorter(groupings).SortByDataPointTimePeriodMostRecentFirst();

            // Assert years 
            Assert.AreEqual(2014, sorted[0].DataPointYear);
            Assert.AreEqual(2014, sorted[1].DataPointYear);
            Assert.AreEqual(2013, sorted[2].DataPointYear);
            Assert.AreEqual(2013, sorted[3].DataPointYear);

            // Assert months 
            Assert.AreEqual(2, sorted[0].DataPointQuarter);
            Assert.AreEqual(1, sorted[1].DataPointQuarter);
            Assert.AreEqual(2, sorted[2].DataPointQuarter);
            Assert.AreEqual(1, sorted[3].DataPointQuarter);
        }

        [TestMethod]
        public void TestBaseLineOrderByYearAndQuarter()
        {
            var groupings = new List<Grouping>
            {
                GetBaseLineGrouping(2014, 1, 1),
                GetBaseLineGrouping(2014, 2, 1),
                GetBaseLineGrouping(2013, 2, 1),
                GetBaseLineGrouping(2013, 1, 1)
            };

            var sorted = new GroupingSorter(groupings).SortByBaseLineTimePeriodEarliestFirst();

            // Assert years 
            Assert.AreEqual(2013, sorted[0].BaselineYear);
            Assert.AreEqual(2013, sorted[1].BaselineYear);
            Assert.AreEqual(2014, sorted[2].BaselineYear);
            Assert.AreEqual(2014, sorted[3].BaselineYear);

            // Assert months 
            Assert.AreEqual(1, sorted[0].BaselineQuarter);
            Assert.AreEqual(2, sorted[1].BaselineQuarter);
            Assert.AreEqual(1, sorted[2].BaselineQuarter);
            Assert.AreEqual(2, sorted[3].BaselineQuarter);
        }

        [TestMethod]
        public void TestMostCommonPolarityId()
        {
            var groupings = new List<Grouping>
            {
                new Grouping { PolarityId = PolarityIds.NotApplicable},
                new Grouping { PolarityId = PolarityIds.BlueOrangeBlue},
                new Grouping { PolarityId = PolarityIds.BlueOrangeBlue},
                new Grouping { PolarityId = PolarityIds.NotApplicable},
                new Grouping { PolarityId = PolarityIds.BlueOrangeBlue}
            };

            var polarityId = new GroupingSorter(groupings).GetMostCommonPolarityId();

            Assert.AreEqual(PolarityIds.BlueOrangeBlue, polarityId);
        }

        [TestMethod]
        public void TestRagHighIsGoodPolarityIdFavouredOverMostCommon()
        {
            var groupings = new List<Grouping>
            {
                new Grouping { PolarityId = PolarityIds.BlueOrangeBlue},
                new Grouping { PolarityId = PolarityIds.RagHighIsGood},
                new Grouping { PolarityId = PolarityIds.BlueOrangeBlue}
            };

            var polarityId = new GroupingSorter(groupings).GetMostCommonPolarityId();

            Assert.AreEqual(PolarityIds.RagHighIsGood, polarityId);
        }

        [TestMethod]
        public void TestRagLowIsGoodPolarityIdFavouredOverMostCommon()
        {
            var groupings = new List<Grouping>
            {
                new Grouping { PolarityId = PolarityIds.BlueOrangeBlue},
                new Grouping { PolarityId = PolarityIds.RagLowIsGood},
                new Grouping { PolarityId = PolarityIds.BlueOrangeBlue}
            };

            var polarityId = new GroupingSorter(groupings).GetMostCommonPolarityId();

            Assert.AreEqual(PolarityIds.RagLowIsGood, polarityId);
        }

        public static Grouping GetGrouping(int year, int quarter, int month)
        {
            return new Grouping
            {
                DataPointYear = year,
                DataPointQuarter = quarter,
                DataPointMonth = month
            };
        }

        public static Grouping GetBaseLineGrouping(int year, int quarter, int month)
        {
            return new Grouping
            {
                BaselineYear = year,
                BaselineQuarter = quarter,
                BaselineMonth = month
            };
        }
    }
}
