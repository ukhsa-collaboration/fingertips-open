
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.Analysis;

namespace PholioVisualisation.AnalysisTest
{
    [TestClass]
    public class IndicatorStatsCalculatorTest
    {
        [TestMethod]
        public void TestGetStatsFor50To60()
        {
            var min = 50;
            var max = 60;
            var values = GetValues(min, max);

            IndicatorStatsCalculator calc = new IndicatorStatsCalculator(values);
            IndicatorStatsPercentiles statsPercentiles = calc.GetStats();
            Assert.AreEqual(min, statsPercentiles.Min);
            Assert.AreEqual(max, statsPercentiles.Max);
            Assert.AreEqual(52.5, statsPercentiles.Percentile25);
            Assert.AreEqual(57.5, statsPercentiles.Percentile75);
        }

        [TestMethod]
        public void TestGetStatsFor1To100()
        {
            var min = 1;
            var max = 100;
            var data = GetValues(min, max);

            IndicatorStatsCalculator calc = new IndicatorStatsCalculator(data);
            IndicatorStatsPercentiles statsPercentiles = calc.GetStats();
            Assert.AreEqual(min, statsPercentiles.Min);
            Assert.AreEqual(max, statsPercentiles.Max);
            Assert.AreEqual(25.75, statsPercentiles.Percentile25);
            Assert.AreEqual(75.25, statsPercentiles.Percentile75);
        }

        private static List<double> GetValues(int min, int max)
        {
            var data = new List<double>();
            for (int i = min; i <= max; i++)
            {
                data.Add(i);
            }
            return data;
        }

        [TestMethod]
        public void TestGetStatsFor1To17()
        {
            var min = 1;
            var max = 17;
            var values = GetValues(min, max);

            IndicatorStatsCalculator calc = new IndicatorStatsCalculator(values);
            IndicatorStatsPercentiles statsPercentiles = calc.GetStats();
            Assert.AreEqual(min, statsPercentiles.Min);
            Assert.AreEqual(max, statsPercentiles.Max);
            Assert.AreEqual(5, statsPercentiles.Percentile25);
            Assert.AreEqual(13, statsPercentiles.Percentile75);
        }

        [TestMethod]
        public void TestStatsReturnedNullIfNoData()
        {
            IndicatorStatsCalculator calc = new IndicatorStatsCalculator(new List<double>());
            Assert.IsNull(calc.GetStats());
        }

        [TestMethod]
        public void TestStatsReturnedNullIfOneDataPoint()
        {
            var values = new List<double>();
            values.Add(4);

            IndicatorStatsCalculator calc = new IndicatorStatsCalculator(values);
            Assert.IsNull(calc.GetStats());
        }

        [TestMethod]
        public void TestStatsNullIfTwoPoints()
        {
            var values = new List<double>();
            values.Add(4);
            values.Add(5);

            IndicatorStatsCalculator calc = new IndicatorStatsCalculator(values);
            Assert.IsNull(calc.GetStats());
        }

        [TestMethod]
        public void TestStatsNullIfThreePoints()
        {
            var data = new List<double>();
            data.Add(4);
            data.Add(5);
            data.Add(6);

            IndicatorStatsCalculator calc = new IndicatorStatsCalculator(data);
            Assert.IsNull(calc.GetStats());
        }

        [TestMethod]
        public void TestStatsNotNullIfFourPoints()
        {
            var data = new List<double>();
            data.Add(4);
            data.Add(5);
            data.Add(6);
            data.Add(6);

            IndicatorStatsCalculator calc = new IndicatorStatsCalculator(data);
            Assert.IsNotNull(calc.GetStats());
        }

        [TestMethod]
        public void TestStatsWithoutPercentiles()
        {
            var data = new List<double>();
            data.Add(4);
            data.Add(5);
            data.Add(6);

            IndicatorStatsCalculator calc = new IndicatorStatsCalculator(data);
            Assert.AreEqual(6, calc.GetStatsWithoutPercentiles().Max);
            Assert.AreEqual(4, calc.GetStatsWithoutPercentiles().Min);
        }

        [TestMethod]
        public void TestStatsWithoutPercentilesNullIfNoData()
        {
            IndicatorStatsCalculator calc = new IndicatorStatsCalculator(new List<double>());
            Assert.IsNull(calc.GetStatsWithoutPercentiles());
        }

        [TestMethod]
        public void TestGetStatsWithDoubleList()
        {
            List<double> data = new List<double>();
            double val = 50;
            for (int i = 0; i < 11; i++)
            {
                data.Add(val);
                val += 1;
            }

            // 50-60
            IndicatorStatsCalculator calc = new IndicatorStatsCalculator(data);
            IndicatorStatsPercentiles statsPercentiles = calc.GetStats();
            Assert.AreEqual(50, statsPercentiles.Min);
            Assert.AreEqual(60, statsPercentiles.Max);
            Assert.AreEqual(52.5, statsPercentiles.Percentile25);
            Assert.AreEqual(57.5, statsPercentiles.Percentile75);
        }

    }
}
