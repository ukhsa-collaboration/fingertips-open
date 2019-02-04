using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsDownloadCsvFile : FingertipsBaseUnitTest
    {
        private readonly double _waitInterval = 3;
        private readonly double _waitDownloadInterval = 5;

        private readonly string _linkPageTartanId = "export-link-csv-tartan";
        private readonly string _linkPageScatterId = "export-link-csv-scatter";
        private readonly string _linkPageMapId = "export-link-csv-map";
        private readonly string _linkPageTrendsId = "export-link-csv-trend";
        private readonly string _linkPageCompareAreasId = "export-link-csv-compare-areas";
        private readonly string _linkPageAreaProfilesId = "export-link-csv-area-profile";
        private readonly string _linkPageInequalitiesId = "export-link-csv-inequalities";
        private readonly string _linkPageEnglandId = "export-link-csv-england";
        private readonly string _linkPagePopulationId = "export-link-csv-population";


        private bool restoreCopies;

        [TestInitialize]
        public void initTest()
        {
            restoreCopies = FingertipsHelper.BackupCsvFiles();
        }

        [TestCleanup]
        public void cleanUpTest()
        {
            if (restoreCopies)
            {
                FingertipsHelper.RestoreCsvFiles();
            }
        }

        [TestMethod]
        public void TestCallFromTartanRugToEndpointForDownloadCsvFileSuccess()
        {
            // Arrange
            navigateTo.FingertipsTartanRug();

            // Test if there is downloaded file
            DownloadCsvTest(_linkPageTartanId);
        }

        [TestMethod]
        public void TestCallFromScatterToEndpointForDownloadCsvFileSuccess()
        {
            // Arrange
            navigateTo.FingertipsScatterplot();

            // Test if there is downloaded file
            DownloadCsvTest(_linkPageScatterId);
        }

        [TestMethod]
        public void TestCallFromMapToEndpointForDownloadCsvFileSuccess()
        {
            // Arrange
            navigateTo.FingertipsMap();

            // Test if there is downloaded file
            DownloadCsvTest(_linkPageMapId);
        }

        [TestMethod]
        public void TestCallFromTrendToEndpointForDownloadCsvFileSuccess()
        {
            // Arrange
            navigateTo.FingertipsTrends();

            // Test if there is downloaded file
            DownloadCsvTest(_linkPageTrendsId);
        }

        [TestMethod]
        public void TestCallFromCompareAreasToEndpointForDownloadCsvFileSuccess()
        {
            // Arrange
            navigateTo.FingertipsCompareAreas();

            // Test if there is downloaded file
            DownloadCsvTest(_linkPageCompareAreasId);
        }

        [TestMethod]
        public void TestCallFromAreaProfilesToEndpointForDownloadCsvFileSuccess()
        {
            // Arrange
            navigateTo.FingertipsAreaProfiles();

            // Test if there is downloaded file
            DownloadCsvTest(_linkPageAreaProfilesId);
        }

        [TestMethod]
        public void TestCallFromInequalitiesToEndpointForDownloadCsvFileSuccess()
        {
            // Arrange
            navigateTo.FingertipsInequalities();

            // Test if there is downloaded file
            DownloadCsvTest(_linkPageInequalitiesId);
        }

        [TestMethod]
        public void TestCallFromEnglandToEndpointForDownloadCsvFileSuccess()
        {
            // Arrange
            navigateTo.FingertipsEngland();

            // Test if there is downloaded file
            DownloadCsvTest(_linkPageEnglandId);
        }

        [TestMethod]
        public void TestCallFromPopulationToEndpointForDownloadCsvFileSuccess()
        {
            // Arrange
            navigateTo.FingertipsPopulation();

            // Test if there is downloaded file
            DownloadCsvTest(_linkPagePopulationId);
        }

        private void DownloadCsvTest(string pageLinkTagId)
        {
            try
            {
                SelectAreaTypeDistrictAndUA();

                // Give time to load the page
                WaitFor.ThreadWait(_waitInterval);

                // Act
                findExportLinkCsv(pageLinkTagId);
            }
            catch (Exception e)
            {
                Assert.Fail("The download has not been initialized:" + e.Message);
            }
        }

        private void SelectAreaTypeDistrictAndUA()
        {
            var areaTypeList = FingertipsHelper.FindElementById(driver, "areaTypes");

            var selectElement = new SelectElement(areaTypeList);

            selectElement.SelectByText("District & UA");
        }

        private void findExportLinkCsv(string idTagName)
        {
            var link = By.Id(idTagName);
            waitFor.ExpectedElementToBeVisible(link);
            driver.FindElement(link);
        }
    }
}