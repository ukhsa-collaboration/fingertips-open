using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Support.UI;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsPageTartanRugAreaTypeEngland : FingertipsBaseUnitTest
    {
        // Increase this number if test is failing because slow refreshing
        private double refreshingPanelIntervalTime = 0.5;

        [TestInitialize]
        public void InitTest()
        {
            // Arrange
            navigateTo.FingertipsTartanRug();

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
        public void TestTableForEnglandAreaTypeIsDisplayedCorrectly()
        {
            // Assert
            CheckScrollablePanelIsHide();
            CheckComparatorHeaderIsOnlyOne();
            CheckComparatorHeaderIsEngland();
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

        private void CheckScrollablePanelIsHide()
        {
            FingertipsHelper.checkElementDisplayed(driver, "scrollable-pane", false, "The scrollable panel cannot be displayed");
        }

        private void CheckComparatorHeaderIsOnlyOne()
        {
            var numberOfAllowedComparatorHeaders = 1;
            FingertipsHelper.checkElementNumbersIsMinorOfMax(driver, "comparator-header", numberOfAllowedComparatorHeaders, "It can be only " + numberOfAllowedComparatorHeaders + " number of comparator headers");
        }

        private void CheckComparatorHeaderIsEngland()
        {
            if (!FingertipsHelper.FindElementsByClass(driver, "comparator-header").Any(x =>
                FingertipsHelper.FindElementsByClass(driver, "verticalText").First().GetAttribute("src")
                    .Contains("England")))
            {
                Assert.Fail("The src should contains the word England");
            }
        }
    }
}