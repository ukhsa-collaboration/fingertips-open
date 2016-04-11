using Ckan.Client;
using Ckan.Exceptions;
using Ckan.Model;
using Ckan.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;

namespace PholioVisualisation.CkanTest.Repositories
{
    [TestClass]
    public class CkanGroupRepositoryTest
    {
        private const string GroupName = "phe-phof";
        public const string Description = "d";

        private CkanGroup ckanGroup = new CkanGroup
        {
            Name = GroupName,
            Title = "title",
            Description = Description
        };

        private const int profileId = ProfileIds.Phof;

        private Profile profile = new Profile(new List<int>
        {
            GroupIds.Phof_HealthProtection
        })
        {
            Id = profileId,
            UrlKey = "phof"
        };

        private Mock<ICkanApi> ckanApi;
        private Mock<IContentProvider> contentProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            ckanApi = new Mock<ICkanApi>(MockBehavior.Strict);
            SetUpCkanApiForWhenGroupExists();

            contentProvider = new Mock<IContentProvider>(MockBehavior.Strict);
        }

        [TestMethod]
        public void WhenGetExistingGroup()
        {
            // Arrange
            SetUpCkanApiForWhenGroupExists();

            // Act
            var group = new CkanGroupRepository(ckanApi.Object, contentProvider.Object)
            {
                WaitIfResourceUploadFails = false
            }.GetExistingGroup(GroupName);

            // Assert
            Assert.IsNotNull(@group);
            VerifyAllMocks();
        }

        [TestMethod]
        public void WhenCkanGroupDoesNotExistThenItIsCreated()
        {
            // Arrange
            ckanApi = new Mock<ICkanApi>(MockBehavior.Strict);
            ckanApi.Setup(x => x.GetGroup(GroupName)).Returns((CkanGroup)null);
            ckanApi.Setup(x => x.CreateGroup(It.IsAny<CkanGroup>())).Returns(ckanGroup);
            
            contentProvider.Setup(x => x.GetContentWithHtmlRemoved(profileId, ContentKeys.CkanDescription))
                .Returns(Description);

            ActAndAssert();
        }

        [TestMethod]
        public void WhenTimeoutThenRetryUntilSuccessForTimeoutError()
        {
            ckanApi = new Mock<ICkanApi>(MockBehavior.Strict);
            ckanApi.SetupSequence(x => x.GetGroup(GroupName))
                .Throws<CkanTimeoutException>()
                .Returns(ckanGroup);

            ActAndAssert();
        }

        [TestMethod]
        public void WhenTimeoutThenRetryUntilSuccessForServerError()
        {
            ckanApi = new Mock<ICkanApi>(MockBehavior.Strict);
            ckanApi.SetupSequence(x => x.GetGroup(GroupName))
                .Throws<CkanInternalServerException>()
                .Returns(ckanGroup);

            ActAndAssert();
        }

        [TestMethod]
        [ExpectedException(typeof(CkanApiException))]
        public void WhenExceptionThenRetryLimitIs10Times()
        {
            ckanApi = new Mock<ICkanApi>(MockBehavior.Strict);
            ckanApi.SetupSequence(x => x.GetGroup(GroupName))
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

            ActAndAssert();
        }

        private void SetUpCkanApiForWhenGroupExists()
        {
            ckanApi.Setup(x => x.GetGroup(GroupName)).Returns(ckanGroup);
        }

        private void ActAndAssert()
        {
            var group = new CkanGroupRepository(ckanApi.Object, contentProvider.Object)
            {
                WaitIfResourceUploadFails = false
            }.CreateOrRetrieveGroup(profile);

            // Assert
            Assert.IsNotNull(@group);
            VerifyAllMocks();
        }

        private void VerifyAllMocks()
        {
            ckanApi.VerifyAll();
            contentProvider.VerifyAll();
        }
    }
}
