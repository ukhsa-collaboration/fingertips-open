using System;
using System.Collections.Generic;
using System.Linq;
using Ckan.DataTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace CkanTest.DataTransformation
{
    [TestClass]
    public class ProfileParametersTest
    {
        public const int ProfileId = ProfileIds.Phof;
        public const string GroupName = "a";

        [TestMethod]
        public void TestNew()
        {
            var areaTypeListProvider = new Mock<IAreaTypeListProvider>();
            areaTypeListProvider.Setup(x => x.GetAllAreaTypeIdsUsedInProfile(ProfileId))
                .Returns(new List<int>{
                    AreaTypeIds.CountyAndUnitaryAuthority,
                    AreaTypeIds.DistrictAndUnitaryAuthority});
            areaTypeListProvider.Setup(x => x.GetCategoryTypeIdsUsedInProfile(ProfileId))
                .Returns(new List<int>{CategoryTypeIds.EthnicGroups5});

            var parameters = new ProfileParameters(areaTypeListProvider.Object, ProfileId,
                GroupName);

            // Assert: properties
            Assert.AreEqual(ProfileId, parameters.ProfileId);
            Assert.AreEqual(GroupName, parameters.CkanGroupName);
            Assert.AreEqual(1, parameters.CategoryTypeIds.Count);

            // Assert: area type ids are split and distinct
            var areaTypeIds = parameters.AreaTypeIds;
            Assert.AreEqual(3, areaTypeIds.Count);
            Assert.IsTrue(areaTypeIds.Contains(AreaTypeIds.LocalAuthority));
            Assert.IsTrue(areaTypeIds.Contains(AreaTypeIds.UnitaryAuthority));
            Assert.IsTrue(areaTypeIds.Contains(AreaTypeIds.County));
        }
    }
}
