using System;
using IndicatorsUI.DomainObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsTabCompareAreasForAreaTypeEngland : FingertipsBaseUnitTest
    {
        public const int MessageTimesNumber = 1;
        public const string Message = "Not applicable for England data";
        public const string TooltipIdName = "tooltip";

        // Increase this number if test is failing because slow refreshing
        private const double refreshingPanelIntervalTime = 0.5;

        [TestInitialize]
        public void TestInitialize()
        {   
            navigateTo.CompareAreasTab();
            SelectAreaTypeEngland();

            // Give time to hide the elements into the panel
            WaitFor.ThreadWaitInSeconds(refreshingPanelIntervalTime);
            waitFor.AjaxLockToBeUnlocked();
        }

        [TestMethod]
        public void Test_No_Data_Display_And_England_Area_Type_Panel_Is_Displayed_Correctly()
        {
            // Assert
            CheckAreasGroupedByIsHidden();
            CheckBenchmarkIsHidden();
            CheckAreasOfAreaTypeIsHidden();
            CheckRegionsIsHidden();
            CheckKeyBarChartLegendIsHidden();
            CheckNoDataMessageIsDisplayed();
        }

        [TestMethod]
        public void Test_Should_Display_Area_Names_Tooltips_For_National_SubNational_And_Local()
        {
            const int positionMoveBack = 1;
            var districtAreaTypeId = AreaTypeIds.DistrictAndUAPreApr2019;
            var parentAreaClassName = "comp-area-parent-area";
            
            var nationalPositionInTable = 1;
            var xPathNational = "//*[@id='indicator-details-table']/tbody/tr[1]/td[6]/div/img[1]";
            var xPathSubNational = "//*[@id='indicator-details-table']/tbody/tr[2]/td[6]/div/img[1]";
            var xPathLocal = "//*[@id='indicator-details-table']/tbody/tr[3]/td[6]/div/img[1]";
            var xPathAreaSort = "//*[@id='indicator-details-table']/thead/tr[1]/th[1]/a[1]";

            fingertipsHelper.SelectAreaType(districtAreaTypeId);

            var foundElement = fingertipsHelper.FindElementsByClass(parentAreaClassName)[nationalPositionInTable - positionMoveBack];

            // Center the view in the element
            var js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'})", foundElement);
            WaitFor.ThreadWaitInSeconds(0.5);

            // Sort areas
            fingertipsHelper.SortAreasByXPath(xPathAreaSort);

            var toolTipsElementsFound = FindTooltipByxPath(xPathNational);
            var nationalAreaTooltipDisplayed = TooltipsContainsText(toolTipsElementsFound, "England");

            toolTipsElementsFound = FindTooltipByxPath(xPathSubNational);
            var subNationalAreaTooltipDisplayed = TooltipsContainsText(toolTipsElementsFound, "East Midlands region");

            toolTipsElementsFound = FindTooltipByxPath(xPathLocal);
            var localAreaTooltipDisplayed = TooltipsContainsText(toolTipsElementsFound, "Amber Valley");

            Assert.IsTrue(nationalAreaTooltipDisplayed);
            Assert.IsTrue(subNationalAreaTooltipDisplayed);
            Assert.IsTrue(localAreaTooltipDisplayed);
        }

        private IWebElement FindTooltipByxPath(string xPath, int elementPosition = 1)
        {
            fingertipsHelper.MoveToElementByXPath(xPath, elementPosition);
            WaitFor.ThreadWaitInSeconds(1);

            return fingertipsHelper.FindElementById(TooltipIdName);
        }

        private static bool TooltipsContainsText(IWebElement toolTipsElement, string text)
        {
            return toolTipsElement.Text.Contains(text);
        }

        private void SelectAreaTypeEngland()
        {
            var areaTypeList = fingertipsHelper.FindElementById("areaTypes");

            var selectElement = new SelectElement(areaTypeList);

            selectElement.SelectByText("England");
        }

        private void CheckAreasGroupedByIsHidden()
        {
            fingertipsHelper.CheckElementDisplayed("parentTypeBox", false, "Areas grouped by dropdown box cannot be displayed");
        }

        private void CheckBenchmarkIsHidden()
        {
            fingertipsHelper.CheckElementDisplayed("benchmark-box", false, "Benchmark dropdown box cannot be displayed");
        }

        private void CheckAreasOfAreaTypeIsHidden()
        {
            fingertipsHelper.CheckElementDisplayed("areaMenuBox", false, "Areas dropdown box cannot be displayed");
        }

        private void CheckRegionsIsHidden()
        {
            fingertipsHelper.CheckElementDisplayed("region-menu-box", false, "Regions dropdown box cannot be displayed");
        }

        private void CheckKeyBarChartLegendIsHidden()
        {
            fingertipsHelper.CheckElementDisplayed("key-bar-chart", false, "Key Bar Chart legend cannot be displayed");
        }

        private void CheckNoDataMessageIsDisplayed()
        {
            fingertipsHelper.CheckTextIsNumberTimesByClassName("do-not-display-for-england", Message, MessageTimesNumber,
                string.Format("The message '{0}' must to be displayed {1} times", Message, MessageTimesNumber));
        }
    }
}