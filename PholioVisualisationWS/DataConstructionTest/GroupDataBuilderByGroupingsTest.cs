
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class GroupDataBuilderByGroupingsTest
    {
        [TestMethod]
        public void TestDataIsAssigned()
        {
            GroupData data = new GroupDataBuilderByGroupings
            {
                GroupId = GroupIds.Phof_HealthProtection,
                ProfileId = ProfileIds.Phof,
                ComparatorMap = GetComparatorMap(ComparatorMapBuilderTest.GetRegion102()),
                ChildAreaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019,
                AssignData = false
            }.Build();

            Assert.IsTrue(data.IsDataOk);
            foreach (GroupRoot groupRoot in data.GroupRoots)
            {
                Assert.AreEqual(0, groupRoot.Data.Count);
                foreach (Grouping grouping in groupRoot.Grouping)
                {
                    Assert.IsNull(grouping.ComparatorData);
                }
            }
        }

        [TestMethod]
        public void TestPhofHealthProtection()
        {
            GroupData data = new GroupDataBuilderByGroupings
            {
                GroupId = GroupIds.Phof_HealthProtection,
                ProfileId = ProfileIds.Phof,
                ComparatorMap = GetComparatorMap(ComparatorMapBuilderTest.GetRegion102()),
                ChildAreaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019
            }.Build();

            Assert.IsTrue(data.GroupRoots.Count > 10);
            Assert.IsTrue(data.Areas.Count > 10);
            var comparatorData = data
                .GroupRoots.First()
                .Grouping.First(x => x.ComparatorId == ComparatorIds.Subnational)
                .ComparatorData;
            Assert.AreEqual(AreaCodes.Gor_EastOfEngland, comparatorData.AreaCode);
        }

        [TestMethod]
        public void TestPhofWiderDeterminantsOfHealth()
        {
            var data = new GroupDataBuilderByGroupings
            {
                GroupId = GroupIds.Phof_WiderDeterminantsOfHealth,
                ProfileId = ProfileIds.Phof,
                ComparatorMap = GetComparatorMap(ComparatorMapBuilderTest.GetRegion102()),
                ChildAreaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019
            }.Build();

            Assert.IsTrue(data.GroupRoots.Count > 15);
            Assert.IsTrue(data.Areas.Count > 10);
            Assert.IsTrue(
                data.GroupRoots.First().Grouping.Any(x => x.ComparatorData.AreaCode == AreaCodes.Gor_EastOfEngland));
        }

        [TestMethod]
        [Ignore]
        public void Build_Returns_Results_With_Trends()
        {
            GroupData data = new GroupDataBuilderByGroupings
            {
                GroupId = GroupIds.Phof_WiderDeterminantsOfHealth,
                ProfileId = ProfileIds.Phof,
                ComparatorMap = GetComparatorMap(ComparatorMapBuilderTest.GetRegion102()),
                ChildAreaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019
            }.Build();

            Assert.IsTrue(data.GroupRoots.Count > 0
                && data.GroupRoots.FirstOrDefault().RecentTrends.Count > 0
                );
        }


        private static ComparatorMap GetComparatorMap(ParentArea parentArea)
        {
            return new ComparatorMapBuilder(parentArea).ComparatorMap;
        }
    }
}
