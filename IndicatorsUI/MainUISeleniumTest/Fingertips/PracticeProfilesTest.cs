using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Profiles.DomainObjects;
using System.Linq;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class PracticeProfilesTest : FingertipsBaseUnitTest
    {
        [TestMethod]
        public void TestAllTabsLoad()
        {
            GoToPracticeProfiles();
            
            var htmlOfLastTab = FingertipsHelper.SelectEachFingertipsTabInTurn(driver);
            TestHelper.AssertTextContains(htmlOfLastTab, "Definition");
        }

        [TestMethod]
        public void TestPracticeProfilesSpineChartLoads()
        {
            GoToPracticeProfiles();

            ClickSpineChartTab();

            waitFor.PracticeProfilesSpineChartsTabToLoad();

            // Check practice summary indicator table has loaded
            var text = driver.FindElement(By.Id("spineTableBox")).Text;
            TestHelper.AssertTextContains(text, "deprivation");
        }

        [TestMethod]
        public void TestPracticeProfilesBarChartLoads()
        {
            GoToPracticeProfiles();

            SearchForAndGoToPractice(AreaCodes.Cambridge);

            driver.FindElement(By.Id("page-bar")).Click();
            waitFor.PracticeProfilesBarChartsTabToLoad();

            // Check sort practice box is displayed
            waitFor.ExpectedElementToBeVisible(By.Id("sortPracticeBox"));
        }

        [TestMethod]
        public void TestEthnicitySummaryIsPresent()
        {
            GoToPracticeProfiles();

            SearchForAndGoToPractice(AreaCodes.Cambridge);

            // Check ethnicity summary
            var text = driver.FindElement(By.Id("ethnicity")).Text;
            TestHelper.AssertTextContains(text, "asian");
            TestHelper.AssertTextContains(text, "non-white");
        }

        [TestMethod]
        public void TestPopulationChartExportLinkIsPresent()
        {
            GoToPracticeProfiles();

            SearchForAndGoToPractice(AreaCodes.Cambridge);

            // Check chart export menu
            waitFor.ExpectedElementToBeVisible((By.ClassName("export-link")));
            var exportLink = driver.FindElement(By.ClassName("export-link"));
            waitFor.ElementToContainText(exportLink, "Export");
        }

        [TestMethod]
        public void TestBarChartExportLinkIsPresent()
        {
            GoToPracticeProfiles();

            SearchForAndGoToPractice(AreaCodes.Cambridge);

            driver.FindElement(By.Id("page-bar")).Click();

            // Check chart export menu
            var exportLink = driver.FindElements(By.ClassName("export-link"));

            waitFor.ElementToContainText(exportLink[0], "Export");
        }

        private void ClickSpineChartTab()
        {
            driver.FindElement(By.Id("page-indicators")).Click();
        }

        private void GoToPracticeProfiles()
        {
            navigateTo.FingertipsDataForPracticeProfiles(ProfileUrlKeys.PracticeProfiles);
            WaitFor.ThreadWait(1); 
            waitFor.PracticeProfilesSearchTabToLoad();
        }

        private void SearchForAndGoToPractice(string area)
        {
            waitFor.PageToFinishLoading();

            var searchBox = driver.FindElement(By.Id("searchText"));          
            searchBox.Click();
            searchBox.SendKeys(area);
            waitFor.AutoCompleteSearchResultsToBeDisplayed();
            searchBox.SendKeys(Keys.Return);

            // Wait for search results
            var resultById = By.Id("D81056-address");
            waitFor.ExpectedElementToBeVisible(resultById);

            // Click on search result for the practice
            driver.FindElement(resultById).FindElements(By.TagName("a")).First().Click();

            waitFor.PracticeProfilesSummaryTabToLoad();
        }
    }
}
