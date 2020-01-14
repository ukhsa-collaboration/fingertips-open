using System.Linq;
using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsTabTrendsForAreaTypeEnglandTest : FingertipsBaseUnitTest
    {
        // Increase this number if test is failing because slow refreshing
        private double refreshingPanelIntervalTime = 0.5;

        [TestInitialize]
        public void TestInitialize()
        {   
            // Arrange
            navigateTo.TrendsTab();

            // Act
            // Pass the test if success the methods below
            SelectAreaTypeEngland();

            // Give time to hide the elements into the panel
            WaitFor.ThreadWaitInSeconds(refreshingPanelIntervalTime);
        }

        [TestMethod]
        public void Test_England_Area_Type_Panel_Is_Displayed_Correctly()
        {
            // Assert
            CheckAreasGroupedByIsHidden();
            CheckBenchmarkIsHidden();
            CheckAreasOfAreaTypeIsHidden();
            CheckRegionsIsHidden();
        }

        [TestMethod]
        public void Test_Table_For_Recent_Trend_Is_Displayed_Correctly()
        {
            // Assert
            CheckTrendsContainerIsShown();
            CheckSignificanceIsNotCalculated();
        }

        private void SelectAreaTypeEngland()
        {
            var areaTypeList = fingertipsHelper.FindElementById("areaTypes");

            var selectElement = new SelectElement(areaTypeList);

            selectElement.SelectByText("England");
        }

        private void CheckAreasGroupedByIsHidden()
        {
            fingertipsHelper.CheckElementDisplayed("parentTypeBox", false, 
                "Areas grouped by dropdown box cannot be displayed");
        }

        private void CheckBenchmarkIsHidden()
        {
            fingertipsHelper.CheckElementDisplayed("benchmark-box", false, 
                "Benchmark dropdown box cannot be displayed");
        }

        private void CheckAreasOfAreaTypeIsHidden()
        {
            fingertipsHelper.CheckElementDisplayed("areaMenuBox", false, 
                "Areas dropdown box cannot be displayed");
        }

        private void CheckRegionsIsHidden()
        {
            fingertipsHelper.CheckElementDisplayed("region-menu-box", false, 
                "Regions dropdown box cannot be displayed");
        }

        private void CheckTrendsContainerIsShown()
        {
            fingertipsHelper.CheckElementDisplayed("trends-container", true, 
                "The trends container must to be displayed");
        }

        private void CheckSignificanceIsNotCalculated()
        {
            // Fail if not all img has no color
            if (!driver.FindElement(By.ClassName("bordered-table"))
                        .FindElements(By.TagName("img"))
                        .All(x => x.GetAttribute("src")
                        .Contains("/images/circle_black_mini.png")))
                
            {
                Assert.Fail("Significance shouldn't be calculated");
            }
        }
    }
}