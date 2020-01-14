using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServicesWeb.Controllers;
using PholioVisualisation.ServicesWebTest.Helpers;
using System.Net;

namespace PholioVisualisation.ServicesWebTest.Controllers
{
    [TestClass]
    public class DataInternalServicesControllerTest
    {
        public const string HeaderText =
            "Indicator ID,Indicator Name,Parent Code,Parent Name,Area Code,Area Name,Area Type,Sex,Age,Category Type,Category,Time period,Value,Lower CI 95.0 limit,Upper CI 95.0 limit,Lower CI 99.8 limit,Upper CI 99.8 limit,Count,Denominator,Value note,Recent Trend,Compared to England value or percentiles,Compared to Region value or percentiles,Time period Sortable,New data,Compared to goal";

        private string _indicatorIds = IndicatorIds.AdultPhysicalActivity + "," + IndicatorIds.ChildrenInLowIncomeFamilies;
        private const int ChildAreaTypeId = AreaTypeIds.DistrictAndUnitaryAuthorityPreApr2019;
        private const int ParentAreaTypeId = AreaTypeIds.GoRegion;
        private const int GroupId = GroupIds.SexualAndReproductiveHealth;
        private const int ProfileId = ProfileIds.Undefined;
        private static readonly string SexId = SexIds.Persons + "," + SexIds.Female;
        private static readonly string AgeId = AgeIds.AllAges + "," + AgeIds.From0To4;
        private string _childAreasCode;
        private string _nullChildAreasCode;
        private string _nullParentAreasCode;
        private string _nullIndicatorIds;
        private string _nullCategoryAreaCode;
        private string _nullInequalities;


        [TestMethod]
        public void ShouldGetCsvLatestPeriodNoInequalitiesByGroupIdAllChild()
        {
            _childAreasCode = AreaCodes.CountyUa_CityOfLondon;

            var downloadDataFileController = new DataInternalServicesController();
            var response = downloadDataFileController.GetLatestDataFileForGroup(ChildAreaTypeId, ParentAreaTypeId, GroupId, _childAreasCode);

            var content = HttpResponseTestHelper.GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            AssertHelper.AssertHeaderText(content, HeaderText);
        }

        [TestMethod]
        public void ShouldGetBadRequestLatestPeriodNoInequalitiesByGroupIdAllChild()
        {
            var expectedErrorsNumber = 1;
            var expectedErrorMessage = "ParentAreaCode cannot be null or empty";
            
            var downloadDataFileController = new DataInternalServicesController();
            var response = downloadDataFileController.GetLatestDataFileForGroup(ChildAreaTypeId, ParentAreaTypeId, GroupId, _nullChildAreasCode, _nullCategoryAreaCode, _nullParentAreasCode);

            var content = HttpResponseTestHelper.GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
            AssertHelper.AssertErrorMessagesContent(content, expectedErrorMessage, expectedErrorsNumber);
        }

        [TestMethod]
        public void ShouldGetCsvLatestPeriodNoInequalitiesByGroupIdOneChild()
        {
            _childAreasCode = AreaCodes.CountyUa_CityOfLondon;

            var downloadDataFileController = new DataInternalServicesController();
            var response = downloadDataFileController.GetLatestDataFileForGroup(ChildAreaTypeId, ParentAreaTypeId, GroupId, _childAreasCode);

            var content = HttpResponseTestHelper.GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            AssertHelper.AssertHeaderText(content, HeaderText);
        }

