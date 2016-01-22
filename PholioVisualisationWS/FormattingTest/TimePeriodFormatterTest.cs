
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace FormattingTest
{
    [TestClass]
    public class TimePeriodFormatterTest
    {
        [TestMethod]
        public void TestYearType1Calendar()
        {
            IndicatorMetadata i = new IndicatorMetadata { YearTypeId = 1 };

            // Year Range
            TestFormat("2006", 2006, 1, -1, i);
            TestFormat("2006 - 07", 2006, 2, -1, i);
            TestFormat("2006 - 08", 2006, 3, -1, i);

            // Quarter 
            TestFormat("2006 Q1", 2006, 1, 1, i);
            TestFormat("2006 Q4", 2006, 1, 4, i);
        }

        [TestMethod]
        public void TestYearType2Fiscal()
        {
            IndicatorMetadata i = new IndicatorMetadata { YearTypeId = 2 };

            // Year Range
            TestFormat("2006/07", 2006, 1, -1, i);
            TestFormat("2006/07 - 07/08", 2006, 2, -1, i);
            TestFormat("2006/07 - 08/09", 2006, 3, -1, i);

            // Quarter 
            TestFormat("2006/07 Q1", 2006, 1, 1, i);
            TestFormat("2006/07 Q4", 2006, 1, 4, i);
        }

        [TestMethod]
        public void TestYearType3Academic()
        {
            IndicatorMetadata i = new IndicatorMetadata { YearTypeId = 3 };

            // Year Range
            TestFormat("2006/07", 2006, 1, -1, i);
            TestFormat("2006/07 - 07/08", 2006, 2, -1, i);
            TestFormat("2006/07 - 08/09", 2006, 3, -1, i);

            // Quarter 
            TestFormat("2006/07 Q1", 2006, 1, 1, i);
            TestFormat("2006/07 Q4", 2006, 1, 4, i);
        }

        [TestMethod]
        public void TestYearType4FinancialRollingYearQuarterly()
        {
            IndicatorMetadata i = new IndicatorMetadata { YearTypeId = 4 };

            // Year Range Q1
            TestFormat("2006/07 Q1 - 2006/07 Q4", 2006, 1, 1, i);
            TestFormat("2006/07 Q1 - 2007/08 Q4", 2006, 2, 1, i);
            TestFormat("2006/07 Q1 - 2008/09 Q4", 2006, 3, 1, i);

            // Year Range Q2
            TestFormat("2006/07 Q2 - 2007/08 Q1", 2006, 1, 2, i);
            TestFormat("2006/07 Q2 - 2008/09 Q1", 2006, 2, 2, i);
            TestFormat("2006/07 Q2 - 2009/10 Q1", 2006, 3, 2, i);

            // Year Range Q3
            TestFormat("2006/07 Q3 - 2007/08 Q2", 2006, 1, 3, i);
            TestFormat("2006/07 Q3 - 2008/09 Q2", 2006, 2, 3, i);
            TestFormat("2006/07 Q3 - 2009/10 Q2", 2006, 3, 3, i);

            // Year Range Q4
            TestFormat("2006/07 Q4 - 2007/08 Q3", 2006, 1, 4, i);
            TestFormat("2006/07 Q4 - 2008/09 Q3", 2006, 2, 4, i);
            TestFormat("2006/07 Q4 - 2009/10 Q3", 2006, 3, 4, i);
        }

        [TestMethod]
        public void TestYearType5CalendarRollingYearQuarterly()
        {
            IndicatorMetadata i = new IndicatorMetadata { YearTypeId = 5 };

            // Year Range Q1
            TestFormat("2006 Q1 - 2006 Q4", 2006, 1, 1, i);
            TestFormat("2006 Q1 - 2007 Q4", 2006, 2, 1, i);
            TestFormat("2006 Q1 - 2008 Q4", 2006, 3, 1, i);

            // Year Range Q2
            TestFormat("2006 Q2 - 2007 Q1", 2006, 1, 2, i);
            TestFormat("2006 Q2 - 2008 Q1", 2006, 2, 2, i);
            TestFormat("2006 Q2 - 2009 Q1", 2006, 3, 2, i);

            // Year Range Q3
            TestFormat("2006 Q3 - 2007 Q2", 2006, 1, 3, i);
            TestFormat("2006 Q3 - 2008 Q2", 2006, 2, 3, i);
            TestFormat("2006 Q3 - 2009 Q2", 2006, 3, 3, i);

            // Year Range Q4
            TestFormat("2006 Q4 - 2007 Q3", 2006, 1, 4, i);
            TestFormat("2006 Q4 - 2008 Q3", 2006, 2, 4, i);
            TestFormat("2006 Q4 - 2009 Q3", 2006, 3, 4, i);
        }

        [TestMethod]
        public void TestYearType7FinancialCumulativeQuarters()
        {
            IndicatorMetadata i = new IndicatorMetadata { YearTypeId = 7 };

            TestFormat("2006/07 Q1", 2006, 1, 1, i);
            TestFormat("2006/07 Q1-Q2", 2006, 1, 2, i);
            TestFormat("2006/07 Q1-Q3", 2006, 1, 3, i);
            TestFormat("2006/07 Q1-Q4", 2006, 1, 4, i);
        }

        [TestMethod]
        public void TestYearType8JulToAug()
        {
            IndicatorMetadata i = new IndicatorMetadata { YearTypeId = 8 };

            // Year Range
            TestFormat("Aug 2006 - Jul 2007", 2006, 1, -1, i);
            TestFormat("Aug 2006 - Jul 2008", 2006, 2, -1, i);
            TestFormat("Aug 2006 - Jul 2009", 2006, 3, -1, i);
        }

        [TestMethod]
        public void TestYearType9MarToFeb()
        {
            IndicatorMetadata i = new IndicatorMetadata { YearTypeId = 9 };

            // Year Range
            TestFormat("Mar 2006 - Feb 2007", 2006, 1, -1, i);
            TestFormat("Mar 2006 - Feb 2008", 2006, 2, -1, i);
            TestFormat("Mar 2006 - Feb 2009", 2006, 3, -1, i);
        }

        [TestMethod]
        public void TestYearType10FinancialMultiYearCumulativeQuarters()
        {
            IndicatorMetadata i = new IndicatorMetadata
            {
                YearTypeId = YearTypeIds.FinancialMultiYearCumulativeQuarter
            };
            const int yearRange = 5;

            // Year Range
            TestFormat("2013/14 Q1", 2013, yearRange, 1, i);
            TestFormat("2013/14 Q1 - 2013/14 Q2", 2013, yearRange, 2, i);
            TestFormat("2013/14 Q1 - 2014/15 Q1", 2013, yearRange, 5, i);
            TestFormat("2013/14 Q1 - 2014/15 Q4", 2013, yearRange, 8, i);
        }

        [TestMethod]
        public void TestMonthlyCalendar()
        {
            const int year = 2009;
            const int yearType = 1;
            int month = 1;
            TestFormatMonthly("Jan 2009", month++, year, yearType);
            TestFormatMonthly("Feb 2009", month++, year, yearType);
            TestFormatMonthly("Mar 2009", month++, year, yearType);
            TestFormatMonthly("Apr 2009", month++, year, yearType);
            TestFormatMonthly("May 2009", month++, year, yearType);
            TestFormatMonthly("Jun 2009", month++, year, yearType);
            TestFormatMonthly("Jul 2009", month++, year, yearType);
            TestFormatMonthly("Aug 2009", month++, year, yearType);
            TestFormatMonthly("Sep 2009", month++, year, yearType);
            TestFormatMonthly("Oct 2009", month++, year, yearType);
            TestFormatMonthly("Nov 2009", month++, year, yearType);
            TestFormatMonthly("Dec 2009", month, year, yearType);
        }

        [TestMethod]
        public void TestMonthlyCalendarRollingYear()
        {
            const int year = 2009;
            const int yearType = 6;
            int month = 1;
            TestFormatMonthly("Jan 2009 - Dec 2009", month++, year, yearType);
            TestFormatMonthly("Feb 2009 - Jan 2010", month++, year, yearType);
            TestFormatMonthly("Mar 2009 - Feb 2010", month++, year, yearType);
            TestFormatMonthly("Apr 2009 - Mar 2010", month++, year, yearType);
            TestFormatMonthly("May 2009 - Apr 2010", month++, year, yearType);
            TestFormatMonthly("Jun 2009 - May 2010", month++, year, yearType);
            TestFormatMonthly("Jul 2009 - Jun 2010", month++, year, yearType);
            TestFormatMonthly("Aug 2009 - Jul 2010", month++, year, yearType);
            TestFormatMonthly("Sep 2009 - Aug 2010", month++, year, yearType);
            TestFormatMonthly("Oct 2009 - Sep 2010", month++, year, yearType);
            TestFormatMonthly("Nov 2009 - Oct 2010", month++, year, yearType);
            TestFormatMonthly("Dec 2009 - Nov 2010", month, year, yearType);
        }

        [TestMethod]
        public void TestMonthlyFiscalAndFinancial()
        {
            int[] yearTypes = { 2, 3 };
            foreach (int yearType in yearTypes)
            {
                const int year = 2009;
                int month = 1;
                TestFormatMonthly("Apr 2009", month++, year, yearType);
                TestFormatMonthly("May 2009", month++, year, yearType);
                TestFormatMonthly("Jun 2009", month++, year, yearType);
                TestFormatMonthly("Jul 2009", month++, year, yearType);
                TestFormatMonthly("Aug 2009", month++, year, yearType);
                TestFormatMonthly("Sep 2009", month++, year, yearType);
                TestFormatMonthly("Oct 2009", month++, year, yearType);
                TestFormatMonthly("Nov 2009", month++, year, yearType);
                TestFormatMonthly("Dec 2009", month++, year, yearType);
                TestFormatMonthly("Jan 2010", month++, year, yearType);
                TestFormatMonthly("Feb 2010", month++, year, yearType);
                TestFormatMonthly("Mar 2010", month, year, yearType);
            }
        }

        [TestMethod]
        public void TestMonthlyInvalid()
        {
            TestFormatMonthly("13 2009", 13, 2009, 1);
        }

        [TestMethod]
        public void TestSpecifiedAndDataPointFormatters()
        {
            IndicatorMetadata i = new IndicatorMetadata { YearTypeId = 1 };
            Grouping g = new Grouping { DataPointYear = 2001, YearRange = 1 };

            new DataPointTimePeriodFormatter().Format(g, i);
            Assert.AreEqual("2001", g.TimePeriodText);

            new SpecifiedTimePeriodFormatter { TimePeriod = new TimePeriod { Year = 2002, YearRange = 1 } }.Format(g, i);
            Assert.AreEqual("2002", g.TimePeriodText);
        }

        private static void TestFormat(string expected, int year, int yearRange, int quarter, IndicatorMetadata metadata)
        {
            Grouping g = new Grouping { DataPointYear = year, DataPointQuarter = quarter, YearRange = yearRange };

            new DataPointTimePeriodFormatter().Format(g, metadata);
            Assert.AreEqual(expected, g.TimePeriodText);
        }

        private static void TestFormatMonthly(string expected, int month, int year, int yearType)
        {
            Grouping g = new Grouping { DataPointMonth = month, DataPointYear = year };
            new DataPointTimePeriodFormatter().Format(g, new IndicatorMetadata { YearTypeId = yearType });
            Assert.AreEqual(expected, g.TimePeriodText);
        }
    }
}
