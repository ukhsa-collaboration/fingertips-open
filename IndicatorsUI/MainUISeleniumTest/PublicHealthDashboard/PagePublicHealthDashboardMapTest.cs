using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace IndicatorsUI.MainUISeleniumTest.PublicHealthDashboard
{
    [TestClass]
    public class PagePublicHealthDashboardMapTest : BaseUnitTest
    {
        [TestMethod]
        public void DomainHelpIconTest()
        {
            navigateTo.PublicHealthDashboardMap();

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id(PublicHealthDashboardIds.Map)));

            //Hover over the Treatment Target domain tool tip
            var actions = new Actions(driver);
            var tooltip = driver.FindElement(By.XPath("//*[@id='domain-1938133161']"));
            actions.MoveToElement(tooltip).Click().Build().Perform();

            //Check to see if the popup tooltip is actually displayed
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='domain-1938133161']/a")));

            //Check to ensure the tooltip has content
            Assert.AreNotEqual(driver.FindElements(By.XPath("//*[@id='domain-1938133161']/a"))[0].Text, string.Empty);
        }
    }
}
