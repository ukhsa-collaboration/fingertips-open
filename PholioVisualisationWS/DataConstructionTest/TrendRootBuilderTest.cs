
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class TrendRootBuilderTest
    {

        [TestMethod]
        public void TestBuild()
        {
            var areaTypeId = AreaTypeIds.CountyAndUnitaryAuthority;
            var profileId = ProfileIds.Phof;
            var profile = ReaderFactory.GetProfileReader().GetProfile(profileId);

            var parentArea = new ParentArea(AreaCodes.Gor_NorthEast, areaTypeId);
            ComparatorMap comparatorMap = new ComparatorMapBuilder(parentArea).ComparatorMap;

            GroupData data = new GroupDataBuilderByGroupings
            {
                GroupId = profile.GroupIds[0],
                ChildAreaTypeId = areaTypeId,
                ProfileId = profile.Id,
                ComparatorMap = comparatorMap,
                AssignData = true
            }.Build();

            IList<TrendRoot> trendRoots = new TrendRootBuilder().Build(data.GroupRoots, 
                comparatorMap, areaTypeId, profileId, data.IndicatorMetadata, false);
            Assert.IsTrue(trendRoots.Count > 0);
            Assert.IsTrue(trendRoots[0].Periods.Count > 0);
            Assert.IsNotNull(trendRoots[0].Limits);
            Assert.IsTrue(trendRoots[0].ComparatorValueFs.Count > 0);

            string s = trendRoots[0].ComparatorValueFs[0][ComparatorIds.Subnational];
            Assert.AreNotEqual("-", s);
        }

        [TestMethod]
        [Ignore]
        public void Build_Returns_Results_With_Trends()
        {
            var comparatorMap = GetComparatorMap(ComparatorMapBuilderTest.GetRegion102());

            var data = new GroupDataBuilderByGroupings
            {
                GroupId = GroupIds.Phof_WiderDeterminantsOfHealth,
                ProfileId = ProfileIds.Phof,
                ComparatorMap = comparatorMap,
                ChildAreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority
            }.Build();

            var trendRoots = new TrendRootBuilder().Build(data.GroupRoots, comparatorMap,
                AreaTypeIds.CountyAndUnitaryAuthority, ProfileIds.Phof,
               data.IndicatorMetadata, false);

            var trendRoot = trendRoots.FirstOrDefault();
            Assert.IsTrue(trendRoots.Any() 
                && trendRoot != null
                && trendRoot.RecentTrends != null
                && trendRoot.RecentTrends.Count > 0
                );
        }

        private static ComparatorMap GetComparatorMap(ParentArea parentArea)
        {
            return new ComparatorMapBuilder(parentArea).ComparatorMap;
        }
    }
}
