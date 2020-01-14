using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Linq;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsAreaSearchTest : FingertipsBaseUnitTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            // Because area search is displayed on introduction page
            OpenProfilePage(ProfileUrlKeys.Phof);
        }

        [TestMethod]
        public void Test_Search_For_Area()
        {
            navigateTo.PhofOverviewTab();
            var areaName = "Croydon";

            fingertipsHelper.SwitchToAreaSearchMode();
            fingertipsHelper.SearchForAnAreaAndSelectFirstResult(areaName);
            fingertipsHelper.LeaveAreaSearchMode();

            // Check area menu contains searched for area
            Assert.AreEqual(areaName, fingertipsHelper.GetSelectedAreaNameFromMenu());
        }

        [TestMethod]
        public void Test_Results_Displayed_Searching_For_PlaceName()
        {
            SearchForAnAreaAndSelectFirstResult("Keyworth");

            // Assert expected results are displayed
            var resultsHtml = GetResultsHtml();
            TestHelper.AssertTextContains(resultsHtml, "Rushcliffe", "Rushcliffe should be displayed as a district result");
            TestHelper.AssertTextContains(resultsHtml, "Nottinghamshire", "Nottinghamshire should be displayed as a county result");
        }

        [TestMethod]
        public void Test_Time_Period_Options_Are_Displayed_On_Results_Page()
        {
            OpenProfilePage(ProfileUrlKeys.MarmotIndicatorsForLocalAuthorities);

            SelectEastOfEnglandInRegionMenu();

            // Assert time period buttons are displayed
            var buttons = driver.FindElement(By.Id("tab-specific-options")).FindElements(By.TagName("button"));
            Assert.IsTrue(buttons.Any());
        }

        [TestMethod]
        public void Test_Results_Displayed_Searching_For_Parent_Area()
        {
            SearchForAnAreaAndSelectFirstResult("Cumbria");

            // Assert expected results are displayed
            var resultsHtml = GetResultsHtml();
            TestHelper.AssertTextContains(resultsHtml, "Allerdale", 
                "Allerdale should be displayed as a district result");
            TestHelper.AssertTextContains(resultsHtml, "Cumbria", 
                "Derbyshire should be displayed as a county result");
        }

        /// <summary>
        /// Erewash is only a district name and not a place name.
        /// </summary>
        [TestMethod]
        public void Test_Districts_Are_Included_In_Search_Result()
        {
            SearchForAnAreaAndSelectFirstResult("erewash");

            // Assert expected results are displayed
            var resultsHtml = GetResultsHtml();
            TestHelper.AssertTextContains(resultsHtml, "Erewash", "Erewash should be displayed as a district result");
        }

        [TestMethod]
        public void Test_Districts_Can_Be_Navigated_To_From_Search_Results()
        {
            SearchForAnAreaAndSelectFirstResult("erewash");

            // Navigate to Fingertips data page
            driver.FindElement(By.LinkText("View data")).Click();
            waitFor.FingertipsSpineChartToLoad();

            // Assert district is selected
            var menu = driver.FindElement(By.Id(FingertipsIds.AreaMenu));
            var text = new SelectElement(menu).SelectedOption.Text;
            Assert.AreEqual(text, "Erewash", "Erewash should be selected on tartan rug page");
        }

        [TestMethod]
        public void Test_No_Results_Found_For_Ignored_Area()
        {
            OpenProfilePage(ProfileUrlKeys.HealthProfiles);
            fingertipsHelper.SearchForAnAreaAndSelectFirstResult("Scilly");
            waitFor.FingertipsAreaSearchResultsPageToLoad();

            // Assert no data message is displayed
            waitFor.ExpectedElementToBeVisible(By.Id("no-data-message"));
        }

        [TestMethod]
        public void Test_Search_Results_Can_Be_Navigated_To_From_Region_Menu()
        {
            SelectEastOfEnglandInRegionMenu();

            // Assert expected results are displayed
            var resultsHtml = GetResultsHtml();
            TestHelper.AssertTextContains(resultsHtml, "Norfolk", "Norfolk should be displayed as a county result");
            TestHelper.AssertTextContains(resultsHtml, "Broxbourne", "Broxbourne should be displayed as a district result");
        }

        private void OpenProfilePage(string profileUrlKey)
        {
            navigateTo.FingertipsFrontPageForProfile(profileUrlKey);
            waitFor.ElementToContainText(GetRegionMenu(), "London");
        }

        private void SelectEastOfEnglandInRegionMenu()
        {
            new SelectElement(GetRegionMenu()).SelectByValue(AreaCodes.RegionEastOfEngland);
            waitFor.FingertipsAreaSearchResultsPageToLoad();
        }

        private IWebElement GetRegionMenu()
        {
            var regionMenu = driver.FindElement(By.Id("parent-area-menu"));
            return regionMenu;
        }

        private void SearchForAnAreaAndSelectFirstResult(string place)
        {
            // Search for a county
            fingertipsHelper.SearchForAnAreaAndSelectFirstResult(place);
            waitFor.FingertipsAreaSearchResultsPageToLoad();
        }

        private string GetResultsHtml()
        {
            var resultsHtml = driver.FindElement(By.Id("area-results-box")).Text;
            return resultsHtml;
        }
    }
}
