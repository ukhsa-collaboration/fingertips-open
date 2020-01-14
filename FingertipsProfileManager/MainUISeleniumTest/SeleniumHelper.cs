using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Fpm.MainUISeleniumTest
{
    public class SeleniumHelper
    {
        /// <summary>
        /// Wait for an element to be present on the page.
        /// </summary>
        public static void WaitForExpectedElement(IWebDriver driver, By element, int timeoutInSeconds = 30)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds))
                .Until(c => c.FindElement(element));
        }

        /// <summary>
        /// Wait for an element to be present on the page.
        /// </summary>
        public static void WaitForExpectedElementToBeVisible(IWebDriver driver, By element, int timeoutInSeconds = 30)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds))
                .Until(c => c.FindElement(element).Displayed);
        }

        /// <summary>
        ///     Wrapper around Thread.Sleep().
        /// </summary>
        public static void ThreadWait(double seconds)
        {
            Thread.Sleep((int)(seconds * 1000));
        }
    }
}
