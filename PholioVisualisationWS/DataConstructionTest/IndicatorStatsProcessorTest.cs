
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class IndicatorStatsProcessorTest
    {
        [TestMethod]
        public void TestPostProcessIndicatorStatsToLowerJsonFootPrint()
        {
            IndicatorStatsPercentiles statsPercentiles = new IndicatorStatsPercentiles
            {
                Min = 0.123456,
                Max = 1.111111111111,
                Percentile25 = 50,
                Percentile75 = 50.22222222222222
            };

            new IndicatorStatsProcessor().Truncate(statsPercentiles);

            Assert.AreEqual(0.12346, statsPercentiles.Min);
            Assert.AreEqual(1.11111, statsPercentiles.Max);
            Assert.AreEqual(50, statsPercentiles.Percentile25);
            Assert.AreEqual(50.22222, statsPercentiles.Percentile75);
        }

        [TestMethod]
        public void TestPostProcessIndicatorStatsToLowerJsonFootPrintNullIgnored()
        {
            new IndicatorStatsProcessor().Truncate(null);
        }
    }
}
