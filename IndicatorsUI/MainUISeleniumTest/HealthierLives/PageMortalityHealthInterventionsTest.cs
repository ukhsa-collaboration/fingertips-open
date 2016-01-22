using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace MainUISeleniumTest.HealthierLives
{
    [TestClass]
    public class PageMortalityHealthInterventionsTest : BaseUnitTest
    {
        [TestMethod]
        public void TestEachHealthInterventionPageCanBeNavigatedToFromAreaDetailsPage()
        {
            var linkCount = GetHealthInterventionLinks(driver).Count;

            // Check each intervention page can be opened
            for (var i = 0; i < linkCount; i++)
            {
                PageMortalityAreaDetailsTest.LoadMortalityAreaDetailsForNorthumberlandPage(driver);
                var healthInterventionLinks = GetHealthInterventionLinks(driver);
                healthInterventionLinks[i].Click();
                new WaitFor(driver).HealthInterventionPageToLoad();
            }
        }

        [TestMethod]
        public void TestCheckNumberOfHealthInterventionLinksOnAreaDetailsPage()
        {
            PageMortalityAreaDetailsTest.LoadMortalityAreaDetailsForNorthumberlandPage(driver);
            var healthInterventionLinks = GetHealthInterventionLinks(driver);

            Assert.AreEqual(4, healthInterventionLinks.Count,
                "Number of health intervention links not as expected");
        }

        private static ReadOnlyCollection<IWebElement> GetHealthInterventionLinks(IWebDriver driver)
        {
            var healthInterventionLinks = driver.FindElements(By.ClassName("health-intervention"));
            return healthInterventionLinks;
        }
    }
}
