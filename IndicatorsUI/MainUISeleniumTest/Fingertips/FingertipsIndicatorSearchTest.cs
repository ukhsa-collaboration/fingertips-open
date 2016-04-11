using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

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
    }
}
