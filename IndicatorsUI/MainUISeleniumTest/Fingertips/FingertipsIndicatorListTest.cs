using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsIndicatorListTest : FingertipsBaseUnitTest
    {
        [TestMethod]
        public void Test_Can_Navigate_To_Sign_In()
        {
            navigateTo.HomePage();

            var menu = driver.FindElement(By.Id("your-account"));

            // Click menu to display options
            var accountButton = menu.FindElement(By.Id("your-account-button"));
            TestHelper.AssertElementTextIsEqual("Your data", accountButton);
            accountButton.Click();

            // Click sign in
            var signIn = menu.FindElement(By.ClassName("dropdown-item"));
            TestHelper.AssertElementTextIsEqual("Sign in", signIn);
            signIn.Click();

            waitFor.SignInPageToLoad();

            // Assert sign in page loaded
            var heading = driver.FindElement(By.Id("frm-login-for-fingertips")).FindElement(By.TagName("h2"));
            TestHelper.AssertElementTextIsEqual("Sign in", heading);
        }
    }
}
