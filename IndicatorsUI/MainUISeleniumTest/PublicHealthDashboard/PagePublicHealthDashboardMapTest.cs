using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace IndicatorsUI.MainUISeleniumTest.PublicHealthDashboard
{
    [TestClass]
    public class PagePublicHealthDashboardMapTest : PublicHealthDashboardBaseTest
    {
        [TestMethod]
        public void DomainHelpIconTest()
        {
            const string xpathNhsHealthChecks = "//*[@id='domain-1938133161']";

            navigateTo.PublicHealthDashboardMap();

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id(PublicHealthDashboardIds.Map)));

            // Scroll link into viewport
            fingertipsHelper.ScrollVertically(600);

            // Click domain link
            var actions = new Actions(driver);
            var domainLink = driver.FindElement(By.XPath(xpathNhsHealthChecks));
            actions.MoveToElement(domainLink).Click().Build().Perform();

            // Check to see if the popup tooltip is actually displayed
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath(xpathNhsHealthChecks + "/a")));

            // Check to ensure the tooltip has content
            Assert.AreNotEqual(driver.FindElements(By.XPath(xpathNhsHealthChecks  + "/a"))[0].Text, string.Empty);
        }
    }
}
