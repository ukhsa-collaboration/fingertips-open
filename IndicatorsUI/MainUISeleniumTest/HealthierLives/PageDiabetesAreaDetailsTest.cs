using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Profiles.DataAccess;
using Profiles.DomainObjects;

namespace MainUISeleniumTest.HealthierLives
{
    [TestClass]
    public class PageDiabetesAreaDetailsTest : BaseUnitTest
    {
        private void LoadHealthChecksAreaDetailsPage(IWebDriver webDriver, string parameters)
        {
            webDriver.Navigate().GoToUrl(AppConfig.Instance.BridgeWsUrl + "topic/nhs-health-check/area-details" + parameters);
        }

        private void LoadDrugsAndAlcoholAreaDetailsPage(IWebDriver webDriver, string parameters)
        {
            webDriver.Navigate().GoToUrl(AppConfig.Instance.BridgeWsUrl + "topic/drugs-and-alcohol/area-details" + parameters);
        }

        [TestMethod]
        public void HealthChecksNoDataTest()
        {
            var parameters = new HashParameters();
            parameters.AddAreaCode(AreaCodes.IslesOfScilly);
            parameters.AddParentAreaCode(AreaCodes.England);

            LoadHealthChecksAreaDetailsPage(driver, parameters.HashParameterString);

            // ImplicitlyWait for the area filter to render
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElement(By.Id("main_ranking")));

            Assert.IsTrue(driver.FindElement(By.Id("main_ranking")).Text.Contains("NO DATA"));
            Assert.IsTrue(driver.FindElement(By.Id("c3")).Text.Contains("No data"));
        }

        [TestMethod]
        public void HealthChecksValidDataForAreaTest()
        {
            var parameters = new HashParameters();
            parameters.AddAreaCode(AreaCodes.Croydon);
            parameters.AddParentAreaCode(AreaCodes.England);

            LoadHealthChecksAreaDetailsPage(driver, parameters.HashParameterString);

            // ImplicitlyWait for the area filter to render
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElement(By.Id("main_ranking")));

            Assert.IsFalse(driver.FindElement(By.Id("main_ranking")).Text.Contains("NO DATA"));
        }

        [TestMethod]
        public void HealthChecksValidValueUnitTest()
        {
            var parameters = new HashParameters();
            parameters.AddAreaCode(AreaCodes.Croydon);
            parameters.AddParentAreaCode(AreaCodes.England);

            LoadHealthChecksAreaDetailsPage(driver, parameters.HashParameterString);

            // ImplicitlyWait for the area filter to render
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElement(By.Id("main_ranking")));

            IList<IWebElement> percentSigns = driver.FindElements(By.CssSelector(".unit.arial"));
            Assert.IsTrue(percentSigns.Count > 0);
        }

        [TestMethod]
        public void HealthCheckHeadlineIndicatorIslesOfScillyNoDataTest()
        {
            var parameters = new HashParameters();
            parameters.AddAreaCode(AreaCodes.IslesOfScilly);
            parameters.AddParentAreaCode(AreaCodes.England);

            LoadHealthChecksAreaDetailsPage(driver, parameters.HashParameterString);

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElement(By.Id("main_ranking")));
            
            Assert.IsTrue(driver.FindElement(By.Id("main_ranking")).GetAttribute("class").Contains("no-data"));
        }

        [TestMethod]
        public void DrugsAndAlcoholHeaderTest()
        {
            var parameters = new HashParameters();
            parameters.AddAreaCode(AreaCodes.Croydon);
            parameters.AddParentAreaCode(AreaCodes.England);

            LoadDrugsAndAlcoholAreaDetailsPage(driver, parameters.HashParameterString);
            
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElement(By.Id("data_page_table")));
            
            Assert.IsTrue(driver.FindElement(By.Id("data_page_header")).Text.Contains("Croydon"));
        }

        [TestMethod]
        public void DrugsAndAlcoholAreaRankingTableNameTest()
        {
            var parameters = new HashParameters();
            parameters.AddAreaCode(AreaCodes.Croydon);
            parameters.AddParentAreaCode(AreaCodes.England);

            LoadDrugsAndAlcoholAreaDetailsPage(driver, parameters.HashParameterString);

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElement(By.Id("data_page_table")));

            Assert.AreEqual("Drugs and Alcohol", driver.FindElement(By.XPath(XPaths.DrugsAndAlcoholAreaRankingsTableName)).Text);
        }

        [TestMethod]
        public void DrugsAndAlcoholIslesOfScillyNoDataTest()
        {
            var parameters = new HashParameters();
            parameters.AddAreaCode(AreaCodes.IslesOfScilly);
            parameters.AddParentAreaCode(AreaCodes.England);

            LoadDrugsAndAlcoholAreaDetailsPage(driver, parameters.HashParameterString);

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElement(By.Id("data_page_table")));

            var rankingTableElements = driver.FindElements(By.Id("drug_treatment_row"));
                                              
            foreach (var webElement in rankingTableElements)
            {
                Assert.AreEqual("none", webElement.GetAttribute("class"));
                Assert.AreEqual(string.Empty, webElement.FindElement(By.ClassName("drugs-n-alcohol")).Text);
            }
        }
    }
}