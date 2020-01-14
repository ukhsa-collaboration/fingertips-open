using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServiceActions;

namespace PholioVisualisation.ServiceActionsTest
{
    [TestClass]
    public class PartitionDataForAllAgesBuilderTest
    {
        [TestMethod]
        public void TestGetPartitionData()
        {
            var indicatorId = IndicatorIds.LifeExpectancyAtBirth;

            var data = new PartitionDataForAllAgesBuilder().GetPartitionData(ProfileIds.Phof,
                AreaCodes.England, indicatorId, SexIds.Male, AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019);

            Assert.AreEqual(AreaCodes.England, data.AreaCode);
            Assert.AreEqual(SexIds.Male, data.SexId);
            Assert.AreEqual(indicatorId, data.IndicatorId);
            Assert.IsTrue(data.Ages.Any(), "No ages");
            CheckData(data);
        }

        [TestMethod]
        public void TestGetPartitionTrendData()
        {
            var indicatorId = IndicatorIds.LifeExpectancyAtBirth;

            var trendData = new PartitionDataForAllAgesBuilder().GetPartitionTrendData(ProfileIds.Phof,
                AreaCodes.England, indicatorId, SexIds.Male, AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019);

            Assert.IsNotNull(trendData.Limits);
            Assert.IsTrue(trendData.Labels.Select(x => x.Name).Contains("All ages"));
            Assert.IsTrue(trendData.Labels.Count < 10, "Ages should be limited to those with data");
            Assert.IsTrue(trendData.Periods.Count > 0);
            Assert.IsTrue(trendData.TrendData[AgeIds.AllAges].Count > 0, "Expected trend data");
        }

        private static void CheckData(PartitionDataForAllAges response)
        {
            Assert.IsTrue(response.Data.Any(), "No data");
            foreach (var coreDataSet in response.Data)
            {
                Assert.AreEqual(coreDataSet.IndicatorId, response.IndicatorId);
                Assert.AreEqual(coreDataSet.SexId, response.SexId);
                Assert.AreEqual(coreDataSet.AreaCode, response.AreaCode);
                Assert.IsNotNull(coreDataSet.ValueFormatted);
                Assert.IsNotNull(coreDataSet.SignificanceAgainstOneBenchmark, "Significance not set");
            }
        }
    }
}
