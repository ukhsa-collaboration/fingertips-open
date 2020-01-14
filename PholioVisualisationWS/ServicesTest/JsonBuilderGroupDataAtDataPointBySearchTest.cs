using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;
using PholioVisualisation.Services;

namespace PholioVisualisation.ServicesTest
{
    [TestClass]
    public class JsonBuilderGroupDataAtDataPointBySearchTest
    {
        private const int areaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019;

        [TestMethod]
        public void Test_Get_Data_For_Child_Areas()
        {
            var parentAreaCode = AreaCodes.Gor_NorthEast;
            var parentArea = new ParentArea(parentAreaCode, areaTypeId);

            var builder = new GroupDataBuilderByIndicatorIds
            {
                IndicatorIds = GetIndicatorIds(),
                ProfileId = ProfileIds.Undefined,
                RestrictSearchProfileIds = GetRestrictSearchProfileIds(),
                ComparatorMap = new ComparatorMapBuilder(parentArea).ComparatorMap,
                ParentAreaCode = parentAreaCode,
                AreaTypeId = areaTypeId
            };

            var groupRoots = new JsonBuilderGroupDataAtDataPointBySearch(builder).GetGroupRoots();

            Assert.IsTrue(groupRoots.Any());
        }

        [TestMethod]
        public void Test_Get_Data_For_Single_Area()
        {
            var builder = new GroupDataBuilderByIndicatorIds
            {
                IndicatorIds = GetIndicatorIds(),
                RestrictSearchProfileIds = GetRestrictSearchProfileIds(),
                ComparatorMap = new ComparatorMapBuilder(areaTypeId).ComparatorMap,
                AreaCode = AreaCodes.CountyUa_Cumbria,
                AreaTypeId = areaTypeId
            };

            var groupRoots = new JsonBuilderGroupDataAtDataPointBySearch(builder).GetGroupRoots();

            Assert.IsTrue(groupRoots.Any());
        }

        [TestMethod]
        public void Test_Get_Data_For_England()
        {
            var builder = new GroupDataBuilderByIndicatorIds
            {
                IndicatorIds = GetIndicatorIds(),
                RestrictSearchProfileIds = GetRestrictSearchProfileIds(),
                ComparatorMap = new ComparatorMapBuilder(areaTypeId).ComparatorMap,
                AreaCode = AreaCodes.England,
                AreaTypeId = areaTypeId
            };

            var groupRoots = new JsonBuilderGroupDataAtDataPointBySearch(builder).GetGroupRoots();

            Assert.IsTrue(groupRoots.Any());
        }

        private static List<int> GetIndicatorIds()
        {
            return new List<int> { IndicatorIds.LifeExpectancyAt65 };
        }

        private static IList<int> GetRestrictSearchProfileIds()
        {
            return new ProfileIdListProvider(ReaderFactory.GetProfileReader()).GetSearchableProfileIds();
        }
    }
}
