using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    /// <summary>
    /// Summary description for PhofInequalityTrendTest
    /// </summary>
    [TestClass]
    public class FingertipsInequalityTest : FingertipsBaseUnitTest
    {
        [TestMethod]
        public void Test_Changing_Between_Latest_Values_And_Trends()
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
            var trendChartAfterClickingFirstOption = GetTrendChart();
            Assert.AreNotEqual(trendChartAfterClearClick, trendChartAfterClickingFirstOption);

            // Click on Latest values
            FingertipsHelper.SelectInequalitiesLatestValues(driver);
            Assert.IsFalse(AreTrendFiltersVisible());
        }

        [TestMethod]
        public void TestInequalitiesChartLoadsForEachIndicatorForAllDomains()
        {
            GoToPhofInequalities();

            var domains = FingertipsDataTest.GetDomainOptions(driver);
            foreach (var domain in domains)
            {
                FingertipsDataTest.SelectDomain(domain, waitFor);

                var nextIndicatorButton = driver.FindElement(By.Id("next-indicator"));

                // Make sure it is possible to view each indicator until get back to the first one
                string initialHeaderText = GetInequalityIndicatorName();
                string headerText = string.Empty;
                while (headerText != initialHeaderText)
                {
                    FingertipsHelper.SelectNextIndicator(nextIndicatorButton, waitFor);
                    headerText = GetInequalityIndicatorName();
                }
            }
        }

        [TestMethod]
        public void Test_Inequalities_Description_Lightbox()
        {
            GoToPhofInequalities();

            var domains = FingertipsDataTest.GetDomainOptions(driver);
            var domain = domains.FirstOrDefault(d => d.Text.ToLower() == "health improvement");

            // Navigate to the health improvement section
            FingertipsDataTest.SelectDomain(domain, waitFor);

            // Navigate to the inequalities section
            FingertipsDataTest.SelectInequalitiesTab(driver);

            // From the indicator dropdown select 2.04 - Under 18 conceptions option
            SelectElement indicatorMenuDropdown = new SelectElement(driver.FindElement(By.Id("indicatorMenu")));
            indicatorMenuDropdown.SelectByText("2.04 - Under 18 conceptions");

            waitFor.FingertipsCategoryTypeDescriptionsToLoad();

            // Check whether the category type description pop-ups are working as expected
            CheckCategoryTypeDescriptionPopUps();
        }
        
        private string GetInequalityIndicatorName()
        {
            return driver.FindElement(By.ClassName("trend-link")).Text;
        }

        private void GoToPhofInequalities()
        {
            navigateTo.PhofInequalities();
            new WaitFor(driver).PhofTrendOptionButtonToLoad();
        }

        private bool AreTrendFiltersVisible()
        {
            var visible = false;
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
            try
            {
                var label = driver.FindElement(By.Id("inequalities-trend-filters"));
                visible = label.Displayed;
            }
            catch (NoSuchElementException)
            {
            }
            var implicitWait = new TimeSpan(0, 0, 0, 10);
            driver.Manage().Timeouts().ImplicitWait = implicitWait;
            return visible;
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

        private void CheckCategoryTypeDescriptionPopUps()
        {
            //int counter = 0;

            // Get the list of information tooltip icons based on the class name
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(By.ClassName(Classes.InformationToolTipWithPosition));

            // Loop through the read-only collection of web elements
            foreach (IWebElement element in elements)
            {
                //var headerTextContainer = driver.FindElements(By.ClassName(Classes.Width90))[counter];
                //var headerTextElement = headerTextContainer.FindElement(By.TagName("a"));
                //string headerText = headerTextElement.Text;

                // Check whether the pop-up is visible
                //CheckCategoryTypeDescriptionPopUpIsVisible(element, headerText);

                CheckCategoryTypeDescriptionPopUpIsVisible(element);

                //counter++;
            }
        }

        //private void CheckCategoryTypeDescriptionPopUpIsVisible(IWebElement element, string headerText)
        private void CheckCategoryTypeDescriptionPopUpIsVisible(IWebElement element)
        {
            // Open the information pop-up
            element.Click();

            // Wait for the pop-up to load
            waitFor.FingertipsCategoryTypeDescriptionPopupToLoad();

            // Get the reference of the information pop-up displayed
            var box = driver.FindElement(By.Id("infoBox"));

            // Check whether the information pop-up is displayed
            Assert.IsNotNull(box);

            // Find the close icon of the opened pop-up
            var closeIcon = driver.FindElement(By.ClassName(Classes.CloseIcon));
            // Close the pop-up
            closeIcon.Click();
        }
    }
}
