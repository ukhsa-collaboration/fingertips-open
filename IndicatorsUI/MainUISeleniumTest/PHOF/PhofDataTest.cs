using IndicatorsUI.MainUISeleniumTest.Fingertips;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Profiles.DomainObjects;

namespace IndicatorsUI.MainUISeleniumTest.Phof
{
    [TestClass]
    public class PhofDataTest : PhofBaseUnitTest
    {
        public const string UrlKey = ProfileUrlKeys.Phof;

        [TestMethod]
        public void TestTartanRugLoadsForAllDomains()
        {
            navigateTo.PhofDataPage();
            FingertipsDataTest.CheckTartanRugLoadsForAllDomains(driver);
        }

        [TestMethod]
        public void TestAllTabsLoad()
        {
            navigateTo.PhofDataPage();
            FingertipsHelper.SelectEachFingertipsTabInTurnAndCheckDownloadIsLast(driver);
        }

        [TestMethod]
        public void TestInequalitiesChartLoadsForEachIndicatorForAllDomains()
        {
            navigateTo.PhofDataPage();
            FingertipsDataTest.SelectInequalitiesTab(driver);

            var domains = FingertipsDataTest.GetDomainOptions(driver);
            foreach (var domain in domains)
            {
                FingertipsDataTest.SelectDomain(domain, waitFor);

                var nextIndicatorButton = driver.FindElement(By.Id("next-indicator"));

                // Make sure it is possible to view each indicator until get back to the first one
                string initialHeaderText = GetInequalityIndicatorName();
                string headerText = string.Empty;
                while (headerText != initialHeaderText)
                {
                    FingertipsHelper.SelectNextIndicator(nextIndicatorButton, waitFor);
                    headerText = GetInequalityIndicatorName();
                }
            }
        }

        [TestMethod]
        public void TestLastSelectedAreaIsRetainedBetweenPageViews()
        {
            navigateTo.PhofDataPage();
            var areaName = "Middlesbrough";

            FingertipsHelper.SwitchToAreaSearchMode(driver);
            FingertipsHelper.SearchForAnArea(driver, areaName);
            FingertipsHelper.LeaveAreaSearchMode(driver);

            // Leave and return to data page
            navigateTo.PhofHomePage();
            navigateTo.PhofDataPage();

            // Check area menu contains searched for area
            Assert.AreEqual(areaName, FingertipsHelper.GetSelectedAreaNameFromMenu(driver));
        }

        [TestMethod]
        public void TestSearchForAnArea()
        {
            navigateTo.PhofDataPage();
            var areaName = "Croydon";

            FingertipsHelper.SwitchToAreaSearchMode(driver);
            FingertipsHelper.SearchForAnArea(driver, areaName);
            FingertipsHelper.LeaveAreaSearchMode(driver);

            // Check area menu contains searched for area
            Assert.AreEqual(areaName, FingertipsHelper.GetSelectedAreaNameFromMenu(driver));
        }

        [TestMethod]
        public void TestAreaCodeCanBeBookmarked()
        {
            var parameters = new HashParameters();
            parameters.AddAreaCode(AreaCodes.Hartlepool);
            parameters.AddAreaTypeId(AreaTypeIds.CountyAndUnitaryAuthority);
            navigateTo.GoToUrl(UrlKey + parameters.HashParameterString);
            waitFor.FingertipsTartanRugToLoad();

            // Check area menu contains searched for area
            Assert.AreEqual("Hartlepool", FingertipsHelper.GetSelectedAreaNameFromMenu(driver));
        }

        [TestMethod]
        public void TestIndicatorAndSexAndAgeCanBeBookmarked()
        {
            var parameters = new HashParameters();
            parameters.AddAreaTypeId(AreaTypeIds.CountyAndUnitaryAuthority);
            parameters.AddIndicatorId(IndicatorIds.GapInLifeExpectancyAtBirth);
            parameters.AddSexId(SexIds.Female);
            parameters.AddAgeId(AgeIds.AllAges);
            parameters.AddTabId(TabIds.BarChart);

            navigateTo.GoToUrl(UrlKey + parameters.HashParameterString);
            waitFor.FingertipsBarChartTableToLoad();

            // Check area menu contains searched for area
            var text = driver.FindElement(By.Id("indicatorDetailsHeader")).Text;
            TestHelper.AssertTextContains(text, "gap in life expectancy at birth");
            TestHelper.AssertTextContains(text, "(Female)");
        }

