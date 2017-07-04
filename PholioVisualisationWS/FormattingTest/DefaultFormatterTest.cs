
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
        private static IndicatorMetadata GetMetadata()
        {
            return new IndicatorMetadata { ValueTypeId = (int)ValueTypeId.Undefined };
        }

        private static IndicatorStatsPercentiles GetStats(double max)
        {
            return new IndicatorStatsPercentiles { Max = max };
        }

        [TestMethod]
        public void TestFactoryMethod()
        {
            Assert.IsTrue(NumericFormatterFactory.NewWithLimits(GetMetadata(), GetStats(1)) is DefaultFormatter);
        }

        [TestMethod]
        public void TestCanFormatWithoutStats()
        {
            CoreDataSet data = new CoreDataSet
            {
                Value = 1
            };

            NumericFormatter formatter = NumericFormatterFactory.NewWithLimits(GetMetadata(), null);
            formatter.Format(data);
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

            // Format first time
            NumericFormatter formatter = NumericFormatterFactory.NewWithLimits(GetMetadata(), null);
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

            NumericFormatter formatter = NumericFormatterFactory.NewWithLimits(GetMetadata(), null);
            formatter.Format(data);
            Assert.AreEqual("2000", data.ValueFormatted);
        }

        [TestMethod]
        public void TestCanFormatWithoutStats100s()
        {
            CoreDataSet data = new CoreDataSet
            {
                Value = 200.122
            };

            NumericFormatter formatter = NumericFormatterFactory.NewWithLimits(GetMetadata(), null);
            formatter.Format(data);
            Assert.AreEqual("200.1", data.ValueFormatted);
        }

        [TestMethod]
        public void TestCanFormatWithoutStatsLessThanOne()
        {
            CoreDataSet data = new CoreDataSet()
            {
                Value = 0.1234
            };

            NumericFormatter formatter = NumericFormatterFactory.NewWithLimits(GetMetadata(), null);
            formatter.Format(data);
            Assert.AreEqual("0.123", data.ValueFormatted);
        }

        [TestMethod]
        public void TestNullStatsAreFormatted()
        {
            NumericFormatter formatter = NumericFormatterFactory.NewWithLimits(GetMetadata(), null);
            IndicatorStatsPercentilesFormatted stats = formatter.FormatStats(null);
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

            NumericFormatter formatter = NumericFormatterFactory.NewWithLimits(GetMetadata(), GetStats(5000));
            formatter.Format(data);

            Assert.AreEqual("4568", data.ValueFormatted);
        }

        [TestMethod]
        public void TestCoreDataSet100s()
        {
            CoreDataSet data = new CoreDataSet()
            {
                Value = 467.77777
            };

            NumericFormatter formatter = NumericFormatterFactory.NewWithLimits(GetMetadata(), GetStats(500));
            formatter.Format(data);

            Assert.AreEqual("467.8", data.ValueFormatted);
        }

        [TestMethod]
        public void TestCoreDataSet10s()
        {
            CoreDataSet data = new CoreDataSet()
            {
                Value = 46.77777
            };

            NumericFormatter formatter = NumericFormatterFactory.NewWithLimits(GetMetadata(), GetStats(50));
            formatter.Format(data);

            Assert.AreEqual("46.8", data.ValueFormatted);
        }

        [TestMethod]
        public void TestCoreDataSetLessThan1()
        {
            CoreDataSet data = new CoreDataSet()
            {
                Value = 0.123
            };

            NumericFormatter formatter = NumericFormatterFactory.NewWithLimits(GetMetadata(), GetStats(0.5));
            formatter.Format(data);

            Assert.AreEqual("0.123", data.ValueFormatted);
        }

        [TestMethod]
        public void TestCoreDataSetMinus1()
        {
            CoreDataSet data = new CoreDataSet()
            {
                Value = -1
            };

            NumericFormatter formatter = NumericFormatterFactory.NewWithLimits(GetMetadata(), GetStats(100));
            formatter.Format(data);

            Assert.AreEqual(NumericFormatter.NoValue, data.ValueFormatted);
        }

        [TestMethod]
        public void TestFormatConfidenceIntervals()
        {
            CoreDataSet data = new CoreDataSet()
            {
                LowerCI = 467.77777,
                UpperCI = 643.1212
            };

            NumericFormatter formatter = NumericFormatterFactory.NewWithLimits(GetMetadata(), GetStats(500));
            formatter.FormatConfidenceIntervals(data);

            Assert.AreEqual("467.8", data.LowerCIF);
            Assert.AreEqual("643.1", data.UpperCIF);
        }

        [TestMethod]
        public void TestFormatConfidenceIntervalsToleratesNull()
        {
            NumericFormatter formatter = NumericFormatterFactory.NewWithLimits(GetMetadata(), GetStats(500));
            formatter.FormatConfidenceIntervals(null);
        }
    }
}
