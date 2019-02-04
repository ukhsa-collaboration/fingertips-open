using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Support.UI;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsCompareAreasForAreaTypeEngland : FingertipsBaseUnitTest
    {
        public int MessageTimesNumber = 1;
        public string Message = "Not applicable for England data";

        // Increase this number if test is failing because slow refreshing
        private double refreshingPanelIntervalTime = 0.5;

        [TestInitialize]
        public void InitTest()
        {   
            // Arrange
            navigateTo.FingertipsCompareAreas();

            // Act
            // Pass the test if success the methods below
            SelectAreaTypeEngland();

            // Give time to hide the elements into the panel
            WaitFor.ThreadWait(refreshingPanelIntervalTime);
        }

        [TestMethod]
        public void TestNoDataDisplayAndEnglandAreaTypePanelIsDisplayedCorrectly()
        {
            // Assert
            CheckAreasGroupedByIsHide();
            CheckBenchmarkIsHide();
            CheckAreasOfAreaTypeIsHide();
            CheckRegionsIsHide();
            CheckKeyBarChartLegendIsHide();
            CheckNoDataMessageIsDisplayed();
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

        private void CheckKeyBarChartLegendIsHide()
        {
            FingertipsHelper.checkElementDisplayed(driver, "key-bar-chart", false, "Key Bar Chart legend cannot be displayed");
        }

        private void CheckNoDataMessageIsDisplayed()
        {
            FingertipsHelper.checkTextIsNumberTimesById(driver, "main-info-message", Message, MessageTimesNumber, string.Format("The message '{0}' must to be displayed {1} times", Message, MessageTimesNumber));
        }
    }
}