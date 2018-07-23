using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace IndicatorsUI.MainUISeleniumTest.HealthierLives
{
    [TestClass]
    public class HealthierLivesTest : BaseUnitTest
    {
        [TestMethod]
        public void Test_Healthier_Lives_Landing_Page()
        {
            navigateTo.HomePage();

            // Check home link is present
            TestHelper.AssertElementTextIsEqual("Home",
                driver.FindElement(By.XPath(XPaths.NavHome)));
        }
    }
}
