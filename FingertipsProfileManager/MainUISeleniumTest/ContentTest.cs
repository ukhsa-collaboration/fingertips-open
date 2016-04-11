using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Fpm.MainUISeleniumTest
{
    [TestClass]
    public class ContentTest : BaseUnitTest
    {
        [TestMethod]
        public void ContentIndex()
        {
            var driver = Driver;
            new NavigateTo(driver).ContentIndexPage();
            new WaitFor(driver).PageWithModalPopUpToLoad();

            // Select option in menu
            var profileSelect = driver.FindElement(By.Id("selectedProfile"));
            var selectElement = new SelectElement(profileSelect);
            selectElement.SelectByValue(ProfileIds.Diabetes.ToString());

            // Check the content item table contains expected
            var contentItemTable = driver.FindElement(By.ClassName("grid"));
            var text = contentItemTable.Text;
            TestHelper.AssertTextContains(text, "about the data");
        }
    }
}
