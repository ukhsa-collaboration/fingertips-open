using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class ContentProviderTest
    {
        private const int ProfileId = 1;
        private const string ContentKey = "a";
        private const string ContentWithHtml = "html";
        private const string ContentWithHtmlRemoved = "html-removed";

        private Mock<HtmlCleaner> _htmlCleaner;
        private Mock<IContentItemRepository> _repository;

        [TestInitialize]
        public void TestInitialize()
        {
               _htmlCleaner = new Mock<HtmlCleaner>(MockBehavior.Strict);
               _repository = new Mock<IContentItemRepository>(MockBehavior.Strict);
        }

        [TestMethod]
        public void TestGetContentWithHtmlRemoved_Html_Is_Removed_For_Html_Content()
        {
            // Arrange
            var htmlContentItem = new ContentItem
            {
                Content = ContentWithHtml,
                IsPlainText = false
            };

            _htmlCleaner.Setup(x => x.RemoveHtml(ContentWithHtml))
                .Returns(ContentWithHtmlRemoved);

            _repository.Setup(x => x.GetContentForProfile(ProfileId, ContentKey))
                .Returns(htmlContentItem);

            // Act
            var content = new ContentProvider(_repository.Object, _htmlCleaner.Object)
                .GetContentWithHtmlRemoved(ProfileId, ContentKey);

            // Assert
            Assert.AreEqual(ContentWithHtmlRemoved, content);
            VerifyAll();
        }

        [TestMethod]
        public void TestGetContentWithHtmlRemoved_Returns_Empty_String_If_Content_Item_Is_Null()
        {
            // Arrange
            _repository.Setup(x => x.GetContentForProfile(ProfileId, ContentKey))
                .Returns((ContentItem)null);

            // Act
            var content = new ContentProvider(_repository.Object, _htmlCleaner.Object)
                .GetContentWithHtmlRemoved(ProfileId, ContentKey);

            // Assert
            Assert.AreEqual(string.Empty, content);
            VerifyAll();
        }

        private void VerifyAll()
        {
            _repository.VerifyAll();
            _htmlCleaner.VerifyAll();
        }
    }
}
