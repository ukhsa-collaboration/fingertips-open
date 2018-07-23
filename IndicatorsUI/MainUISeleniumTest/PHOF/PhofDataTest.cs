using IndicatorsUI.MainUISeleniumTest.Fingertips;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUISeleniumTest.Phof
{
    [TestClass]
    public class PhofDataTest : FingertipsBaseUnitTest
    {
        public const string UrlKey = ProfileUrlKeys.Phof;

        [TestMethod]
        public void TestTartanRugLoadsForAllDomains()
        {
            navigateTo.PhofTartanRug();
            FingertipsDataTest.CheckTartanRugLoadsForAllDomains(driver);
        }

        [TestMethod]
        public void TestAllPhofTabsLoad()
        {
            navigateTo.PhofTartanRug();
            FingertipsHelper.SelectEachFingertipsTabInTurnAndCheckDownloadIsLast(driver);
        }

        [TestMethod]
        public void TestLastSelectedAreaIsRetainedBetweenPageViews()
        {
            navigateTo.PhofTartanRug();
            var areaName = "Middlesbrough";

            FingertipsHelper.SwitchToAreaSearchMode(driver);
            FingertipsHelper.SearchForAnAreaAndSelectFirstResult(driver, areaName);
            FingertipsHelper.LeaveAreaSearchMode(driver);

            // Leave and return to data page
            navigateTo.HomePage();
            navigateTo.PhofTartanRug();

            // Check area menu contains searched for area
            Assert.AreEqual(areaName, FingertipsHelper.GetSelectedAreaNameFromMenu(driver));
        }

        [TestMethod]
        public void TestSearchForAnArea()
        {
            navigateTo.PhofTartanRug();
            var areaName = "Croydon";

            FingertipsHelper.SwitchToAreaSearchMode(driver);
            FingertipsHelper.SearchForAnAreaAndSelectFirstResult(driver, areaName);
            FingertipsHelper.LeaveAreaSearchMode(driver);

            // Check area menu contains searched for area
            Assert.AreEqual(areaName, FingertipsHelper.GetSelectedAreaNameFromMenu(driver));
        }


    }
}
