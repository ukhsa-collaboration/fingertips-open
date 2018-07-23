using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsScatterPlotTest : FingertipsBaseUnitTest
    {
        [TestMethod]
        public void TestScatterPlotChangingAreaTypeRepopulatesButRetainsSelectedSupportingIndicator()
        {
            // Navidate to scatter plot
            var parameters = new HashParameters();
            parameters.AddAreaTypeId(AreaTypeIds.CountyAndUnitaryAuthority);
            parameters.AddIndicatorId(IndicatorIds.LifeExpectancyAtBirth);
            parameters.AddSexId(SexIds.Male);
            parameters.AddAgeId(AgeIds.AllAges);
            parameters.AddTabId(TabIds.ScatterPlot);
            navigateTo.GoToUrl(ProfileUrlKeys.Phof + parameters.HashParameterString);
            waitFor.FingertipsScatterPlotChartToLoad();

            var countyUaIndicatorCount = GetSupportingIndicatorCount();

            // Choose supporting indicator
            driver.FindElement(By.CssSelector("div.chosen-container a.chosen-single")).Click();
            var searchText = driver.FindElement(By.CssSelector("div.chosen-search input"));
            searchText.SendKeys("pupil absence");
            searchText.SendKeys(Keys.Return);
            waitFor.AjaxLockToBeUnlocked();

            // Change the area type from CountyUa to District
            var areaTypeDropdown = driver.FindElement(By.Id("areaTypes"));
            SelectElement clickThis = new SelectElement(areaTypeDropdown);
            clickThis.SelectByText("District & UA");

            // Wait for scatter plot to reload
            waitFor.AjaxLockToBeUnlocked();

            // Assert: area count is different
            var distictUaIndicatorCount = GetSupportingIndicatorCount();
            Assert.AreNotEqual(countyUaIndicatorCount, distictUaIndicatorCount);

            // Assert: indicator is as expected
            var selectedSupportingIndicator = driver.FindElement(
                By.CssSelector("div.chosen-container a.chosen-single span"));
            TestHelper.AssertTextContains(selectedSupportingIndicator.Text, "Pupil absence");
        }

        private int GetSupportingIndicatorCount()
        {
            var countyUaAreaCount = driver.FindElements(By.CssSelector("#supporting-indicators option")).Count;
            return countyUaAreaCount;
        }

        [TestMethod]
        public void TestScatterPlotSupportingIndicatorCanBeSelected()
        {
            // Navidate to scatter plot
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.HealthProfiles);
            FingertipsHelper.SelectFingertipsTab(driver, "page-scatter");
            waitFor.FingertipsScatterPlotChartToLoad();

            //Set supporting indicator
            var menu = driver.FindElement(By.CssSelector("div.chosen-container a.chosen-single"));
            menu.Click();
            var searchText = driver.FindElement(By.CssSelector("div.chosen-search input"));
            searchText.SendKeys("gcse");
            searchText.SendKeys(Keys.Return);
            waitFor.AjaxLockToBeUnlocked();

            // Assert: selected indicator is selected
            var selectedSupportinIndicator = driver.FindElement(By.CssSelector("div.chosen-container a.chosen-single span"));
            TestHelper.AssertTextContains(selectedSupportinIndicator.Text, "gcse");
        }

    }
}
