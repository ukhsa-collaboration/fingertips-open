using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServiceActions;

namespace PholioVisualisation.ServiceActionsTest
{
    [TestClass]
    public class MostRecentDataForAllAgesActionTest
    {
        [TestMethod]
        public void TestResponse()
        {
            var indicatorId = IndicatorIds.LifeExpectancyAtBirth;

            var response = new MostRecentDataForAllAgesAction().GetResponse(ProfileIds.Phof,
                AreaCodes.England, indicatorId, SexIds.Male, AreaTypeIds.CountyAndUnitaryAuthority);

            Assert.AreEqual(AreaCodes.England, response.AreaCode);
            Assert.AreEqual(SexIds.Male, response.SexId);
            Assert.AreEqual(indicatorId, response.IndicatorId);
            Assert.IsTrue(response.Ages.Any(), "No ages");
            CheckData(response);
        }

        private static void CheckData(MostRecentDataForAllAges response)
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
