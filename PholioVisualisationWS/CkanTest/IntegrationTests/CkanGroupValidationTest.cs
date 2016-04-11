using System;
using System.Linq;
using Ckan.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PholioVisualisation.CkanTest.IntegrationTests
{
    [TestClass]
    public class CkanGroupValidationTest
    {
        public const string GroupName = "phe-public-health-outcomes-framework";
        private static CkanGroup ckanGroup;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var ckanApi = CkanTestHelper.CkanApi();
            ckanGroup = ckanApi.GetGroup(GroupName);
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestAllPackagesHaveTwoResources()
        {
            foreach (var ckanPackage in ckanGroup.Packages)
            {
                Assert.AreEqual(2, ckanPackage.Resources.Count, "Package should have 2 resources: " +
                    ckanPackage.Name);
            }
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestNoResourceUrlsHaveBeenTruncated()
        {
            foreach (var ckanPackage in ckanGroup.Packages)
            {
                foreach (var resource in ckanPackage.Resources)
                {
                    Assert.IsTrue(resource.Url.EndsWith(".csv"),
                        "Name of file should not be truncated for package: " + ckanPackage.Name);
                }
            }
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestEveryPackageContainsBothTypesOfResources()
        {
            foreach (var ckanPackage in ckanGroup.Packages)
            {
                Assert.IsTrue(ckanPackage.Resources.Select(x => x.Name).Contains("Data"),
                    "Package should contain resource called data: " + ckanPackage.Name);
                Assert.IsTrue(ckanPackage.Resources.Select(x => x.Name).Contains("Metadata"),
                    "Package should contain resource called metadata: " + ckanPackage.Name);
            }
        }
    }
}
