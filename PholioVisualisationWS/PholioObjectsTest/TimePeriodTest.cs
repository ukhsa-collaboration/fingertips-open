
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PholioObjectsTest
{
    [TestClass]
    public class TimePeriodTest
    {
        [TestMethod]
        public void TestGetBaseline()
        {
            Grouping g = new Grouping
            {
                BaselineYear = 2001,
                BaselineQuarter = 3,
                DataPointQuarter = 4,
                DataPointYear = 2003,
                YearRange = 1
            };

            TimePeriod baseline = TimePeriod.GetBaseline(g);
            TimePeriod dataPoint = TimePeriod.GetDataPoint(g);

            Assert.AreEqual(g.DataPointQuarter, dataPoint.Quarter);
            Assert.AreEqual(g.DataPointYear, dataPoint.Year);
            Assert.AreEqual(g.YearRange, dataPoint.YearRange);

            Assert.AreEqual(g.BaselineQuarter, baseline.Quarter);
            Assert.AreEqual(g.BaselineYear, baseline.Year);
            Assert.AreEqual(g.YearRange, baseline.YearRange);
        }

        [TestMethod]
        public void TestGetRollingQuarterlyEndPoint()
        {
            int yearRange = 1;
            AssertExpectedRollingQuarterlyEndPoint(2000, 1, yearRange, 2000, 4);
            AssertExpectedRollingQuarterlyEndPoint(2000, 2, yearRange, 2001, 1);
            AssertExpectedRollingQuarterlyEndPoint(2000, 3, yearRange, 2001, 2);
            AssertExpectedRollingQuarterlyEndPoint(2000, 4, yearRange, 2001, 3);

            yearRange = 2;
            AssertExpectedRollingQuarterlyEndPoint(2000, 1, yearRange, 2001, 4);
            AssertExpectedRollingQuarterlyEndPoint(2000, 2, yearRange, 2002, 1);
            AssertExpectedRollingQuarterlyEndPoint(2000, 3, yearRange, 2002, 2);
            AssertExpectedRollingQuarterlyEndPoint(2000, 4, yearRange, 2002, 3);
        }

        private static void AssertExpectedRollingQuarterlyEndPoint(int year, int quarter, int yearRange,
            int expectedYear, int expectedQuarter)
        {
            TimePoint p = new TimePeriod { Year = year, Quarter = quarter, YearRange = yearRange }.GetRollingQuarterlyEndPoint();
            Assert.AreEqual(expectedYear, p.Year);
            Assert.AreEqual(expectedQuarter, p.Quarter);
        }

        [TestMethod]
        public void TestGetRollingMonthlyEndPointToleratesUndefinedYearRange()
        {
            TimePoint p = new TimePeriod {Year = 2000, Month = 1}.GetRollingMonthlyEndPoint();
            Assert.AreEqual(2000, p.Year);
            Assert.AreEqual(12, p.Month);
        }

        [TestMethod]
        public void TestGetRollingMonthlyEndPoint()
        {
            int yearRange = 1;
            AssertExpectedRollingMonthlyEndPoint(2000, 1, yearRange, 2000, 12);
            for (int i = 2; i <= 12; i++)
            {
                AssertExpectedRollingMonthlyEndPoint(2000, i, yearRange, 2001, i-1);
            }

            yearRange = 2;
            AssertExpectedRollingMonthlyEndPoint(2000, 1, yearRange, 2001, 12);
            for (int i = 2; i <= 12; i++)
            {
                AssertExpectedRollingMonthlyEndPoint(2000, i, yearRange, 2002, i - 1);
            }
        }

        [TestMethod]
        public void TestGetTimePeriodForYearBefore()
        {
            var period = new TimePeriod
            {
                Year = 2001,
                YearRange = 2,
                Month = TimePoint.Undefined,
                Quarter = TimePoint.Undefined
            };

            var yearBefore = period.GetTimePeriodForYearBefore();
            Assert.AreEqual(2000, yearBefore.Year);
            Assert.AreEqual(2, yearBefore.YearRange);
            Assert.AreEqual(TimePoint.Undefined, yearBefore.Month);
            Assert.AreEqual(TimePoint.Undefined, yearBefore.Quarter);
        }

        private static void AssertExpectedRollingMonthlyEndPoint(int year, int month, int yearRange,
            int expectedYear, int expectedMonth)
        {
            TimePoint p = new TimePeriod { Year = year, Month = month, YearRange = yearRange }.GetRollingMonthlyEndPoint();
            Assert.AreEqual(expectedYear, p.Year);
            Assert.AreEqual(expectedMonth, p.Month);
        }
    }
}
