using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioObjectsTest
{
    [TestClass]
    public class PlaceNameTest
    {
        [TestMethod]
        public void TestWeighting()
        {
            var city = new PlaceName {PlaceType = "c"};
            var town = new PlaceName { PlaceType = "t" };
            var other = new PlaceName { PlaceType = "o" };

            Assert.IsTrue(city.PlaceTypeWeighting > town.PlaceTypeWeighting);
            Assert.IsTrue(town.PlaceTypeWeighting > other.PlaceTypeWeighting);
        }
    }
}
