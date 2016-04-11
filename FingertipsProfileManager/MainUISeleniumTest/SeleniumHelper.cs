using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Fpm.MainUISeleniumTest
{
    public class SeleniumHelper
    {
        /// <summary>
        /// Wait for an element to be present on the page.
        /// </summary>
        public static void WaitForExpectedElement(IWebDriver driver, By element, int timeoutInSeconds = 10)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds))
                .Until(ExpectedConditions.ElementExists(element));
        }

        /// <summary>
        /// Wait for an element to be present on the page.
        /// </summary>
        public static void WaitForExpectedElementToBeVisible(IWebDriver driver, By element, int timeoutInSeconds = 10)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds))
                .Until(ExpectedConditions.ElementIsVisible(element));
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
