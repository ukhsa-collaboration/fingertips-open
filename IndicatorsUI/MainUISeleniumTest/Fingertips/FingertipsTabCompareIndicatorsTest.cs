using System.Net;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Threading;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsTabCompareIndicatorsTest : FingertipsBaseUnitTest
    {
        [TestMethod]
        public void Test_Changing_Area_Type_Repopulates_But_Retains_Selected_Supporting_Indicator()
        {
            const string indicatorText = "fuel poverty";

            // Navigate to scatter plot
            var parameters = new HashParameters();
            parameters.AddAreaTypeId(AreaTypeIds.CountyAndUnitaryAuthorityPre2019);
            parameters.AddIndicatorId(IndicatorIds.LifeExpectancyAtBirth);
            parameters.AddSexId(SexIds.Male);
            parameters.AddAgeId(AgeIds.AllAges);
            parameters.AddTabId(TabIds.CompareIndicators);
            navigateTo.GoToUrl(ProfileUrlKeys.Phof + parameters.HashParameterString);
            waitFor.FingertipsScatterPlotChartToLoad();

            fingertipsHelper.SelectCompareIndicatorsSupportingIndicator(indicatorText);

            // Change the area type from CountyUa to District
            var areaTypeDropdown = driver.FindElement(By.Id("areaTypes"));
            SelectElement clickThis = new SelectElement(areaTypeDropdown);
            clickThis.SelectByText("District & UA");

            // Wait for scatter plot to reload
            Thread.Sleep(500);
            waitFor.AjaxLockToBeUnlocked();

            // Assert: indicator is as expected
            var selectedSupportingIndicator = driver.FindElement(By.Id("y-axis-indicator"));
            TestHelper.AssertTextContains(selectedSupportingIndicator.Text, indicatorText);
        }

        [TestMethod]
        public void Test_Scatter_Plot_Supporting_Indicator_Can_Be_Selected()
        {
            // Navigate to scatter plot
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.Phof);
            fingertipsHelper.SelectTab(FingertipsIds.TabCompareIndicators);
            waitFor.FingertipsScatterPlotChartToLoad();

            // Set supporting indicator
            var text = "A02c - Inequality";
            fingertipsHelper.SelectCompareIndicatorsSupportingIndicator(text);

            // Assert: selected indicator is selected
            var selectedSupportingIndicator = driver.FindElement(By.Id("y-axis-indicator"));
            TestHelper.AssertTextContains(selectedSupportingIndicator.Text, text);
        }

        [TestMethod]
        public void Test_Y_Axis_Menu_Displayed_When_No_Data()
        {
            var indicatorName = "A02b - Inequality in healthy life expectancy at birth ENGLAND (Male)";

            // Navigate to scatter plot
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.Phof);
            fingertipsHelper.SelectTab(FingertipsIds.TabCompareIndicators);
            waitFor.FingertipsScatterPlotChartToLoad();

            // Select indicator
            fingertipsHelper.SelectIndicatorByName(indicatorName);
            waitFor.ExpectedElementToBeVisible(By.Id("compare-indicators-no-data-message"));

            // Assert: no data is displayed
            var noDataMessageText = driver.FindElement(By.Id("compare-indicators-no-data-message")).Text;
            Assert.IsTrue(noDataMessageText.Contains(indicatorName));

            // Assert: Y axis menu is displayed
            Assert.IsTrue(driver.FindElement(By.Id("y-axis-indicator")).Displayed);
        }

        private int GetSupportingIndicatorCount()
        {
            var indicatorCount = driver.FindElements(By.CssSelector("ul.available-items li")).Count;
            return indicatorCount;
        }

    }
}
