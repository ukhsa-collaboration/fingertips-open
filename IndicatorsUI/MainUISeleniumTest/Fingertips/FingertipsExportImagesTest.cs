using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsExportImagesTest : FingertipsBaseUnitTest
    {
        private const string ChartFileName = "chart.png";
        private const string MessageExportTable = "Export table as image";
        private const string MessageExportChart = "Export chart as image";

        private IWebElement _link;
        private HashParameters _parameters;

        [TestInitialize]
        public void TestInitialize()
        {
            _link = null;
            _parameters = new HashParameters();
            DownloadFileHelper.DeleteAllFilesInDownloadDir();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            DownloadFileHelper.DeleteAllFilesInDownloadDir();
        }

        [TestMethod]
        public void Test_Image_Export_Overview()
        {
            navigateTo.OverviewTab();

            CheckLinkText("overview-export-image", MessageExportTable);
            CheckImageCanBeDownloaded("OverviewTable.png");
        }

        [TestMethod]
        public void Test_Image_Export_Compare_Indicators()
        {
            _parameters.AddTabId(TabIds.CompareIndicators);
            _parameters.AddAreaTypeId(AreaTypeIds.DistrictAndUAPreApr2019);
            GoToUrl();

            CheckLinkText("compare-indicators-export-image", MessageExportChart);
            CheckImageCanBeDownloaded(ChartFileName);
        }

        [TestMethod]
        public void Test_Image_Export_Map_Map()
        {
            _parameters.AddTabId(TabIds.Map);
            _parameters.AddAreaTypeId(AreaTypeIds.DistrictAndUAPreApr2019);
            GoToUrl();

            CheckLinkText("map-export-map-image", "Export map as image");
            CheckImageCanBeDownloaded("Map.png");
        }

        [TestMethod]
        public void Test_Image_Export_Map_Bar_Chart()
        {
            _parameters.AddTabId(TabIds.Map);
            _parameters.AddAreaTypeId(AreaTypeIds.DistrictAndUAPreApr2019);
            GoToUrl();

            CheckLinkText("map-export-chart-image", MessageExportChart);
            CheckImageCanBeDownloaded(ChartFileName);
        }

        [TestMethod]
        public void Test_Image_Export_Trends()
        {
            _parameters.AddTabId(TabIds.Trends);
            _parameters.AddAreaTypeId(AreaTypeIds.DistrictAndUAPreApr2019);
            GoToUrl();

            CheckLinkText("export-link-trend-chart", MessageExportChart);
            CheckImageCanBeDownloaded(ChartFileName);
        }

        [TestMethod]
        public void Test_Image_Export_Compare_Areas()
        {
            _parameters.AddTabId(TabIds.CompareAreas);
            _parameters.AddAreaTypeId(AreaTypeIds.DistrictAndUAPreApr2019);
            GoToUrl();

            CheckLinkText("compare-areas-export-image", MessageExportTable);
            CheckImageCanBeDownloaded("CompareAreasTable.png");
        }

        [TestMethod]
        public void Test_Image_Export_Area_Profiles()
        {
            navigateTo.AreaProfilesTab();

            CheckLinkText("area-profiles-export-image", MessageExportTable);
            CheckImageCanBeDownloaded("AreaProfilesTable.png");
        }

        [TestMethod]
        public void Test_Image_Export_Inequalities_Recent_Values()
        {
            navigateTo.InequalitiesTab();

            CheckLinkText("inequalities-export-image", MessageExportChart);
            CheckImageCanBeDownloaded(ChartFileName);
        }

        [TestMethod]
        public void Test_Image_Export_Inequalities_Trends()
        {
            navigateTo.InequalitiesTab();
            fingertipsHelper.SelectInequalitiesTrends();

            CheckLinkText("inequalities-export-image", MessageExportChart);
            CheckImageCanBeDownloaded(ChartFileName);
        }

        [TestMethod]
        public void Test_Image_Export_England()
        {
            navigateTo.EnglandTab();

            CheckLinkText("england-export-image", MessageExportTable);
            CheckImageCanBeDownloaded("england.png");
        }

        [TestMethod]
        public void Test_Image_Export_Population()
        {
            navigateTo.PopulationTab();

            CheckLinkText("population-export-image", MessageExportChart);
            CheckImageCanBeDownloaded(ChartFileName);
        }

        private void CheckLinkText(string linkId, string message)
        {
            // Get link
            var by = By.Id(linkId);
            waitFor.ExpectedElementToBeVisible(by);
            _link = driver.FindElement(by);

            Assert.AreEqual(message, _link.Text, "Unexpected image link text");
        }

        private void CheckImageCanBeDownloaded(string fileName)
        {
            _link.Click();
            DownloadFileHelper.CheckFileWasDownloaded(fileName);
        }

        private void GoToUrl()
        {
            navigateTo.GoToUrl(ProfileUrlKeys.DevelopmentProfileForTesting + _parameters.HashParameterString);
            waitFor.PageToFinishLoading();
            waitFor.AjaxLockToBeUnlocked();
        }
    }
}