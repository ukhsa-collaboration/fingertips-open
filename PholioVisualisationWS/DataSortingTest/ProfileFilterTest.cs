using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataSortingTest
{
    [TestClass]
    public class ProfileFilterTest
    {
        [TestMethod]
        public void TestRemoveSystemProfileIds()
        {
            var ids = new List<int>
            {
                ProfileIds.Phof, ProfileIds.Search, ProfileIds.Unassigned
            };

            var filteredIds = ProfileFilter.RemoveSystemProfileIds(ids);

            // Assert: all but Phof have been removed
            Assert.AreEqual(1, filteredIds.Count);
            Assert.AreEqual(ProfileIds.Phof, filteredIds.First());
        }

        [TestMethod]
        public void TestRemoveSystemProfiles()
        {
            var profiles = new List<ProfileConfig>
            {
                new ProfileConfig { ProfileId = ProfileIds.Phof},
                new ProfileConfig { ProfileId = ProfileIds.Search}
            };

            var filteredIds = ProfileFilter.RemoveSystemProfiles(profiles);

            // Assert: all but Phof have been removed
            Assert.AreEqual(1, filteredIds.Count);
            Assert.AreEqual(ProfileIds.Phof, filteredIds.First().ProfileId);
        }
    }
}
