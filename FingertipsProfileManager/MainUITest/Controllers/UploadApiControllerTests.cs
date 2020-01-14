using Fpm.MainUI.Controllers;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Fpm.MainUITest.Controllers
{
    [TestClass]
    public class UploadApiControllerTests
    {
        private UploadApiController _controller;

        private Mock<IUploadJobRepository> _mockUploadJobRepository;
        private Mock<IUserRepository> _mockUserRepository;

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

            _mockUploadJobRepository = new Mock<IUploadJobRepository>();
            _mockUserRepository = new Mock<IUserRepository>();

            _mockRequest.SetupGet(x => x.Headers).Returns( new System.Net.WebHeaderCollection { {"X-Requested-With", "XMLHttpRequest"}});

            _mockContext = new Mock<HttpContextBase>();
            _mockContext.SetupGet(x => x.Request).Returns(_mockRequest.Object);

            _mockControllerContext = new Mock<ControllerContext>();
            _mockControllerContext.Setup(frm => frm.HttpContext.Request.Form.Set("Options", "1"));

            _controller = new UploadApiController(_mockUploadJobRepository.Object, _mockUserRepository.Object);

            _controller.ControllerContext = new ControllerContext(_mockContext.Object, new RouteData(), _controller);
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
            var response = _controller.UploadBatchUpload();
            var result = response as HttpStatusCodeResult;

            // Assert
            _mockFileCollection.Verify(x => x[0], Times.Once);
            _mockRequest.Verify(x => x.Form, Times.Once);
            Assert.AreEqual(200, result.StatusCode);
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
            var response = _controller.UploadBatchUpload();
            var result = response as HttpStatusCodeResult;

            // Assert
            _mockFileCollection.Verify(x => x[0], Times.Never);
            Assert.AreEqual(500, result.StatusCode);
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
            var response = _controller.UploadBatchUpload();
            var result = response as HttpStatusCodeResult;

            // Assert
            _mockRequest.Verify(x => x.Form, Times.Once);
            Assert.AreEqual(500, result.StatusCode);
        }
    }
}
