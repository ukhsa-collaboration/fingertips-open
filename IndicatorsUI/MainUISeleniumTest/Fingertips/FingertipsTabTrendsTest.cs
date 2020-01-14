using System.Linq;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsTabTrendsTest : FingertipsBaseUnitTest
    {
        [TestMethod]
        public void Test_Trends_Chart_Export_Menu_Is_Present()
        {
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.SexualHealth);
            fingertipsHelper.SelectTrendsTab();

            CheckExportLinkPresentForTrendsTab();
        }

        private void CheckExportLinkPresentForTrendsTab()
        {
            var byExportLink = By.Id("export-link-trend-chart");
            waitFor.ExpectedElementToBeVisible(byExportLink);
            var exportMenu = driver.FindElement(byExportLink);
            waitFor.ElementToContainText(exportMenu, "Export");
        }
    }
}