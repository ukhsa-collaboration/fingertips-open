﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Profiles.DomainObjects;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsDataTest : FingertipsBaseUnitTest
    {
        [TestMethod]
        public void TestAllDomainsLoadForHealthProfiles()
        {
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.HealthProfiles);
            CheckTartanRugLoadsForAllDomains(driver);
        }

        [TestMethod]
        public void TestAllDomainsLoadForTbStrategy()
        {
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.TbStrategy);
            CheckTartanRugHasLoaded(driver);
        }

        [TestMethod]
        public void TestAllDomainsLoadForSexualHealth()
        {
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.SexualHealth);
            CheckTartanRugLoadsForAllDomains(driver);
        }

        [TestMethod]
        public void TestAllTabsLoadForHealthProfiles()
        {
            CheckAllTabsLoadForProfile(ProfileUrlKeys.HealthProfiles);
        }

        [TestMethod]
        public void TestAllTabsLoadForSexualHealth()
        {
            CheckAllTabsLoadForProfile(ProfileUrlKeys.SexualHealth);
        }

        [TestMethod]
        public void TestAllTabsLoadForTbStrategy()
        {
            CheckAllTabsLoadForProfile(ProfileUrlKeys.TbStrategy);
        }

        [TestMethod]
        public void TestCompareIndicatorChartExportMenuIsPresent()
        {
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.SexualHealth);
            SelectTab("page-scatter");

            CheckExportLinkPresent();
        }

        [TestMethod]
        public void TestMapChartExportMenuIsPresent()
        {
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.SexualHealth);
            SelectTab("page-map");

            CheckExportLinkPresent();
        }

        [TestMethod]
        public void TestTrendsChartExportMenuIsPresent()
        {
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.SexualHealth);
            SelectTab("page-trends");

            CheckExportLinkPresent();
        }

        /// <summary>
        /// 'undefined' on a page indicates some data has not been found in JavaScript
        /// </summary>
        [TestMethod]
        public void TestNoPageContainsUndefined()
        {
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.HealthProfiles);

            var tabIds = new [] { "page-map", "page-scatter", "page-trends", "page-indicators",
                "page-areas", "page-inequalities", "page-metadata", "page-overview"};

            foreach (var tabId in tabIds)
            {
                AssertPageDoesNotContainUndefined(tabId);
            }
        }

        [TestMethod]
        public void TestScatterPlotSupportingIndicatorCanBeSelected()
        {
            // Navidate to scatter plot
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.HealthProfiles);
            SelectTab("page-scatter");
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

        public void CheckAllTabsLoadForProfile(string urlKey)
        {
            navigateTo.FingertipsDataForProfile(urlKey);
            FingertipsHelper.SelectEachFingertipsTabInTurnAndCheckDownloadIsLast(driver);
        }

        public static void CheckTartanRugLoadsForAllDomains(IWebDriver driver)
        {
            var waitFor = new WaitFor(driver);

            // Click through each domain
            var domains = GetDomainOptions(driver);
            string previousIndicatorName = string.Empty;
            int previousIndicatorCount = 0;
            foreach (var domain in domains)
            {
                SelectDomain(domain, waitFor);
                var indicatorName = driver.FindElement(By.Id(LongerLivesIds.TartanRugIndicatorNameOnFirstRow)).Text;

                var indicatorCount = driver.FindElements(By.ClassName("rug-indicator")).Count;

                // Check tartan rug is different for each domain
                Assert.IsFalse(
                    string.Equals(previousIndicatorName, indicatorName) &&
                    previousIndicatorCount == indicatorCount
                    );

                previousIndicatorCount = indicatorCount;
                previousIndicatorName = indicatorName;
            }
        }

        public static void SelectDomain(IWebElement domain, WaitFor waitFor)
        {
            domain.Click();
            WaitFor.ThreadWait(0.1);
            waitFor.AjaxLockToBeUnlocked();
        }

        public static ReadOnlyCollection<IWebElement> GetDomainOptions(IWebDriver driver)
        {
            var domains = driver.FindElements(By.ClassName("domain-option"));
            return domains;
        }

        public static void SelectInequalitiesTab(IWebDriver driver)
        {
            driver.FindElement(By.Id("page-inequalities")).Click();
            new WaitFor(driver).InequalitiesTabToLoad();
        }

        public void SelectTab(string tabId)
        {
            FingertipsHelper.SelectFingertipsTab(driver, tabId);
        }

        private void AssertPageDoesNotContainUndefined(string tabId)
        {
            SelectTab(tabId);
            var body = driver.FindElement(By.TagName("body"));
            var html = body.GetAttribute("innerHTML");

            var regex = new Regex(Regex.Escape(html));
            var undefinedCount = regex.Matches("undefined").Count;
            var ignoreUndefinedCount = regex.Matches("highcharts-color-undefined").Count;

            Assert.AreEqual(0, undefinedCount - ignoreUndefinedCount, "'undefined' found on " + tabId);
        }

        private void CheckExportLinkPresent()
        {
            By byExportMenuId = By.ClassName("export-link");
            waitFor.ExpectedElementToBeVisible(byExportMenuId);
            var exportMenu = driver.FindElement(byExportMenuId);
            waitFor.ElementToContainText(exportMenu, "Export");
        }
    }
}