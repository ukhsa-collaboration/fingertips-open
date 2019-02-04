using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Support.UI;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsPopulationForAreaTypeEngland : FingertipsBaseUnitTest
    {
        // Increase this number if test is failing because slow refreshing
        private double refreshingPanelIntervalTime = 1;

        private int FemaleLegendTimesNumber = 1;
        private string FemaleLegendName = "England (Female)";

        private int MaleLegendTimesNumber = 1;
        private string MaleLegendName = "England (Male)";

        private int OnlyEnglandTimesNumber = 0;
        private string OnlyEnglandLegendName = "England";

        [TestInitialize]
        public void InitTest()
        {   
            // Arrange
            navigateTo.FingertipsPopulation();

            // Act
            // Pass the test if success the methods below
            SelectAreaTypeEngland();

            // Give time to hide the elements into the panel
            WaitFor.ThreadWait(refreshingPanelIntervalTime);
        }

        [TestMethod]
        public void TestOnlyBarsAreDisplayedAndEnglandAreaTypePanelIsDisplayedCorrectly()
        {
            // Assert
            CheckAreasGroupedByIsHide();
            CheckBenchmarkIsHide();
            CheckAreasOfAreaTypeIsHide();
            CheckRegionsIsHide();

            CheckEnglandFemaleExist();
            CheckEnglandMaleExist();

            CheckOnlyEnglandNotExist();
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

        private void CheckEnglandFemaleExist()
        {
            FingertipsHelper.checkTextIsNumberTimesbyTagName(driver, "tspan", FemaleLegendName, FemaleLegendTimesNumber, string.Format("The {0} must to be displayed {1} times",FemaleLegendName, FemaleLegendTimesNumber));
        }

        private void CheckEnglandMaleExist()
        {
            FingertipsHelper.checkTextIsNumberTimesbyTagName(driver, "tspan", MaleLegendName, MaleLegendTimesNumber, string.Format("The {0} must to be displayed {1} times", MaleLegendName,MaleLegendTimesNumber));
        }

        private void CheckOnlyEnglandNotExist()
        {
            FingertipsHelper.checkTextIsNumberTimesbyTagName(driver, "tspan", OnlyEnglandLegendName, OnlyEnglandTimesNumber, string.Format("The {0} must to be displayed {1} times", OnlyEnglandLegendName ,OnlyEnglandTimesNumber));
        }
    }
}