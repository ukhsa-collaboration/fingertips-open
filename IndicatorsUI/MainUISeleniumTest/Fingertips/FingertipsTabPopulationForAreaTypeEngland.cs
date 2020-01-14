using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Support.UI;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsTabPopulationForAreaTypeEngland : FingertipsBaseUnitTest
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
        public void TestInitialize()
        {
            // Arrange
            navigateTo.PopulationTab();

            // Act
            // Pass the test if success the methods below
            SelectAreaTypeEngland();

            // Give time to hide the elements into the panel
            WaitFor.ThreadWaitInSeconds(refreshingPanelIntervalTime);

            waitFor.AjaxLockToBeUnlocked();
        }

        [TestMethod]
        public void Test_Only_Bars_Are_Displayed_And_England_Area_Type_Panel_Is_Displayed_Correctly()
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
            var areaTypeList = fingertipsHelper.FindElementById("areaTypes");

            var selectElement = new SelectElement(areaTypeList);

            selectElement.SelectByText("England");
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

        private void CheckEnglandFemaleExist()
        {
            CheckTagText(FemaleLegendName, FemaleLegendTimesNumber);
        }

        private void CheckEnglandMaleExist()
        {
            CheckTagText(MaleLegendName, MaleLegendTimesNumber);
        }

        private void CheckOnlyEnglandNotExist()
        {
            CheckTagText(OnlyEnglandLegendName, OnlyEnglandTimesNumber);
        }

        private void CheckTagText(string text, int count)
        {
            var errorMessage = string.Format("The {0} must to be displayed {1} times", text, count);
            fingertipsHelper.CheckTextIsNumberTimesByTagName("tspan", text, count, errorMessage);
        }
    }
}