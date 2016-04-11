using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PholioObjectsTest
{
    [TestClass]
    public class ParentAreaWithChildAreasTest
    {
        [TestMethod]
        public void TestGetChildAreas()
        {
            var code1 = "a";
            var code2 = "b";
            var code3 = "c";

            var areas = new List<Area>
            {
                Area(code1),Area(code2),Area(code3)
            };

            var parentAreaWithChildAreas = new ParentAreaWithChildAreas(null, areas, AreaTypeIds.County);
            var codes = parentAreaWithChildAreas.GetChildAreaCodes();

            Assert.AreEqual(3, codes.Count());

            foreach (var area in areas)
            {
                Assert.IsTrue(codes.Contains(area.Code));
            }
        }

        private static Area Area(string code1)
        {
            return new Area { Code = code1 };
        }
    }
}
