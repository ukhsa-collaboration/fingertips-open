using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Support.UI;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsTabCompareIndicatorsForAreaTypeEngland : FingertipsBaseUnitTest
    {
        // Increase this number if test is failing because slow refreshing
        private double refreshingPanelIntervalTime = 0.5;

        private int MessageTimesNumber = 1;
        private string Message = "Not applicable for England data";

        [TestInitialize]
        public void TestInitialize()
        {   
            navigateTo.CompareIndicatorsTab();

            // Act
            // Pass the test if success the methods below
            SelectAreaTypeEngland();

            // Give time to hide the elements into the panel
            WaitFor.ThreadWaitInSeconds(refreshingPanelIntervalTime);

            waitFor.AjaxLockToBeUnlocked();
        }

        [TestMethod]
        public void Test_England_Area_Type_Panel_Is_Displayed_Correctly()
        {
            // Assert
            CheckAreasGroupedByIsHidden();
            CheckBenchmarkIsHide();
            CheckAreasOfAreaTypeIsHidden();
            CheckRegionsIsHide();
        }

        [TestMethod]
        public void Test_Table_For_England_AreaType_Is_Displayed_Correctly()
        {
            // Assert
            CheckScatterplotFiltersIsHidden();
            CheckExportChartBoxIsHidden();
            CheckNoDataMessageIsDisplayed();
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

        private void CheckBenchmarkIsHide()
        {
            fingertipsHelper.CheckElementDisplayed("benchmark-box", false,
                "Benchmark dropdown box cannot be displayed");
        }

        private void CheckAreasOfAreaTypeIsHidden()
        {
            fingertipsHelper.CheckElementDisplayed("areaMenuBox", false,
                "Areas dropdown box cannot be displayed");
        }

        private void CheckRegionsIsHide()
        {
            fingertipsHelper.CheckElementDisplayed("region-menu-box", false,
                "Regions dropdown box cannot be displayed");
        }

        private void CheckScatterplotFiltersIsHidden()
        {
            fingertipsHelper.CheckElementDisplayed("scatterplot-filters", false, 
                "The scatter plot filters cannot be displayed");
        }

        private void CheckExportChartBoxIsHidden()
        {
            fingertipsHelper.CheckElementDisplayed("export-chart-box", false, 
                "The export chart box cannot be displayed");
        }

        private void CheckNoDataMessageIsDisplayed()
        {
            fingertipsHelper.CheckTextIsNumberTimesByClassName("do-not-display-for-england", 
                Message, MessageTimesNumber, 
                string.Format("The message '{0}' must to be displayed {1} times", Message, MessageTimesNumber));
        }
    }
}