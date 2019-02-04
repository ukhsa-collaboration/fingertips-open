using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;


namespace IndicatorsUI.MainUISeleniumTest.PublicHealthDashboard
{
    [TestClass]
    public class PagePublicHealthDashboardRankingsTest : BaseUnitTest
    {
        [TestMethod]
        public void TestPublicHealthDashboardgRankingsLoads()
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
            WaitFor.ThreadWait(3);

            SeleniumHelper.ClickLinkText(driver, "Summary rank");
            SeleniumHelper.ClickLinkText(driver, "Air Quality");

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