using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace Fpm.MainUISeleniumTest
{
    public class WaitFor
    {
        private IWebDriver driver;
        public const int TimeoutLimitInSeconds = 30;


        public WaitFor(IWebDriver driver)
        {
            this.driver = driver;
        }

        public void PageWithModalPopUpToLoad()
        {
            SeleniumHelper.WaitForExpectedElement(driver,
                By.ClassName("a-modal"));
        }

        public void EditUserPageToLoad()
        {
            SeleniumHelper.WaitForExpectedElement(driver,
                By.Id("Confirm"));
        }

        public void ProfilesPageToLoad()
        {
            SeleniumHelper.WaitForExpectedElement(driver,
                By.Id("IndicatorManagementForm"));
        }

        public void EditProfilePageToLoad()
        {
            SeleniumHelper.WaitForExpectedElement(driver,
                By.Id("main"));
        }

        /// <summary>
        /// Wait for an element to be present on the page.
        /// </summary>
        public void ExpectedElementToBePresent(By element)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(TimeoutLimitInSeconds))
                .Until(ExpectedConditions.ElementExists(element));
        }
    }
}
