using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServicesWeb.Controllers;
using System.Collections.Generic;
using System.Net;
using PholioVisualisation.ServicesWebTest.Helpers;

namespace PholioVisualisation.ServicesWebTest.Controllers
{
    [TestClass]
    public class DataControllerTest
    {
        public const string HeaderText =
            "Indicator ID,Indicator Name,Parent Code,Parent Name,Area Code,Area Name,Area Type,Sex,Age,Category Type,Category,Time period,Value,Lower CI 95.0 limit,Upper CI 95.0 limit,Lower CI 99.8 limit,Upper CI 99.8 limit,Count,Denominator,Value note,Recent Trend,Compared to England value or percentiles,Compared to Region value or percentiles,Time period Sortable,New data,Compared to goal";

        private readonly string _indicatorIds = IndicatorIds.AdultPhysicalActivity + "," + IndicatorIds.ChildrenInLowIncomeFamilies;
        private const int ChildAreaTypeId = AreaTypeIds.DistrictAndUnitaryAuthorityPreApr2019;
        private const int ParentAreaTypeId = AreaTypeIds.GoRegion;
        private const int GroupId = GroupIds.SexualAndReproductiveHealth;
        private const int ProfileId = ProfileIds.ChildAndMaternalHealth;
        private string _childAreasCode;
        private const string NullChildAreasCode= null;
        private const string NullParentAreasCode = null;
        private const string NullIndicatorIds = null;
        private const string NullCategoryAreaCode = null;

        [TestMethod]
        public void ShouldGetDataFileForIndicatorList()
        {
            _childAreasCode = AreaCodes.CountyUa_CityOfLondon;

            var downloadDataFileController = new DataController();
            var response = downloadDataFileController.GetDataFileForIndicatorList(_indicatorIds, ChildAreaTypeId, ParentAreaTypeId);

            var content = HttpResponseTestHelper.GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            AssertHelper.AssertHeaderText(content, HeaderText);
        }

        [TestMethod]
        public void ShouldGetDataFileForIndicatorListBadRequest()
        {
            var expectedErrorsNumber = 2;
            var expectedErrorMessage = "IndicatorIds cannot be null or empty, ParentAreaCode cannot be null or empty";

            var downloadDataFileController = new DataController();
            var response = downloadDataFileController.GetDataFileForIndicatorList(NullIndicatorIds, ChildAreaTypeId, ParentAreaTypeId, ProfileId, NullCategoryAreaCode, NullParentAreasCode);

            var content = HttpResponseTestHelper.GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
            AssertHelper.AssertErrorMessagesContent(content, expectedErrorMessage, expectedErrorsNumber);
        }

        [TestMethod]
        public void ShouldGetDataFileForIndicatorListInternalServerError()
        {;
            var childAreaTypeId = AreaTypeIds.areaTypeMinusOne;
            var parentAreaTypeId = AreaTypeIds.areaTypeMinusOne;

            var downloadDataFileController = new DataController();
            var response = downloadDataFileController.GetDataFileForIndicatorList(_indicatorIds, childAreaTypeId, parentAreaTypeId);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public void ShouldGetDataFileForGroup()
        {
            _childAreasCode = AreaCodes.CountyUa_CityOfLondon;

            var downloadDataFileController = new DataController();
            var response = downloadDataFileController.GetDataFileForGroup(ChildAreaTypeId, ParentAreaTypeId, GroupId);

            var content = HttpResponseTestHelper.GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            AssertHelper.AssertHeaderText(content, HeaderText);
        }

        [TestMethod]
        public void ShouldGetDataFileForGroupBadRequest()
        {
            var expectedErrorsNumber = 1;
            var expectedErrorMessage = "ParentAreaCode cannot be null or empty";

            var downloadDataFileController = new DataController();
            var response = downloadDataFileController.GetDataFileForGroup(ChildAreaTypeId, ParentAreaTypeId, GroupId, NullCategoryAreaCode, NullParentAreasCode);

            var content = HttpResponseTestHelper.GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
            AssertHelper.AssertErrorMessagesContent(content, expectedErrorMessage, expectedErrorsNumber);
        }

        [TestMethod]
        public void ShouldGetDataFileForGroupInternalServerError()
        {
            var groupId = GroupIds.Undefined;
            var childAreaTypeId = AreaTypeIds.areaTypeMinusOne;
            var parentAreaTypeId = AreaTypeIds.areaTypeMinusOne;
            _childAreasCode = AreaCodes.NotAnActualCode;

            var downloadDataFileController = new DataController();
            var response = downloadDataFileController.GetDataFileForGroup(childAreaTypeId, parentAreaTypeId, groupId, _childAreasCode, NullCategoryAreaCode);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public void ShouldGetDataFileForProfile()
        {
            _childAreasCode = AreaCodes.CountyUa_CityOfLondon;

            var downloadDataFileController = new DataController();
            var response = downloadDataFileController.GetDataFileForProfile(ChildAreaTypeId, ParentAreaTypeId, ProfileId);

            var content = HttpResponseTestHelper.GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            AssertHelper.AssertHeaderText(content, HeaderText);
        }

        [TestMethod]
        public void ShouldGetDataFileForProfileBadRequest()
        {
            var expectedErrorsNumber = 1;
            var expectedErrorMessage = "ParentAreaCode cannot be null or empty";

            var downloadDataFileController = new DataController();
            var response = downloadDataFileController.GetDataFileForProfile(ChildAreaTypeId, ParentAreaTypeId, ProfileId, NullCategoryAreaCode, NullParentAreasCode);

            var content = HttpResponseTestHelper.GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
            AssertHelper.AssertErrorMessagesContent(content, expectedErrorMessage, expectedErrorsNumber);
        }

        [TestMethod]
        public void ShouldGetDataFileForProfileInternalServerError()
        {
            var childAreaTypeId = AreaTypeIds.areaTypeMinusOne;
            var parentAreaTypeId = AreaTypeIds.areaTypeMinusOne;

            var downloadDataFileController = new DataController();
            var response = downloadDataFileController.GetDataFileForProfile(childAreaTypeId, parentAreaTypeId, ProfileId);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public void TestAreaValues()
        {
            var values = new DataController().GetAreaValues(GroupIds.Phof_HealthcarePrematureMortality,
                AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019, AreaCodes.England, ComparatorIds.England,
                IndicatorIds.ExcessWinterDeathsIndex, SexIds.Persons, AgeIds.AllAges, ProfileIds.Phof
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
            var areaTypeId = AreaTypeIds.CcgsPostApr2019;

            var data = new DataController().GetQuinaryPopulation(areaCode,
                areaTypeId, 0);

            Assert.AreEqual(data["Code"], areaCode);
            Assert.IsTrue(((IList<string>)data["Labels"]).Contains("35-39"), 
                "Suggested fix: if not already there then copy population indicator to this area type in Populations profile");
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
        public void TestGetIndicatorStatisticsForBoxPlot()
        {
            var data = new DataController().GetIndicatorStatisticsTrendsForIndicator(IndicatorIds.LifeExpectancyAtBirth,
                SexIds.Male, AgeIds.AllAges, AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019, AreaCodes.England);

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
