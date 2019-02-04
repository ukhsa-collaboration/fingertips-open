using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class NearbyAreasBuilderTest
    {
        [TestMethod]
        public void TestBuildReturnsSomeAreas()
        {
            Assert.IsTrue(NearByAreas().Count > 0);
        }

        [TestMethod]
        public void TestDistanceValFIsDefined()
        {
            Assert.IsFalse(string.IsNullOrEmpty(NearByAreas().First().DistanceValF));
        }

        [TestMethod]
        public void TestDistanceFormattedTo1DP()
        {
            var areas = NearByAreas();
            Assert.AreEqual("0.6", areas[1].DistanceValF);
            Assert.AreEqual("0.7", areas[2].DistanceValF);
        }

        private static IList<NearByAreas> NearByAreas()
        {
            var areas = new NearbyAreasBuilder().Build("504149", "439837", AreaTypeIds.GpPractice);
            return areas;
        }
    }
}
