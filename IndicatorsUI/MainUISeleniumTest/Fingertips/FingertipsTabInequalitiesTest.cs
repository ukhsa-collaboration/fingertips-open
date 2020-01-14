using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsTabInequalitiesTest : FingertipsBaseUnitTest
    {
        [TestMethod]
        public void Test_Changing_Between_Latest_Values_And_Trends()
        {
            GoToPhofInequalities();

            // Click on Trends button
            fingertipsHelper.SelectInequalitiesTrends();
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
            fingertipsHelper.SelectInequalitiesLatestValues();
            Assert.IsFalse(AreTrendFiltersVisible());
        }

        [TestMethod]
        public void Test_Inequalities_Chart_Loads_For_Each_Indicator_For_All_Domains()
        {
            GoToPhofInequalities();

            var domains = fingertipsHelper.GetDomainOptions();
            foreach (var domain in domains)
            {
                domain.Click();
                waitFor.AjaxLockToBeUnlocked();

                // Make sure it is possible to view each indicator until get back to the first one
                string initialHeaderText = GetInequalityIndicatorName();
                string headerText = string.Empty;
                while (headerText != initialHeaderText)
                {
                    fingertipsHelper.SelectNextIndicator();
                    headerText = GetInequalityIndicatorName();
                }
            }
        }

        [TestMethod]
        public void Test_Inequalities_Description_Lightbox()
        {
            GoToPhofInequalities();
            fingertipsHelper.SelectDomainWithText("healthcare and premature mortality");
            waitFor.FingertipsCategoryTypeDescriptionsToLoad();

            CheckCategoryTypeDescriptionPopUpAsExpected();
        }

        [TestMethod]
        public void Test_Inequalities_Trend_Table()
        {
            GoToPhofInequalities();
            fingertipsHelper.SelectDomainWithText("healthcare and premature mortality");

            // Navigate to the inequalities section
            fingertipsHelper.SelectInequalitiesTab();

            // Click on trends tab button
            fingertipsHelper.SelectInequalitiesTrends();

            // Check whether data is displayed
            var firstDataCell = driver.FindElement(By.XPath(
                "//*[@id=\"inequalities-container\"]/div/div[4]/div/div[3]/ft-inequalities-trend-table/div/table/tbody/tr[1]/td[2]"));
            Assert.IsTrue(firstDataCell.Displayed);

        }

        [TestMethod]
        public void Test_Inequalities_Indicator_With_Notes_Should_Be_Displayed_For_Latest_Values()
        {
            var cssSelector = "rect.highcharts-point:nth-child(1)";
            var tooltipCssSelector = "g.highcharts-label:nth-child(19) > text:nth-child(5) > tspan:nth-child(1)";

            GoToUrl();

            new WaitFor(driver).ExpectedElementToBeVisible(By.LinkText("Sex"));
            fingertipsHelper.ClickLinkByText("Sex");
            WaitFor.ThreadWaitInSeconds(0.5);
            var sexTooltipMessage = GetTooltipMessage(cssSelector, tooltipCssSelector);

            fingertipsHelper.ClickLinkByText("Age");
            WaitFor.ThreadWaitInSeconds(0.5);
            var ageTooltipMessage = GetTooltipMessage(cssSelector, tooltipCssSelector);

            fingertipsHelper.ClickLinkByText("District & UA deprivation deciles in England (IMD2015, 4/19 geog.)");
            WaitFor.ThreadWaitInSeconds(0.5);
            var decileTooltipMessage = GetTooltipMessage(cssSelector, tooltipCssSelector);

            fingertipsHelper.ClickLinkByText("Disability");
            WaitFor.ThreadWaitInSeconds(0.5);
            var disabilityTooltipMessage = GetTooltipMessage(cssSelector, tooltipCssSelector);

            Assert.IsTrue(sexTooltipMessage.Contains("*"));
            Assert.IsTrue(ageTooltipMessage.Contains("*"));
            Assert.IsTrue(decileTooltipMessage.Contains("*"));
            Assert.IsTrue(disabilityTooltipMessage.Contains("*"));
        }

        [TestMethod]
        public void Test_Inequalities_Indicator_With_Notes_Should_Be_Displayed_For_Trends_Values()
        {
            var trendsButtonId = "inequalities-tab-option-1";
            var cssSelector = "path.highcharts-color-1:nth-child(2)";
            var tooltipCssSelector = ".highcharts-label > text:nth-child(5) > tspan:nth-child(3)";
            var tableCssSelector = ".table > tbody:nth-child(2) > tr:nth-child(1) > td:nth-child(2)";

            GoToUrl();

            new WaitFor(driver).ExpectedElementToBeVisible(By.Id(trendsButtonId));
            fingertipsHelper.ClickElementById(trendsButtonId);

            // Sex
            new WaitFor(driver).ExpectedElementToBeVisible(By.LinkText("Sex"));
            fingertipsHelper.ClickLinkByText("Sex");
            WaitFor.ThreadWaitInSeconds(0.5);
            var sexTooltipMessage = GetTooltipMessage(cssSelector, tooltipCssSelector);
            var sexTableMessage = GetTextByCssSelector(tableCssSelector);
            Assert.IsTrue(sexTooltipMessage.Contains("*"));
            Assert.IsTrue(sexTableMessage.Contains("*"));

            // Age
            fingertipsHelper.ClickLinkByText("Age");
            WaitFor.ThreadWaitInSeconds(0.5);
            var ageTooltipMessage = GetTooltipMessage(cssSelector, tooltipCssSelector);
            var ageTableMessage = GetTextByCssSelector(tableCssSelector);
            Assert.IsTrue(ageTooltipMessage.Contains("*"));
            Assert.IsTrue(ageTableMessage.Contains("*"));
            
            // Deprivation
            fingertipsHelper.ClickLinkByText("District & UA deprivation deciles in England (IMD2015, 4/19 geog.)");
            WaitFor.ThreadWaitInSeconds(0.5);
            var decileTooltipMessage = GetTooltipMessage(cssSelector, tooltipCssSelector);
            var decileTableMessage = GetTextByCssSelector(tableCssSelector);
            Assert.IsTrue(decileTooltipMessage.Contains("*"));
            Assert.IsTrue(decileTableMessage.Contains("*"));

            // Disability
            fingertipsHelper.ClickLinkByText("Disability");
            WaitFor.ThreadWaitInSeconds(0.5);
            var disabilityTooltipMessage = GetTooltipMessage(cssSelector, tooltipCssSelector);
            var disabilityTableMessage = GetTextByCssSelector(tableCssSelector);
            Assert.IsTrue(disabilityTooltipMessage.Contains("*"));
            Assert.IsTrue(disabilityTableMessage.Contains("*"));
        }

        private string GetTooltipMessage(string categoryCssSelector, string tooltipCssSelector)
        {
            var byCssSelector = By.CssSelector(categoryCssSelector);
            fingertipsHelper.MoveOverElement(byCssSelector);
            WaitFor.ThreadWaitInSeconds(0.5);
            return GetTextByCssSelector(tooltipCssSelector);
        }

        private string GetTextByCssSelector(string cssSelector)
        {
            var element = fingertipsHelper.FindElementsByCssSelector(cssSelector).FirstOrDefault();
            var text = element != null ? element.Text : "";
            return text;
        }

        private string GetInequalityIndicatorName()
        {
            return driver.FindElement(By.ClassName("trend-link")).Text;
        }

        private void GoToPhofInequalities()
        {
            navigateTo.PhofInequalitiesTab();
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
            driver.FindElement(By.XPath("//*[@id='inequalities-trend-filters']/div[1]/a")).Click();
        }

        private IList<IWebElement> GetTrendChart()
        {
            return driver.FindElements(By.Id("inequalities-trend-chart"));
        }

        private IList<IWebElement> GetTrendFilters()
        {
            return driver.FindElements(By.Id("inequalities-trend-filters"));
        }

        private void CheckCategoryTypeDescriptionPopUpAsExpected()
        {
            // Get the list of information tooltip icons based on the class name
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(By.ClassName(Classes.InformationToolTipWithPosition));

            foreach (IWebElement element in elements)
            {
                CheckCategoryTypeDescriptionPopUpIsVisible(element);
            }
        }

        private void CheckCategoryTypeDescriptionPopUpIsVisible(IWebElement element)
        {
            // Open the information pop-up
            element.Click();

            // Wait for the pop-up to load
            waitFor.FingertipsCategoryTypeDescriptionPopupToLoad();

            // Get the reference of the information pop-up displayed
            var box = driver.FindElement(By.ClassName("info-box"));

            // Check whether the information pop-up is displayed
            Assert.IsNotNull(box);

            // Find the close icon of the opened pop-up
            var closeIcon = driver.FindElement(By.ClassName("active"));

            // Close the pop-up
            closeIcon.Click();
        }

        private void SelectAreaType(string optionText)
        {
            fingertipsHelper.SelectOptionByText("areaTypes", optionText);

        }

        private void SelectIndicator(string optionText)
        {
            fingertipsHelper.SelectOptionByText("indicatorMenu", optionText);
        }

        private void GoToUrl()
        {
            var _parameters = new HashParameters();

            _parameters.AddTabId(TabIds.Inequalities);
            _parameters.Add("gid", "1938133272");
            _parameters.Add("pat", 6);
            _parameters.AddParentAreaCode(AreaCodes.RegionEastOfEngland);
            _parameters.AddAreaTypeId(AreaTypeIds.CountyAndUnitaryAuthorityPre2019);
            _parameters.AddAreaCode("E06000055");
            _parameters.AddIndicatorId(IndicatorIds.IndicatorForTestNotes);
            _parameters.AddAgeId(204);
            _parameters.AddSexId(SexIds.Persons);

            navigateTo.GoToUrl(ProfileUrlKeys.DevelopmentProfileForTesting + _parameters.HashParameterString);
            waitFor.PageToFinishLoading();
            waitFor.AjaxLockToBeUnlocked();
        }
    }
}
