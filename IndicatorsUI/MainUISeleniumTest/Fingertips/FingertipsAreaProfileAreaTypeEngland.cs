using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsAreaProfileAreaTypeEngland : FingertipsBaseUnitTest
    {
        // Increase this number if test is failing because slow refreshing
        private double refreshingPanelIntervalTime = 1;

        private int elementsNumberDisplayedInHeader = 6;
        private int elementsNumberDisplayedInBody = 5;

        [TestInitialize]
        public void InitTest()
        {
            // Arrange
            navigateTo.FingertipsAreaProfiles();

            // Act
            // Pass the test if success the methods below
            SelectAreaTypeEngland();

            // Give time to hide the elements into the panel
            WaitFor.ThreadWait(refreshingPanelIntervalTime);
        }

        [TestMethod]
        public void TestTableIsNotComparingWithItSelfAndEnglandAreaTypePanelIsDisplayedCorrectly()
        {
            // Assert
            CheckAreasGroupedByIsHide();
            CheckBenchmarkIsHide();
            CheckAreasOfAreaTypeIsHide();
            CheckRegionsIsHide();
            CheckTableDisplaySixHeadersOnly();
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

        private void CheckTableDisplaySixHeadersOnly()
        {
            FingertipsHelper.checkElementDisplayed(driver, "single-area-table", true, "The single-area-table should be displayed");

            // Compare the number of elements that it should be shown in the header
            var asd = FingertipsHelper.FindElementById(driver, "single-area-table").FindElements(By.TagName("th")).Count;
            if ( asd != elementsNumberDisplayedInHeader)
            {
                Assert.Fail("Header should contains only"+ elementsNumberDisplayedInHeader + " elements");
            }

            // The rest of elements number divided by elements, expected in one row, should be 0
            if (FingertipsHelper.FindElementById(driver, "single-area-table").FindElements(By.TagName("td")).Count % elementsNumberDisplayedInBody != 0)
            {
                Assert.Fail("Body should contains only "+ elementsNumberDisplayedInBody + " elements");
            }
        }
    }
}