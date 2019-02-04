using System.Collections.Generic;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DomainObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace IndicatorsUI.MainUISeleniumTest.PublicHealthDashboard
{
    [TestClass]
    public class PagePublicHealthDashboardAreaDetailsTest : BaseUnitTest
    {

        [TestMethod]
        public void PublicHealthDashboardNoDataTest()
        {
            var parameters = new HashParameters();
            parameters.AddAreaCode(AreaCodes.IslesOfScilly);
            parameters.AddParentAreaCode(AreaCodes.England);

            LoadPublicHealthDashboardAreaDetailsPage(driver, parameters.HashParameterString);

            // Assert: no data labels 
            Assert.IsTrue(driver.FindElement(By.Id("a_row")).GetAttribute("class").Equals("no-data"));
            Assert.IsTrue(driver.FindElement(By.Id("c_row")).GetAttribute("class").Equals("no-data"));
        }

        [TestMethod]
        public void PublicHealthDashboardValidDataForAreaTest()
        {
            var parameters = new HashParameters();
            parameters.AddAreaCode(AreaCodes.Croydon);
            parameters.AddParentAreaCode(AreaCodes.England);

            LoadPublicHealthDashboardAreaDetailsPage(driver, parameters.HashParameterString);

            Assert.IsTrue(driver.FindElement(By.Id("a_row")).GetAttribute("class").Contains("grade"));
            Assert.IsTrue(driver.FindElement(By.Id("c_row")).GetAttribute("class").Contains("grade"));
        }

        [TestMethod]
        public void HealthChecksValidValueUnitTest()
        {
            var parameters = new HashParameters();
            parameters.AddAreaCode(AreaCodes.Croydon);
            parameters.AddParentAreaCode(AreaCodes.England);

            LoadPublicHealthDashboardAreaDetailsPage(driver, parameters.HashParameterString);

            IList<IWebElement> percentSigns = driver.FindElements(By.CssSelector(".unit.arial"));
            Assert.IsTrue(percentSigns.Count > 0);
        }

        private void LoadPublicHealthDashboardAreaDetailsPage(IWebDriver webDriver, string parameters)
        {
            webDriver.Navigate().GoToUrl(AppConfig.Instance.BridgeWsUrl + "topic/public-health-dashboard/area-details" + parameters);
            waitFor.AjaxLockToBeUnlocked();
            waitFor.ExpectedElementToBeVisible(By.Id("ranking_bar_charts"));
        }
    }
}