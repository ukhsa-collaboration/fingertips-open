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
        public void TestCanChangeAreaTypeOnCcgRankingsPage()
        {
            var driver = LoadCcgRankingsPage();

            // Wait for CountyUA link to be rendered
            new WaitFor(driver).ExpectedElementToBePresent(By.Id(LongerLivesIds.AreaTypeLinkCountyUa));

            // CCG to County & UA
            SelectCountyUas(driver);
            List<string> countyUaAreas = GetAreaNamesFromRankingTable(driver);
            Assert.IsTrue(countyUaAreas.Contains("Isles of Scilly"));

            // County & UA back to CCG
            SelectCcgs(driver);
            List<string> ccgAreas = GetAreaNamesFromRankingTable(driver);
            Assert.IsTrue(ccgAreas.Contains("NHS St Helens CCG"));
        }

        [TestMethod]
        public void TestValueNoteIconsExist()
        {
            var driver = LoadCcgRankingsPage();
            SelectCcgs(driver);
            new WaitFor(driver).AjaxLockToBeUnlocked();

            // Click on an indicator which has a value note
            driver.FindElement(By.LinkText("People with diabetes meeting treatment targets")).Click();
            new WaitFor(driver).AjaxLockToBeUnlocked();
            new WaitFor(driver).ExpectedElementToBeVisible(
                By.ClassName(Classes.PrimaryValueNoteTooltip));

            // Check value note is displayed
            var valueNoteTooltip = driver.FindElement(By.ClassName(Classes.PrimaryValueNoteTooltip));
            Assert.IsTrue(valueNoteTooltip.Displayed);
        }

        [TestMethod]
        public void TestNavigationFromRankingsPageToPracticeDetailsPage()
        {
            // Go to rankings page
            var driver = LoadCcgRankingsPage();

            // Click on West Lancashire CCG
            driver.FindElement(By.LinkText(AreaNames.CcgWestLancashire)).Click();
            waitFor.PracticeRankingsForWestLancashireCcgToLoad();

            // Click on practice name
            driver.FindElement(By.LinkText("PARKGATE SURGERY")).Click();
            waitFor.PracticeDetailsToLoad();

            // Check practice page has loaded
            Assert.AreEqual("PARKGATE SURGERY", driver.FindElement(By.ClassName("area_name")).Text);
        }

        private static void SelectCountyUas(IWebDriver driver)
        {
            driver.FindElement(By.XPath(XPaths.AreaTypeLinkCountyUas)).Click();
            new WaitFor(driver).AjaxLockToBeUnlocked();
            var tableHeader = driver.FindElement(By.Id(LongerLivesIds.GpCount));
            new WaitFor(driver).ElementToContainText(tableHeader, "Counties & Unitary Authorities");
        }

        private static void SelectCcgs(IWebDriver driver)
        {
            driver.FindElement(By.XPath(XPaths.AreaTypeLinkCcgs)).Click();
            new WaitFor(driver).AjaxLockToBeUnlocked();
            new WaitFor(driver).ExpectedElementToBePresent(By.Id(LongerLivesIds.GpCount));
            IWebElement tableHeader = driver.FindElement(By.Id(LongerLivesIds.GpCount));
            new WaitFor(driver).ElementToContainText(tableHeader, "CCGs");
        }

        private IWebDriver LoadCcgRankingsPage()
        {
            navigateTo.DiabetesRankings();
            new WaitFor(driver).CcgRankingsToLoad();
            return driver;
        }

        private List<string> GetAreaNamesFromRankingTable(IWebDriver webDriver)
        {
            var areas = new List<string>();

            IWebElement table =
                (new WebDriverWait(webDriver, TimeSpan.FromSeconds(10)).Until(
                    ExpectedConditions.ElementExists(By.Id(LongerLivesIds.DiabetesRankingsTable))));

            ReadOnlyCollection<IWebElement> rows = table.FindElements(By.XPath(XPaths.DiabetesRankingsTableTrs));

            foreach (var cells in rows.Select(x => x.FindElements(By.XPath("td"))))
            {
                areas.Add(cells.First().Text);
            }

            return areas;
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