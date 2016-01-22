
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioObjectsTest
{
    [TestClass]
    public class TimePeriodIteratorTest
    {
        YearType calendarYearType = new YearType
        {
            Id = YearTypeIds.Calendar
        };

        [TestMethod]
        public void TestYearly()
        {
            TimePeriodIterator iterator = new TimePeriodIterator(
                new TimePeriod { Year = 2002, YearRange = 1 }, 
                new TimePeriod { Year = 2010, YearRange = 1 }, 
                calendarYearType);
            Assert.AreEqual(9, iterator.TimePeriods.Count);

            for (int i = 0; i < iterator.TimePeriods.Count; i++)
            {
                AssertTimePeriod(iterator.TimePeriods[0], 2002, TimePoint.Undefined, TimePoint.Undefined);
            }
        }

        [TestMethod]
        public void TestMonthly()
        {
            TimePeriodIterator iterator = new TimePeriodIterator(
                new TimePeriod { Year = 2005, Month = 3 }, 
                new TimePeriod { Year = 2006, Month = 2 },
                calendarYearType);
            Assert.AreEqual(12, iterator.TimePeriods.Count);

            int i = 0;
            AssertTimePeriod(iterator.TimePeriods[i++], 2005, TimePoint.Undefined, 3);
            AssertTimePeriod(iterator.TimePeriods[i++], 2005, TimePoint.Undefined, 4);
            AssertTimePeriod(iterator.TimePeriods[i++], 2005, TimePoint.Undefined, 5);
            AssertTimePeriod(iterator.TimePeriods[i++], 2005, TimePoint.Undefined, 6);
            AssertTimePeriod(iterator.TimePeriods[i++], 2005, TimePoint.Undefined, 7);
            AssertTimePeriod(iterator.TimePeriods[i++], 2005, TimePoint.Undefined, 8);
            AssertTimePeriod(iterator.TimePeriods[i++], 2005, TimePoint.Undefined, 9);
            AssertTimePeriod(iterator.TimePeriods[i++], 2005, TimePoint.Undefined, 10);
            AssertTimePeriod(iterator.TimePeriods[i++], 2005, TimePoint.Undefined, 11);
            AssertTimePeriod(iterator.TimePeriods[i++], 2005, TimePoint.Undefined, 12);
            AssertTimePeriod(iterator.TimePeriods[i++], 2006, TimePoint.Undefined, 1);
            AssertTimePeriod(iterator.TimePeriods[i++], 2006, TimePoint.Undefined, 2);
        }

        [TestMethod]
        public void TestQuarterly()
        {
            TimePeriodIterator iterator = new TimePeriodIterator(
                new TimePeriod { Year = 2002, YearRange = 1, Quarter = 2 },
                new TimePeriod { Year = 2004, YearRange = 1, Quarter = 3 },
                calendarYearType);

            Assert.AreEqual(10, iterator.TimePeriods.Count);

            AssertTimePeriod(iterator.TimePeriods[0], 2002, 2, TimePoint.Undefined);
            AssertTimePeriod(iterator.TimePeriods[1], 2002, 3, TimePoint.Undefined);
            AssertTimePeriod(iterator.TimePeriods[2], 2002, 4, TimePoint.Undefined);
            AssertTimePeriod(iterator.TimePeriods[3], 2003, 1, TimePoint.Undefined);
            AssertTimePeriod(iterator.TimePeriods[4], 2003, 2, TimePoint.Undefined);
            AssertTimePeriod(iterator.TimePeriods[5], 2003, 3, TimePoint.Undefined);
            AssertTimePeriod(iterator.TimePeriods[6], 2003, 4, TimePoint.Undefined);
            AssertTimePeriod(iterator.TimePeriods[7], 2004, 1, TimePoint.Undefined);
            AssertTimePeriod(iterator.TimePeriods[8], 2004, 2, TimePoint.Undefined);
            AssertTimePeriod(iterator.TimePeriods[9], 2004, 3, TimePoint.Undefined);
        }

        [TestMethod]
        public void TestQuarterlyFinancialMultiYearCumulativeQuarter()
        {
            var yearType = new YearType {Id = YearTypeIds.FinancialMultiYearCumulativeQuarter};

            TimePeriodIterator iterator = new TimePeriodIterator(
                new TimePeriod { Year = 2013, YearRange = 2, Quarter = 2 },
                new TimePeriod { Year = 2014, YearRange = 2, Quarter = 6 },
                yearType);

            Assert.AreEqual(13, iterator.TimePeriods.Count);

            AssertTimePeriod(iterator.TimePeriods[0], 2013, 2, TimePoint.Undefined);
            AssertTimePeriod(iterator.TimePeriods[1], 2013, 3, TimePoint.Undefined);
            AssertTimePeriod(iterator.TimePeriods[2], 2013, 4, TimePoint.Undefined);
            AssertTimePeriod(iterator.TimePeriods[3], 2013, 5, TimePoint.Undefined);
            AssertTimePeriod(iterator.TimePeriods[4], 2013, 6, TimePoint.Undefined);
            AssertTimePeriod(iterator.TimePeriods[5], 2013, 7, TimePoint.Undefined);
            AssertTimePeriod(iterator.TimePeriods[6], 2013, 8, TimePoint.Undefined);
            AssertTimePeriod(iterator.TimePeriods[7], 2014, 1, TimePoint.Undefined);
            AssertTimePeriod(iterator.TimePeriods[8], 2014, 2, TimePoint.Undefined);
            AssertTimePeriod(iterator.TimePeriods[9], 2014, 3, TimePoint.Undefined);
            AssertTimePeriod(iterator.TimePeriods[10], 2014, 4, TimePoint.Undefined);
            AssertTimePeriod(iterator.TimePeriods[11], 2014, 5, TimePoint.Undefined);
            AssertTimePeriod(iterator.TimePeriods[12], 2014, 6, TimePoint.Undefined);
        }

        public static void AssertTimePeriod(TimePeriod period, int year, int quarter, int month)
        {
            Assert.AreEqual(year, period.Year);
            Assert.AreEqual(quarter, period.Quarter);
            Assert.AreEqual(month, period.Month);
        }

    }
}
