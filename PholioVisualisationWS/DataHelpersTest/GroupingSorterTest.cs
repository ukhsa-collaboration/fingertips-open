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

        public static Grouping GetGrouping(int year, int quarter, int month)
        {
            return new Grouping
            {
                DataPointYear = year,
                DataPointQuarter = quarter,
                DataPointMonth = month
            };
        }
    }
}
