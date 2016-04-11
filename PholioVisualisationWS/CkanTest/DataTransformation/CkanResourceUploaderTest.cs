using System;
using System.Collections.Generic;
using System.Linq;
using Ckan;
using Ckan.Client;
using Ckan.DataTransformation;
using Ckan.Exceptions;
using Ckan.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace PholioVisualisation.CkanTest.DataTransformation
{
    [TestClass]
    public class WhenUsingCkanResourceUploader
    {
        private string packageId = "package";
        private CkanResource resource1;
        private CkanResource resource2;
        private CkanPackage package;

        [TestInitialize]
        public void RunBeforeEachTest()
        {
            resource1 = new CkanResource { Name = "1" };
            resource2 = new CkanResource { Name = "2" };
            package = new CkanPackage
            {
                Id = packageId,
                Resources = new List<CkanResource>()
            };
        }

        [TestMethod]
        public void ThenMultipleResourcesCanBeUploaded()
        {
            // Arrange: two successful uploads
            var ckanApi = new Mock<ICkanApi>(MockBehavior.Strict);
            ckanApi.Setup(x => x.CreateResource(resource1)).Returns(resource1);
            ckanApi.Setup(x => x.CreateResource(resource2)).Returns(resource2);

            // Act: upload resources
            var resources = UploadResources(ckanApi);

            // Assert: expected number of resources
            Assert.AreEqual(2, resources.Count);
            ckanApi.VerifyAll();
        }

        [TestMethod]
        public void ThenIfOneResourceFailsToUploadThenAllAreReloaded()
        {
            // Arrange: unsuccessful resource upload followed by two successful uploads
            var ckanApi = new Mock<ICkanApi>(MockBehavior.Strict);
            ckanApi.Setup(x => x.CreateResource(resource1)).Returns(resource1);
            ckanApi.SetupSequence(x => x.CreateResource(resource2))
                .Throws<CkanTimeoutException>()
                .Throws<CkanInternalServerException>()
                .Returns(resource2);

            // Arrange: package update after failed resource upload
            package.Resources.Add(resource1);
            ckanApi.Setup(x => x.GetPackage(packageId)).Returns(package);
            ckanApi.Setup(x => x.UpdatePackage(package)).Returns(package);

            // Act: upload resources
            var resources = UploadResources(ckanApi);

            // Assert: expected number of resources
            Assert.AreEqual(2, resources.Count);

            // Assert: package resources have been cleared
            Assert.AreEqual(0, package.Resources.Count);
            ckanApi.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(CkanApiException))]
        public void ThenIfExceptionOnly10Retries()
        {
            // Arrange: unsuccessful resource upload followed by two successful uploads
            var ckanApi = new Mock<ICkanApi>(MockBehavior.Strict);
            ckanApi.SetupSequence(x => x.CreateResource(resource1))
                .Throws<CkanTimeoutException>()
                .Throws<CkanTimeoutException>()
                .Throws<CkanTimeoutException>()
                .Throws<CkanTimeoutException>()
                .Throws<CkanTimeoutException>()
                .Throws<CkanTimeoutException>()
                .Throws<CkanTimeoutException>()
                .Throws<CkanTimeoutException>()
                .Throws<CkanTimeoutException>()
                .Throws<CkanTimeoutException>()
                .Throws<CkanTimeoutException>()
                .Throws<CkanTimeoutException>();

            ckanApi.Setup(x => x.GetPackage(packageId)).Returns(package);
            ckanApi.Setup(x => x.UpdatePackage(package)).Returns(package);

            // Act: upload resources
            UploadResources(ckanApi);
        }

        private IList<CkanResource> UploadResources(Mock<ICkanApi> ckanApi)
        {
            var resources = new CkanResourceUploader
            {
                CkanApi = ckanApi.Object,
                WaitIfResourceUploadFails = false
            }.AddResourcesToPackage(packageId, resource1, resource2);
            return resources;
        }
    }
}
