using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DomainObjects;
using System.Collections.Generic;

namespace IndicatorsUI.MainUISeleniumTest.HealthierLives
{
    [TestClass]
    public class PageDiabetesAreaDetailsTest : BaseUnitTest
    {
        private void LoadHealthChecksAreaDetailsPage(IWebDriver webDriver, string parameters)
        {
            webDriver.Navigate().GoToUrl(AppConfig.Instance.BridgeWsUrl + "topic/nhs-health-check/area-details" + parameters);
            waitFor.AjaxLockToBeUnlocked();
            waitFor.ExpectedElementToBeVisible(By.Id("main_ranking"));
        }

        private void LoadDrugsAndAlcoholAreaDetailsPage(IWebDriver webDriver, string parameters)
        {
            webDriver.Navigate().GoToUrl(AppConfig.Instance.BridgeWsUrl + "topic/drugs-and-alcohol/area-details" + parameters);
            waitFor.AjaxLockToBeUnlocked();
            waitFor.ExpectedElementToBeVisible(By.Id("data_page_table"));
        }

        [TestMethod]
        public void HealthChecksNoDataTest()
        {
            var parameters = new HashParameters();
            parameters.AddAreaCode(AreaCodes.IslesOfScilly);
            parameters.AddParentAreaCode(AreaCodes.England);

            LoadHealthChecksAreaDetailsPage(driver, parameters.HashParameterString);

            // Assert: no data labels 
            Assert.IsTrue(driver.FindElement(By.Id("main_ranking")).Text.Contains("NO DATA"));
            var text = driver.FindElement(By.Id("c3")).Text;
            Assert.IsTrue(text.Contains("No data"));
        }

        [TestMethod]
        public void HealthChecksValidDataForAreaTest()
        {
            var parameters = new HashParameters();
            parameters.AddAreaCode(AreaCodes.Croydon);
            parameters.AddParentAreaCode(AreaCodes.England);

            LoadHealthChecksAreaDetailsPage(driver, parameters.HashParameterString);

            Assert.IsFalse(driver.FindElement(By.Id("main_ranking")).Text.Contains("NO DATA"));
        }

        [TestMethod]
        public void HealthChecksValidValueUnitTest()
        {
            var parameters = new HashParameters();
            parameters.AddAreaCode(AreaCodes.Croydon);
            parameters.AddParentAreaCode(AreaCodes.England);

            LoadHealthChecksAreaDetailsPage(driver, parameters.HashParameterString);

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

            Assert.IsTrue(driver.FindElement(By.Id("main_ranking")).GetAttribute("class").Contains("no-data"));
        }

        [TestMethod]
        public void DrugsAndAlcoholHeaderTest()
        {
            var parameters = new HashParameters();
            parameters.AddAreaCode(AreaCodes.Croydon);
            parameters.AddParentAreaCode(AreaCodes.England);

            LoadDrugsAndAlcoholAreaDetailsPage(driver, parameters.HashParameterString);

            Assert.IsTrue(driver.FindElement(By.Id("data_page_header")).Text.Contains("Croydon"));
        }

        [TestMethod]
        public void DrugsAndAlcoholAreaRankingTableNameTest()
        {
            var parameters = new HashParameters();
            parameters.AddAreaCode(AreaCodes.Croydon);
            parameters.AddParentAreaCode(AreaCodes.England);

            LoadDrugsAndAlcoholAreaDetailsPage(driver, parameters.HashParameterString);

            Assert.AreEqual("Drugs and Alcohol", driver.FindElement(By.XPath(XPaths.DrugsAndAlcoholAreaRankingsTableName)).Text);
        }

        [TestMethod]
        public void DrugsAndAlcoholIslesOfScillyNoDataTest()
        {
            var parameters = new HashParameters();
            parameters.AddAreaCode(AreaCodes.IslesOfScilly);
            parameters.AddParentAreaCode(AreaCodes.England);

            LoadDrugsAndAlcoholAreaDetailsPage(driver, parameters.HashParameterString);

            var rankingTableElements = driver.FindElements(By.Id("a_row"));

            foreach (var webElement in rankingTableElements)
            {
                Assert.AreEqual("no-data", webElement.GetAttribute("class"));
                TestHelper.AssertTextContains(webElement.Text, "no data");
            }
        }
    }
}