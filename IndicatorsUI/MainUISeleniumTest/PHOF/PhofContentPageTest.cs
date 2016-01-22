using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace MainUISeleniumTest.Phof
{
    [TestClass]
    public class PhofContentPageTest : PhofBaseUnitTest
    {
        const string Key = "ShowUpdateDelayedMessage";

        [TestMethod]
        public void TestHoldingMessageCanBeDisplayed()
        {           
            SeleniumHelper.SetValueInWebConfig(Key, "true");
            LoadPhofPage();
            Assert.IsTrue(IsUpdateDelayedPresent());
        }

        [TestMethod]
        public void TestHoldingMessageDisabled()
        {            
            SeleniumHelper.SetValueInWebConfig(Key, "false");           
            LoadPhofPage();
            Assert.IsFalse(IsUpdateDelayedPresent());            
        }

        private void LoadPhofPage()
        {
            navigateTo.GoToUrl("");
            new WaitFor(driver).PhofDomainsToLoad();
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