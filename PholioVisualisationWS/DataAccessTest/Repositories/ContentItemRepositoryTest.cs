using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccessTest.Repositories
{
    [TestClass]
    public class ContentItemRepositoryTest
    {
        public const string ContentKey = ContentKeys.Test;
        public const string ContentValue = "Test Item";

        private ContentItemRepository _repository;

        [TestInitialize]
        public void TestInitialize()
        {
            _repository = new ContentItemRepository();
        }

        [TestMethod]
        public void TestGetContentForProfile()
        {
            var text = _repository.GetContentForProfile(ProfileIds.PhysicalActivity, ContentKey);
            Assert.AreEqual(ContentValue, text.Content);
        }

        [TestMethod]
        public void TestPublishContentItem()
        {
            _repository.SaveContentItems(Publish());
        }

        private IList<ContentItem> Publish()
        {
            return new List<ContentItem>()
            {
                new ContentItem
                {
                    Id = 13,
                    ContentKey = "contact-us",
                    ProfileId = ProfileIds.Phof,
                    Description = "Contact us",
                    Content = "<i>Test</i> <b>content<b> item.",
                    IsPlainText = true
                },
                new ContentItem
                {
                    Id = 14,
                    ContentKey = "further-info",
                    ProfileId = ProfileIds.Phof,
                    Description = "Further information",
                    Content = "<i>Test</i> <b>content<b> item.",
                    IsPlainText = true
                }
            };
        }
    }
}
