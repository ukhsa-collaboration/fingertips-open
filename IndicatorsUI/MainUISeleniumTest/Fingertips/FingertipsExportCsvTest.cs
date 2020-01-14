using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    public static class ExportFilenames
    {
        public const string AreaProfiles = "Area_profiles.csv";
        public const string England = "England.csv";
        public const string Inequalities = "Inequalities.csv";
        public const string CompareAreas = "Compare_areas.csv";
        public const string CompareIndicators = "Compare_indicators.csv"; 
        public const string Trends = "Trends.csv";
        public const string Map = "Map.csv";
        public const string DistrictUA = "indicators-DistrictUApre419.data.csv";
    }

    [TestClass]
    public class FingertipsExportCsvTest : FingertipsBaseUnitTest
    {
        private const string LinkPageTartanId = "export-link-csv-tartan";
        private const string LinkPageScatterId = "export-link-csv-scatter";
        private const string LinkPageMapId = "export-link-csv-map";
        private const string LinkPageTrendsId = "export-link-csv-trend";
        private const string LinkPageCompareAreasId = "export-link-csv-compare-areas";
        private const string LinkPageAreaProfilesId = "export-link-csv-area-profile";
        private const string LinkPageInequalitiesId = "export-link-csv-inequalities";
        private const string LinkPageEnglandId = "export-link-csv-england";
        private const string LinkPagePopulationId = "export-link-csv-population";

        private const string IndicatorWithChildAreaInequalityData = "Hip fractures in people aged 65 and over";

        private IList<CsvRow> _rows;
        private HashParameters _parameters;

        [TestInitialize]
        public void TestInitialize()
        {
            DownloadFileHelper.DeleteAllFilesInDownloadDir();
            _parameters = new HashParameters();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            DownloadFileHelper.DeleteAllFilesInDownloadDir();
            _rows = null;
        }

        [TestMethod]
        public void Test_Csv_Export_Overview()
        {
            // Arrange
            SetBroxbourneDistrictUA();
            _parameters.AddTabId(TabIds.Overview);
            GoToUrl();

            CheckLinkAndDownloadFile(LinkPageTartanId, ExportFilenames.DistrictUA);

            // Assert: check CSV contents
            AssertContainsEnglandData();
            AssertContainsAreaCode(AreaCodes.Broxbourne);
            AssertMoreThanOneIndicator();
            AssertMoreThanOneTimePeriod();
        }

        [TestMethod]
        public void Test_Csv_Export_Compare_Indicators()
        {
            // Arrange
            SetBroxbourneDistrictUA();
            _parameters.AddTabId(TabIds.CompareIndicators);
            GoToUrl();
            fingertipsHelper.SelectCompareIndicatorsSupportingIndicator("percentage of physically inactive");

            CheckLinkAndDownloadFile(LinkPageScatterId, ExportFilenames.CompareIndicators);

            // Assert: check CSV contents
            AssertContainsAreaCode(AreaCodes.Broxbourne);
            AssertIndicatorCountEquals(2);
        }

        [TestMethod]
        public void Test_Csv_Export_Map()
        {
            // Arrange
            SetBroxbourneDistrictUA();
            _parameters.AddTabId(TabIds.Map);
            GoToUrl();

            CheckLinkAndDownloadFile(LinkPageMapId, ExportFilenames.Map);

            // Assert: check CSV contents
            AssertContainsAreaCode(AreaCodes.Broxbourne);
            AssertIndicatorCountEquals(1);
            AssertOneTimePeriod();
        }

        [TestMethod]
        public void Test_Csv_Export_Trends()
        {
            // Arrange
            SetBroxbourneDistrictUA();
            _parameters.AddTabId(TabIds.Trends);
            GoToUrl();

            CheckLinkAndDownloadFile(LinkPageTrendsId, ExportFilenames.Trends);

            // Assert: check CSV contents
            AssertContainsEnglandData();
            AssertContainsAreaCode(AreaCodes.Broxbourne);
            AssertIndicatorCountEquals(1);
            AssertMoreThanOneTimePeriod();
        }

        [TestMethod]
        public void Test_Csv_Export_Compare_Areas()
        {
            // Arrange
            SetBroxbourneDistrictUA();
            _parameters.AddTabId(TabIds.CompareAreas);
            GoToUrl();

            CheckLinkAndDownloadFile(LinkPageCompareAreasId, ExportFilenames.CompareAreas);

            // Assert: check CSV contents
            AssertContainsEnglandData();
            AssertContainsAreaCode(AreaCodes.Broxbourne);
            AssertIndicatorCountEquals(1);
            AssertOneTimePeriod();
        }

        [TestMethod]
        public void Test_Csv_Export_Area_Profiles()
        {
            // Arrange
            SetBroxbourneDistrictUA();
            _parameters.AddTabId(TabIds.AreaProfile);
            GoToUrl();

            CheckLinkAndDownloadFile(LinkPageAreaProfilesId, ExportFilenames.AreaProfiles);

            // Assert: check CSV contents
            AssertContainsEnglandData();
            AssertContainsAreaCode(AreaCodes.Broxbourne);
            AssertMoreThanOneIndicator();
            AssertMoreThanOneTimePeriod();
        }

        [TestMethod]
        public void Test_Csv_Export_Inequalities_Recent_Values_England()
        {
            // Go to inequalities tab
            SetBroxbourneDistrictUA();
            GoToInequalitiesTab();

            CheckLinkAndDownloadFile(LinkPageInequalitiesId, ExportFilenames.Inequalities);

            // Assert: check CSV contents
            AssertContainsEnglandData();
            AssertIndicatorCountEquals(1);
            AssertOneTimePeriod();
        }

        [TestMethod]
        public void Test_Csv_Export_Inequalities_Recent_Values_Child_Area()
        {
            // Go to inequalities tab
            SetBroxbourneDistrictUA();
            GoToInequalitiesTab();
            fingertipsHelper.SelectInequalitiesForChildArea();
            fingertipsHelper.SelectIndicatorByName(IndicatorWithChildAreaInequalityData);

            CheckLinkAndDownloadFile(LinkPageInequalitiesId, ExportFilenames.Inequalities);

            // Assert: check CSV contents
            AssertContainsAreaCode(AreaCodes.Broxbourne);
            AssertIndicatorCountEquals(1);
            AssertOneTimePeriod();
        }

        [TestMethod]
        public void Test_Csv_Export_Inequalities_Trends_England()
        {
            // Go to inequalities tab
            SetBroxbourneDistrictUA();
            GoToInequalitiesTab();
            fingertipsHelper.SelectInequalitiesTrends();

            CheckLinkAndDownloadFile(LinkPageInequalitiesId, ExportFilenames.Inequalities);

            // Assert: check CSV contents
            AssertContainsEnglandData();
            AssertIndicatorCountEquals(1);
            AssertMoreThanOneTimePeriod();
        }

        [TestMethod]
        public void Test_Csv_Export_Inequalities_Trends_Child_Area()
        {
            // Go to inequalities tab
            SetBroxbourneDistrictUA();
            GoToInequalitiesTab();

            // Select options
            fingertipsHelper.SelectInequalitiesTrends();
            WaitFor.ThreadWaitInSeconds(2);

            fingertipsHelper.SelectIndicatorByName(IndicatorWithChildAreaInequalityData);
            WaitFor.ThreadWaitInSeconds(2);

            fingertipsHelper.SelectInequalitiesForChildArea();

            CheckLinkAndDownloadFile(LinkPageInequalitiesId, ExportFilenames.Inequalities);

            // Assert: check CSV contents
            AssertContainsAreaCode(AreaCodes.Broxbourne);
            AssertIndicatorCountEquals(1);
            AssertMoreThanOneTimePeriod();
        }

        [TestMethod]
        public void Test_Csv_Export_England()
        {
            // Arrange
            _parameters.AddAreaTypeId(AreaTypeIds.England);
            _parameters.AddTabId(TabIds.England);
            GoToUrl();

            CheckLinkAndDownloadFile(LinkPageEnglandId, ExportFilenames.England);

            // Assert: check CSV contents
            AssertContainsEnglandData();
            AssertMoreThanOneIndicator();
            AssertMoreThanOneTimePeriod();
        }

        [TestMethod]
        public void Test_Csv_Export_Population()
        {
            // Arrange
            SetBroxbourneDistrictUA();
            _parameters.AddTabId(TabIds.Population);
            GoToUrl();

            CheckLinkAndDownloadFile(LinkPagePopulationId, ExportFilenames.DistrictUA);

            // Assert: check CSV contents
            AssertContainsEnglandData();
            AssertContainsAreaCode(AreaCodes.Broxbourne);
            AssertIndicatorCountEquals(1);
            AssertOneTimePeriod();
        }

        private void CheckLinkAndDownloadFile(string linkId, string fileName)
        {
            // Need to wait for Jenkins
            WaitFor.ThreadWaitInSeconds(5);

            // Check link is present
            var link = WaitUntilLinkIsVisible(linkId);
            link.Click();
            DownloadCsvFile(linkId, fileName);
        }

        private void ScrollToPageTop()
        {
            /* Scroll to top of page to avoid error:
            OpenQA.Selenium.ElementNotInteractableException: 
            Element <button id="inequalities-tab-option-3" class=""> could not be scrolled into view
             */
            fingertipsHelper.ScrollVertically(-1000);
        }

        private void SetBroxbourneDistrictUA()
        {
            _parameters.AddAreaTypeId(AreaTypeIds.DistrictAndUAPreApr2019);
            _parameters.AddParentAreaCode(AreaCodes.RegionEastOfEngland);
            _parameters.AddAreaCode(AreaCodes.Broxbourne);
        }

        private void GoToInequalitiesTab()
        {
            _parameters.AddTabId(TabIds.Inequalities);
            GoToUrl();
            waitFor.InequalitiesTabToLoad();
            ScrollToPageTop();
        }

        private IWebElement WaitUntilLinkIsVisible(string linkId)
        {
            var link = fingertipsHelper.FindElementById(linkId);
            waitFor.ExpectedElementToBeVisible(By.Id(linkId));
            return link;
        }

        private void DownloadCsvFile(string linkId, string fileName)
        {
            // Download CSV file
            var link = WaitUntilLinkIsVisible(linkId);
            link.Click();
            _rows = DownloadFileHelper.DownloadCsvFile(fileName);

            // Check file contents
            if (_rows == null)
            {
                Assert.Fail("CSV file was not downloaded");
            }
            Assert.IsTrue(_rows.Any(), "File contains no data rows");
        }

        private void AssertContainsEnglandData()
        {
            AssertContainsAreaCode(AreaCodes.England);
        }

        private void AssertMoreThanOneIndicator()
        {
            Assert.IsTrue(GetIndicatorCount() > 1);
        }

        private void AssertIndicatorCountEquals(int expectedCount)
        {
            Assert.AreEqual(expectedCount, GetIndicatorCount());
        }

        private int GetIndicatorCount()
        {
            return _rows.Select(x => x.IndicatorId).Distinct().Count();
        }

        private void AssertOneTimePeriod()
        {
            Assert.AreEqual(1, GetTimePeriodCount());
        }

        private void AssertMoreThanOneTimePeriod()
        {
            Assert.IsTrue(GetTimePeriodCount() > 1);
        }

        private int GetTimePeriodCount()
        {
            return _rows.Select(x => x.TimePeriod).Distinct().Count();
        }

        private void AssertContainsAreaCode(string areaCode)
        {
            var areaCodes = _rows.Select(x => x.AreaCode).Distinct();
            Assert.IsTrue(areaCodes.Contains(areaCode), "Area code not found:" + areaCode);
        }

        private void GoToUrl()
        {
            navigateTo.GoToUrl(ProfileUrlKeys.DevelopmentProfileForTesting + _parameters.HashParameterString);
            waitFor.PageToFinishLoading();
            waitFor.AjaxLockToBeUnlocked();
        }
    }
}