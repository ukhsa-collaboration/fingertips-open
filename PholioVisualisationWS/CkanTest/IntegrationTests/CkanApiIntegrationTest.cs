using Ckan.Client;
using Ckan.DataTransformation;
using Ckan.Exceptions;
using Ckan.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.CkanTest.IntegrationTests
{
    [TestClass]
    public class CkanApiIntegrationTest
    {
        // Must be name of a group that exists on the CKAN server
        public const string GroupName = GroupNames.Phof;

        // Must be name of a package that exists on the CKAN server
        public const string PackageName = "phe-indicator-90842";

        public const string PackageTitle = "PHE Test Package";

        // Must be name of a organisation that exists on the CKAN server
        public const string OrganisationName = OrganisationNames.PublicHealthEngland;

        public const string Description = "A group used by PHE for integration testing";

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestGetGroupIds()
        {
            var packageIds = CkanApi().GetGroupIds();
            Assert.IsTrue(packageIds.Any());
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestGetGroup()
        {
            var ckanGroup = CkanApi().GetGroup(GroupName);

            // Check group
            Assert.IsNotNull(ckanGroup);
            Assert.AreEqual(ckanGroup.Name,GroupName);
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestGetOrganisation()
        {
            var organisation = CkanApi().GetOrganization(OrganisationName);

            // Check organisation
            Assert.IsNotNull(organisation);
            Assert.AreEqual(OrganisationName, organisation.Name);
        }

        // Expect to fail with exception on all but the first call as
        // the object will already exist.
        [TestMethod, TestCategory("ExcludeFromJenkins")]
        [ExpectedException(typeof(CkanNotAuthorizedException))]
        public void TestCreateGroup()
        {
            var group = new CkanGroup();
            group.Name = GroupName;
            group.Title = "Public Health Outcomes Framework";
            group.Description = Description;
            var ckanGroup = CkanApi().CreateGroup(group);

            // Check group
            Assert.IsNotNull(ckanGroup);
            Assert.AreEqual(Description, ckanGroup.Description);
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestUpdateGroup()
        {
            var newDescription = Description + " - Updated: " + DateTime.Now;

            var ckan = CkanApi();
            var group = ckan.GetGroup(GroupName);
            group.Description = newDescription;
            var updatedGroup = ckan.UpdateGroup(group);

            // Check group
            Assert.IsNotNull(updatedGroup);
            Assert.AreEqual(newDescription, updatedGroup.Description);
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestCreatePackage()
        {
            var ckan = CkanApi();

            // Create a random name
            var guid = Guid.NewGuid().ToString().Substring(0, 8);
            var packageName = "phe-test-package-" + guid;

            var group = ckan.GetGroup(GroupName);

            var unsavedPackage = new CkanPackage
            {
                Name = packageName,
                Title = PackageTitle + " " + guid,
                Author = "PHE author",
                AuthorEmail = "PHE author email",
                Maintainer = "PHE maintainer",
                MaintainerEmail = "PHE MaintainerEmail",
                Notes = "notes for the package",
                Version = "4.2",
                Groups = new List<CkanGroup> {group.GetMinimalGroupForSendingToCkan()},
                OwnerOrganization = OrganisationName
            };
            var savedPackage = ckan.CreatePackage(unsavedPackage);

            // Check package
            Assert.IsNotNull(savedPackage);
            Assert.AreEqual(savedPackage.Name, packageName);
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestGetPackage()
        {
            var ckan = CkanApi();
            var package = ckan.GetPackage(PackageName);

            // Check package
            Assert.IsNotNull(package);
            Assert.AreEqual(PackageName, package.Name);
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestGetPackageThatDoesNotExistReturnsNull()
        {
            Assert.IsNull(CkanApi().GetPackage("a-package-that-does-not-exist"));
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestUpdatePackage()
        {
            var ckan = CkanApi();

            var group = ckan.GetGroup(GroupName).GetMinimalGroupForSendingToCkan();

            // Set up package
            var package = ckan.GetPackage(PackageName);
            var parameters = new ProfileParameters();
            package.Title = PackageTitle;
            package.Source = parameters.ProfileUrl;
            package.Homepage = parameters.ProfileUrl;
            package.Origin = parameters.OrganisationTitle; // source
            package.OwnerOrganization = OrganisationNames.PublicHealthEngland;
            package.Groups.Add(group);
            package.Frequency.Add(CkanFrequency.Annually);
            package.CoverageStartDate = "1999-11-01";
            package.CoverageEndDate = "2014-12-27";
            package.Resources.Clear();

            // Update package
            var updatedPackage = ckan.UpdatePackage(package);

            // Check package
            Assert.IsNotNull(updatedPackage);
            Assert.AreEqual(PackageTitle, updatedPackage.Title);
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestCreateResource()
        {
            var ckan = CkanApi();
            var unsavedResource = new CkanResource();
            unsavedResource.PackageId = PackageName;
            unsavedResource.Description = Description;
            unsavedResource.Format = "CSV";
            unsavedResource.Name = "resource name 6";
            unsavedResource.File = new CkanResourceFile
            {
                FileName = "test.csv",
                FileContents = GetBytes("a,b")
            };
            var savedResource = ckan.CreateResource(unsavedResource);

            // Check package
            Assert.IsNotNull(savedResource);
            Assert.AreEqual(savedResource.Format, "CSV");
        }

        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static ICkanApi CkanApi()
        {
            return CkanTestHelper.CkanApi();
        }
    }
}
