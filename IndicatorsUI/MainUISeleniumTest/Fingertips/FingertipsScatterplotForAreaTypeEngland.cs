using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Support.UI;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsScatterplotForAreaTypeEngland : FingertipsBaseUnitTest
    {
        // Increase this number if test is failing because slow refreshing
        private double refreshingPanelIntervalTime = 0.5;

        private int MessageTimesNumber = 1;
        private string Message = "Not applicable for England data";

        [TestInitialize]
        public void InitTest()
        {   
            // Arrange
            navigateTo.FingertipsScatterplot();

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
            CheckScatterplotFiltersIsHide();
            CheckExportChartBoxIsHide();
            CheckSupportingIndicatorsWrapper();
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

        private void CheckScatterplotFiltersIsHide()
        {
            FingertipsHelper.checkElementDisplayed(driver, "scatterplot-filters", false, "The scatter plot filters cannot be displayed");
        }

        private void CheckExportChartBoxIsHide()
        {
            FingertipsHelper.checkElementDisplayed(driver, "export-chart-box", false, "The export chart box cannot be displayed");
        }

        private void CheckSupportingIndicatorsWrapper()
        {
            FingertipsHelper.checkElementDisplayed(driver, "supporting-indicators-wrapper", false, "The supporting indicators wrapper cannot be displayed");
        }

        private void CheckNoDataMessageIsDisplayed()
        {
            FingertipsHelper.checkTextIsNumberTimesbyTagName(driver, "p", Message, MessageTimesNumber, string.Format("The message '{0}' must to be displayed {1} times", Message, MessageTimesNumber));
        }
    }
}