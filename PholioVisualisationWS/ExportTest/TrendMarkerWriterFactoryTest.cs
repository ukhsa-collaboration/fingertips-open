using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;
using PholioVisualisation.Export;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ExportTest
{
    [TestClass]
    public class TrendMarkerWriterFactoryTest
    {
        public const int TimePeriodThreshold = TrendMarkerCalculator.MinimumNumberOfPoints;

        [TestMethod]
        public void Test_Null_Writer_Returned_If_Not_Have_Trendmarkers()
        {
            var trendMarkerWriter = TrendMarkerWriterFactory.New(null, PolarityIds.NotApplicable, GetTimePeriods(TimePeriodThreshold), false);

            Assert.IsTrue(trendMarkerWriter is NullTrendMarkerWriter);
        }

        [TestMethod]
        public void Test_Null_Writer_Returned_If_Not_Enough_Time_Periods()
        {
            var trendMarkerWriter = TrendMarkerWriterFactory.New(null, PolarityIds.NotApplicable, GetTimePeriods(TimePeriodThreshold - 1), true);

            Assert.IsTrue(trendMarkerWriter is NullTrendMarkerWriter);
        }

        [TestMethod]
        public void Test_Writer_Returned_If_Has_Trendmarkers_And_Enough_Time_Periods()
        {
            var trendMarkerWriter = TrendMarkerWriterFactory.New(null, PolarityIds.NotApplicable, GetTimePeriods(TimePeriodThreshold), true);

            Assert.IsTrue(trendMarkerWriter is TrendMarkerWriter);
        }

        private static IList<TimePeriod> GetTimePeriods(int count)
        {
            var timePeriods = new List<TimePeriod>();
            for (int i = 0; i < count; i++)
            {
                timePeriods.Add(new TimePeriod());
            }
            return timePeriods;
        }
    }
}
