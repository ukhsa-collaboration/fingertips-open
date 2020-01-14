using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUI.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

namespace IndicatorsUI.MainUITest.Helpers
{
    [TestClass]
    public class MeasurementProtocolDownloadFilesTest
    {
        private Mock<HttpMessageHandler> _handlerMock;

        public MeasurementProtocolDownloadFilesTest()
        {
            _handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        }

        [TestMethod]
        public void Test_Should_Not_Throw_Exception()
        {
            AppConfig.AppSettings["Environment"] = "Development";

            _handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                { StatusCode = HttpStatusCode.NotFound, Content = new StringContent("Test exception successful") })
                .Verifiable();

            var httpClient = new HttpClient(_handlerMock.Object) { BaseAddress = new Uri("http://test.phe.gov.uk/") };

            var measurementProtocol = new MeasurementProtocolDownloadFiles
            {
                Client = httpClient
            };

            measurementProtocol.LogFileDownloadWithGoogleAnalytics("TestUserAgent", "TestFileName");
        }

        [TestMethod]
        public void Test_Should_Use_TrackingDevelopmentId()
        {
            AppConfig.AppSettings["Environment"] = "Development";

            var measurementProtocol = new MeasurementProtocolDownloadFiles();

            var privateObject = new PrivateObject(measurementProtocol);

            var trackingId = privateObject.GetField("tid");

            Assert.AreNotEqual(GoogleAnalytics.TrackingLiveId, trackingId);
        }

        [TestMethod]
        public void Test_Should_Use_TrackingLiveId()
        {
            AppConfig.AppSettings["Environment"] = "Live";

            var measurementProtocol = new MeasurementProtocolDownloadFiles();

            var privateObject = new PrivateObject(measurementProtocol);

            var trackingId = privateObject.GetField("tid");

            Assert.AreEqual(GoogleAnalytics.TrackingLiveId, trackingId);
        }

        [TestMethod]
        public void Test_Should_Have_Correct_Required_Values()
        {
            AppConfig.AppSettings["Environment"] = "Development";

            var measurementProtocol = new MeasurementProtocolDownloadFiles();

            var privateObject = new PrivateObject(measurementProtocol);

            var version = privateObject.GetField("version");
            var clientId = privateObject.GetField("cid");
            var hitType = privateObject.GetField("t");

            Assert.AreEqual(1, version);
            Assert.AreEqual(555, clientId);
            Assert.AreEqual("event", hitType);
        }

        [TestMethod]
        public void Test_Should_Save_Correctly_Label_And_UserAgent_Values()
        {
            AppConfig.AppSettings["Environment"] = "Development";
            var testLabelName = "TestFileName";
            var testUserAgentName = "TestUserAgent";

            var measurementProtocol = new MeasurementProtocolDownloadFiles();

            measurementProtocol.LogFileDownloadWithGoogleAnalytics(testUserAgentName, testLabelName);

            var privateObject = new PrivateObject(measurementProtocol);

            var eventLabel = privateObject.GetField("_eventLabel");
            var userAgent = privateObject.GetField("_userAgent");

            Assert.AreEqual(testLabelName, eventLabel);
            Assert.AreEqual(testUserAgentName, userAgent);
        }
    }
}
