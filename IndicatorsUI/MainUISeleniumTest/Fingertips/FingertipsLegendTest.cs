using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsLegendTest : FingertipsBaseUnitTest
    {
        [TestMethod]
        public void Test_Legend_Show_Hide_Functionality()
        {
            GoToPhofOverview();
            fingertipsHelper.SelectDomainWithText("health improvement");

            // Find the legend link
            var legendLink = driver.FindElement(By.ClassName("overview-legend-link"));

            // Find the recent trend legend
            var recentTrend = driver.FindElement(By.Id("overview-trend-marker-legend"));
            
            // Test recent legend must be displayed
            Assert.IsTrue(recentTrend.Displayed);

            // Click on the legend link
            legendLink.Click();

            // Test recent legend must be hidden
            Assert.IsFalse(recentTrend.Displayed);
        }

        private void GoToPhofOverview()
        {
            navigateTo.PhofOverviewTab();
            waitFor.FingertipsOverviewTabToLoad();
        }

    }
}
