
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class GroupDataBuilderByIndicatorIdsTest
    {
        [TestMethod]
        public void TestOneIndicatorCountyAndUnitaryAuthority()
        {
            GroupData data = new GroupDataBuilderByIndicatorIds
            {
                IndicatorIds = new List<int> { IndicatorIds.DeathsFromLungCancer },
                ProfileId = ProfileIds.Search,
                RestrictSearchProfileIds = new List<int> { ProfileIds.Tobacco },
                ComparatorMap = new ComparatorMapBuilder(GetGorEastOfEngland()).ComparatorMap,
                ParentAreaCode = AreaCodes.Gor_EastOfEngland,
                AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority
            }.Build();
            Assert.IsTrue(data.IsDataOk);
            Assert.IsTrue(data.GroupRoots.Count > 0);
            Assert.AreEqual(1, data.IndicatorMetadata.Count);
        }

        [TestMethod]
        public void TestTwoIndicators()
        {
            GroupData data = new GroupDataBuilderByIndicatorIds
            {
                IndicatorIds = new List<int> { 
                    IndicatorIds.DeathsFromLungCancer, 
                    IndicatorIds.OralCancerRegistrations 
                },
                ProfileId = ProfileIds.Search,
                RestrictSearchProfileIds = new List<int> { ProfileIds.Tobacco },
                ComparatorMap = new ComparatorMapBuilder(GetGorEastOfEngland()).ComparatorMap,
                ParentAreaCode = AreaCodes.Gor_EastOfEngland,
                AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority
            }.Build();
            Assert.IsTrue(data.IsDataOk);
            Assert.IsTrue(data.GroupRoots.Count > 0);
            Assert.AreEqual(2, data.IndicatorMetadata.Count);
        }

        [TestMethod]
        public void TestAreaTypeIds()
        {
            GroupData data = new GroupDataBuilderByIndicatorIds
            {
                IndicatorIds = new List<int> { IndicatorIds.DeathsFromLungCancer },
                ProfileId = ProfileIds.Search,
                RestrictSearchProfileIds = new List<int> { ProfileIds.Tobacco },
                ComparatorMap = new ComparatorMapBuilder(GetGorEastOfEngland()).ComparatorMap,
                ParentAreaCode = AreaCodes.Gor_EastOfEngland,
                AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority
            }.Build();
            Assert.IsTrue(data.IsDataOk);
            Assert.IsTrue(data.GroupRoots.Count > 0);
            Assert.AreEqual(1, data.IndicatorMetadata.Count);
        }

        [TestMethod]
        public void TestAreaTypeIdsWhereNoGroupingsForRequestedAreaTypeIds()
        {
            GroupData data = new GroupDataBuilderByIndicatorIds
            {
                IndicatorIds = new List<int> { IndicatorIds.DeathsFromLungCancer },
                ProfileId = ProfileIds.Search,
                RestrictSearchProfileIds = new List<int> { ProfileIds.Tobacco },
                ComparatorMap = new ComparatorMapBuilder(ComparatorMapBuilderTest.GetRegion102()).ComparatorMap,
                ParentAreaCode = AreaCodes.Sha_EastOfEngland,
                AreaTypeId = AreaTypeIds.County
            }.Build();

            Assert.IsFalse(data.IsDataOk);
            Assert.IsTrue(data.GroupRoots.Count == 0);
            Assert.AreEqual(0, data.IndicatorMetadata.Count);
            Assert.AreEqual(0, data.Areas.Count);
        }

        public static ParentArea GetGorEastOfEngland()
        {
            return new ParentArea(AreaCodes.Gor_EastOfEngland, AreaTypeIds.GoRegion);
        }
    }
}
