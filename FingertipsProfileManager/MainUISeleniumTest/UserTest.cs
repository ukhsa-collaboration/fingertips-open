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
        public void OnCreateUserDefaultGroupPermissionIsAssigned()
        {
            var driver = Driver;

            GoToUserIndex(driver);

            var counter = 1;
            var table = driver.FindElement(By.Id("userTable"));
            var displayName = "Test User";
            while (true)
            {
                if (!table.Text.Contains(displayName + counter))
                {
                    displayName = displayName + counter;
                    break;
                }

                counter++;
            }

            var userName = "phe\\" + displayName.Replace(' ', '.');

            driver.FindElement(By.Id("create-new-user")).Click();
            new WaitFor(driver).CreateUserPageToLoad();

            driver.FindElement(By.Id("DisplayName")).SendKeys(displayName);
            driver.FindElement(By.Id("UserName")).SendKeys(userName);
            driver.FindElement(By.Id("Confirm")).Click();
            new WaitFor(driver).UserIndexPageToLoad();

            driver.FindElement(By.LinkText(displayName)).Click();
            new WaitFor(driver).EditUserPageToLoad();

            Assert.IsTrue(driver.FindElement(By.Id("tbl-profiles-user-can-edit")).Displayed);
            Assert.IsTrue(driver.FindElement(By.Id("tbl-profiles-user-can-edit")).Text.Contains(ProfileIds.IndicatorsForReview.ToString()));
        }

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

            // Switch to inactive user list
            driver.FindElement(By.Id("toggle-current-users")).Click();

            SelectUserToEdit(driver, UserDisplayNames.UserWithNoRightsToAnything);

            // Check for message that explains the user has no permissions
            var profileSubheading = driver.FindElement(By.Id("no-permissions"));
            Assert.AreEqual("This user does not have permission to edit any profiles", profileSubheading.Text);
        }

        [TestMethod]
        public void OnEditUserPageCanUserBeSetAsReviewer()
        {
            var driver = Driver;

            GoToUserIndex(driver);
            SelectUserToEdit(driver, UserDisplayNames.UserToTestIsReviewerAccess);

            // Provide reviewer access to the user
            driver.FindElement(By.Id("IsReviewer")).Click();

            // Save the user details
            driver.FindElement(By.Id("Confirm")).Click();

            // Wait for the user list page to load
            new WaitFor(driver).UserIndexPageToLoad();

            // Select the same user to edit
            SelectUserToEdit(driver, UserDisplayNames.UserToTestIsReviewerAccess);

            // Check whether the reviewer checkbox is checked
            IWebElement isReviewerCheckboxElement = driver.FindElement(By.Id("IsReviewer"));
            Assert.IsTrue(isReviewerCheckboxElement.Selected);

            // Revoke reviewer access to the user
            driver.FindElement(By.Id("IsReviewer")).Click();

            // Save the user details
            driver.FindElement(By.Id("Confirm")).Click();

            // Wait for the user list page to load
            new WaitFor(driver).UserIndexPageToLoad();
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
