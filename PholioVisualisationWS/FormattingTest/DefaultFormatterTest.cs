
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.FormattingTest
{
    [TestClass]
    public class DefaultFormatterTest
    {

        [TestMethod]
        public void TestFactoryMethod()
        {
            Assert.IsTrue(DefaultFormatterWithNullLimits() is DefaultFormatter);
        }

        [TestMethod]
        public void TestCanFormatWithoutStats()
        {
            CoreDataSet data = new CoreDataSet
            {
                Value = 1
            };

            DefaultFormatterWithNullLimits().Format(data);
            Assert.AreEqual("1.00", data.ValueFormatted);
        }

        /// <summary>
        /// Only want formatting to happen once. Otherwise may get formatting of truncated values.
        /// </summary>
        [TestMethod]
        public void TestCanOnlyFormatOnce()
        {
            CoreDataSet data = new CoreDataSet
            {
                Value = 2000
            };

            var formatter = DefaultFormatterWithNullLimits();

            // Format first time
            formatter.Format(data);
            Assert.AreEqual("2000", data.ValueFormatted);

            // Once formatted then cannot be reformatted
            data.Value = 2001;
            formatter.Format(data);
            Assert.AreEqual("2000", data.ValueFormatted);
        }

        [TestMethod]
        public void TestCanFormatWithoutStats1000s()
        {
            CoreDataSet data = new CoreDataSet()
            {
                Value = 2000.12
            };

            DefaultFormatterWithNullLimits().Format(data);
            Assert.AreEqual("2000", data.ValueFormatted);
        }

        [TestMethod]
        public void TestCanFormatWithoutStats100s()
        {
            CoreDataSet data = new CoreDataSet
            {
                Value = 200.122
            };

            DefaultFormatterWithNullLimits().Format(data);
            Assert.AreEqual("200.1", data.ValueFormatted);
        }

        [TestMethod]
        public void TestCanFormatWithoutStatsLessThanOne()
        {
            CoreDataSet data = new CoreDataSet()
            {
                Value = 0.1234
            };

            DefaultFormatterWithNullLimits().Format(data);
            Assert.AreEqual("0.123", data.ValueFormatted);
        }

        [TestMethod]
        public void TestNullStatsAreFormatted()
        {
            IndicatorStatsPercentilesFormatted stats = DefaultFormatterWithNullLimits().FormatStats(null);
            Assert.AreEqual(NumericFormatter.NoValue, stats.Min);
            Assert.AreEqual(NumericFormatter.NoValue, stats.Max);
            Assert.AreEqual(NumericFormatter.NoValue, stats.Median);
            Assert.AreEqual(NumericFormatter.NoValue, stats.Percentile25);
            Assert.AreEqual(NumericFormatter.NoValue, stats.Percentile75);
        }

        [TestMethod]
        public void TestCoreDataSet1000s()
        {
            CoreDataSet data = new CoreDataSet()
            {
                Value = 4567.77777
            };

            DefaultFormatter(5000).Format(data);

            Assert.AreEqual("4568", data.ValueFormatted);
        }

        [TestMethod]
        public void TestCoreDataSet100s()
        {
            CoreDataSet data = new CoreDataSet()
            {
                Value = 467.77777
            };

            DefaultFormatter(500).Format(data);

            Assert.AreEqual("467.8", data.ValueFormatted);
        }

        [TestMethod]
        public void TestCoreDataSet10s()
        {
            CoreDataSet data = new CoreDataSet()
            {
                Value = 46.77777
            };

            DefaultFormatter(50).Format(data);

            Assert.AreEqual("46.8", data.ValueFormatted);
        }

        [TestMethod]
        public void TestCoreDataSetLessThan1()
        {
            CoreDataSet data = new CoreDataSet()
            {
                Value = 0.123
            };

            DefaultFormatter(0.5).Format(data);

            Assert.AreEqual("0.123", data.ValueFormatted);
        }

        [TestMethod]
        public void TestCoreDataSetMinus1()
        {
            CoreDataSet data = new CoreDataSet()
            {
                Value = -1
            };

            DefaultFormatter(100).Format(data);

            Assert.AreEqual(NumericFormatter.NoValue, data.ValueFormatted);
        }

        [TestMethod]
        public void TestFormatConfidenceIntervals()
        {
            CoreDataSet data = new CoreDataSet()
            {
                LowerCI95 = 467.77777,
                UpperCI95 = 643.1212,
                LowerCI99_8 = 123.3434,
                UpperCI99_8 = 345.6789
            };

            DefaultFormatter(500).FormatConfidenceIntervals(data);

            Assert.AreEqual("467.8", data.LowerCI95F);
            Assert.AreEqual("643.1", data.UpperCI95F);
            Assert.AreEqual("123.3", data.LowerCI99_8F);
            Assert.AreEqual("345.7", data.UpperCI99_8F);
        }

        [TestMethod]
        public void TestFormatConfidenceIntervalsToleratesNull()
        {
            DefaultFormatter(1).FormatConfidenceIntervals(null);
        }

        private DefaultFormatter DefaultFormatter(double max)
        {
            var metadata = new IndicatorMetadata {ValueTypeId = ValueTypeId.Undefined};
            var percentiles = new IndicatorStatsPercentiles { Max = max };
            return new NumericFormatterFactory(null).NewWithLimits(metadata, percentiles) as DefaultFormatter;
        }

        private DefaultFormatter DefaultFormatterWithNullLimits()
        {
            var metadata = new IndicatorMetadata { ValueTypeId = ValueTypeId.Undefined };
            return new NumericFormatterFactory(null).NewWithLimits(metadata, null) as DefaultFormatter;
        }
    }
}
