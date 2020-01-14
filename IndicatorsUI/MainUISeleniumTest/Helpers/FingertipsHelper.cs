using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUISeleniumTest.Fingertips;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace IndicatorsUI.MainUISeleniumTest.Helpers
{
    public class FingertipsHelper
    {
        private IWebDriver _driver;
        private WaitFor _waitFor;

        public FingertipsHelper(IWebDriver driver)
        {
            _driver = driver;
            _waitFor = new WaitFor(_driver);
        }

        public void SelectNearestNeighbours()
        {
            _driver.FindElement(By.Id("nearest-neighbour-link")).Click();
            _waitFor.FingertipsNearestNeighboursLinksToLoad();
            WaitForAjaxLockToBeUnlocked();
        }

        public void ExitNearestNeighbours()
        {
            _driver.FindElement(By.Id("exit-nearest-neighbours")).Click();
            WaitForAjaxLockToBeUnlocked();
        }

        public void SelectEnglandTab()
        {
            SelectTab(FingertipsIds.TabEngland);
            _waitFor.FingertipsEnglandTableToLoad();
        }

        public void SelectInequalitiesTab()
        {
            SelectTab(FingertipsIds.TabInequalities);
            _waitFor.InequalitiesTabToLoad();
        }

        public void SelectCompareIndicatorsTab()
        {
            SelectTab(FingertipsIds.TabCompareIndicators);
        }

        public void SelectDefinitionsTab()
        {
            SelectTab(FingertipsIds.TabDefinitions);
            _waitFor.FingertipsDefinitionsTableToLoad();
        }

        public void SelectTrendsTab()
        {
            SelectTab(FingertipsIds.TabTrends);
        }

        public void SelectMapTab()
        {
            SelectTab(FingertipsIds.TabMap);
        }

        public void SelectTab(string elementId)
        {
            ClickElementById(elementId);
            WaitForAjaxLockToBeUnlocked();
        }

        public void SelectEachFingertipsTabInTurnAndCheckDownloadIsLast()
        {
            var lastText = SelectEachFingertipsTabInTurn();

            Assert.IsTrue(lastText.Contains("Download"),
                "Download should be last tab");
        }

        /// <summary>
        /// Selects each Fingertips tab in order from left to right.
        /// </summary>
        /// <returns>The HTML of the last selected tab.</returns>
        public string SelectEachFingertipsTabInTurn()
        {
            // Click through each domain
            var tabs = _driver.FindElements(By.ClassName("page"));
            string lastText = string.Empty;
            foreach (var tab in tabs)
            {
                tab.Click();
                WaitForAjaxLockToBeUnlocked();

                // Check tab
                var text = tab.Text;
                Assert.AreNotEqual(lastText, text, "Tab clicked but was not selected");
                lastText = text;
            }

            return lastText;
        }

        public void SelectAreaType(int areaTypeId)
        {
            var by = By.Id("areaTypes");
            _waitFor.ExpectedElementToBeVisible(by);
            var areasDropdown = _driver.FindElement(by);
            var selectElements = new SelectElement(areasDropdown);
            selectElements.SelectByValue(areaTypeId.ToString());
            WaitForAjaxLockToBeUnlocked();
        }

        /// <summary>
        /// Have to select by name because option values are group root indexes not indicator Ids
        /// </summary>
        public void SelectIndicatorByName(string indicatorName)
        {
            SelectElement indicatorMenuDropdown = new SelectElement(_driver.FindElement(By.Id("indicatorMenu")));
            indicatorMenuDropdown.SelectByText(indicatorName);
            WaitForAjaxLockToBeUnlocked();
        }

        public void SelectCompareIndicatorsSupportingIndicator(string text)
        {
            _waitFor.ExpectedElementToBeVisible(By.Id("change-indicator"));

            _driver.FindElement(By.Id("change-indicator")).Click();
            _waitFor.ExpectedElementToBeVisible(By.Id("indicator-search-text"));

            _driver.FindElement(By.Id("indicator-search-text")).SendKeys(text);
            _waitFor.ExpectedElementToBeVisible(By.ClassName("ng-tns-c46-0"));

            _driver.FindElement(By.ClassName("ng-tns-c46-0")).Click();
            WaitForAjaxLockToBeUnlocked();
        }

        public string GetSelectedAreaNameFromMenu()
        {
            var areasDropdown = _driver.FindElement(By.Id(FingertipsIds.AreaMenu));
            var selectElements = new SelectElement(areasDropdown);
            var selectArea = selectElements.SelectedOption.Text;
            return selectArea;
        }

        public void SearchForAnAreaAndSelectFirstResult(string text)
        {
            var id = By.Id(FingertipsIds.AreaSearchText);
            var searchText = _driver.FindElement(id);
            searchText.SendKeys(text);
            _waitFor.ExpectedElementToBeVisible(By.Id(PublicHealthDashboardIds.AreaSearchAutocompleteOptions));
            searchText.SendKeys(Keys.Return);
            WaitForAjaxLockToBeUnlocked();
        }

        public void SearchForPractice(string text)
        {
            var id = By.Id(FingertipsIds.GpPracticeSearchText);
            var searchText = _driver.FindElement(id);
            searchText.SendKeys(text);
            _waitFor.ExpectedElementToBeVisible(By.TagName(FingertipsIds.GpPracticeAutoComplete));
        }

        public void LeaveAreaSearchMode()
        {
            var searchLink = _driver.FindElement(By.Id(FingertipsIds.AreaSearchLink));
            searchLink.Click();
            _waitFor.ExpectedElementToBeVisible(By.Id(FingertipsIds.AreaMenu));
        }

        public void SwitchToAreaSearchMode()
        {
            var searchLink = _driver.FindElement(By.Id(FingertipsIds.AreaSearchLink));
            searchLink.Click();
            _waitFor.ExpectedElementToBeVisible(By.Id(FingertipsIds.AreaSearchText));
        }

        public void SelectNextIndicator()
        {
            var nextIndicatorButton = _driver.FindElement(By.Id("next-indicator"));
            nextIndicatorButton.Click();
            WaitForAjaxLockToBeUnlocked();
        }

        public void SelectInequalitiesTrends()
        {
            var trendsButton = _driver.FindElement(By.Id(FingertipsIds.InequalitiesTrends));
            trendsButton.Click();
            _waitFor.InequalitiesTrendTableToLoad();
            WaitForAjaxLockToBeUnlocked();
        }

        public void SelectInequalitiesLatestValues()
        {
            var trendsButton = _driver.FindElement(By.Id(FingertipsIds.InequalitiesLatestValues));
            trendsButton.Click();
            WaitForAjaxLockToBeUnlocked();
        }

        public void SelectInequalitiesForChildArea()
        {
            FindElementById("inequalities-tab-option-3").Click();
            WaitForAjaxLockToBeUnlocked();
        }

        public void SelectTrendsOnOverviewTab()
        {
            var trendsButton = _driver.FindElement(By.Id("tab-option-1"));
            trendsButton.Click();
        }

        public ReadOnlyCollection<IWebElement> GetDomainOptions()
        {
            var domains = _driver.FindElements(By.ClassName("domain-option"));
            return domains;
        }

        public void SelectDomainWithText(string text)
        {
            // Find domain
            var lowerText = text.ToLower();
            var domains = GetDomainOptions();
            var domain = domains.FirstOrDefault(d => d.Text.ToLower().Contains(lowerText));

            // Check matching domain was found
            if (domain == null)
            {
                throw new FingertipsException("Could not find domain with text: " + text);
            }
            
            // Select domain
            domain.Click();
            WaitForAjaxLockToBeUnlocked();
        }

        public void ClickLinkByText(string linkText)
        {
            try
            {
                var byLink = By.LinkText(linkText);
                var link = _driver.FindElement(byLink);
                _waitFor.ExpectedElementToBeVisible(byLink);

                link.Click();
            }
            catch (Exception e)
            {
                throw new Exception("There was an error during clicking." + e.Message);
            }
        }

        public void ClickElementById(string elementId)
        {
            var element = _driver.FindElement(By.Id(elementId));
            element.Click();
            WaitForAjaxLockToBeUnlocked();
        }


        public void CheckTartanRugHasLoaded()
        {
            IList<IWebElement> firstRow = _driver.FindElements(By.Id(FingertipsIds.TartanRugIndicatorNameOnFirstRow));
            Assert.IsTrue(firstRow.Any());
        }

        public void CheckElementDisplayed(string elementId, bool shouldBeDisplayed, string errorMessage)
        {
            bool elementDisplayed;

            try
            {
                var element = FindElementById(elementId);
                elementDisplayed = element != null && element.Displayed;
            }
            catch (Exception)
            {
                elementDisplayed = false;
            }
            

            if (shouldBeDisplayed && !elementDisplayed)
            {
                Assert.Fail(errorMessage);
            }

            if (!shouldBeDisplayed && elementDisplayed)
            {
                Assert.Fail(errorMessage);
            }
        }

        public void CheckElementNumbersIsMinorOfMax(string elementClassName, int maximumElement, 
            string errorMessage = "The class is contained more times than specified")
        {
            var elementList = FindElementsByClass(elementClassName);

            if (elementList.Count > maximumElement)
            {
                Assert.Fail(errorMessage);
            }
        }

        public void CheckTextIsNumberTimesByTagName(string tagName, string text, int timesNumber,
            string errorMessage = "The class is contained more times than specified")
        {
            var elementList = FindElementsTextByTagName(tagName, text).ToList();

            if (elementList.Count != timesNumber)
            {
                Assert.Fail(errorMessage);
            }
        }

        public void CheckTextIsNumberTimesByClassName(string className, string text, int timesNumber, 
            string errorMessage = "The class is contained more times than specified")
        {
            var elementList = FindElementsTextByClassName(className, text).ToList();

            if (elementList.Count != timesNumber)
            {
                Assert.Fail(errorMessage);
            }
        }

        public void SelectOptionByText(string elementId, string optionText)
        {
            var element = this.FindElementById(elementId);
            var selectElement = new SelectElement(element);
            selectElement.SelectByText(optionText);
            WaitForAjaxLockToBeUnlocked();
        }

        /// <summary>
        /// Positive number of pixels to scroll down. Negative number to scroll up.
        /// </summary>
        public void ScrollVertically(int pixels)
        {
            (_driver as IJavaScriptExecutor).ExecuteScript("scroll(0," + pixels + ")");
        }

        public IWebElement FindElementById(string elementId)
        {
            var byId = getById(elementId);
            _waitFor.ExpectedElementToBePresent(byId);

            var element = _driver.FindElement(byId);
            return element;
        }

        public ReadOnlyCollection<IWebElement> FindElementsByXPath(string xPath)
        {
            var byXPath = getByXPath(xPath);
            _waitFor.ExpectedElementToBePresent(byXPath);

            var element = _driver.FindElements(byXPath);
            return element;
        }

        public ReadOnlyCollection<IWebElement> FindElementsByCssSelector(string elementCssSelector)
        {
            var byCssSelector = getByCssSelector(elementCssSelector);
            _waitFor.ExpectedElementToBePresent(byCssSelector);

            var element = _driver.FindElements(byCssSelector);
            return element;
        }

        public ReadOnlyCollection<IWebElement> FindElementsByClass(string elementClassName)
        {
            var byClass = getByClass(elementClassName);
            _waitFor.ExpectedElementToBePresent(byClass);

            var element = _driver.FindElements(byClass);
            return element;
        }

        public IEnumerable<IWebElement> FindElementsTextByTagName(string tagName, string text)
        {
            var byTagName = getByTagName(tagName);

            var element = _driver.FindElements(byTagName).Where(x=> x.Text == text);
            return element;
        }

        public  IEnumerable<IWebElement> FindElementsTextByClassName(string className, string text)
        {
            var byClassName = getByClass(className);

            var element = _driver.FindElements(byClassName).Where(x => x.Text == text);
            return element;
        }

        public bool IsElementVisible(By by)
        {
            return _driver.FindElement(by).Displayed;
        }

        public string MoveOverElementAndGetTooltipText(By @by)
        {
            // Move pointer over element
            var marker = _driver.FindElements(@by).FirstOrDefault();
            new Actions(_driver).MoveToElement(marker).Perform();

            // Wait for tooltip to be displayed
            var tooltipId = "tooltip";
            _waitFor.ExpectedElementToBeVisible(By.Id(tooltipId));

            // Return tooltip content
            var text = FindElementById(tooltipId).Text;
            return text;
        }

        public void MoveOverElement(By @by)
        {
            // Move pointer over element
            var marker = _driver.FindElements(@by).FirstOrDefault();
            new Actions(_driver).MoveToElement(marker).Perform();
        }

        public ReadOnlyCollection<IWebElement> MoveToElementByXPath(string xPath, int elementPosition = 1)
        {
            const int positionMoveBack = 1;
            try
            {
                var foundElement = FindElementsByXPath(xPath);

                if (elementPosition <= 0 || elementPosition - positionMoveBack > foundElement.Count)
                {
                    throw new ArgumentException("The position of element to catch is invalid");
                }
                var action = new Actions(_driver);
                action.MoveToElement(foundElement[elementPosition - positionMoveBack]).Perform();

               return foundElement;
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
                return null;
            }
        }

        public void SortAreasByXPath(string xPath)
        {
            FindElementsByXPath(xPath)[0].Click();
            WaitFor.ThreadWaitInSeconds(0.5);
        }

        private void WaitForAjaxLockToBeUnlocked()
        {
            WaitFor.ThreadWaitInSeconds(0.1);
            _waitFor.AjaxLockToBeUnlocked();
        }

        private static By getById(string elementId)
        {
            return By.Id(elementId);
        }

        private static By getByClass(string elementClassName)
        {
            return By.ClassName(elementClassName);
        }

        private static By getByCssSelector(string elementCssSelector)
        {
            return By.CssSelector(elementCssSelector);
        }

        private static By getByXPath(string xPath)
        {
            return By.XPath(xPath);
        }

        private static By getByTagName(string tagName)
        {
            return By.TagName(tagName);
        }
  
    }
}