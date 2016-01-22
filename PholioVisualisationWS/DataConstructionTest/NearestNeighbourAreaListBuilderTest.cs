using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class NearestNeighbourAreaListBuilderTest
    {
        [TestMethod]
        public void TestAreas()
        {
            var code = NearestNeighbourArea.CreateAreaCode(1, "a");
            var area = new NearestNeighbourArea(code);
            area.Neighbours = new []
            {
                new AreaCodeNeighbourMapping{ NeighbourAreaCode = AreaCodes.CountyUa_Cambridgeshire},
                new AreaCodeNeighbourMapping{ NeighbourAreaCode = AreaCodes.CountyUa_Cumbria}
            };

            var areas = new NearestNeighbourAreaListBuilder(ReaderFactory.GetAreasReader(),
                area).Areas;

            Assert.AreEqual(2, areas.Count);
        }
    }
}
