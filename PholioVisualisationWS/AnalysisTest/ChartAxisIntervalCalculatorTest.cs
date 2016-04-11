using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.AnalysisTest
{
    [TestClass]
    public class ChartAxisIntervalCalculatorTest
    {
        [TestMethod]
        public void TestWhereIntervalExactly1000()
        {
            Assert.AreEqual(200, Step(0, 1000));
        }

        [TestMethod]
        public void TestWhereIntervalJustUnder1000()
        {
            Assert.AreEqual(100, Step(200, 1000));
        }

        [TestMethod]
        public void TestWhereIntervalExactly100()
        {
            Assert.AreEqual(20, Step(0, 100));
        }

        [TestMethod]
        public void TestWhereIntervalJustOver100()
        {
            Assert.AreEqual(20, Step(20, 140));
        }

        [TestMethod]
        public void TestWhereIntervalJustUnder100()
        {
            Assert.AreEqual(10, Step(20, 100));
        }

        [TestMethod]
        public void TestWhereIntervalExactly50()
        {
            Assert.AreEqual(10, Step(0, 50));
        }

        private static double Step(double min, double max)
        {
            return new ChartAxisIntervalCalculator(new Limits
            {
                Min = min,
                Max = max
            }).Step.Value;
        }
    }
}
