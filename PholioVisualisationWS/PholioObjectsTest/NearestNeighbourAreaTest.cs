using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioObjectsTest
{
    [TestClass]
    public class NearestNeighbourAreaTest
    {
        [TestMethod]
        public void TestCreateAreaCode()
        {
            var code = NearestNeighbourArea.CreateAreaCode(1, "a");
            Assert.AreEqual("nn-1-a", code);
        }

        [TestMethod]
        public void TestNeighbourAreaCodes()
        {
            var code = NearestNeighbourArea.CreateAreaCode(1, "a");
            var area = new NearestNeighbourArea(code);
            area.Neighbours = new []
            {
                new AreaCodeNeighbourMapping{NeighbourAreaCode = "b"},
                new AreaCodeNeighbourMapping{NeighbourAreaCode = "c"}
            };

            Assert.AreEqual(2, area.Neighbours.Count);
            Assert.AreEqual(2, area.NeighbourAreaCodes.Count);
            Assert.AreEqual("b", area.NeighbourAreaCodes[0]);
            Assert.AreEqual("c", area.NeighbourAreaCodes[1]);
        }
    }
}
