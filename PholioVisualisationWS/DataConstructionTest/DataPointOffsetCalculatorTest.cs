
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class DataPointOffsetCalculatorTest
    {
        YearType calendarYearType = new YearType
        {
            Id = YearTypeIds.Calendar
        };

        [TestMethod]
        public void TestZeroOffset()
        {
            DataPointOffsetCalculator calc = new DataPointOffsetCalculator(
                new Grouping
                {
                    DataPointYear = 2010,
                    BaselineYear = 2009,
                    YearRange = 1
                }, 0, calendarYearType);

            Assert.AreEqual(2010, calc.TimePeriod.Year);
            Assert.AreEqual(1, calc.TimePeriod.YearRange);
        }

        [TestMethod]
        public void TestBaselineCannotBeOvershot()
        {
            DataPointOffsetCalculator calc = new DataPointOffsetCalculator(
                new Grouping
                {
                    DataPointYear = 2010,
                    BaselineYear = 2009,
                    YearRange = 1
                }, 100, calendarYearType);

            Assert.AreEqual(DataPointOffsetCalculator.InvalidYear, calc.TimePeriod.Year);
        }

        [TestMethod]
        public void TestBaselineCannotBeOvershotIdenticalBaselineAndDatapoint()
        {
            DataPointOffsetCalculator calc = new DataPointOffsetCalculator(
                new Grouping
                {
                    DataPointYear = 2010,
                    BaselineYear = 2010,
                    YearRange = 1
                }, 1, calendarYearType);

            Assert.AreEqual(DataPointOffsetCalculator.InvalidYear, calc.TimePeriod.Year);
        }

        [TestMethod]
        public void TestDataPointCannotBeExceeded()
        {
            DataPointOffsetCalculator calc = new DataPointOffsetCalculator(
                new Grouping
                {
                    DataPointYear = 2010,
                    BaselineYear = 2009,
                    YearRange = 1
                }, -100, calendarYearType);

            Assert.AreEqual(2010, calc.TimePeriod.Year);
            Assert.AreEqual(1, calc.TimePeriod.YearRange);
        }

        [TestMethod]
        public void TestYearOffset()
        {
            DataPointOffsetCalculator calc = new DataPointOffsetCalculator(
                new Grouping
                {
                    DataPointYear = 2010,
                    BaselineYear = 2009,
                    YearRange = 1
                }, 1, calendarYearType);

            Assert.AreEqual(2009, calc.TimePeriod.Year);
            Assert.AreEqual(1, calc.TimePeriod.YearRange);
        }

        [TestMethod]
        public void TestQuarterOffset()
        {
            DataPointOffsetCalculator calc = new DataPointOffsetCalculator(
                new Grouping
                {
                    DataPointYear = 2010,
                    DataPointQuarter = 3,
                    BaselineYear = 2009,
                    BaselineQuarter = 1
                }, 1, calendarYearType);

            Assert.AreEqual(2010, calc.TimePeriod.Year);
            Assert.AreEqual(2, calc.TimePeriod.Quarter);
        }

        [TestMethod]
        public void TestMonthOffset()
        {
            DataPointOffsetCalculator calc = new DataPointOffsetCalculator(
                new Grouping
                {
                    DataPointYear = 2010,
                    DataPointMonth = 3,
                    BaselineYear = 2009,
                    BaselineMonth = 1
                }, 1, calendarYearType);

            Assert.AreEqual(2010, calc.TimePeriod.Year);
            Assert.AreEqual(2, calc.TimePeriod.Month);
        }
    }
}
