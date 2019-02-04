using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Fpm.MainUI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fpm.MainUITest.Controllers
{
    [TestClass]
    public class UploadControllerTest
    {
        private UploadController controller;

        private Mock<HttpRequestBase> _mockRequest;
        private Mock<HttpPostedFileBase> _mockFile;
        private Mock<HttpFileCollectionBase> _mockFileCollection;
        private Mock<HttpContextBase> _mockContext;
        private Mock<ControllerContext> _mockControllerContext;

        [TestInitialize]
        public void StartUp()
        {
            _mockRequest = new Mock<HttpRequestBase>();
            _mockFile = new Mock<HttpPostedFileBase>();
            _mockFileCollection = new Mock<HttpFileCollectionBase>();
            _mockRequest.SetupGet(x => x.Headers).Returns( new System.Net.WebHeaderCollection { {"X-Requested-With", "XMLHttpRequest"}});

            _mockContext = new Mock<HttpContextBase>();
            _mockContext.SetupGet(x => x.Request).Returns(_mockRequest.Object);

            _mockControllerContext = new Mock<ControllerContext>();
            _mockControllerContext.Setup(frm => frm.HttpContext.Request.Form.Set("Options", "1"));

            controller = new UploadController();

            controller.ControllerContext = new ControllerContext(_mockContext.Object, new RouteData(), controller );
        }

        [TestMethod]
        public void UploadBatchUpload_Ok_Response()
        {
            // Arrange
            var requestParams = new NameValueCollection
            {
                { "selectedOption", "true"}
            };

            _mockFile.SetupGet(x => x.FileName).Returns("TestName.test");
            _mockFileCollection.SetupGet(x => x.Count).Returns(1);
            _mockFileCollection.SetupGet(x => x[0]).Returns(_mockFile.Object);
            _mockRequest.SetupGet(x => x.Files).Returns(_mockFileCollection.Object);
            _mockRequest.Setup(x => x.Form).Returns(requestParams);

            // Act
            var response = controller.UploadBatchUpload((HttpPostedFileBase)null);

            var result = response as ContentResult;

            // Assert
            _mockFileCollection.Verify(x => x[0], Times.Once);
            _mockRequest.Verify(x => x.Form, Times.Once);

            Assert.AreEqual("ok", result.Content); 
        }

        [TestMethod]
        public void UploadBatchUpload_Fail_Response_No_Files()
        {
            // Arrange
            var requestParams = new NameValueCollection
            {
                { "selectedOption", "true"}
            };

            _mockFile.SetupGet(x => x.FileName).Returns("TestName.test");
            _mockFileCollection.SetupGet(x => x.Count).Returns(1);
            _mockFileCollection.SetupGet(x => x[0]).Returns(_mockFile.Object);
            _mockRequest.SetupGet(x => x.Files).Returns((HttpFileCollectionBase) null);
            _mockRequest.Setup(x => x.Form).Returns(requestParams);

            // Act
            var response = controller.UploadBatchUpload((HttpPostedFileBase)null);

            var result = response as ContentResult;

            // Assert

            _mockFileCollection.Verify(x => x[0], Times.Never);

            Assert.AreEqual("fail", result.Content);
        }

        [TestMethod]
        public void UploadBatchUpload_Fail_Response_No_Form_Value()
        {
            // Arrange
            var requestParams = new NameValueCollection();

            _mockFile.SetupGet(x => x.FileName).Returns("TestName.test");
            _mockFileCollection.SetupGet(x => x.Count).Returns(1);
            _mockFileCollection.SetupGet(x => x[0]).Returns(_mockFile.Object);
            _mockRequest.SetupGet(x => x.Files).Returns(_mockFileCollection.Object);
            _mockRequest.Setup(x => x.Form).Returns(requestParams);

            // Act
            var response = controller.UploadBatchUpload((HttpPostedFileBase)null);

            var result = response as ContentResult;

            // Assert
            _mockRequest.Verify(x => x.Form, Times.Once);

            Assert.AreEqual("fail", result.Content);
        }
    }
}
