using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsTabMapTest : FingertipsBaseUnitTest
    {
        [TestMethod]
        public void Test_Map_Export_Menu_Is_Present()
        {
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.SexualHealth);
            fingertipsHelper.SelectMapTab();

            // Assert link is present
            var tab = driver.FindElement(By.Id("tab-specific-options"));
            var link = tab.FindElement(By.ClassName("export-link"));
            Assert.IsNotNull(link);
        }
    }
}
