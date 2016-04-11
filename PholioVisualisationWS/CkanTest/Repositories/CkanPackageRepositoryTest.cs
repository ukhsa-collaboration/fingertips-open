using System;
using System.Collections.Generic;
using System.Linq;
using Ckan.Client;
using Ckan.DataTransformation;
using Ckan.Exceptions;
using Ckan.Model;
using Ckan.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace PholioVisualisation.CkanTest.Repositories
{
    [TestClass]
    public class WhenUsingCkanPackageProvider
    {
        private const string PackageId = "package-id";
        private CkanPackage package;

        [TestInitialize]
        public void RunOnceBeforeEachTest()
        {
            package = new CkanPackage
            {
                Id = PackageId
            };
        }

        [TestMethod]
        public void Then_Package_Returned_From_Ckan_Is_Provided()
        {
            // Arrange: api returns package
            var ckanApi = new Mock<ICkanApi>(MockBehavior.Strict);
            ckanApi.Setup(x => x.GetPackage(PackageId)).Returns(package);

            // Arrange: ID provider called once
            var idProvider = new Mock<IPackageIdProvider>(MockBehavior.Strict);
            idProvider.Setup(x => x.NextID).Returns(PackageId);

            // Act: get package
            var packageFromProvider = new CkanPackageRepository(ckanApi.Object).
                RetrieveOrGetNew(idProvider.Object);

            // Assert
            Assert.AreEqual(PackageId,packageFromProvider.Id);
            ckanApi.VerifyAll();
            idProvider.VerifyAll();
        }

        [TestMethod]
        public void Then_New_Package_Created_If_Non_Returned()
        {
            // Arrange: api returns null
            var ckanApi = new Mock<ICkanApi>(MockBehavior.Strict);
            ckanApi.Setup(x => x.GetPackage(PackageId)).Returns((CkanPackage)null);

            // Arrange: ID provider called once
            var idProvider = new Mock<IPackageIdProvider>(MockBehavior.Strict);
            idProvider.Setup(x => x.NextID).Returns(PackageId);

            // Act: get package
            var packageFromProvider = new CkanPackageRepository(ckanApi.Object).
                RetrieveOrGetNew(idProvider.Object);

            // Assert
            Assert.IsNull(packageFromProvider.Id, "Id should not be set");
            Assert.AreEqual(PackageId, packageFromProvider.Name, "Name should be set");
            ckanApi.VerifyAll();
            idProvider.VerifyAll();
        }

        [TestMethod]
        public void Then_Another_ID_Is_Tried_If_Previous_Ones_Are_Not_Allowd()
        {
            var id1 = PackageId + "1";
            var id2 = PackageId + "2";
            var id3 = PackageId + "3";

            // Arrange: api returns package on third attempt
            package.Id = id3;
            var ckanApi = new Mock<ICkanApi>(MockBehavior.Strict);
            ckanApi.Setup(x => x.GetPackage(id1)).Throws<CkanNotAuthorizedException>();
            ckanApi.Setup(x => x.GetPackage(id2)).Throws<CkanNotAuthorizedException>();
            ckanApi.Setup(x => x.GetPackage(id3)).Returns(package);

            // Arrange: ID provider
            var idProvider = new Mock<IPackageIdProvider>(MockBehavior.Strict);
            idProvider.SetupSequence(x => x.NextID)
                .Returns(id1)
                .Returns(id2)
                .Returns(id3);

            // Act: get package
            var packageFromProvider = new CkanPackageRepository(ckanApi.Object).
                RetrieveOrGetNew(idProvider.Object);

            // Assert
            Assert.AreEqual(id3, packageFromProvider.Id);
            ckanApi.VerifyAll();
            idProvider.VerifyAll();
        }
    }
}

