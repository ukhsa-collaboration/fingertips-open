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
        private ChildAreaListBuilder _childAreaListBuilder;

        [TestInitialize]
        public void TestInitialize()
        {
            _childAreaListBuilder = new ChildAreaListBuilder(ReaderFactory.GetAreasReader());
        }

        [TestMethod]
        public void TestChildAreas_WhenParentIsArea()
        {
            var areas = _childAreaListBuilder.GetChildAreas(AreaCodes.Gor_EastMidlands, 
                AreaTypeIds.CountyAndUnitaryAuthority);

            var count = areas.Count;
            Assert.IsTrue(count > 5 && count < 10);
        }

        [TestMethod]
        public void TestChildAreas_WhenParentIsCategoryArea()
        {
            var areaCode = CategoryArea.CreateAreaCode(CategoryTypeIds.DeprivationDecileCountyAndUA2010,
                1);

            var areas = _childAreaListBuilder.GetChildAreas(areaCode, AreaTypeIds.CountyAndUnitaryAuthority);

            var count = areas.Count;
            Assert.IsTrue(count > 10 && count < 20);
        }

        [TestMethod]
        public void TestChildAreas_WhenParentIsCcgAndChildAreaTypeIsGpPractice()
        {
            var areas = _childAreaListBuilder.GetChildAreas(AreaCodes.Ccg_Barnet, AreaTypeIds.GpPractice);

            var count = areas.Count;
            Assert.IsTrue(count > 20 && count < 100);
        }
    }
}
