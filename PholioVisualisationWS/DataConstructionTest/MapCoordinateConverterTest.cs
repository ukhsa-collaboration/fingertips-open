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
    public class MapCoordinateConverterTest
    {
        [TestMethod]
        public void TestConvertEastingNorthingToLatitudeLongitude()
        {
            LatitudeLongitude coordinate =
                MapCoordinateConverter.ConvertEastingNorthingToLatitudeLongitude(209459, 76874);

            Assert.AreEqual(50.55992, coordinate.Latitude);
            Assert.AreEqual(-4.69052, coordinate.Longitude);
        }

        [TestMethod]
        public void TestGetLatitudeLongitude()
        {
            Assert.IsNotNull(MapCoordinateConverter.GetLatitudeLongitude(new EastingNorthing { Easting = 2094, Northing = 7687 }));
            Assert.IsNull(MapCoordinateConverter.GetLatitudeLongitude(null));
        }
    }
}
