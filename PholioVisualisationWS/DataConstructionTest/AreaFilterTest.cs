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
    public class AreaFilterTest
    {
        [TestMethod]
        public void TestFilterAreas()
        {
            var codes = new[] { "a", "b" };
            var filteredAreas = new AreaFilter(TestAreas()).RemoveWithAreaCode(codes);

            Assert.AreEqual(1, filteredAreas.Count());
            Assert.AreEqual("c", filteredAreas.First().Code);
        }

        [TestMethod]
        public void TestFilterAreasNoCodesToIgnore()
        {
            var filteredAreas = new AreaFilter(TestAreas()).RemoveWithAreaCode(new string[] { });
            Assert.AreEqual(3, filteredAreas.Count());

            filteredAreas = new AreaFilter(TestAreas()).RemoveWithAreaCode(null);
            Assert.AreEqual(3, filteredAreas.Count());
        }

        [TestMethod]
        public void TestFilterAreasCaseInsensitive()
        {
            // WRT ignored codes
            var filteredAreas = new AreaFilter(TestAreas()).RemoveWithAreaCode(new[] { "A", "B" });

            Assert.AreEqual(1, filteredAreas.Count());
            Assert.AreEqual("c", filteredAreas.First().Code);

            // WRT coredataset area codes
            var areas = TestAreas();
            foreach (var area in areas)
            {
                area.Code = area.Code.ToUpper();
            }
            filteredAreas = new AreaFilter(areas).RemoveWithAreaCode(new[] { "a", "b" });

            Assert.AreEqual(1, filteredAreas.Count());
            Assert.AreEqual("C", filteredAreas.First().Code);
        }
        public IEnumerable<Area> TestAreas()
        {
            return new[]
                {
                    new Area{Code = "a"},
                    new Area{Code = "b"},
                    new Area{Code = "c"}
                };
        }
    }
}
