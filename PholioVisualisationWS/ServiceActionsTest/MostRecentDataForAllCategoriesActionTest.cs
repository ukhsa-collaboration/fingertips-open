using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServiceActions;

namespace ServiceActionsTest
{
    [TestClass]
    public class MostRecentDataForAllCategoriesActionTest
    {
        [TestMethod]
        public void TestResponse()
        {
            var indicatorId = IndicatorIds.LifeExpectancyAtBirth;

            var response = new MostRecentDataForAllCategoriesAction().GetResponse(ProfileIds.Phof,
                AreaCodes.England, indicatorId, SexIds.Male,AgeIds.AllAges, AreaTypeIds.CountyAndUnitaryAuthority);

            Assert.AreEqual(AreaCodes.England, response.AreaCode);
            Assert.AreEqual(SexIds.Male, response.SexId);
            Assert.AreEqual(AgeIds.AllAges, response.AgeId);
            Assert.AreEqual(indicatorId, response.IndicatorId);
            CheckData(response);
            Assert.IsTrue(response.CategoryTypes.Any(), "No category types");
        }

        [TestMethod]
        public void TestResponseWhereNoData()
        {
            var response = new MostRecentDataForAllCategoriesAction().GetResponse(ProfileIds.SexualHealth,
                AreaCodes.England, IndicatorIds.SyphilisDiagnosis, SexIds.Persons, AgeIds.AllAges,
                AreaTypeIds.CountyAndUnitaryAuthority);

            Assert.AreEqual(AreaCodes.England, response.AreaCode);
            Assert.AreEqual(SexIds.Persons, response.SexId);
            Assert.AreEqual(AgeIds.AllAges, response.AgeId);
            Assert.IsFalse(response.Data.Any(), "There should be no data");
            Assert.IsFalse(response.CategoryTypes.Any(), "There should be no category types");
        }

        private static void CheckData(MostRecentDataForAllCategories response)
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
