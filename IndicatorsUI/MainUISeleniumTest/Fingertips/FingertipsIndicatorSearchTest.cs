using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsIndicatorSearchTest : FingertipsBaseUnitTest
    {
        [TestMethod]
        public void Test_Search_Results()
        {
            CheckSearchFindsSomeIndicators(driver, "hip");
        }

        [TestMethod]
        public void Check_Search_Indicators_Per_Profiles_Pop_Up_Displays()
        {
            CheckSearchIndicatorsPerProfilesPopUpDisplays(driver, "hip");
        }

        [TestMethod]
        public void Test_No_Search_Results_Found()
        {
            CheckSearchFindsNoMatchingIndicators(driver, "zxzxzxzx");
        }

        [TestMethod]
        public void Test_All_Tabs_Load_For_Search_Results()
        {
            navigateTo.FingertipsIndicatorSearchResults("falls");
            waitFor.FingertipsOverviewTabToLoad();

            fingertipsHelper.SelectEachFingertipsTabInTurnAndCheckDownloadIsLast();
        }

        [TestMethod]
        public void Test_Area_Types_With_No_Results_Are_Not_Displayed()
        {
            navigateTo.FingertipsIndicatorSearchResults("hiv");
            waitFor.FingertipsOverviewTabToLoad();

            var html = driver.FindElement(By.Id("searchResultText")).Text;
            Assert.IsTrue(html.Contains("Region"));
            Assert.IsFalse(html.Contains("Acute"), "Results for acute trust not expected");
        }

        [TestMethod]
        public void Check_Search_Finds_Indicators()
        {
            CheckSearchFindsSomeIndicators(driver, "smoking");
        }

        public void CheckSearchFindsSomeIndicators(IWebDriver driver, string searchText)
        {
            new NavigateTo(driver).FingertipsIndicatorSearchResults(searchText);
            new WaitFor(driver).FingertipsOverviewTabToLoad();
            fingertipsHelper.CheckTartanRugHasLoaded();          
        }

        public static void CheckSearchIndicatorsPerProfilesPopUpDisplays(IWebDriver driver, string searchText)
        {
            new NavigateTo(driver).FingertipsIndicatorSearchResults(searchText);
            new WaitFor(driver).FingertipsOverviewTabToLoad();
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
