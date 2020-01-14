using System.Linq;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsTabAreaProfilesTest : FingertipsBaseUnitTest
    {
        // Increase this number if test is failing because slow refreshing
        private const double RefreshingPanelIntervalTime = 1;

        private const int ElementsNumberDisplayedInHeader = 6;
        private const int ElementsNumberDisplayedInBody = 5;

        [TestInitialize]
        public void TestInitialize()
        {
            navigateTo.AreaProfilesTab();
            waitFor.FingertipsSpineChartToLoad();

            // Give time to hide the elements into the panel
            WaitFor.ThreadWaitInSeconds(RefreshingPanelIntervalTime);
            waitFor.AjaxLockToBeUnlocked();
        }

        [TestMethod]
        public void Test_Table_Is_Not_Comparing_With_It_Self_And_England_Area_Type_Panel_Is_Displayed_Correctly()
        {
            fingertipsHelper.SelectAreaType(AreaTypeIds.England);

            // Assert
            CheckAreasGroupedByIsHide();
            CheckBenchmarkIsHide();
            CheckAreasOfAreaTypeIsHide();
            CheckRegionsIsHide();
            CheckTableDisplaySixHeadersOnly();
        }

        [TestMethod]
        public void Clicking_Recent_Trends_Icon_Navigates_To_Trends_Tab()
        {
            fingertipsHelper.SelectAreaType(AreaTypeIds.Region);

            // Click on recent trend cell
            driver.FindElement(By.Id("single-area-table"))
                .FindElement(By.TagName("tbody"))
                .FindElements(By.TagName("tr")).First()
                .FindElements(By.TagName("td"))[2].Click();

            // Success if trends table loads
            waitFor.FingertipsTrendsTableToLoad();
        }

        [TestMethod]
        public void Spine_Chart_Tooltips_Are_Displayed()
        {
            fingertipsHelper.SelectAreaType(AreaTypeIds.Region);

            // Move pointer over spine chart elements and check tooltip text
            // Note: can't do q2 as average line is on top
            CheckTooltipForElement("marker", "region");
            CheckTooltipForElement("q1", "lowest");
            CheckTooltipForElement("q4", "highest");
            CheckTooltipForElement("average250", "england");
        }

        private void CheckTooltipForElement(string className, string text)
        {
            var html = fingertipsHelper.MoveOverElementAndGetTooltipText(By.ClassName(className));
            TestHelper.AssertTextContains(html, text);
        }

        private void CheckAreasGroupedByIsHide()
        {
            fingertipsHelper.CheckElementDisplayed("parentTypeBox", false, "Areas grouped by dropdown box cannot be displayed");
        }

        private void CheckBenchmarkIsHide()
        {
            fingertipsHelper.CheckElementDisplayed("benchmark-box", false, "Benchmark dropdown box cannot be displayed");
        }

        private void CheckAreasOfAreaTypeIsHide()
        {
            fingertipsHelper.CheckElementDisplayed("areaMenuBox", false, "Areas dropdown box cannot be displayed");
        }

        private void CheckRegionsIsHide()
        {
            fingertipsHelper.CheckElementDisplayed("region-menu-box", false, "Regions dropdown box cannot be displayed");
        }

        private void CheckTableDisplaySixHeadersOnly()
        {
            fingertipsHelper.CheckElementDisplayed("single-area-table", true, "The single-area-table should be displayed");

            // Compare the number of elements that it should be shown in the header
            var asd = fingertipsHelper.FindElementById("single-area-table").FindElements(By.TagName("th")).Count;
            if ( asd != ElementsNumberDisplayedInHeader)
            {
                Assert.Fail("Header should contains only"+ ElementsNumberDisplayedInHeader + " elements");
            }

            // The rest of elements number divided by elements, expected in one row, should be 0
            if (fingertipsHelper.FindElementById("single-area-table").FindElements(By.TagName("td")).Count % ElementsNumberDisplayedInBody != 0)
            {
                Assert.Fail("Body should contains only "+ ElementsNumberDisplayedInBody + " elements");
            }
        }
    }
}