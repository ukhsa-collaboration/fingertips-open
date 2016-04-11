using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Fpm.ProfileData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace Fpm.MainUISeleniumTest
{
    [TestClass]
    public class UserTest : BaseUnitTest
    {
        [TestMethod]
        public void OnEditUserPageProfilesAreListedWhenUserHasSomeToEdit()
        {
            var driver = Driver;

            GoToUserIndex(driver);
            SelectUserToEdit(driver, UserDisplayNames.Doris);

            // Check user has some profiles to edit
            var cells = driver.FindElement(By.ClassName("grid")).FindElements(By.TagName("td"));
            Assert.IsTrue(cells.Count > 10);
        }

        [TestMethod]
        public void OnEditUserPageNoProfilesAreListedWhenUserHasNoPermissions()
        {
            var driver = Driver;

            GoToUserIndex(driver);
            SelectUserToEdit(driver, UserDisplayNames.UserWithNoRightsToAnything);

            // Check for message that explains the user has no permissions
            var profileSubheading = driver.FindElement(By.XPath("//*[@id=\"EditUser\"]/h3"));
            Assert.AreEqual("This user does not have permission to edit any profiles", profileSubheading.Text);
        }

        private static void SelectUserToEdit(IWebDriver driver, string userName)
        {
            var links = driver.FindElements(By.ClassName("user-link"));
            links.First(x => x.Text.ToLower().Contains(userName.ToLower())).Click();
            new WaitFor(driver).EditUserPageToLoad();
        }

        private void GoToUserIndex(IWebDriver driver)
        {
            new NavigateTo(driver).UserIndexPage();
            new WaitFor(driver).PageWithModalPopUpToLoad();
        }
    }
}
