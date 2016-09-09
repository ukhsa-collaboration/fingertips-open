using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServiceActions;

namespace PholioVisualisation.ServiceActionsTest
{
    [TestClass]
    public class PartitionDataForAllSexesBuilderTest
    {
        [TestMethod]
        public void TestGetPartitionDataWhereNoPersonsData()
        {
            var indicatorId = IndicatorIds.LifeExpectancyAtBirth;

            var data = GetPartitionData(indicatorId);

            Assert.AreEqual(AreaCodes.England, data.AreaCode);
            Assert.AreEqual(AgeIds.AllAges, data.AgeId);
            Assert.AreEqual(indicatorId, data.IndicatorId);
            Assert.IsTrue(data.Sexes.Any(), "No sexes");
            CheckData(data);

            // Check significance is none because person data is not available
            foreach (var coreDataSet in data.Data)
            {
                Assert.AreEqual((int)Significance.None, coreDataSet.SignificanceAgainstOneBenchmark);
            }
        }

        [TestMethod]
        public void TestGetPartitionDataWhereIsPersonsData()
        {
            var indicatorId = IndicatorIds.MortalityRateFromCausesConsideredPreventable;

            var data = GetPartitionData(indicatorId);

            Assert.AreEqual(AreaCodes.England, data.AreaCode);
            Assert.AreEqual(AgeIds.AllAges, data.AgeId);
            Assert.AreEqual(indicatorId, data.IndicatorId);
            Assert.IsTrue(data.Sexes.Any(), "No sexes");
            CheckData(data);

            // Check significance is set because person data is available
            foreach (var coreDataSet in data.Data)
            {
                Assert.AreNotEqual((int)Significance.None, coreDataSet.SignificanceAgainstOneBenchmark);
            }
        }

        [TestMethod]
        public void TestGetPartitionTrendData()
        {
            var indicatorId = IndicatorIds.MortalityRateFromCausesConsideredPreventable;

            var trendData = new PartitionDataForAllSexesBuilder().GetPartitionTrendData(ProfileIds.Phof,
                AreaCodes.England, indicatorId, AgeIds.AllAges, AreaTypeIds.CountyAndUnitaryAuthority);

            Assert.IsNotNull(trendData.Limits);
            Assert.IsTrue(trendData.Labels.Select(x=>x.Name).Contains("Male"));
            Assert.IsTrue(trendData.Periods.Count > 0);
            Assert.IsTrue(trendData.TrendData[SexIds.Male].Count > 0, "Expected trend data");
        }

        private static PartitionDataForAllSexes GetPartitionData(int indicatorId)
        {
            var response = new PartitionDataForAllSexesBuilder().GetPartitionData(ProfileIds.Phof,
                AreaCodes.England, indicatorId, AgeIds.AllAges, AreaTypeIds.CountyAndUnitaryAuthority);
            return response;
        }

        private static void CheckData(PartitionDataForAllSexes response)
        {
            Assert.IsTrue(response.Data.Any(), "No data");
            foreach (var coreDataSet in response.Data)
            {
                Assert.AreEqual(coreDataSet.IndicatorId, response.IndicatorId);
                Assert.AreEqual(coreDataSet.AgeId, response.AgeId);
                Assert.AreEqual(coreDataSet.AreaCode, response.AreaCode);
                Assert.IsNotNull(coreDataSet.ValueFormatted);
            }
        }
    }
}
