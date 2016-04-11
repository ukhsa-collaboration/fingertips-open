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
    public class ChildAreaListBuilderTest
    {
        [TestMethod]
        public void TestChildAreas_WhenParentIsArea()
        {
            var areas = new ChildAreaListBuilder(ReaderFactory.GetAreasReader(),
                AreaCodes.Gor_EastMidlands, AreaTypeIds.CountyAndUnitaryAuthority).ChildAreas;

            var count = areas.Count;
            Assert.IsTrue(count > 5 && count < 10);
        }

        [TestMethod]
        public void TestChildAreas_WhenParentIsCategoryArea()
        {
            var areaCode = CategoryArea.CreateAreaCode(CategoryTypeIds.DeprivationDecileCountyAndUnitaryAuthority,
                1);

            var areas = new ChildAreaListBuilder(ReaderFactory.GetAreasReader(),
                areaCode, AreaTypeIds.CountyAndUnitaryAuthority).ChildAreas;

            var count = areas.Count;
            Assert.IsTrue(count > 10 && count < 20);
        }

        [TestMethod]
        public void TestChildAreas_WhenParentIsCcgAndChildAreaTypeIsGpPractice()
        {
            var areas = new ChildAreaListBuilder(ReaderFactory.GetAreasReader(),
                AreaCodes.Ccg_Barnet, AreaTypeIds.GpPractice).ChildAreas;

            var count = areas.Count;
            Assert.IsTrue(count > 20 && count < 100);
        }
    }
}
