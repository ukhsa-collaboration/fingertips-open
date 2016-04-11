using System;
using System.Collections.Generic;
using System.Linq;
using IndicatorsUI.MainUISeleniumTest.Fingertips;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Profiles.DomainObjects;

namespace IndicatorsUI.MainUISeleniumTest
{
    [TestClass]
    public class FingertipsAreaSearchTest : FingertipsBaseUnitTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            navigateTo.FingertipsFrontPageForProfile(ProfileUrlKeys.HealthProfiles);

            // Wait for region menu to load
            waitFor.ElementToContainText(GetRegionMenu(), "London");
        }

        [TestMethod]
        public void TestResultsDisplayedSearchingForPlaceName()
        {
            // Search for a place name
            FingertipsHelper.SearchForAnArea(driver, "Keyworth");
            waitFor.FingertipsAreaSearchResultsPageToLoad();

            // Assert expected results are displayed
            var resultsHtml = GetResultsHtml();
            TestHelper.AssertTextContains(resultsHtml, "Rushcliffe", "Rushcliffe should be displayed as a district result");
            TestHelper.AssertTextContains(resultsHtml, "Nottinghamshire", "Nottinghamshire should be displayed as a county result");
        }

        [TestMethod]
        public void TestResultsDisplayedSearchingForParentArea()
        {
            // Search for a county
            FingertipsHelper.SearchForAnArea(driver, "Derbyshire");
            waitFor.FingertipsAreaSearchResultsPageToLoad();

            // Assert expected results are displayed
            var resultsHtml = GetResultsHtml();
            TestHelper.AssertTextContains(resultsHtml, "Bolsover", "Bolsover should be displayed as a district result");
            TestHelper.AssertTextContains(resultsHtml, "Erewash", "Bolsover should be displayed as a district result");
            TestHelper.AssertTextContains(resultsHtml, "Derbyshire", "Derbyshire should be displayed as a county result");
        }

        [TestMethod] public void TestNoResultsFoundForIgnoredArea()
        {
            FingertipsHelper.SearchForAnArea(driver, "Scilly");
            waitFor.FingertipsAreaSearchResultsPageToLoad();

            // Assert no data message is displayed
            waitFor.ExpectedElementToBeVisible(By.Id("no-data-message"));
        }

        [TestMethod]
        public void TestSearchDisplayedForRegion()
        {
            // Select an option
            new SelectElement(GetRegionMenu()).SelectByValue(AreaCodes.RegionEastOfEngland);

            waitFor.FingertipsAreaSearchResultsPageToLoad();

            // Assert expected results are displayed
            var resultsHtml = GetResultsHtml();
            TestHelper.AssertTextContains(resultsHtml, "Norfolk", "Norfolk should be displayed as a county result");
            TestHelper.AssertTextContains(resultsHtml, "Broxbourne", "Broxbourne should be displayed as a district result");
        }

        private string GetResultsHtml()
        {
            var resultsHtml = driver.FindElement(By.Id("area-results-box")).Text;
            return resultsHtml;
        }

        private IWebElement GetRegionMenu()
        {
            var regionMenu = driver.FindElement(By.Id("parent-area-menu"));
            return regionMenu;
        }
    }
}
