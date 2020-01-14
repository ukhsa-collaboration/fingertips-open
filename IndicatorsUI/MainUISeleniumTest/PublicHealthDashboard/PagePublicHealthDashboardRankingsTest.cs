using System;
using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;


namespace IndicatorsUI.MainUISeleniumTest.PublicHealthDashboard
{
    [TestClass]
    public class PagePublicHealthDashboardRankingsTest : PublicHealthDashboardBaseTest
    {
        [TestMethod]
        public void TestPublicHealthDashboardRankingsLoads()
        {
            // Act
            var seleniumDriver = LoadRankingsPage();

            // Assert
            Assert.AreEqual("selected", seleniumDriver.FindElement(By.Id(PublicHealthDashboardIds.RankingsNav)).GetAttribute("class"), "Rankings header is not selected");
            Assert.AreEqual("National comparisons", GetRankingAreaHeaderText(seleniumDriver), "Page title not as expected");
            Assert.IsNotNull(seleniumDriver.FindElement(By.ClassName(Classes.AreaName)).Text);
        }

        [TestMethod]
        public void TestPublicHealthDashboardRankingsNavigationHeadersAsExpected()
        {
            navigateTo.PublicHealthDashboardRankings();
            WaitFor.ThreadWaitInSeconds(3);

            fingertipsHelper.ClickLinkByText("Summary rank");
            fingertipsHelper.ClickLinkByText("Air Quality");

            PublicHealthDashboardHelper.CheckNavigationHeaders(LoadRankingsPage());
        }

        [TestMethod]
        public void TestNavigationFromRankingsPageToAreaDetailsPage()
        {
            // Go to rankings page
            var seleniumDriver = LoadRankingsPage();

            // Click on East Sussex area
            seleniumDriver.FindElement(By.LinkText(AreaNames.EastSussex)).Click();
            waitFor.AreaRankingsToLoad();

            // Check area name page has loaded
            Assert.IsNotNull(seleniumDriver.FindElement(By.ClassName(Classes.AreaName)).Text);
        }

        private IWebDriver LoadRankingsPage()
        {
            navigateTo.PublicHealthDashboardRankings();
            new WaitFor(driver).RankingsToLoad();
            return driver;
        }

        private string GetRankingAreaHeaderText(IWebDriver webDriver)
        {
            IWebElement rankingHeader =
              (new WebDriverWait(webDriver, TimeSpan.FromSeconds(20)).Until(
                  SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id(PublicHealthDashboardIds.RankingsHeader))));

            return rankingHeader.Text;
        }
    }
}