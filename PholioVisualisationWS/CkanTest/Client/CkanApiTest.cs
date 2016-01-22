using Ckan.Client;
using Ckan.Exceptions;
using Ckan.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CkanTest.Client
{
    [TestClass]
    public class CkanApiTest
    {
        const string groupId = "public-health-outcomes-framework2";
        
        [TestMethod]
        public void GetGroupIds_Returns_Id_List()
        {
            // Arrange: mock HTTP client
            var mock = new Moq.Mock<ICkanHttpClient>();
            var json = CkanTestHelper.GetExampleResponseFromFile("group_list.json");
            var requestParameters = new Dictionary<string, string>();
            mock.Setup(x => x.GetAction(ActionNames.GroupList, requestParameters))
                .Returns(json);

            // Act: get group ids
            var ids = new CkanApi(mock.Object).GetGroupIds();

            // Assert
            Assert.IsTrue(ids.Contains("phof"));
        }

        [TestMethod]
        public void GetOrganisation_Returns_Organisation()
        {
            // Arrange: mock HTTP client
            var organisationName = OrganisationNames.PublicHealthEngland;
            var mock = new Moq.Mock<ICkanHttpClient>();
            var json = CkanTestHelper.GetExampleResponseFromFile("organization_show.json");
            var requestParameters = new Dictionary<string, string> { { "id", organisationName } };
            mock.Setup(x => x.GetAction(ActionNames.OrganizationShow, requestParameters))
                .Returns(json);

            // Act: get organisation
            var organisation = new CkanApi(mock.Object).GetOrganization(organisationName);

            // Assert
            Assert.AreEqual(organisationName, organisation.Name);
        }

        [TestMethod]
        public void GetPackage_Returns_Package()
        {
            const string packageId = "phe-indicator-19-90366";

            // Arrange: mock HTTP client
            var mock = new Moq.Mock<ICkanHttpClient>();
            var json = CkanTestHelper.GetExampleResponseFromFile("package_show.json");
            var requestParameters = new Dictionary<string, string> { { "id", packageId } };
            mock.Setup(x => x.GetAction(ActionNames.PackageShow, requestParameters))
                .Returns(json);

            // Act: get package
            var package = new CkanApi(mock.Object).GetPackage(packageId);

            // Assert
            Assert.AreEqual(packageId, package.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(CkanNotAuthorizedException))]
        public void GetPackage_Throws_Exception_If_Access_Not_Authorized()
        {
            const string packageId = "phe-indicator-19-90366";

            // Arrange: mock HTTP client
            var mock = new Moq.Mock<ICkanHttpClient>();
            var json = CkanTestHelper.GetExampleResponseFromFile("package_show_not_authorized.json");
            var requestParameters = new Dictionary<string, string> { { "id", packageId } };
            mock.Setup(x => x.GetAction(ActionNames.PackageShow, requestParameters))
                .Returns(json);

            // Act: get package
            new CkanApi(mock.Object).GetPackage(packageId);
        }

        [TestMethod]
        public void GetPackage_Returns_Null_If_Not_Found()
        {
            const string packageId = "not-a-valid-id";

            // Arrange: mock HTTP client
            var mock = new Moq.Mock<ICkanHttpClient>();
            var json = CkanTestHelper.GetExampleResponseFromFile("package_show_not_found.json");
            var requestParameters = new Dictionary<string, string> { { "id", packageId } };
            mock.Setup(x => x.GetAction(ActionNames.PackageShow, requestParameters))
                .Returns(json);

            // Act: get package
            var package = new CkanApi(mock.Object).GetPackage(packageId);

            Assert.IsNull(package);
        }

        [TestMethod]
        public void GetGroup_Returns_Group()
        {
            // Arrange: mock HTTP client
            var mock = new Moq.Mock<ICkanHttpClient>();
            var json = CkanTestHelper.GetExampleResponseFromFile("group_show.json");
            var requestParameters = new Dictionary<string, string> { { "id", groupId } };
            mock.Setup(x => x.GetAction(ActionNames.GroupShow, requestParameters))
                .Returns(json);

            // Act: get package
            var ckanGroup = new CkanApi(mock.Object).GetGroup(groupId);

            // Assert
            Assert.AreEqual(groupId, ckanGroup.Name);
            Assert.AreEqual(152, ckanGroup.PackageCount);
            Assert.AreEqual(ckanGroup.PackageCount, ckanGroup.Packages.Count);
        }

        [TestMethod]
        public void GetGroup_Returns_Null_If_Not_Found()
        {
            // Arrange: mock HTTP client
            var mock = new Moq.Mock<ICkanHttpClient>();
            var json = CkanTestHelper.GetExampleResponseFromFile("group_show_not_found.json");
            var requestParameters = new Dictionary<string, string> { { "id", groupId } };
            mock.Setup(x => x.GetAction(ActionNames.GroupShow, requestParameters))
                .Returns(json);

            // Act: get package
            var ckanGroup = new CkanApi(mock.Object).GetGroup(groupId);

            // Assert
            Assert.IsNull(ckanGroup);
        }

        [TestMethod]
        public void GetUpdatePackage_Returns_Package()
        {
            // Arrange: mock HTTP client
            var mock = new Moq.Mock<ICkanHttpClient>();
            var json = CkanTestHelper.GetExampleResponseFromFile("update_package.json");
            var requestPackage = new CkanPackage();
            mock.Setup(x => x.PostAction(ActionNames.PackageUpdate, requestPackage))
                .Returns(json);

            // Act: get package
            var package = new CkanApi(mock.Object).UpdatePackage(requestPackage);

            // Assert
            Assert.AreEqual("phe-indicator-19-22402", package.Name);
        }

        [TestMethod]
        public void GetCreateResource_Returns_Resource()
        {
            // Arrange: mock HTTP client
            var mock = new Moq.Mock<ICkanHttpClient>();
            var json = CkanTestHelper.GetExampleResponseFromFile("resource_create.json");
            var requestResource = new CkanResource();
            mock.Setup(x => x.UploadResource(ActionNames.ResourceCreate, requestResource))
                .Returns(json);

            // Act: get package
            var resource = new CkanApi(mock.Object).CreateResource(requestResource);

            // Assert
            Assert.AreEqual("Metadata", resource.Name);
            Assert.AreEqual(2015, resource.Created.Year);
            Assert.AreEqual("6ff56435-b348-46f3-83fb-c1378cc0c39c", resource.ResourceGroupId);
        }

        [TestMethod]
        [ExpectedException(typeof(CkanTimeoutException))]
        public void GetCreateResource_Throws_Exception_If_Timeout()
        {
            // Arrange: mock HTTP client
            var mock = new Moq.Mock<ICkanHttpClient>();
            var json = CkanTestHelper.GetExampleResponseFromFile("resource_create_timeout.txt");
            var requestResource = new CkanResource();
            mock.Setup(x => x.UploadResource(ActionNames.ResourceCreate, requestResource))
                .Returns(json);

            // Act: get package
            new CkanApi(mock.Object).CreateResource(requestResource);
        }
    }
}
