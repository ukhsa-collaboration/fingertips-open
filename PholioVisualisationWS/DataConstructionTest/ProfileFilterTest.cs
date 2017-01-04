using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class ProfileFilterTest
    {
        [TestMethod]
        public void TestRemoveSystemProfileIds()
        {
            var ids = new List<int>
            {
                ProfileIds.Phof, ProfileIds.Archived, ProfileIds.Search, ProfileIds.Unassigned
            };

            var filteredIds = ProfileFilter.RemoveSystemProfileIds(ids);

            // Assert: all but Phof have been removed
            Assert.AreEqual(1, filteredIds.Count);
            Assert.AreEqual(ProfileIds.Phof, filteredIds.First());
        }
    }
}
