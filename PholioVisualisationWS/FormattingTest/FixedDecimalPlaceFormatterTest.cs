using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.FormattingTest
{
    [TestClass]
    public class FixedDecimalPlaceFormatterTest
    {
        private static IndicatorMetadata GetMetadata(int decimalPlacesDisplayed)
        {
            return new IndicatorMetadata { DecimalPlacesDisplayed = decimalPlacesDisplayed };
        }

        [TestMethod]
        public void TestFactoryMethod()
        {
            Assert.IsTrue(NumericFormatterFactory.NewWithLimits(GetMetadata(1), null) is FixedDecimalPlaceFormatter);
        }

        [TestMethod]
        public void TestFactoryMethod_FixedDecimalPlaceFormatterNotReturnedIfDecimalPlacesLessThanZero()
        {
            Assert.IsFalse(NumericFormatterFactory.NewWithLimits(GetMetadata(-1), null) is FixedDecimalPlaceFormatter);
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
            NumericFormatter formatter = NumericFormatterFactory.NewWithLimits(GetMetadata(0), null);
            formatter.Format(data);
            Assert.AreEqual("1", data.ValueFormatted);

            // Once formatted then cannot be reformatted
            data.Value = 2;
            formatter.Format(data);
            Assert.AreEqual("1", data.ValueFormatted);
        }

        [TestMethod]
        public void TestFormat()
        {
            var number = 1.11111111111;
            Assert.AreEqual("1", FormatValue(number, 0));
            Assert.AreEqual("1.1", FormatValue(number, 1));
            Assert.AreEqual("1.11", FormatValue(number, 2));
            Assert.AreEqual("1.111", FormatValue(number, 3));
            Assert.AreEqual("1.1111", FormatValue(number, 4));
            Assert.AreEqual("1.11111", FormatValue(number, 5));
        }

        [TestMethod]
        public void TestFormat_NullValue()
        {
            Assert.AreEqual(NumericFormatter.NoValue, FormatValue(ValueData.NullValue, 0));
        }

        [TestMethod]
        public void TestFormat_BankersRoundingNotApplied()
        {
            for (double val = 0; val < 4; val += 0.1)
            {
                var expected = Math.Round(val, 0, MidpointRounding.AwayFromZero);

                Assert.AreEqual(expected.ToString(), FormatValue(val, 0));
            }
        }
 
        [TestMethod]
        public void TestFormatStats()
        {
            var formatter = NumericFormatterFactory.NewWithLimits(GetMetadata(1), null);
            var stats = new IndicatorStatsPercentiles
            {
                Min = 1.111111,
                Max = 2.222222,
                Median = 3.33333,
                Percentile5 = 4.4444444,
                Percentile25 = 6.666666,
                Percentile75 = 7.777777,
                Percentile95 = 5.555555
            };
            var statsF = formatter.FormatStats(stats);

            Assert.AreEqual("1.1", statsF.Min);
            Assert.AreEqual("2.2", statsF.Max);
            Assert.AreEqual("3.3", statsF.Median);
            Assert.AreEqual("4.4", statsF.Percentile5);
            Assert.AreEqual("6.7", statsF.Percentile25);
            Assert.AreEqual("7.8", statsF.Percentile75);
            Assert.AreEqual("5.6", statsF.Percentile95);
        }

        [TestMethod]
        public void TestFormatConfidenceIntervals()
        {
            var formatter = NumericFormatterFactory.NewWithLimits(GetMetadata(1), null);
            var data = new ValueWithCIsData
            {
                LowerCI = 1.1111,
                UpperCI = 9.9999
            };
            formatter.FormatConfidenceIntervals(data);

            Assert.AreEqual("1.1", data.LowerCIF);
            Assert.AreEqual("10.0", data.UpperCIF);
        }

        [TestMethod]
        public void TestFormatConfidenceIntervals_NullValues()
        {
            var formatter = NumericFormatterFactory.NewWithLimits(GetMetadata(1), null);
            var data = new ValueWithCIsData
            {
                LowerCI = ValueData.NullValue,
                UpperCI = ValueData.NullValue
            };
            formatter.FormatConfidenceIntervals(data);

            Assert.AreEqual(NumericFormatter.NoValue, data.LowerCIF);
            Assert.AreEqual(NumericFormatter.NoValue, data.UpperCIF);
        }

        private string FormatValue(double val, int decimalPlacesDisplayed)
        {
            var formatter = NumericFormatterFactory.NewWithLimits(GetMetadata(decimalPlacesDisplayed), null);
            var data = new ValueData { Value = val };
            formatter.Format(data);
            return data.ValueFormatted;
        }
    }
}
