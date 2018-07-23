using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Export.File;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServicesWeb.Controllers;
using System.Collections.Generic;

namespace PholioVisualisation.ServicesWebTest.Controllers
{
    [TestClass]
    public class DataControllerTest
    {
        [TestMethod]
        public void TestAreaValues()
        {
            var values = new DataController().GetAreaValues(GroupIds.Phof_HealthcarePrematureMortality,
                AreaTypeIds.CountyAndUnitaryAuthority, AreaCodes.England, ComparatorIds.England,
                IndicatorIds.ExcessWinterDeaths, SexIds.Persons, AgeIds.AllAges, ProfileIds.Phof
                );

            // Assert: All values in England
            Assert.IsTrue(values.Count > 100);
        }

        [TestMethod]
        public void TestGetTimePeriod()
        {
            var timePeriod = new DataController().GetTimePeriod(2001, -1, -1, 1, YearTypeIds.Calendar);
            Assert.AreEqual("2001", timePeriod);
        }

        [TestMethod]
        public void TestGetQuinaryPopulation()
        {
            var areaCode = AreaCodes.Ccg_AireDaleWharfdaleAndCraven;
            var areaTypeId = AreaTypeIds.CcgsPreApr2017;

            var data = new DataController().GetQuinaryPopulation(areaCode,
                areaTypeId, 0);

            Assert.AreEqual(data["Code"], areaCode);
            Assert.IsTrue(((IList<string>)data["Labels"]).Contains("35-39"));
        }

        [TestMethod]
        public void TestGetQuinaryPopulationByIndicatorId()
        {
            var areaCode = AreaCodes.England;
            var areaTypeId = AreaTypeIds.DistrictAndUnitaryAuthority;
            var profileId = ProfileIds.HealthProfilesSupportingIndicators;
            var indicatorId = IndicatorIds.PopulationProjection;

            var data = new DataController().GetQuinaryPopulationByIndicatorId(areaCode,
                areaTypeId, indicatorId, profileId, 0);

            Assert.AreEqual(data["Code"], areaCode);
            Assert.IsTrue(((IList<string>)data["Labels"]).Contains("35-39"));
        }

        [TestMethod]
        public void TestGetQuinaryPopulationSummary()
        {
            var areaCode = AreaCodes.Gp_KingStreetBlackpool;
            var areTypeId = AreaTypeIds.GpPractice;

            var data = new DataController().GetQuinaryPopulationSummary(areaCode,
                areTypeId);

            Assert.AreEqual(data["Code"], areaCode);
        }

        [TestMethod]
        public void TestGetAllDataCsvByGroup_For_GPs()
        {
            var data = new DataController(new DummyFileBuilder())
                .GetDataFileForGroup(AreaTypeIds.GpPractice, AreaTypeIds.CcgsPreApr2017, GroupIds.PracticeProfiles_Diabetes);

            Assert.IsNotNull(data);
        }

        [TestMethod]
        public void TestGetIndicatorStatisticsForBoxPlot()
        {
            var data = new DataController().GetIndicatorStatisticsTrendsForIndicator(IndicatorIds.LifeExpectancyAtBirth,
                SexIds.Male, AgeIds.AllAges, AreaTypeIds.CountyAndUnitaryAuthority, AreaCodes.England);

            Assert.IsNotNull(data);
        }

        [TestMethod]
        public void TestGetAvailableDataForGrouping()
        {
            var data = new DataController().GetAvailableDataForGrouping();
            Assert.IsNotNull(data);
        }

        [TestMethod]
        public void TestGetDataChanges()
        {
            var data = new DataController().GetDataChanges(IndicatorIds.AdultUnder75MortalityRateCancer);
            Assert.IsNotNull(data);
        }
    }
}
