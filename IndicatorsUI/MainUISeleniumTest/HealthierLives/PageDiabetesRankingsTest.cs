using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IndicatorsUI.MainUISeleniumTest.HealthierLives
{
    [TestClass]
    public class PageDiabetesCcgRankingsBaseUnitTest : BaseUnitTest
    {
        [TestMethod]
        public void TestDiabetesCcgRankingsLoads()
        {
            var driver = LoadCcgRankingsPage();

            Assert.AreEqual("selected",
                driver.FindElement(By.Id(LongerLivesIds.RankingsNav)).GetAttribute("class"),
                "Rankings header is not selected");

            Assert.AreEqual("National comparisons", GetRankingAreaHeaderText(driver),
                "Page title not as expected");

            Assert.AreEqual("England", driver.FindElement(By.ClassName(Classes.AreaName)).Text,
                "Area name was not England");
        }

        [TestMethod]
        public void TestDiabetesCcgRankingsInfoBoxes()
        {
            var driver = LoadCcgRankingsPage();

            TestHelper.AssertTextContains(GetPopulation(), "population");
            TestHelper.AssertTextContains(driver.FindElement(By.Id(LongerLivesIds.InfoBox2)).Text, "diabetes");
        }

        [TestMethod]
        public void TestDiabetesCcgRankingsNavigationHeadersAsExpected()
        {
            var driver = LoadCcgRankingsPage();
            LongerLivesHelper.CheckNavigationHeaders(driver);
        }

        [TestMethod]
        public void TestNavigationFromRankingsPageToPracticeDetailsPage()
        {
            const string PracticeName = "Parkgate Surgery";

            // Go to rankings page
            var driver = LoadCcgRankingsPage();

            // Click on West Lancashire CCG
            driver.FindElement(By.LinkText(AreaNames.CcgWestLancashire)).Click();
            waitFor.PracticeRankingsForWestLancashireCcgToLoad();

            // Click on practice name
            driver.FindElement(By.LinkText(PracticeName)).Click();
            waitFor.PracticeDetailsToLoad();

            // Check practice page has loaded
            Assert.AreEqual(PracticeName, driver.FindElement(By.ClassName("area_name")).Text);
        }

        private IWebDriver LoadCcgRankingsPage()
        {
            navigateTo.DiabetesRankings();
            new WaitFor(driver).CcgRankingsToLoad();
            return driver;
        }

        private string GetRankingAreaHeaderText(IWebDriver webDriver)
        {
            IWebElement rankingHeader =
              (new WebDriverWait(webDriver, TimeSpan.FromSeconds(20)).Until(
                  ExpectedConditions.ElementExists(By.Id(LongerLivesIds.RankingsHeader))));

            return rankingHeader.Text;
        }

        private string GetPopulation()
        {
            var text = driver.FindElement(By.Id(LongerLivesIds.InfoBox1)).Text;
            return text;
        }
    }
}