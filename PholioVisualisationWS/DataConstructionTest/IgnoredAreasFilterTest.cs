using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class IgnoredAreasFilterTest
    {
        private string areaCodeToKeep = "a";
        private string areaCodeToRemove = "b";

        [TestMethod]
        public void TestRemoveAreasIgnoredEverywhere()
        {
            var areas = new List<Area>
            {
                new Area {Code = areaCodeToKeep},
                new Area {Code = areaCodeToRemove}
            };

            var filter = new IgnoredAreasFilter(AreaCodesToRemove());
            IEnumerable<IArea> filteredAreas = filter.RemoveAreasIgnoredEverywhere(areas);

            Assert.AreEqual(1, filteredAreas.Count());
            Assert.AreEqual(areaCodeToKeep, filteredAreas.First().Code);
        }

        [TestMethod]
        public void TestRemoveAreaCodesIgnoredEverywhere()
        {
            var areaCodes = new List<string> {areaCodeToKeep, areaCodeToRemove};

            var filter = new IgnoredAreasFilter(AreaCodesToRemove());
            IEnumerable<string> filteredCodes = filter.RemoveAreaCodesIgnoredEverywhere(areaCodes);

            Assert.AreEqual(1, filteredCodes.Count());
            Assert.AreEqual(areaCodeToKeep, filteredCodes.First());
        }

        private List<string> AreaCodesToRemove()
        {
            return new List<string> {areaCodeToRemove};
        }
    }
}