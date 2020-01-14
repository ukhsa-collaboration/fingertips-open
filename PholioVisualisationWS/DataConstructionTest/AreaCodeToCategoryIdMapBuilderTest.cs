using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class AreaCodeToCategoryIdMapBuilderTest
    {
        [TestMethod]
        public void TestBuild()
        {
            var builder = new AreaCodeToCategoryIdMapBuilder
            {
                ChildAreaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019,
                CategoryTypeId = CategoryTypeIds.DeprivationDecileCountyAndUA2010
            };

            Dictionary<string, int> map = builder.Build();
            Assert.AreEqual(9, map[AreaCodes.CountyUa_Cambridgeshire]);
        }

        [TestMethod]
        public void TestLowerTierAuthorityBuild()
        {
            var builder = new AreaCodeToCategoryIdMapBuilder
            {
                ChildAreaTypeId = AreaTypeIds.DistrictAndUnitaryAuthorityPreApr2019,
                CategoryTypeId = CategoryTypeIds.DeprivationDecileDistrictAndUA2010
            };

            Dictionary<string, int> map = builder.Build();
            Assert.AreEqual(10, map[AreaCodes.DistrictUa_SouthCambridgeshire]);
        }
    }
}