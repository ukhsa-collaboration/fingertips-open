using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Fpm.MainUISeleniumTest
{
    /// <summary>
    /// All these tests fail in Jenkins. Don't know why.
    /// </summary>
    [TestClass]
    public class ProfilesTest : BaseUnitTest
    {

        [TestInitialize]
        public void TestInitialize()
        {
            LoadProfilesPage();
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void Test_Can_Boolean_Flags_Be_Edited_For_Profile()
        {
            var checkBoxIds = new List<string> { "StartZeroYAxis","IsLive","HasOwnFrontPage",
                "ShowDataQuality", "IsNational", "AreIndicatorsExcludedFromSearch", "IsProfileViewable",
                "ShouldBuildExcel", "HasTrendMarkers", "UseTargetBenchmarkByDefault",
                "HasAnyData", "HasStaticReports"};
            var startingStates = new Dictionary<string, bool>();

            // Click first profile link
            ClickFirstProfileLink();
            waitFor.EditProfilePageToLoad();

            // Click each check box
            foreach (var checkBoxId in checkBoxIds)
            {
                var checkbox = Driver.FindElement(By.Id(checkBoxId));
                startingStates.Add(checkBoxId, checkbox.Selected);
                checkbox.Click();
            }

            // Save profile
            Driver.FindElement(By.Id("update-profile")).Click();
            waitFor.ProfilesPageToLoad();

            // Click first profile link again
            ClickFirstProfileLink();
            waitFor.EditProfilePageToLoad();

            // Assert each checkbox state is different
            foreach (var checkBoxId in checkBoxIds)
            {
                var checkbox = Driver.FindElement(By.Id(checkBoxId));
                Assert.AreNotEqual(startingStates[checkBoxId], checkbox.Selected,
                    "Flag has not changed for: " + checkBoxId);
            }
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestAddRemoveUserToProfile()
        {
            ClickFirstProfileLink();
            waitFor.EditProfilePageToLoad();

            // Define elements
            SelectElement userMenu = new SelectElement(Driver.FindElement(By.Id("userId")));
            var userList = Driver.FindElement(By.Id("user-listing"));

            // Select first user
            userMenu.SelectByIndex(1);
            var name = userMenu.Options[1].Text;

            // Remove user
            Driver.FindElement(By.Id("RemoveBtn")).Click();
            waitFor.ElementToNotContainText(userList, name);

            // Add
            Driver.FindElement(By.Id("AddBtn")).Click();
            waitFor.ElementToContainText(userList, name);

            // Remove
            Driver.FindElement(By.Id("RemoveBtn")).Click();
            waitFor.ElementToNotContainText(userList, name);
        }

        [TestMethod]
        public void TestMyPermissionLinkIsVisibleOnNavigationMenu()
        {
            var yourProfiles = Driver.FindElement(By.Id("your-profiles"));
            Assert.IsTrue(yourProfiles.Displayed);
        }

        [TestMethod]
        public void TestNewProfileButtonIsDisplayed()
        {
            // Navigate to profile manager page
            Driver.FindElement(By.XPath("//*[@id='header']/nav[1]/ul/li[1]/a")).Click();
            waitFor.ProfilesPageToLoad();

            // New profile button must be displayed for admin users
            Assert.IsTrue(Driver.FindElement(By.Id("create-profile-button")).Displayed);
        }

        private void ClickFirstProfileLink()
        {
            var profileLink = Driver.FindElements(By.ClassName("profile-link")).First();
            profileLink.Click();
        }

        private void LoadProfilesPage()
        {
            navigateTo.ProfilesPage();
            waitFor.ProfilesPageToLoad();
        }
    }
}
