using MainUISeleniumTest.Fingertips;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Profiles.DomainObjects;

namespace MainUISeleniumTest.Phof
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

        private string GetInequalityIndicatorName()
        {
            return driver.FindElement(By.ClassName("trendLink")).Text;
        }
    }
}