        [TestMethod]
        public void TestChangingAreaTypeRepopulatesButRetainsSelectedSupportingIndicator()
        {
            var parameters = new HashParameters();
            parameters.AddAreaTypeId(AreaTypeIds.CountyAndUnitaryAuthority);
            parameters.AddIndicatorId(IndicatorIds.GapInLifeExpectancyAtBirth);
            parameters.AddSexId(SexIds.Persons);
            parameters.AddAgeId(AgeIds.AllAges);
            parameters.AddTabId(TabIds.ScatterPlot);
            navigateTo.GoToUrl(UrlKey + parameters.HashParameterString);
            waitFor.FingertipsScatterPlotChartToLoad();
            var countyUaAreaCount = driver.FindElements(By.CssSelector("#supportingIndicators option"));

            //Set supporting indicator
            driver.FindElement(By.CssSelector("div.chosen-container a.chosen-single")).Click();
            var searchText = driver.FindElement(By.CssSelector("div.chosen-search input"));
            searchText.SendKeys("pupil absence");
            searchText.SendKeys(Keys.Return);

            //Change the area type from CountyUa to District
            var areaTypeDropdown = driver.FindElement(By.Id("areaTypes"));
            SelectElement clickThis = new SelectElement(areaTypeDropdown);
            clickThis.SelectByText("District & UA");
            waitFor.FingertipsScatterPlotChartToLoad();
            var distictUaAreaCount = driver.FindElements(By.CssSelector("#supportingIndicators option"));

            var selectedSupportinIndicator = driver.FindElement(By.CssSelector("div.chosen-container a.chosen-single span"));

            Assert.AreNotEqual(countyUaAreaCount, distictUaAreaCount);
            TestHelper.AssertTextContains(selectedSupportinIndicator.Text, "Pupil absence");
        }

        [TestMethod]
        public void TestPdfLinkIsDisplayedForCountyUa()
        {
            // Navigate to PHOF data page for county & UA
            navigateTo.PhofDataPage();
            waitFor.FingertipsTartanRugToLoad();

            // Select download tab
            FingertipsHelper.SelectFingertipsTab(driver, FingertipsIds.TabDownload);
            waitFor.ExpectedElementToBeVisible(By.Id("pdf-download-text"));

            // Assert PDF message is correct
            var text = driver.FindElement(By.Id("pdf-download-text")).Text;
            TestHelper.AssertTextContains(text, "Download a detailed profile", "");

            // Assert PDF link is visible
            waitFor.ExpectedElementToBeVisible(By.ClassName("pdf"));
        }

        [TestMethod]
        public void TestPdfsNotAvailableMessageIsDisplayedForDistrictUa()
        {
            // Navigate to PHOF data page for district & UA
            var parameters = new HashParameters();
            parameters.AddAreaTypeId(AreaTypeIds.DistrictAndUnitaryAuthority);
            navigateTo.GoToUrl(UrlKey + parameters.HashParameterString);
            waitFor.FingertipsTartanRugToLoad();

            // Select download tab
            FingertipsHelper.SelectFingertipsTab(driver, FingertipsIds.TabDownload);
            waitFor.ExpectedElementToBeVisible(By.Id("pdf-download-text"));

            // Assert no PDF message is displayed
            var text = driver.FindElement(By.Id("pdf-download-text")).Text;
            TestHelper.AssertTextContains(text, "PDF profiles are not currently available for District & UA");
        }

        private string GetInequalityIndicatorName()
        {
            return driver.FindElement(By.ClassName("trendLink")).Text;
        }
    }
}
