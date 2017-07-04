using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.MainUISeleniumTest
{
    public class WaitFor
    {
        private IWebDriver driver;
        public const int TimeoutLimitInSeconds = 30;
        const double ShortWaitDurationInSeconds = 0.2;


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
                By.ClassName("grid"));
        }

        public void ProfilesForNonAdminToLoad()
        {
            SeleniumHelper.WaitForExpectedElement(driver,
                By.Id("profile-menu"));
        }

        public void EditProfilePageToLoad()
        {
            SeleniumHelper.WaitForExpectedElement(driver,
                By.Id("main"));
        }

        public void ContentIndexPageToLoad()
        {
            SeleniumHelper.WaitForExpectedElement(driver,
                By.Id("profileId"));
        }

        /// <summary>
        /// Wait for an element to be present on the page.
        /// </summary>
        public void ExpectedElementToBePresent(By element)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(TimeoutLimitInSeconds))
                .Until(ExpectedConditions.ElementExists(element));
        }

        public void ElementToContainText(IWebElement element, string text)
        {
            const double totalSecondsToCheckFor = TimeoutLimitInSeconds;
            double secondsCheckedFor = 0;
            while (secondsCheckedFor < totalSecondsToCheckFor)
            {
                if (element.Text.ToLower().Contains(text.ToLower()))
                {
                    return;
                }

                // Wait until next try
                ThreadWait(ShortWaitDurationInSeconds);
                secondsCheckedFor += ShortWaitDurationInSeconds;
            }

            Assert.Fail("Element did not contain expected text after waiting");
        }

        public void ElementToNotContainText(IWebElement element, string text)
        {
            const double totalSecondsToCheckFor = TimeoutLimitInSeconds;
            double secondsCheckedFor = 0;
            while (secondsCheckedFor < totalSecondsToCheckFor)
            {
                if (element.Text.ToLower().Contains(text.ToLower()) == false)
                {
                    return;
                }

                // Wait until next try
                ThreadWait(ShortWaitDurationInSeconds);
                secondsCheckedFor += ShortWaitDurationInSeconds;
            }

            Assert.Fail("Element did not contain expected text after waiting");
        }

        /// <summary>
        /// DO NOT USE unless you really have to!!
        /// Use ExpectedElementToBeVisible to be visible instead.
        /// </summary>
        public static void ThreadWait(double seconds)
        {
            Thread.Sleep((int)(seconds * 1000));
        }
    }
}
