
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioObjectsTest
{
    [TestClass]
    public class CoreDataTimeFilterTest
    {
        [TestMethod]
        public void TestFilterYearly()
        {
            List<CoreDataSet> data = new List<CoreDataSet> { 
                new CoreDataSet{ Year = 2009,YearRange = 1, Quarter = -1, Month = -1}
               };

            Assert.AreEqual(1, new CoreDataTimeFilter(data).Filter(
                new TimePeriod { Year = 2009, YearRange = 1 },
                new TimePeriod { Year = 2010, YearRange = 1 }
                ).Count());
        }

        [TestMethod]
        public void TestFilterOutYearly()
        {
            List<CoreDataSet> data = new List<CoreDataSet> { 
                new CoreDataSet{ Year = 2009,YearRange = 1, Quarter = -1, Month = -1}
               };

            Assert.AreEqual(0, new CoreDataTimeFilter(data).Filter(
                new TimePeriod { Year = 2007, YearRange = 1 },
                new TimePeriod { Year = 2008, YearRange = 1 }
                ).Count());
        }

        [TestMethod]
        public void TestFilterMonthly()
        {
            List<CoreDataSet> data = new List<CoreDataSet> { 
                new CoreDataSet{ Year = 2009,YearRange = 1, Quarter = -1, Month = 5}
               };

            Assert.AreEqual(1, new CoreDataTimeFilter(data).Filter(
                new TimePeriod { Year = 2009, YearRange = 1, Month = 3 },
                new TimePeriod { Year = 2009, YearRange = 1, Month = 6 }
                ).Count());
        }

        [TestMethod]
        public void TestFilterOutMonthlyAfterSameYear()
        {
            AssertFilteredOutMonthly(new CoreDataSet { Year = 2009, YearRange = 1, Quarter = -1, Month = 10 });
        }

        [TestMethod]
        public void TestFilterOutMonthlyBeforeSameYear()
        {
            AssertFilteredOutMonthly(new CoreDataSet { Year = 2009, YearRange = 1, Quarter = -1, Month = 2 });
        }

        [TestMethod]
        public void TestFilterOutMonthlyAfterDifferentYear()
        {
            AssertFilteredOutMonthly(new CoreDataSet { Year = 2010, YearRange = 1, Quarter = -1, Month = 3 });
        }

        [TestMethod]
        public void TestFilterOutMonthlyBeforeDifferentYear()
        {
            AssertFilteredOutMonthly(new CoreDataSet { Year = 2008, YearRange = 1, Quarter = -1, Month = 3 });
        }

        private static void AssertFilteredOutMonthly(CoreDataSet data)
        {
            List<CoreDataSet> list = new List<CoreDataSet> { data };

            Assert.AreEqual(0, new CoreDataTimeFilter(list).Filter(
                new TimePeriod { Year = 2009, YearRange = 1, Month = 3 },
                new TimePeriod { Year = 2009, YearRange = 1, Month = 6 }
                ).Count());
        }

        [TestMethod]
        public void TestFilterQuarterly()
        {
            List<CoreDataSet> data = new List<CoreDataSet> { 
                new CoreDataSet{ Year = 2009,YearRange = 1, Quarter = 2, Month = -1}
               };

            Assert.AreEqual(1, new CoreDataTimeFilter(data).Filter(
                new TimePeriod { Year = 2009, YearRange = 1, Quarter = 2 },
                new TimePeriod { Year = 2009, YearRange = 1, Quarter = 3 }
                ).Count());
        }

        [TestMethod]
        public void TestFilterOutQuarterlyAfterSameYear()
        {
            AssertFilteredOutQuarterly(new CoreDataSet { Year = 2009, YearRange = 1, Quarter = 4, Month = -1 });
        }

        [TestMethod]
        public void TestFilterOutQuarterlyBeforeSameYear()
        {
            AssertFilteredOutQuarterly(new CoreDataSet { Year = 2009, YearRange = 1, Quarter = 1, Month = -1 });
        }

        [TestMethod]
        public void TestFilterOutQuarterlyAfterDifferentYear()
        {
            AssertFilteredOutQuarterly(new CoreDataSet { Year = 2010, YearRange = 1, Quarter = 2, Month = -1 });
        }

        [TestMethod]
        public void TestFilterOutQuarterlyBeforeDifferentYear()
        {
            AssertFilteredOutQuarterly(new CoreDataSet { Year = 2008, YearRange = 1, Quarter = 2, Month = -1 });
        }

        private static void AssertFilteredOutQuarterly(CoreDataSet data)
        {
            List<CoreDataSet> list = new List<CoreDataSet> { data };

            Assert.AreEqual(0, new CoreDataTimeFilter(list).Filter(
                new TimePeriod { Year = 2009, YearRange = 1, Quarter = 2 },
                new TimePeriod { Year = 2009, YearRange = 1, Quarter = 3 }
                                   ).Count());
        }

    }
}
