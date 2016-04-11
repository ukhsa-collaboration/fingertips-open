using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class ParentAreasOfChildAreasBuilderTest
    {
        [TestMethod]
        public void TestAreaMappingWithCcgAndGpPractice()
        {
            var map = new ParentAreasOfChildAreasBuilder()
                .GetAreaMapping(AreaCodes.Ccg_Barnet, AreaTypeIds.GpPractice, AreaTypeIds.Shape);

            var count = map.Count;
            Assert.IsTrue(count > 20 && count < 100);

            var parentAreaCode = map.Values.First().Code;
            Assert.IsTrue(parentAreaCode.StartsWith("S_"));
        }
    }
}
