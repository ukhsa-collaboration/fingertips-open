using IndicatorsUI.MainUISeleniumTest.Phof;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace IndicatorsUI.MainUISeleniumTest.PHOF
{
    /// <summary>
    /// Summary description for PhofInequalityTrendTest
    /// </summary>
    [TestClass]
    public class PhofInequalityTrendTest : PhofBaseUnitTest
    {
        [TestMethod]
        public void TestTrendsSwitch()
        {
            GoToPhofInequalities();
            // Click on Trends button
            FingertipsHelper.SelectInequalityTrends(driver);
            new WaitFor(driver).PhofInequalitiesFilters();

            // Check if Filters loaded
            var filters = GetTrendFilters();
            Assert.IsTrue(filters[0].Text.Contains("Display on chart:"));

            // Check if Chart loaded properly 
            new WaitFor(driver).InequalitiesTrendChart();
            var trendChart = GetTrendChart();
            Assert.IsTrue(trendChart.Count > 0);

            // Click clear link
            ClickFilterOptionClear();
            var trendChartAfterClearClick = GetTrendChart();
            Assert.AreNotEqual(trendChartAfterClearClick, trendChart);

            // Now select the first option as chart is empty
            ClickFilterOptionOne();
            var trendChartAfterClickingFirsttOption = GetTrendChart();
            Assert.AreNotEqual(trendChartAfterClearClick, trendChartAfterClickingFirsttOption);

            // Click on Latest values
            FingertipsHelper.SelectInequalityTrends(driver);
            Assert.IsFalse(DoesFiltersExist());
        }

        private void GoToPhofInequalities()
        {
            navigateTo.PhofInequalities();
            new WaitFor(driver).PhofTrendOptionButtonToLoad();
        }

        private bool DoesFiltersExist()
        {
            var present = false;
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.Zero);
            try
            {
                present = driver.FindElement(By.Id("nequalities-trend-filters")).Displayed;
            }
            catch (NoSuchElementException)
            {
            }
            var implicitWait = new TimeSpan(0, 0, 0, 10);
            driver.Manage().Timeouts().ImplicitlyWait(implicitWait);
            return present;
        }

        private void ClickFilterOptionClear()
        {
            driver.FindElement(By.XPath("//*[@id='inequalities-trend-filters']/div[2]/a")).Click();
        }

        private void ClickFilterOptionOne()
        {
            driver.FindElement(By.XPath("//*[@id='inequalities-trend-filters']/div[4]/a")).Click();
        }

        private IList<IWebElement> GetTrendChart()
        {
            return driver.FindElements(By.Id("inequalities-trend-chart"));
        }

        private IList<IWebElement> GetTrendFilters()
        {
            return driver.FindElements(By.Id("inequalities-trend-filters"));
        }
    }
}
