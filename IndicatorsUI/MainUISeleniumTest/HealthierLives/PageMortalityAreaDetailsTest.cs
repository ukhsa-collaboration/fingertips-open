using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Profiles.DataAccess;
using Profiles.DomainObjects;

namespace IndicatorsUI.MainUISeleniumTest.HealthierLives
{
    [TestClass]
    public class PageMortalityAreaDetailsTest : BaseUnitTest
    {
        public static void LoadMortalityAreaDetailsForNorthumberlandPage(
            IWebDriver driver)
        {
            var parameters = new HashParameters();
            parameters.AddAreaCode(AreaCodes.Northumberland);
            parameters.AddParentAreaCode(AreaCodes.England);

            driver.Navigate().GoToUrl(AppConfig.Instance.BridgeWsUrl +
                "topic/mortality/area-details" + parameters.HashParameterString);

            // ImplicitlyWait for the area filter to render
            new WaitFor(driver).MortalityAreaDetailRankingToLoad();
        }

        [TestMethod]
        public void MortalityOverallPrematureDataTest()
        {
            LoadMortalityAreaDetailsForNorthumberlandPage(driver);

            Assert.IsTrue(driver.FindElement(By.Id("main_ranking")).Text.Contains("Overall premature deaths"));
            Assert.IsTrue(driver.FindElement(By.Id("cancer_row")).Displayed);
            Assert.IsTrue(driver.FindElement(By.Id("lung_cancer_row")).Displayed);
            Assert.IsTrue(driver.FindElement(By.Id("breast_cancer_row")).Displayed);
            Assert.IsTrue(driver.FindElement(By.Id("colorectal_cancer_row")).Displayed);
            Assert.IsTrue(driver.FindElement(By.Id("heart_disease_and_stroke_row")).Displayed);
            Assert.IsTrue(driver.FindElement(By.Id("heart_disease_row")).Displayed);
            Assert.IsTrue(driver.FindElement(By.Id("stroke_row")).Displayed);
            Assert.IsTrue(driver.FindElement(By.Id("lung_disease_row")).Displayed);
            Assert.IsTrue(driver.FindElement(By.Id("liver_disease_row")).Displayed);
            Assert.IsTrue(driver.FindElement(By.Id("injuries_row")).Displayed);
        }

    }
}
