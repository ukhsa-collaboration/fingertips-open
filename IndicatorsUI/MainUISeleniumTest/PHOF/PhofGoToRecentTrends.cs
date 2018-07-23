using IndicatorsUI.MainUISeleniumTest.Fingertips;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace IndicatorsUI.MainUISeleniumTest.Phof
{
    /// <summary>
    /// Tests clicking a recent trend icon navigates to the trend tab
    /// </summary>
    [TestClass]
    public class PhofGoToRecentTrends : FingertipsBaseUnitTest
    {
        [TestMethod]
        public void TartanRugToTrendsPageTest()
        {
            navigateTo.PhofTartanRug();
            waitFor.FingertipsTartanRugToLoad();
            FingertipsHelper.SelectTrendsOnTartanRug(driver);
            waitFor.FingertipsTartanRugToLoad();

            // Click on first column of tartan rug            
            driver.FindElement(By.Id("tc-1-0")).Click();
            
            // Success if trends table loads
            waitFor.FingertipsTrendsTableToLoad();
        }

        [TestMethod]
        public void AreaProfileToTrendsPage()
        {
            navigateTo.PhofAreaProfile();
            waitFor.FingertipsSpineChartToLoad();

            // Click on recent trend cell
            driver.FindElement(By.Id("spine-trend_0")).Click();

            // Success if trends table loads
            waitFor.FingertipsTrendsTableToLoad();
        }

        [TestMethod]
        public void GoToCompareAreas()
        {
            navigateTo.PhofCompareAreas();
            waitFor.FingertipsBarChartTableToLoad();

            // Click England trend icon
            driver.FindElement(By.Id("bar-trend_E92000001")).Click();

            // Success if trends table loads
            waitFor.FingertipsTrendsTableToLoad();
        }

    }
}
