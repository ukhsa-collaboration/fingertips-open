using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServicesWeb.Controllers;
using PholioVisualisation.ServicesWeb.Helpers;

namespace PholioVisualisation.ServicesWebTest.Controllers
{
    [TestClass]
    public class ContentControllerTest
    {
        private const int ProfileId = ProfileIds.Phof;
        private const string ContentKey = "contact-us";

        private Mock <IContentItemRepository> _repository;
        private Mock<IRequestContentParserHelper> _parserHelper;

        private ContentController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _repository = new Mock<IContentItemRepository>(MockBehavior.Strict);
            _parserHelper = new Mock<IRequestContentParserHelper>(MockBehavior.Strict);
            _controller = CreateController();
        }

        [TestMethod]
        public void TestGetContent_Content_Found()
        {
            // Arrange
            var contentItem = new ContentItem
            {
                Content = "a"
            };
            _repository.Setup(x => x.GetContentForProfile(ProfileId, ContentKey)).Returns(contentItem);

            // Act
            var content = GetContent();

            // Assert: content is defined
            Assert.IsFalse(string.IsNullOrEmpty(content));

            VerifyAll();
        }

        [TestMethod]
        public void TestGetContent_Empty_String_Returned_If_No_Content()
        {
            // Arrange
            _repository.Setup(x => x.GetContentForProfile(ProfileId, ContentKey)).Returns((ContentItem)null);

            // Act
            var content = GetContent();

            // Assert: no content item found
            Assert.AreEqual(string.Empty, content);

            VerifyAll();
        }

        private string GetContent()
        {
            string content = _controller.GetContent(ProfileId, ContentKey);
            return content;
        }

        private ContentController CreateController()
        {
            return new ContentController(_repository.Object, _parserHelper.Object);
        }

        private void VerifyAll()
        {
            _repository.VerifyAll();
            _parserHelper.VerifyAll();
        }
    }
}
