using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class AreaTypeListProviderTest
    {
        [TestMethod]
        public void TestGetAreaTypesUsedInProfiles()
        {
            var profileIds = new List<int> { 1, 2 };

            var groupIdProvider = new Mock<GroupIdProvider>();
            groupIdProvider.Setup(x => x.GetGroupIds(1)).Returns(new List<int> { 3, 4 });
            groupIdProvider.Setup(x => x.GetGroupIds(2)).Returns(new List<int> { 5, 6 });

            var areasReader = new Mock<AreasReader>();
            areasReader.Setup(x => x.GetAreaTypes(It.IsAny<IList<int>>())).Returns(new List<AreaType> { new AreaType() });

            var groupDataReader = new Mock<GroupDataReader>();
            var distinctGroupIds = new List<int> { 10, 11 };
            groupDataReader.Setup(x => x.GetDistinctGroupingAreaTypeIds(It.IsAny<IList<int>>())).Returns(distinctGroupIds);

            var areaTypes = new AreaTypeListProvider(groupIdProvider.Object, areasReader.Object, groupDataReader.Object)
                .GetChildAreaTypesUsedInProfiles(profileIds);

            Assert.IsNotNull(areaTypes[0]);
        }

        [TestMethod]
        public void TestGetAllAreaTypeIdsUsedInProfile()
        {
            var areaTypeIds = AreaTypeListProvider()
                .GetAllAreaTypeIdsUsedInProfile(ProfileIds.Phof);

            Assert.IsTrue(areaTypeIds.Contains(AreaTypeIds.County), 
                "Should return individual child area types");
            Assert.IsFalse(areaTypeIds.Contains(AreaTypeIds.CountyAndUnitaryAuthority), 
                "Should not return composite child area types");
            Assert.IsTrue(areaTypeIds.Contains(AreaTypeIds.GoRegion),
                "Should return parent area types");
            Assert.IsTrue(areaTypeIds.Contains(AreaTypeIds.Country), 
                "Should return country area types");
        }

        [TestMethod]
        public void TestGetParentAreaTypeIdsUsedInProfile()
        {
            var areaTypeIds = AreaTypeListProvider()
                .GetParentAreaTypeIdsUsedInProfile(ProfileIds.Phof);

            Assert.IsTrue(areaTypeIds.Contains(AreaTypeIds.PheCentresFrom2015));
        }

        [TestMethod]
        public void TestGetParentAreaTypeIdsUsedInProfile_When_Generic_Parent_Options_Are_Used()
        {
            var areaTypeIds = AreaTypeListProvider()
                .GetParentAreaTypeIdsUsedInProfile(ProfileIds.CancerServices, AreaTypeIds.GpPractice);

            Assert.IsTrue(areaTypeIds.Contains(AreaTypeIds.CcgsPostApr2018));
        }

        [TestMethod]
        public void TestGetCategoryTypeIdsUsedInProfile()
        {
            var categoryTypeIds = AreaTypeListProvider()
                .GetCategoryTypeIdsForExport();

            Assert.IsTrue(categoryTypeIds.Contains(CategoryTypeIds.DeprivationDecileCountyAndUA2010));
            Assert.IsFalse(categoryTypeIds.Contains(CategoryTypeIds.LimitsForHealthProfilesLifeExpectancyChart));
        }

        [TestMethod]
        public void TestGetCategoryTypeIdsUsedInProfile_When_Child_Has_No_Parent_Options()
        {
            var categoryTypeIds = AreaTypeListProvider()
                .GetParentCategoryTypeIdsUsedInProfile(ProfileIds.Phof, AreaTypeIds.GoRegion);

            Assert.IsFalse(categoryTypeIds.Any());
        }

        private static AreaTypeListProvider AreaTypeListProvider()
        {
            return new AreaTypeListProvider(
                new GroupIdProvider(ReaderFactory.GetProfileReader()), 
                ReaderFactory.GetAreasReader(),
                ReaderFactory.GetGroupDataReader());
        }
    }
}
