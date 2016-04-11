using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.ProfileDataTest
{
    [TestClass]
    public class DefaultProfileContentWriterTest
    {
        public const int ProfileId = ProfileIds.HealthProfiles;

        private static ProfilesWriter profilesWriter = ReaderFactory.GetProfilesWriter();

        [TestMethod]
        public void TestCreateContentItems()
        {
            // Delete any existing content items
            profilesWriter.DeleteContentItem(ContentKeys.Description, ProfileId);
            profilesWriter.DeleteContentItem(ContentKeys.Introduction, ProfileId);
            profilesWriter.DeleteContentItem(ContentKeys.RecentUpdates, ProfileId);

            var contentWriter = new DefaultProfileContentWriter(profilesWriter,
                ProfileId, "UnitTester");

            contentWriter.CreateContentItems();

            AssertProperty(ContentKeys.Description,
                DefaultProfileContentWriter.ContentDescription);
            AssertProperty(ContentKeys.Introduction,
                DefaultProfileContentWriter.ContentIntroduction);
            AssertProperty(ContentKeys.RecentUpdates,
                DefaultProfileContentWriter.ContentRecentUpdates);
        }

        private static void AssertProperty(string tag, string expectedContent)
        {
            var contentItem = profilesWriter.GetContentItem(tag, ProfileId);
            Assert.IsTrue(contentItem.Content.Equals(expectedContent));
        }
    }
}
