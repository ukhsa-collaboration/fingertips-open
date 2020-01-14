using System.Linq;
using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Support.UI;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsTabOverviewAreaTypeEngland : FingertipsBaseUnitTest
    {
        // Increase this number if test is failing because slow refreshing
        private double refreshingPanelIntervalTime = 0.5;

        [TestInitialize]
        public void TestInitialize()
        {
            // Arrange
            navigateTo.OverviewTab();

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
            CheckAreasGroupedByIsHide();
            CheckBenchmarkIsHide();
            CheckAreasOfAreaTypeIsHide();
            CheckRegionsIsHide();
        }

        [TestMethod]
        public void Test_Table_For_England_Area_Type_Is_Displayed_Correctly()
        {
            // Assert
            CheckScrollablePanelIsHide();
            CheckComparatorHeaderIsOnlyOne();
            CheckComparatorHeaderIsEngland();
        }

        private void SelectAreaTypeEngland()
        {
            var areaTypeList = fingertipsHelper.FindElementById("areaTypes");

            var selectElement = new SelectElement(areaTypeList);

            selectElement.SelectByText("England");
        }

        private void CheckAreasGroupedByIsHide()
        {
            fingertipsHelper.CheckElementDisplayed("parentTypeBox", false, "Areas grouped by dropdown box should not be displayed");
        }

        private void CheckBenchmarkIsHide()
        {
            fingertipsHelper.CheckElementDisplayed("benchmark-box", false, "Benchmark dropdown box should not be displayed");
        }

        private void CheckAreasOfAreaTypeIsHide()
        {
            fingertipsHelper.CheckElementDisplayed("areaMenuBox", false, "Areas dropdown box should not be displayed");
        }

        private void CheckRegionsIsHide()
        {
            fingertipsHelper.CheckElementDisplayed("region-menu-box", false, "Regions dropdown box should not be displayed");
        }

        private void CheckScrollablePanelIsHide()
        {
            fingertipsHelper.CheckElementDisplayed("scrollable-pane", false, "The scrollable panel should not be displayed");
        }

        private void CheckComparatorHeaderIsOnlyOne()
        {
            var numberOfAllowedComparatorHeaders = 1;
            fingertipsHelper.CheckElementNumbersIsMinorOfMax("comparator-header", 
                numberOfAllowedComparatorHeaders, "It can be only " + numberOfAllowedComparatorHeaders + " number of comparator headers");
        }

        private void CheckComparatorHeaderIsEngland()
        {
            if (!fingertipsHelper.FindElementsByClass("comparator-header").Any(x =>
                fingertipsHelper.FindElementsByClass("verticalText").First().GetAttribute("src")
                    .Contains("England")))
            {
                Assert.Fail("The src should contains the word England");
            }
        }
    }
}