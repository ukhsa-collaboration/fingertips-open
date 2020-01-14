using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Fpm.MainUISeleniumTest
{
    [TestClass]
    public class ProfilesForNonAdminTest : BaseUnitTest
    {
        [TestMethod]
        public void Profiles_Page_Loads()
        {
            LoadProfilesPage();
        }

        [TestMethod]
        public void Test_Do_Profile_Details_Load_On_Profile_Selection()
        {
            var driver = Driver;

            LoadProfileIndexAndSelectFirstProfile();

            var profileNameInMenu = GetProfileMenu().Options[1].Text;

            WaitForProfileDetailsToLoad();

            //This will check the text selected from dropdown & value in the textbox
            var nameInput = driver.FindElement(By.Id("Name"));
            var profileNameInTextBox = nameInput.GetAttribute("value");
            Assert.AreEqual(profileNameInTextBox, profileNameInMenu);
        }

        [TestMethod]
        public void Test_Can_Boolean_Flags_Be_Edited_For_ProfileNonAdmin()
        {
            var checkBoxIds = new List<string> { "StartZeroYAxis", "ShowDataQuality",
                "HasTrendMarkers", "UseTargetBenchmarkByDefault" };
            var startingStates = new Dictionary<string, bool>();

            // Load profile
            LoadProfileIndexAndSelectFirstProfile();
            WaitForProfileDetailsToLoad();

            // Click each check box
            foreach (var checkBoxId in checkBoxIds)
            {
                var checkbox = Driver.FindElement(By.Id(checkBoxId));
                startingStates.Add(checkBoxId, checkbox.Selected);
                checkbox.Click();
            }

            // Save profile
            Driver.FindElement(By.Id("update-profile")).Click();

            // Reload the profile
            LoadProfileIndexAndSelectFirstProfile();
            WaitForProfileDetailsToLoad();

            // Assert each checkbox state is different
            foreach (var checkBoxId in checkBoxIds)
            {
                var checkbox = Driver.FindElement(By.Id(checkBoxId));
                Assert.AreNotEqual(startingStates[checkBoxId], checkbox.Selected,
                    "Flag has not changed for: " + checkBoxId);
            }
        }

        private void LoadProfileIndexAndSelectFirstProfile()
        {
            LoadProfilesPage();

            var profile = GetProfileMenu();

            //Select first item from the drowdown
            profile.SelectByIndex(1);
        }

        private SelectElement GetProfileMenu()
        {
            SelectElement profile = new SelectElement(Driver.FindElement(By.Id("profile-menu")));
            return profile;
        }

        /// <summary>
        /// Wait until the partial view is loaded with data. This is checked by trying to find the textbox having css profilename
        /// </summary>
        private void WaitForProfileDetailsToLoad()
        {
            waitFor.ExpectedElementToBePresent(By.Id("Name"));
        }

        private void LoadProfilesPage()
        {
            navigateTo.ProfilesForNonAdminPage();
            waitFor.ProfilesForNonAdminToLoad();
        }
    }
}
