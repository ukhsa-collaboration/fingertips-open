using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsIndicatorSearchTest : FingertipsBaseUnitTest
    {
        [TestMethod]
        public void TestSearchResults()
        {
            CheckSearchFindsSomeIndicators(driver, "hip");
        }

        [TestMethod]
        public void CheckSearchIndicatorsPerProfilesPopUpDisplays()
        {
            CheckSearchIndicatorsPerProfilesPopUpDisplays(driver, "hip");
        }

        [TestMethod]
        public void TestNoSearchResultsFound()
        {
            CheckSearchFindsNoMatchingIndicators(driver, "zxzxzxzx");
        }

        [TestMethod]
        public void TestAllTabsLoadForSearchResults()
        {
            navigateTo.FingertipsIndicatorSearchResults("falls");
            waitFor.FingertipsTartanRugToLoad();

            FingertipsHelper.SelectEachFingertipsTabInTurnAndCheckDownloadIsLast(driver);
        }

        [TestMethod]
        public void TestAreaTypesWithNoResultsAreNotDisplayed()
        {
            navigateTo.FingertipsIndicatorSearchResults("hiv");
            waitFor.FingertipsTartanRugToLoad();

            var html = driver.FindElement(By.Id("searchResultText")).Text;
            Assert.IsTrue(html.Contains("Region"));
            Assert.IsFalse(html.Contains("Acute"), "Results for acute trust not expected");
        }

        public static void CheckSearchFindsSomeIndicators(IWebDriver driver, string searchText)
        {
            new NavigateTo(driver).FingertipsIndicatorSearchResults(searchText);
            new WaitFor(driver).FingertipsTartanRugToLoad();
            CheckTartanRugHasLoaded(driver);          
        }

        public static void CheckSearchIndicatorsPerProfilesPopUpDisplays(IWebDriver driver, string searchText)
        {
            new NavigateTo(driver).FingertipsIndicatorSearchResults(searchText);
            new WaitFor(driver).FingertipsTartanRugToLoad();
            CheckProfilesPerIndicatorLinkExists(driver);
            CheckProfilesPerIndicatorPopup(driver);
        }

        public static void CheckSearchFindsNoMatchingIndicators(IWebDriver driver, string searchText)
        {
            new NavigateTo(driver).FingertipsIndicatorSearchResults(searchText);
            new WaitFor(driver).SearchResultNotFoundToLoad();

            var text = driver.FindElement(By.ClassName(Classes.NoSearchResultMessage)).Text;
            TestHelper.AssertTextContains(text, "no matching indicators");
            TestHelper.AssertTextContains(text, searchText);
        }

        public static void CheckProfilesPerIndicatorPopup(IWebDriver driver)
        {
            // Click show me which profiles these indicators are in
            var profilePerIndicator = driver
                .FindElement(By.Id(FingertipsIds.ProfilePerIndicator))
                .FindElement(By.TagName("a"));
            profilePerIndicator.Click();

            // Wait for indicator menu to be visible in pop up
            var byIndicatorMenu = By.Id(FingertipsIds.ListOfIndicators);
            new WaitFor(driver).ExpectedElementToBeVisible(byIndicatorMenu);

            // Select 2nd indicator in list
            var listOfIndicators = driver.FindElement(byIndicatorMenu);
            var selectMenu = new SelectElement(listOfIndicators);
            selectMenu.SelectByIndex(1);

            // Check list of profiles is displayed
            var listOfProfiles = driver.FindElements(By.XPath(XPaths.ListOfProfilesInPopup));
            Assert.IsTrue(listOfProfiles.Count > 0);
        }

        public static void CheckProfilesPerIndicatorLinkExists(IWebDriver driver)
        {
            var profilePerIndicatorLink = driver
                .FindElement(By.Id(FingertipsIds.ProfilePerIndicator))
                .FindElement(By.TagName("a"));
            Assert.IsTrue(profilePerIndicatorLink != null);
        }
    }
}
