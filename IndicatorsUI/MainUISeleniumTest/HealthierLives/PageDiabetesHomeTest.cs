using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUISeleniumTest.HealthierLives
{
    [TestClass]
    public class PageDiabetesHomeTest : BaseUnitTest
    {
        [TestMethod]
        public void TestDiabetesHomeNavigationHeaders()
        {
            navigateTo.DiabetesHome();
            LongerLivesHelper.CheckNavigationHeaders(driver);
        }

        [TestMethod]
        public void CheckTextOnHomePageReflectsCcgAreaType()
        {
            navigateTo.DiabetesHome();
            SelectAreaTypeAndWaitForPageToLoad(driver, AreaTypeIds.CcgPreApr2017.ToString());
            CheckTextOnHomePageReflectsSelectedAreaType("CCG");
        }

        [TestMethod]
        public void DomainHelpIconTest()
        {
            navigateTo.DiabetesHome();

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            wait.Until(ExpectedConditions.ElementExists(By.Id(LongerLivesIds.Map)));

            //Hover over the Treatment Target domain tool tip
            var actions = new Actions(driver);
            var tooltip = driver.FindElement(By.XPath("//*[@id='domain-1938132700']/span"));
            actions.MoveToElement(tooltip).Click().Build().Perform();

            //Check to see if the popup tooltip is actually displayed
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='domain-1938132700']/span/i")));

            //Check to ensure the tooltip has content
            Assert.AreNotEqual(driver.FindElements(By.XPath("//*[@id='domain-1938132700']/span/i"))[0].Text, string.Empty);
        }

        [TestMethod]
        public void TestPopUpOpensForCcg()
        {
            navigateTo.DiabetesHome();
            SelectAreaTypeAndWaitForPageToLoad(driver, LongerLivesIds.AreaTypeLinkCcgs);
            LongerLivesHelper.ClickOnMapAndWaitForPopUp(driver, 600, 500);

            // Check pop up contains expected text
            var text = driver.FindElement(By.ClassName(Classes.MapPopUp)).Text;
            TestHelper.AssertTextContains(text, "out of 209 CCGs", "Ranking text not found");
            TestHelper.AssertTextContains(text, "Adults with diabetes", "Expected pop up text not found");
        }

        public static void SelectAreaTypeAndWaitForPageToLoad(IWebDriver driver, string areaTypeLinkId)
        {
            var waitFor = new WaitFor(driver);
            var byAreaTypeLink = By.Id(areaTypeLinkId);
            waitFor.ExpectedElementToBeVisible(byAreaTypeLink);
            driver.FindElement(byAreaTypeLink).Click();

            // Added because this method was finishing before the page had fully switched area type
            WaitFor.ThreadWait(0.1);        
            waitFor.PageToFinishLoading();
            waitFor.GoogleMapToLoad();
        }

        private void CheckTextOnHomePageReflectsSelectedAreaType(string areaTypeString)
        {
            // Check map help reflects area type
            var text = driver.FindElement(By.Id("map-help")).Text;
            TestHelper.AssertTextContains(text, areaTypeString);

            // Check search heading reflects area type
            text = driver.FindElement(By.ClassName("home_search")).Text;
            TestHelper.AssertTextContains(text, areaTypeString);

            // Check search text box reflects area type
            text = driver.FindElement(By.Id("search_text")).GetAttribute("title");
            TestHelper.AssertTextContains(text, areaTypeString);
        }
    }
}