        [TestMethod]
        public void ShouldGetCsvLatestPeriodNoInequalitiesByGroupIdInternalServerError()
        {
            var groupId = GroupIds.Undefined;
            var childAreaTypeId = AreaTypeIds.areaTypeMinusOne;
            var parentAreaTypeId = AreaTypeIds.areaTypeMinusOne;
            _childAreasCode = AreaCodes.NotAnActualCode;

            var downloadDataFileController = new DataInternalServicesController();
            var response = downloadDataFileController.GetLatestDataFileForGroup(childAreaTypeId, parentAreaTypeId, groupId, _childAreasCode);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public void ShouldGetCsvLatestPeriodNoInequalitiesByIndicatorIdAllChild()
        {
            var downloadDataFileController = new DataInternalServicesController();
            var response = downloadDataFileController.GetLatestDataFileForIndicator(_indicatorIds, ChildAreaTypeId, ParentAreaTypeId, SexId, AgeId, _childAreasCode);

            var content = HttpResponseTestHelper.GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            AssertHelper.AssertHeaderText(content, HeaderText);
        }


        [TestMethod]
        public void ShouldGetBadRequestLatestPeriodNoInequalitiesByIndicatorIdAllChild()
        {
            var expectedErrorsNumber = 2;
            var expectedErrorMessage = "IndicatorIds cannot be null or empty, ParentAreaCode cannot be null or empty";

            var downloadDataFileController = new DataInternalServicesController();
            var response = downloadDataFileController.GetLatestDataFileForIndicator(_nullIndicatorIds, ChildAreaTypeId, ParentAreaTypeId, SexId, AgeId, _nullChildAreasCode, 
                ProfileId, _nullParentAreasCode, _nullCategoryAreaCode);

            var content = HttpResponseTestHelper.GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
            AssertHelper.AssertErrorMessagesContent(content, expectedErrorMessage, expectedErrorsNumber);
        }

        [TestMethod]
        public void ShouldGetCsvLatestPeriodNoInequalitiesByIndicatorIdParentAreaCodeIsCategoryAreaCode()
        {
            _childAreasCode = AreaCodes.CountyUa_CityOfLondon;
            var parentAreaCode = "cat-39-1";

            var downloadDataFileController = new DataInternalServicesController();
            var response = downloadDataFileController.GetLatestDataFileForIndicator(_indicatorIds, ChildAreaTypeId, ParentAreaTypeId, SexId, AgeId, _childAreasCode, - 1, parentAreaCode, parentAreaCode);

            var content = HttpResponseTestHelper.GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            AssertHelper.AssertHeaderText(content, HeaderText);
        }

        [TestMethod]
        public void ShouldGetCsvLatestPeriodNoInequalitiesByIndicatorIdOneChild()
        {
            _childAreasCode = AreaCodes.CountyUa_CityOfLondon;

            var downloadDataFileController = new DataInternalServicesController();
            var response = downloadDataFileController.GetLatestDataFileForIndicator(_indicatorIds, ChildAreaTypeId, ParentAreaTypeId, SexId, AgeId, _childAreasCode);

            var content = HttpResponseTestHelper.GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            AssertHelper.AssertHeaderText(content, HeaderText);
        }

        [TestMethod]
        public void ShouldGetCsvLatestPeriodNoInequalitiesByIndicatorIdInternalServerError()
        {
            _indicatorIds = "-1,-1";
            var childAreaTypeId = AreaTypeIds.areaTypeMinusOne;
            var parentAreaTypeId = AreaTypeIds.areaTypeMinusOne;
            _childAreasCode = AreaCodes.NotAnActualCode;
            var downloadDataFileController = new DataInternalServicesController();
            var response = downloadDataFileController.GetLatestDataFileForIndicator(_indicatorIds, childAreaTypeId, parentAreaTypeId, SexId, AgeId, _childAreasCode);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public void ShouldGetLatestPopulationDataFile()
        {
            _childAreasCode = AreaCodes.Gor_London;

            var downloadDataFileController = new DataInternalServicesController();
            var response = downloadDataFileController.GetLatestPopulationDataFile(ChildAreaTypeId, ParentAreaTypeId, _childAreasCode);

            var content = HttpResponseTestHelper.GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            AssertHelper.AssertHeaderText(content, HeaderText);
        }

        [TestMethod]
        public void ShouldGetLatestPopulationDataFileInternalServerError()
        {
            var childAreaTypeId = AreaTypeIds.areaTypeMinusOne;
            var parentAreaTypeId = AreaTypeIds.areaTypeMinusOne;
            _childAreasCode = AreaCodes.NotAnActualCode;

            var downloadDataFileController = new DataInternalServicesController();
            var response = downloadDataFileController.GetLatestPopulationDataFile(childAreaTypeId, parentAreaTypeId, _childAreasCode, SexId, AgeId);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.InternalServerError);
        }


        [TestMethod]
        public void ShouldGetBadRequestLatestPopulationDataFile()
        {
            var expectedErrorsNumber = 2;
            var expectedErrorMessage = "AreaCode cannot be null or empty, ParentAreaCode cannot be null or empty";

            var downloadDataFileController = new DataInternalServicesController();
            var response = downloadDataFileController.GetLatestPopulationDataFile(ChildAreaTypeId, ParentAreaTypeId, _nullChildAreasCode, _nullCategoryAreaCode, _nullParentAreasCode);

            var content = HttpResponseTestHelper.GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
            AssertHelper.AssertErrorMessagesContent(content, expectedErrorMessage, expectedErrorsNumber);
        }

        [TestMethod]
        public void ShouldGetAllPeriodDataFileByIndicator()
        {
            _childAreasCode = AreaCodes.England;

            var downloadDataFileController = new DataInternalServicesController();
            var response = downloadDataFileController.GetAllPeriodDataFileByIndicator(_indicatorIds, ChildAreaTypeId, ParentAreaTypeId, SexId, AgeId, _childAreasCode);

            var content = HttpResponseTestHelper.GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            AssertHelper.AssertHeaderText(content, HeaderText);
        }

        [TestMethod]
        public void ShouldGetBadRequestAllPeriodDataFileByIndicator()
        {
            var expectedErrorsNumber = 3;
            var expectedErrorMessage = "IndicatorIds cannot be null or empty, AreaCode cannot be null or empty, ParentAreaCode cannot be null or empty";

            var downloadDataFileController = new DataInternalServicesController();
            var response = downloadDataFileController.GetAllPeriodDataFileByIndicator(_nullIndicatorIds, ChildAreaTypeId, ParentAreaTypeId, SexId, AgeId, _nullChildAreasCode, ProfileId, _nullCategoryAreaCode, _nullParentAreasCode);

            var content = HttpResponseTestHelper.GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
            AssertHelper.AssertErrorMessagesContent(content, expectedErrorMessage, expectedErrorsNumber);
        }

        [TestMethod]
        public void ShouldGetAllPeriodDataFileByIndicatorInternalServerError()
        {
            _indicatorIds = "-1,-1";
            var childAreaTypeId = AreaTypeIds.areaTypeMinusOne;
            var parentAreaTypeId = AreaTypeIds.areaTypeMinusOne;
            _childAreasCode = AreaCodes.England;

            var downloadDataFileController = new DataInternalServicesController();
            var response = downloadDataFileController.GetAllPeriodDataFileByIndicator(_indicatorIds, childAreaTypeId, parentAreaTypeId, SexId, AgeId, _childAreasCode);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public void ShouldGetBadRequestLatestWithInequalitiesDataFileForIndicator()
        {
            var expectedErrorsNumber = 3;
            var expectedErrorMessage = "IndicatorIds cannot be null or empty, Inequalities cannot be null or empty, ParentAreaCode cannot be null or empty";

            var downloadDataFileController = new DataInternalServicesController();
            var response = downloadDataFileController.GetLatestWithInequalitiesDataFileForIndicator(_nullIndicatorIds, ChildAreaTypeId, ParentAreaTypeId, _nullInequalities, _nullChildAreasCode,
               ProfileId, _nullParentAreasCode, _nullCategoryAreaCode);

            var content = HttpResponseTestHelper.GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
            AssertHelper.AssertErrorMessagesContent(content, expectedErrorMessage, expectedErrorsNumber);
        }

        [TestMethod]
        public void ShouldGetBadRequestAllPeriodsWithInequalitiesDataFileForIndicator()
        {
            var expectedErrorsNumber = 3;
            var expectedErrorMessage = "IndicatorIds cannot be null or empty, Inequalities cannot be null or empty, ParentAreaCode cannot be null or empty";

            var downloadDataFileController = new DataInternalServicesController();
            var response = downloadDataFileController.GetAllPeriodsWithInequalitiesDataFileForIndicator(_nullIndicatorIds, ChildAreaTypeId, ParentAreaTypeId, _nullInequalities, _nullChildAreasCode, ProfileId,
                _nullCategoryAreaCode, _nullParentAreasCode);

            var content = HttpResponseTestHelper.GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
            AssertHelper.AssertErrorMessagesContent(content, expectedErrorMessage, expectedErrorsNumber);
        }
    }
}
