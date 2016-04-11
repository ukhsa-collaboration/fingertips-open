using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PholioObjectsTest
{
    [TestClass]
    public class GeographicalSearchResultTest
    {
        [TestMethod]
        public void TestShouldSerializeParentAreaCode()
        {
            CheckSerialisation("PolygonAreaCode", new GeographicalSearchResult {PolygonAreaCode = "a"});
        }

        [TestMethod]
        public void TestShouldSerializeEasting()
        {
            CheckSerialisation("Easting", new GeographicalSearchResult { Easting = 1 });
        }

        [TestMethod]
        public void TestShouldSerializeNorthing()
        {
            CheckSerialisation("Northing", new GeographicalSearchResult { Northing = 1 });
        }

        private static void CheckSerialisation(string propertyName, GeographicalSearchResult result)
        {
            JsonTestHelper.AssertPropertyIsSerialised(result, propertyName);
            JsonTestHelper.AssertPropertyIsNotSerialised(new GeographicalSearchResult(), propertyName);
        }
    }
}