using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServicesWeb.Controllers;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace PholioVisualisation.ServicesWebTest.Controllers
{
    [TestClass]
    public class DownloadDataFileControllerTest
    {
        private readonly string _indicatorIds = IndicatorIds.AdultPhysicalActivity + "," + IndicatorIds.ChildrenInLowIncomeFamilies;
        private const int ChildAreaTypeId = AreaTypeIds.DistrictAndUnitaryAuthority;
        private const int ParentAreaTypeId = AreaTypeIds.GoRegion;
        private const int GroupId = GroupIds.SexualAndReproductiveHealth;
        private string _childAreasCode;

        [TestMethod]
        public void ShouldGetCsvLatestPeriodNoInequalitiesByGroupIdAllChild()
        {
            _childAreasCode = null;
            const int bytesExpected = 1049934;

            var downloadDataFileController = new DownloadDataFileController();
            var response = downloadDataFileController.GetLatestDataFileForGroup(ChildAreaTypeId, ParentAreaTypeId, GroupId, _childAreasCode);

            var content = GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            Assert.IsNotNull(content);
            Assert.AreEqual(bytesExpected, content.Length);
        }

        [TestMethod]
        public void ShouldGetCsvLatestPeriodNoInequalitiesByGroupIdOneChild()
        {
            _childAreasCode = AreaCodes.CountyUa_CityOfLondon;
            const int bytesExpected = 9339;

            var downloadDataFileController = new DownloadDataFileController();
            var response = downloadDataFileController.GetLatestDataFileForGroup(ChildAreaTypeId, ParentAreaTypeId, GroupId, _childAreasCode);

            var content = GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            Assert.IsNotNull(content);
            Assert.AreEqual(bytesExpected, content.Length);
        }

        [TestMethod]
        public void ShouldGetCsvLatestPeriodNoInequalitiesByGroupIdInternalServerError()
        {
            var groupId = GroupIds.groupIdMinusOne;

            var downloadDataFileController = new DownloadDataFileController();
            var response = downloadDataFileController.GetLatestDataFileForGroup(ChildAreaTypeId, ParentAreaTypeId, groupId, null);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public void ShouldGetCsvLatestPeriodNoInequalitiesByIndicatorIdAllChild()
        {
            _childAreasCode = null;
            const int bytesExpected = 164514;

            var downloadDataFileController = new DownloadDataFileController();
            var response = downloadDataFileController.GetLatestDataFileForIndicator(_indicatorIds, ChildAreaTypeId, ParentAreaTypeId, _childAreasCode);

            var content = GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            Assert.IsNotNull(content);
            Assert.AreEqual(bytesExpected, content.Length);
        }

        [TestMethod]
        public void ShouldGetCsvLatestPeriodNoInequalitiesByIndicatorIdOneChild()
        {
            _childAreasCode = AreaCodes.CountyUa_CityOfLondon;
            const int bytesExpected = 1779;

            var downloadDataFileController = new DownloadDataFileController();
            var response = downloadDataFileController.GetLatestDataFileForIndicator(_indicatorIds, ChildAreaTypeId, ParentAreaTypeId, _childAreasCode);

            var content = GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            Assert.IsNotNull(content);
            Assert.AreEqual(bytesExpected, content.Length);
        }

        [TestMethod]
        public void ShouldGetCsvLatestPeriodNoInequalitiesByIndicatorIdInternalServerError()
        {
            var childAreaTypeId = AreaTypeIds.areaTypeMinusOne;
            var downloadDataFileController = new DownloadDataFileController();
            var response = downloadDataFileController.GetLatestDataFileForIndicator(_indicatorIds, childAreaTypeId, ParentAreaTypeId, _childAreasCode);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public void ShouldGetLatestPopulationDataFile()
        {
            _childAreasCode = AreaCodes.Gor_London;
            const int bytesExpected = 35579;

            var downloadDataFileController = new DownloadDataFileController();
            var response = downloadDataFileController.GetLatestPopulationDataFile( ChildAreaTypeId, ParentAreaTypeId, _childAreasCode);

            var content = GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            Assert.IsNotNull(content);
            Assert.AreEqual(bytesExpected, content.Length);
        }

        [TestMethod]
        public void ShouldGetLatestPopulationDataFileInternalServerError()
        {
            var childAreaTypeId = AreaTypeIds.areaTypeMinusOne;
            var downloadDataFileController = new DownloadDataFileController();
            var response = downloadDataFileController.GetLatestPopulationDataFile( childAreaTypeId, ParentAreaTypeId, _childAreasCode);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public void ShouldGetAllPeriodDataFileByIndicator()
        {
            _childAreasCode = AreaCodes.England;
            const int bytesExpected = 2935;

            var downloadDataFileController = new DownloadDataFileController();
            var response = downloadDataFileController.GetAllPeriodDataFileByIndicator(_indicatorIds, ChildAreaTypeId, ParentAreaTypeId, _childAreasCode);

            var content = GetStreamContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            Assert.IsNotNull(content);
            Assert.AreEqual(bytesExpected, content.Length);
        }

        [TestMethod]
        public void ShouldGetAllPeriodDataFileByIndicatorInternalServerError()
        {
            var childAreaTypeId = AreaTypeIds.areaTypeMinusOne;
            var downloadDataFileController = new DownloadDataFileController();
            var response = downloadDataFileController.GetAllPeriodDataFileByIndicator(_indicatorIds, childAreaTypeId, ParentAreaTypeId, _childAreasCode);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.InternalServerError);
        }

        private static string GetStreamContent(HttpResponseMessage response)
        {
            var receiveStream = response.Content.ReadAsStreamAsync();
            var readStream = new StreamReader(receiveStream.Result, Encoding.UTF8);
            var content = readStream.ReadToEnd();
            return content;
        }
    }
}
