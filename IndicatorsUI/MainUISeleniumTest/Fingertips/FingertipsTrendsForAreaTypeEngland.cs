using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsTrendsForAreaTypeEngland : FingertipsBaseUnitTest
    {
        // Increase this number if test is failing because slow refreshing
        private double refreshingPanelIntervalTime = 0.5;

        [TestInitialize]
        public void InitTest()
        {   
            // Arrange
            navigateTo.FingertipsTrends();

            // Act
            // Pass the test if success the methods below
            SelectAreaTypeEngland();

            // Give time to hide the elements into the panel
            WaitFor.ThreadWait(refreshingPanelIntervalTime);
        }

        [TestMethod]
        public void TestEnglandAreaTypePanelIsDisplayedCorrectly()
        {
            // Assert
            CheckAreasGroupedByIsHide();
            CheckBenchmarkIsHide();
            CheckAreasOfAreaTypeIsHide();
            CheckRegionsIsHide();
        }

        [TestMethod]
        public void TestTableForRecentTendIsDisplayedCorrectly()
        {
            // Assert
            CheckTrendsContainerIShown();
            CheckSignificanceIsNotCalculated();
        }

        private void SelectAreaTypeEngland()
        {
            var areaTypeList = FingertipsHelper.FindElementById(driver, "areaTypes");

            var selectElement = new SelectElement(areaTypeList);

            selectElement.SelectByText("England");
        }

        private void CheckAreasGroupedByIsHide()
        {
            FingertipsHelper.checkElementDisplayed(driver, "parentTypeBox", false, "Areas grouped by dropdown box cannot be displayed");
        }

        private void CheckBenchmarkIsHide()
        {
            FingertipsHelper.checkElementDisplayed(driver, "benchmark-box", false, "Benchmark dropdown box cannot be displayed");
        }

        private void CheckAreasOfAreaTypeIsHide()
        {
            FingertipsHelper.checkElementDisplayed(driver, "areaMenuBox", false, "Areas dropdown box cannot be displayed");
        }

        private void CheckRegionsIsHide()
        {
            FingertipsHelper.checkElementDisplayed(driver, "region-menu-box", false, "Regions dropdown box cannot be displayed");
        }

        private void CheckTrendsContainerIShown()
        {
            FingertipsHelper.checkElementDisplayed(driver, "trends-container", true, "The trends container must to be displayed");
        }

        private void CheckSignificanceIsNotCalculated()
        {
            // Fail if not all img has no color
            if (!driver.FindElement(By.ClassName("trendTableBox"))
                        .FindElements(By.TagName("img"))
                        .All(x => x.GetAttribute("src")
                        .Contains("/images/circle_black_mini.png")))
                
            {
                Assert.Fail("Significance shouldn't be calculated");
            }
        }
    }
}