using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MainUISeleniumTest.HealthierLives
{
    [TestClass]
    public class PageMortalityRankingsTest : BaseUnitTest
    {
        [TestMethod]
        public void TestRankingsPageLoads()
        {
            LoadMortalityRankingsPage();
            new WaitFor(driver).CountyUaRankingsToLoad();

            // Check population
            var population = driver.FindElement(By.Id(LongerLivesIds.MortalityRankingPopulation)).Text;
            Assert.AreNotEqual(string.Empty, population);

            // Check premature deaths
            var prematureDeaths = driver.FindElement(By.Id(LongerLivesIds.MortalityPrematureDeaths)).Text;
            Assert.AreNotEqual(string.Empty, prematureDeaths);

            Assert.IsTrue(GetRankingHeaderText().Contains("Mortality rankings"));

            // Check selected area type as expected
            Assert.AreEqual("areaFilter active", driver.FindElement(By.Id(LongerLivesIds.AreaTypeLinkCountyUa)).GetAttribute("class"));
        }

        [TestMethod]
        public void TestRankingsPageLoadsForCountyUa()
        {
            LoadMortalityRankingsPage();
            new WaitFor(driver).CountyUaRankingsToLoad();
            
            driver.FindElement(By.XPath(XPaths.AreaTypeLinkCountyUas)).Click();

            new WaitFor(driver).CountyUaRankingsToLoad();

            List<string> countyUaAreas = GetAreaNamesFromRankingTable();
            Assert.IsTrue(countyUaAreas.Contains("Westminster"));
        }

        [TestMethod]
        public void TestRankingsPageLoadsForDistrictUa()
        {
            LoadMortalityRankingsPage();
            new WaitFor(driver).CountyUaRankingsToLoad();

            driver.FindElement(By.XPath(XPaths.AreaTypeLinkDistrictUas)).Click();

            new WaitFor(driver).DistrictUaRankingsToLoad();

            List<string> districtUaAreas = GetAreaNamesFromRankingTable();
            Assert.IsTrue(districtUaAreas.Contains("Fenland"));
        }

        private void LoadMortalityRankingsPage()
        {
            navigateTo.MortalityRankings();
        }

        private string GetRankingHeaderText()
        {
            var text = driver.FindElement(By.ClassName(Classes.MortalityRankingsHeader)).Text;
            return text;
        }

        private List<string> GetAreaNamesFromRankingTable()
        {
            var areas = new List<string>();

            IWebElement table =
                (new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(
                    ExpectedConditions.ElementExists(By.Id(LongerLivesIds.MortalityRankingsTable))));

            ReadOnlyCollection<IWebElement> rows = table.FindElements(By.XPath(XPaths.MortalityRankingsTableTrs));
            
            foreach (var cells in rows.Select(x=> x.FindElements(By.XPath("td[2]/span/a"))))
            {
                areas.Add(cells.First().Text);
            }

            return areas;
        }

        private static string GetRankingTable(IWebDriver driver)
        {
            return driver.FindElement(By.Id(LongerLivesIds.MortalityRankingsTable)).Text;
        }

        [TestMethod]
        public void ClickEachIndicatorInTurnAndCheckRankingTableChanges()
        {
            LoadMortalityRankingsPage();
            new WaitFor(driver).CountyUaRankingsToLoad();

            // Overall mortality
            driver.FindElement(By.Id("Overall-Premature")).Click();
            WaitFor.ThreadWait(1);
            var prematureDeathsTable = GetRankingTable(driver);

            // Overall Cancer
            driver.FindElement(By.Id("Overall-Cancer")).Click();
            WaitFor.ThreadWait(1);
            var cancerDeathsTable = GetRankingTable(driver);
            Assert.IsFalse(cancerDeathsTable.Equals(prematureDeathsTable));

            // Overall Lung Cancer
            driver.FindElement(By.Id("Overall-Lung-Cancer")).Click();
            WaitFor.ThreadWait(1);
            var lungCancerDeathsTable = GetRankingTable(driver);
            Assert.IsFalse(lungCancerDeathsTable.Equals(cancerDeathsTable));

            // Overall Breast Cancer
            driver.FindElement(By.Id("Overall-Breast-Cancer")).Click();
            WaitFor.ThreadWait(1);
            var breastCancerDeathsTable = GetRankingTable(driver);
            Assert.IsFalse(breastCancerDeathsTable.Equals(lungCancerDeathsTable));

            // Overall Colorectal Cancer
            driver.FindElement(By.Id("Overall-Colorectal-Cancer")).Click();
            WaitFor.ThreadWait(1);
            var colorectalCancerDeathsTable = GetRankingTable(driver);
            Assert.IsFalse(colorectalCancerDeathsTable.Equals(breastCancerDeathsTable));

            // Overall Heart disease & stroke
            driver.FindElement(By.Id("Overall-Heart-Disease-And-Stroke")).Click();
            WaitFor.ThreadWait(1);
            var heartDiseaseAndStrokeDeathsTable = GetRankingTable(driver);
            Assert.IsFalse(heartDiseaseAndStrokeDeathsTable.Equals(colorectalCancerDeathsTable));

            // Overall Heart disease
            driver.FindElement(By.Id("Overall-Heart-Disease")).Click();
            WaitFor.ThreadWait(1);
            var heartDiseaseDeathsTable = GetRankingTable(driver);
            Assert.IsFalse(heartDiseaseDeathsTable.Equals(heartDiseaseAndStrokeDeathsTable));

            // Overall stroke
            driver.FindElement(By.Id("Overall-Stroke")).Click();
            WaitFor.ThreadWait(1);
            var strokeDeathsTable = GetRankingTable(driver);
            Assert.IsFalse(strokeDeathsTable.Equals(heartDiseaseDeathsTable));

            // Overall Lung disease
            driver.FindElement(By.Id("Overall-Lung-Disease")).Click();
            WaitFor.ThreadWait(1);
            var lungDiseaseTable = GetRankingTable(driver);
            Assert.IsFalse(lungDiseaseTable.Equals(heartDiseaseAndStrokeDeathsTable));

            // Overall Liver disease
            driver.FindElement(By.Id("Overall-Liver-Disease")).Click();
            WaitFor.ThreadWait(1);
            var liverDiseaseTable = GetRankingTable(driver);
            Assert.IsFalse(liverDiseaseTable.Equals(lungDiseaseTable));

            // Overall Injury
            driver.FindElement(By.Id("Overall-Injury")).Click();
            WaitFor.ThreadWait(1);
            var injuryTable = GetRankingTable(driver);
            Assert.IsFalse(injuryTable.Equals(liverDiseaseTable));
        }
    }
}
