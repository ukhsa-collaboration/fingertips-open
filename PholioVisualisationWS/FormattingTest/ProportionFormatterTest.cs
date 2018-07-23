
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.FormattingTest
{
    [TestClass]
    public class ProportionFormatterTest
    {
        private static IndicatorMetadata GetMetadata()
        {
            return new IndicatorMetadata { ValueTypeId = ValueTypeId.Proportion };
        }

        [TestMethod]
        public void TestFactoryMethod()
        {
            Assert.IsTrue(new NumericFormatterFactory(null).NewWithLimits(GetMetadata(), null) is ProportionFormatter);
            Assert.IsFalse(new NumericFormatterFactory(null).NewWithLimits(new IndicatorMetadata
            {
                ValueTypeId = (int)ValueTypeId.CrudeRate
            }, new IndicatorStatsPercentiles { Max = 1 }) is ProportionFormatter);
        }

        [TestMethod]
        public void TestNullStatsAreFormatted()
        {
            NumericFormatter formatter = new NumericFormatterFactory(null).NewWithLimits(GetMetadata(), null);
            IndicatorStatsPercentilesFormatted stats = formatter.FormatStats(null);
            Assert.AreEqual(NumericFormatter.NoValue, stats.Min);
            Assert.AreEqual(NumericFormatter.NoValue, stats.Max);
            Assert.AreEqual(NumericFormatter.NoValue, stats.Median);
            Assert.AreEqual(NumericFormatter.NoValue, stats.Percentile5);
            Assert.AreEqual(NumericFormatter.NoValue, stats.Percentile25);
            Assert.AreEqual(NumericFormatter.NoValue, stats.Percentile75);
            Assert.AreEqual(NumericFormatter.NoValue, stats.Percentile95);
        }

        /// <summary>
        /// Only want formatting to happen once. Otherwise may get formatting of truncated values.
        /// </summary>
        [TestMethod]
        public void TestCanOnlyFormatOnce()
        {
            CoreDataSet data = new CoreDataSet
            {
                Value = 1
            };

            // Format first time
            NumericFormatter formatter = new NumericFormatterFactory(null).NewWithLimits(GetMetadata(), null);
            formatter.Format(data);
            Assert.AreEqual("1.0", data.ValueFormatted);

            // Once formatted then cannot be reformatted
            data.Value = 2;
            formatter.Format(data);
            Assert.AreEqual("1.0", data.ValueFormatted);
        }

        [TestMethod]
        public void TestCoreDataSet1Dp()
        {
            CoreDataSet data = new CoreDataSet()
            {
                Value = 45.77777
            };

            NumericFormatter formatter = new NumericFormatterFactory(null).NewWithLimits(GetMetadata(), null);
            formatter.Format(data);

            Assert.AreEqual("45.8", data.ValueFormatted);
        }

        [TestMethod]
        public void TestNullObjectFormattedCorrectlyIfNoStats()
        {
            NumericFormatter formatter = new NumericFormatterFactory(null).NewWithLimits(GetMetadata(), null);
            CoreDataSet data = CoreDataSet.GetNullObject(null);
            formatter.Format(data);
            Assert.AreEqual(NumericFormatter.NoValue, data.ValueFormatted);
        }

        [TestMethod]
        public void TestNullObjectFormattedCorrectlyIfStats3Dp()
        {
            IndicatorStatsPercentiles statsPercentiles = new IndicatorStatsPercentiles()
            {
                Min = 0.111,
                Max = 0.111,
                Percentile5 = 0,
                Percentile25 = 0,
                Percentile75 = 0,
                Percentile95 = 0,
            };

            NumericFormatter formatter = new NumericFormatterFactory(null).NewWithLimits(GetMetadata(), statsPercentiles);
            CoreDataSet data = CoreDataSet.GetNullObject(null);
            formatter.Format(data);
            Assert.AreEqual(NumericFormatter.NoValue, data.ValueFormatted);
        }

        [TestMethod]
        public void TestNullObjectFormattedCorrectlyIfStats2Dp()
        {
            IndicatorStatsPercentiles statsPercentiles = new IndicatorStatsPercentiles()
            {
                Min = 8.22,
                Max = 8.22,
                Percentile25 = 0,
                Percentile75 = 0
            };

            NumericFormatter formatter = new NumericFormatterFactory(null).NewWithLimits(GetMetadata(), statsPercentiles);
            CoreDataSet data = CoreDataSet.GetNullObject(null);
            formatter.Format(data);
            Assert.AreEqual(NumericFormatter.NoValue, data.ValueFormatted);
        }

        [TestMethod]
        public void TestNullObjectFormattedCorrectlyIfStats1Dp()
        {
            IndicatorStatsPercentiles statsPercentiles = new IndicatorStatsPercentiles()
            {
                Min = 8.2,
                Max = 8.2,
                Percentile25 = 0,
                Percentile75 = 0
            };

            NumericFormatter formatter = new NumericFormatterFactory(null).NewWithLimits(GetMetadata(), statsPercentiles);
            CoreDataSet data = CoreDataSet.GetNullObject(null);
            formatter.Format(data);
            Assert.AreEqual(NumericFormatter.NoValue, data.ValueFormatted);
        }

        [TestMethod]
        public void TestNullObjectFormattedCorrectlyIfStats0Dp()
        {
            IndicatorStatsPercentiles statsPercentiles = new IndicatorStatsPercentiles()
            {
                Min = 1000,
                Max = 1000,
                Percentile25 = 0,
                Percentile75 = 0
            };

            NumericFormatter formatter = new NumericFormatterFactory(null).NewWithLimits(GetMetadata(), statsPercentiles);
            CoreDataSet data = CoreDataSet.GetNullObject(null);
            formatter.Format(data);
            Assert.AreEqual(NumericFormatter.NoValue, data.ValueFormatted);
        }

        [TestMethod]
        public void TestCoreDataSetZero()
        {
            CoreDataSet data = new CoreDataSet()
            {
                Value = 0
            };

            NumericFormatter formatter = new NumericFormatterFactory(null).NewWithLimits(GetMetadata(), null);
            formatter.Format(data);

            Assert.AreEqual("0.0", data.ValueFormatted);
        }

        [TestMethod]
        public void TestCoreDataSetProportion100()
        {
            CoreDataSet data = new CoreDataSet()
            {
                Value = 100
            };

            NumericFormatter formatter = new NumericFormatterFactory(null).NewWithLimits(GetMetadata(), null);
            formatter.Format(data);

            Assert.AreEqual("100", data.ValueFormatted);
        }

        [TestMethod]
        public void TestCanFormatDataWithoutStats()
        {
            CoreDataSet data = new CoreDataSet()
            {
                Value = 100
            };

            NumericFormatter formatter = new NumericFormatterFactory(null).NewWithLimits(GetMetadata(), null);
            formatter.Format(data);
        }

        [TestMethod]
        public void TestCoreDataSetValueMinusOne()
        {
            CoreDataSet data = new CoreDataSet()
            {
                Value = -1
            };

            NumericFormatter formatter = new NumericFormatterFactory(null).NewWithLimits(GetMetadata(), null);
            formatter.Format(data);
            Assert.AreEqual(NumericFormatter.NoValue, data.ValueFormatted);
        }

        [TestMethod]
        public void TestFormatIndicatorStats()
        {
            IndicatorStatsPercentiles statsPercentiles = new IndicatorStatsPercentiles()
            {
                Min = 45.11,
                Max = 89.34,
                Median = 63,
                Percentile5 = 52.765,
                Percentile25 = 54,
                Percentile75 = 75.88888,
                Percentile95 = 81
            };

            NumericFormatter formatter = new NumericFormatterFactory(null).NewWithLimits(new IndicatorMetadata { ValueTypeId = (int)ValueTypeId.Proportion }, statsPercentiles);
            IndicatorStatsPercentilesFormatted statsFormatted = formatter.FormatStats(statsPercentiles);

            Assert.AreEqual("45.1", statsFormatted.Min);
            Assert.AreEqual("89.3", statsFormatted.Max);
            Assert.AreEqual("63.0", statsFormatted.Median);
            Assert.AreEqual("52.8", statsFormatted.Percentile5);
            Assert.AreEqual("54.0", statsFormatted.Percentile25);
            Assert.AreEqual("75.9", statsFormatted.Percentile75);
            Assert.AreEqual("81.0", statsFormatted.Percentile95);
        }

        [TestMethod]
        public void TestFormatIndicatorStatsMinMaxAtExpectedLimits()
        {
            IndicatorStatsPercentiles statsPercentiles = new IndicatorStatsPercentiles()
            {
                Min = 0,
                Max = 100,
                Percentile25 = 50,
                Percentile75 = 50
            };

            NumericFormatter formatter = new NumericFormatterFactory(null).NewWithLimits(new IndicatorMetadata { ValueTypeId = (int)ValueTypeId.Proportion }, statsPercentiles);
            IndicatorStatsPercentilesFormatted statsFormatted = formatter.FormatStats(statsPercentiles);

            Assert.AreEqual("0.0", statsFormatted.Min);
            Assert.AreEqual("100", statsFormatted.Max);
        }

        [TestMethod]
        public void TestFormatConfidenceIntervals()
        {
            CoreDataSet data = new CoreDataSet()
            {
                LowerCI95 = 52.3,
                UpperCI95 = 65.3,
                LowerCI99_8 = 23.4,
                UpperCI99_8 = 67.8
            };

            NumericFormatter formatter = new NumericFormatterFactory(null).NewWithLimits(GetMetadata(), null);
            formatter.FormatConfidenceIntervals(data);

            Assert.AreEqual("52.3", data.LowerCI95F);
            Assert.AreEqual("65.3", data.UpperCI95F);
            Assert.AreEqual("23.4", data.LowerCI99_8F);
            Assert.AreEqual("67.8", data.UpperCI99_8F);
        }

        [TestMethod]
        public void TestFormatConfidenceIntervalsToleratesNull()
        {
            NumericFormatter formatter = new NumericFormatterFactory(null).NewWithLimits(GetMetadata(), null);
            formatter.FormatConfidenceIntervals(null);
        }

        [TestMethod]
        public void TestFormatProportionsLessThan1()
        {
            IndicatorStatsPercentiles statsPercentiles = new IndicatorStatsPercentiles
            {
                Max = 0.792910447761194
            };

            AssertValueAsExpected(statsPercentiles, "0.432", 0.432154729525536);
        }

        [TestMethod]
        public void TestFormatProportionsLessThan10()
        {
            IndicatorStatsPercentiles statsPercentiles = new IndicatorStatsPercentiles
            {
                Max = 4.792910447761194
            };

            AssertValueAsExpected(statsPercentiles, "2.43", 2.432154729525536);
        }

        [TestMethod]
        public void TestFormatProportionsCloseTo100()
        {
            IndicatorStatsPercentiles statsPercentiles = new IndicatorStatsPercentiles
            {
                Max = 100,
                Min = 0
            };

            AssertValueAsExpected(statsPercentiles, "100", 100);
            AssertValueAsExpected(statsPercentiles, "100", 99.9999999);
            AssertValueAsExpected(statsPercentiles, "100", 99.95);
            AssertValueAsExpected(statsPercentiles, "99.9", 99.94);
        }

        private static void AssertValueAsExpected(IndicatorStatsPercentiles statsPercentiles, string expected, double val)
        {

            NumericFormatter formatter = new NumericFormatterFactory(null).NewWithLimits(GetMetadata(), statsPercentiles);

            CoreDataSet data = new CoreDataSet
            {
                Value = val
            };
            formatter.Format(data);
            Assert.AreEqual(expected, data.ValueFormatted);
        }
    }
}
