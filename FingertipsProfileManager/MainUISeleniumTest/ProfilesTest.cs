using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace Fpm.MainUISeleniumTest
{
    [TestClass]
    public class ProfilesTest : BaseUnitTest
    {
        [TestMethod]
        public void Profiles_Page_Loads()
        {
            LoadProfilesPage();
        }

        [TestMethod]
        public void Test_Can_Boolean_Flags_Be_Edited_For_Profile()
        {
            var checkBoxIds = new List<string> { "StartZeroYAxis","IsLive","HasOwnFrontPage",
                "ShowDataQuality", "IsNational", "AreIndicatorsExcludedFromSearch", "IsProfileViewable",
                "ShouldBuildExcel", "HasTrendMarkers", "UseTargetBenchmarkByDefault", "ShowAreaSearchOnProfileFrontPage",
                "HasAnyData", "HasStaticReports"};
            var startingStates = new Dictionary<string,bool>();

            LoadProfilesPage();

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
            Driver.FindElement(By.Id("update_profile")).Click();
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
