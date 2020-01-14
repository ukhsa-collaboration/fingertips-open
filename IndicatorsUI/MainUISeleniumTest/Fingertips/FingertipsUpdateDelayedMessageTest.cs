using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUISeleniumTest.Helpers;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class PhofUpdateDelayedMessageTest : FingertipsBaseUnitTest
    {
        const string Key = "ShowUpdateDelayedMessage";

        [TestMethod]
        public void Test_Holding_Message_Can_Be_Displayed()
        {           
            SeleniumHelper.SetValueInWebConfig(Key, "true");
            LoadPhofPage();
            Assert.IsTrue(IsUpdateDelayedPresent());
        }

        [TestMethod]
        public void Test_Holding_Message_Disabled()
        {            
            SeleniumHelper.SetValueInWebConfig(Key, "false");           
            LoadPhofPage();
            Assert.IsFalse(IsUpdateDelayedPresent());            
        }

        private void LoadPhofPage()
        {
            navigateTo.FingertipsFrontPageForProfile(ProfileUrlKeys.Phof);
            waitFor.PhofDomainsToLoad();
        }

        private bool IsUpdateDelayedPresent()
        {
            try
            {
                return driver.FindElement(By.Id("update-delayed")).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}