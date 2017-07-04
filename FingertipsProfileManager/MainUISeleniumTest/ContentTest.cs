using Fpm.ProfileData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

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
            new WaitFor(driver).ContentIndexPageToLoad();

            // Select option in menu
            var profileSelect = driver.FindElement(By.Id("profileId"));
            var selectElement = new SelectElement(profileSelect);
            selectElement.SelectByValue(ProfileIds.Diabetes.ToString());
            new WaitFor(driver).ContentIndexPageToLoad();

            // Check the content item table contains expected
            var contentItemTable = driver.FindElement(By.ClassName("grid"));
            var text = contentItemTable.Text;
            TestHelper.AssertTextContains(text, "about the data");
        }
    }
}
