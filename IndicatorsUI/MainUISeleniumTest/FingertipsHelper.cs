using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace IndicatorsUI.MainUISeleniumTest
{
    public class FingertipsHelper
    {
        public static void SelectEachFingertipsTabInTurnAndCheckDownloadIsLast(IWebDriver driver)
        {
            var lastText = SelectEachFingertipsTabInTurn(driver);

            Assert.IsTrue(lastText.Contains("Download"),
                "Download should be last tab");
        }

        /// <summary>
        /// Selects each Fingertips tab in order from left to right.
        /// </summary>
        /// <returns>The HTML of the last selected tab.</returns>
        public static string SelectEachFingertipsTabInTurn(IWebDriver driver)
        {
            var waitFor = new WaitFor(driver);

            // Click through each domain
            var tabs = driver.FindElements(By.ClassName("page"));
            string lastText = string.Empty;
            foreach (var tab in tabs)
            {
                tab.Click();
                WaitFor.ThreadWait(0.1);
                waitFor.AjaxLockToBeUnlocked();

                // Check tab
                var text = tab.Text;
                Assert.AreNotEqual(lastText, text, "Tab clicked but was not selected");
                lastText = text;
            }

            return lastText;
        }

        public static void SelectAreaType(IWebDriver driver, int areaTypeId)
        {
            var areasDropdown = driver.FindElement(By.Id("areaTypes"));
            var selectElements = new SelectElement(areasDropdown);
            selectElements.SelectByValue(areaTypeId.ToString());
            WaitForAjaxLock(driver);
        }

        public static void SelectDomain(IWebDriver driver, int groupId)
        {
            ClickElement(driver, "domain" + groupId);
        }

        public static void SelectFingertipsTab(IWebDriver driver, string pageId)
        {
            ClickElement(driver, pageId);
        }

        public static string GetSelectedAreaNameFromMenu(IWebDriver driver)
        {
            var areasDropdown = driver.FindElement(By.Id(FingertipsIds.AreaMenu));
            var selectElements = new SelectElement(areasDropdown);
            var selectArea = selectElements.SelectedOption.Text;
            return selectArea;
        }

        public static void SearchForAnAreaAndSelectFirstResult(IWebDriver driver, string text)
        {
            var id = By.Id(FingertipsIds.AreaSearchText);
            var searchText = driver.FindElement(id);
            searchText.SendKeys(text);
            new WaitFor(driver).ExpectedElementToBeVisible(By.Id(LongerLivesIds.AreaSearchAutocompleteOptions));
            searchText.SendKeys(Keys.Return);
            WaitForAjaxLock(driver);
        }

        public static void SearchForPractice(IWebDriver driver, string text)
        {
            var id = By.Id(FingertipsIds.GpPracticeSearchText);
            var searchText = driver.FindElement(id);
            searchText.SendKeys(text);
            new WaitFor(driver).ExpectedElementToBeVisible(By.TagName(FingertipsIds.GpPracticeAutoComplete));
        }

        public static void LeaveAreaSearchMode(IWebDriver driver)
        {
            var searchLink = driver.FindElement(By.Id(FingertipsIds.AreaSearchLink));
            searchLink.Click();
            new WaitFor(driver).ExpectedElementToBeVisible(By.Id(FingertipsIds.AreaMenu));
        }

        public static void SwitchToAreaSearchMode(IWebDriver driver)
        {
            var searchLink = driver.FindElement(By.Id(FingertipsIds.AreaSearchLink));
            searchLink.Click();
            new WaitFor(driver).ExpectedElementToBeVisible(By.Id(FingertipsIds.AreaSearchText));
        }

        public static void SelectNextIndicator(IWebElement nextIndicatorButton, WaitFor waitFor)
        {
            nextIndicatorButton.Click();
            WaitFor.ThreadWait(0.1);
            waitFor.AjaxLockToBeUnlocked();
        }

        public static void SelectInequalityTrends(IWebDriver driver)
        {
            var trendsButton = driver.FindElement(By.Id(FingertipsIds.InequalitiesTrends));
            trendsButton.Click();
        }

        public static void SelectInequalitiesLatestValues(IWebDriver driver)
        {
            var trendsButton = driver.FindElement(By.Id(FingertipsIds.InequalitiesLatestValues));
            trendsButton.Click();
        }

        public static void SelectTrendsOnTartanRug(IWebDriver driver)
        {
            var trendsButton = driver.FindElement(By.Id("tab-option-1"));
            trendsButton.Click();
        }

        public static void ClickElement(IWebDriver driver, string pageId)
        {
            var tab = driver.FindElement(By.Id(pageId));
            tab.Click();
            WaitForAjaxLock(driver);
        }

        private static void WaitForAjaxLock(IWebDriver driver)
        {
            WaitFor.ThreadWait(0.1);
            new WaitFor(driver).AjaxLockToBeUnlocked();
        }
    }
}