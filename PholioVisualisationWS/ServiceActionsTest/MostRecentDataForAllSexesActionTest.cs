using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServiceActions;

namespace ServiceActionsTest
{
    [TestClass]
    public class MostRecentDataForAllSexesActionTest
    {
        [TestMethod]
        public void TestIndicatorWhereNoPersonsData()
        {
            var indicatorId = IndicatorIds.LifeExpectancyAtBirth;

            var response = new MostRecentDataForAllSexesAction().GetResponse(ProfileIds.Phof,
                AreaCodes.England, indicatorId, AgeIds.AllAges, AreaTypeIds.CountyAndUnitaryAuthority);

            Assert.AreEqual(AreaCodes.England, response.AreaCode);
            Assert.AreEqual(AgeIds.AllAges, response.AgeId);
            Assert.AreEqual(indicatorId, response.IndicatorId);
            Assert.IsTrue(response.Sexes.Any(), "No sexes");
            CheckData(response);

            // Check significance is none because person data is not available
            foreach (var coreDataSet in response.Data)
            {
                Assert.AreEqual((int)Significance.None, coreDataSet.SignificanceAgainstOneBenchmark);
            }
        }

        [TestMethod]
        public void TestIndicatorWhereIsPersonsData()
        {
            var indicatorId = IndicatorIds.MortalityRateFromCausesConsideredPreventable;

            var response = new MostRecentDataForAllSexesAction().GetResponse(ProfileIds.Phof,
                AreaCodes.England, indicatorId, AgeIds.AllAges, AreaTypeIds.CountyAndUnitaryAuthority);

            Assert.AreEqual(AreaCodes.England, response.AreaCode);
            Assert.AreEqual(AgeIds.AllAges, response.AgeId);
            Assert.AreEqual(indicatorId, response.IndicatorId);
            Assert.IsTrue(response.Sexes.Any(), "No sexes");
            CheckData(response);

            // Check significance is set because person data is available
            foreach (var coreDataSet in response.Data)
            {
                Assert.AreNotEqual((int)Significance.None, coreDataSet.SignificanceAgainstOneBenchmark);
            }
        }

        private static void CheckData(MostRecentDataForAllSexes response)
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
