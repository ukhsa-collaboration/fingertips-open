using System;
using Fpm.MainUI.Helpers;
using Fpm.ProfileData.Entities.Profile;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.MainUITest.Helpers
{
    [TestClass]
    public class ProfileDetailsCleanerTest
    {
        [TestMethod]
        public void TestWhitespaceRemoved()
        {
            var details = new ProfileDetails
            {
                Name = " b ",
                AreasIgnoredEverywhere = " a , b ",
                AreasIgnoredForSpineChart = " c , d "
            };

            ProfileDetailsCleaner.CleanUserInput(details);

            Assert.AreEqual("b", details.Name);
            Assert.AreEqual("a,b", details.AreasIgnoredEverywhere);
            Assert.AreEqual("c,d", details.AreasIgnoredForSpineChart);
        }

        [TestMethod]
        public void TestEmptyAreaStringsConvertedToNulls()
        {
            var details = new ProfileDetails
            {
                AreasIgnoredEverywhere = string.Empty,
                AreasIgnoredForSpineChart = string.Empty
            };

            ProfileDetailsCleaner.CleanUserInput(details);

            Assert.IsNull(details.AreasIgnoredEverywhere);
            Assert.IsNull(details.AreasIgnoredForSpineChart);
        }

        [TestMethod]
        public void TestExtraFilesStringsConvertedToNulls()
        {
            var details = new ProfileDetails
            {
                ExtraJsFiles = string.Empty,
                ExtraCssFiles = string.Empty
            };

            ProfileDetailsCleaner.CleanUserInput(details);

            Assert.IsNull(details.ExtraJsFiles);
            Assert.IsNull(details.ExtraCssFiles);
        }
    }
}
