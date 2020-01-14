using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Linq;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsTabEnglandTest : FingertipsBaseUnitTest
    {

        [TestMethod]
        public void Test_England_Data_Displayed()
        {
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.HealthProfiles);
            fingertipsHelper.SelectEnglandTab();

            // Assert expected properties are displayed
            var rows = driver.FindElement(By.Id("england-table")).FindElements(By.TagName("tr"));
            Assert.IsTrue(rows.Any());
        }

    }
}
