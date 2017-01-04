using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServiceActions;

namespace PholioVisualisation.ServiceActionsTest
{
    [TestClass]
    public class PartitionDataForAllCategoriesBuilderTest
    {
        [TestMethod]
        public void TestGetPartitionData()
        {
            var indicatorId = IndicatorIds.LifeExpectancyAtBirth;

            var data = new PartitionDataForAllCategoriesBuilder().GetPartitionData(ProfileIds.Phof,
                AreaCodes.England, indicatorId, SexIds.Male,AgeIds.AllAges, AreaTypeIds.CountyAndUnitaryAuthority);

            Assert.AreEqual(AreaCodes.England, data.AreaCode);
            Assert.AreEqual(SexIds.Male, data.SexId);
            Assert.AreEqual(AgeIds.AllAges, data.AgeId);
            Assert.AreEqual(indicatorId, data.IndicatorId);
            CheckData(data);
            Assert.IsTrue(data.CategoryTypes.Any(), "No category types");
        }

        [TestMethod]
        public void TestGetPartitionData_When_No_Grouping_Returns_Empty_Data()
        {
            var data = new PartitionDataForAllCategoriesBuilder().GetPartitionData(ProfileIds.SexualHealth,
                AreaCodes.England, IndicatorIds.SyphilisDiagnosis, SexIds.Persons, AgeIds.From0To4,
                AreaTypeIds.CountyAndUnitaryAuthority);

            Assert.IsNull(data.CategoryTypes);
            Assert.IsNull(data.Data);
        }

        [TestMethod]
        public void TestGetPartitionTrendData()
        {
            var indicatorId = IndicatorIds.LifeExpectancyAtBirth;

            var trendData = new PartitionDataForAllCategoriesBuilder().GetPartitionTrendData(ProfileIds.Phof,
                AreaCodes.England, indicatorId, SexIds.Male, AgeIds.AllAges, 
                CategoryTypeIds.LsoaDeprivationDecilesWithinArea2010, AreaTypeIds.CountyAndUnitaryAuthority);

            Assert.IsNotNull(trendData.Limits);
            Assert.AreEqual(10, trendData.Labels.Count, "Expected 10 deciles");
            Assert.IsTrue(trendData.Periods.Any());
            Assert.IsTrue(trendData.TrendData[CategoryIds.MostDeprivedDecile].Any(), "Expected trend data");
        }

        [TestMethod]
        public void TestGetPartitionTrendDataWhenNoData()
        {
            var indicatorId = IndicatorIds.LifeExpectancyAtBirth;

            var trendData = new PartitionDataForAllCategoriesBuilder().GetPartitionTrendData(ProfileIds.Phof,
                AreaCodes.England, indicatorId, SexIds.Male, AgeIds.AllAges,
                CategoryTypeIds.EthnicGroups5, AreaTypeIds.CountyAndUnitaryAuthority);

            Assert.IsNull(trendData.Limits);
            Assert.AreEqual(5, trendData.Labels.Count, "Expected 5 ethnic groups");
            Assert.IsFalse(trendData.Periods.Any(), "No periods if no data");
            Assert.IsFalse(trendData.TrendData[CategoryIds.EthnicityAsian].Any());
        }

        private static void CheckData(PartitionDataForAllCategories response)
        {
            Assert.IsTrue(response.Data.Any(), "No data");
            foreach (var coreDataSet in response.Data)
            {
                Assert.AreEqual(coreDataSet.IndicatorId, response.IndicatorId);
                Assert.AreEqual(coreDataSet.SexId, response.SexId);
                Assert.AreEqual(coreDataSet.AgeId, response.AgeId);
                Assert.AreEqual(coreDataSet.AreaCode, response.AreaCode);
                Assert.IsNotNull(coreDataSet.ValueFormatted);
                Assert.IsNotNull(coreDataSet.SignificanceAgainstOneBenchmark, "Significance not set");
            }
        }
    }
}
