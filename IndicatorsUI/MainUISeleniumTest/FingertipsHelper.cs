using MainUISeleniumTest.Fingertips;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Profiles.DomainObjects;

namespace MainUISeleniumTest
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

        public static string GetSelectedAreaNameFromMenu(IWebDriver driver)
        {
            var areasDropdown = driver.FindElement(By.Id(FingertipsIds.AreaMenu));
            var selectElements = new SelectElement(areasDropdown);
            var selectArea = selectElements.SelectedOption.Text;
            return selectArea;
        }

        public static void SearchForAnArea(IWebDriver driver, string text)
        {
            var id = By.Id(FingertipsIds.AreaSearchText);
            var searchText = driver.FindElement(id);
            searchText.SendKeys(text);
            new WaitFor(driver).ExpectedElementToBeVisible(By.Id(LongerLivesIds.AreaSearchAutocompleteOptions));
            searchText.SendKeys(Keys.Return);
            WaitFor.ThreadWait(0.1);
            new WaitFor(driver).AjaxLockToBeUnlocked();
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
    }
}