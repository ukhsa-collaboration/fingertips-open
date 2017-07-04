using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Export.File;
using ServicesWeb.Controllers;
using PholioVisualisation.PholioObjects;

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
        public void TestGetQuinaryPopulationData()
        {
            var areaCode = AreaCodes.Gp_Burnham;

            var data = new DataController().GetQuinaryPopulationData(areaCode,
                GroupIds.PracticeProfiles_SupportingIndicators, 0);

            Assert.AreEqual(data["Code"], areaCode);
        }

        [TestMethod]
        public void TestGetQuinaryPopulation()
        {
            var areaCode = AreaCodes.Ccg_AireDaleWharfdaleAndCraven;
            var areaTypeId = AreaTypeIds.Ccg;

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
                .GetDataFileForGroup(AreaTypeIds.GpPractice, AreaTypeIds.Ccg, GroupIds.PracticeProfiles_Diabetes);

            Assert.IsNotNull(data);
        }

        [TestMethod]
        public void TestGetIndicatorStatisticsForBoxPlot()
        {
            var data = new DataController().GetIndicatorStatisticsTrendsForIndicator(IndicatorIds.LifeExpectancyAtBirth,
                SexIds.Male, AgeIds.AllAges, AreaTypeIds.CountyAndUnitaryAuthority, AreaCodes.England);

            Assert.IsNotNull(data);
        }
    }
}
