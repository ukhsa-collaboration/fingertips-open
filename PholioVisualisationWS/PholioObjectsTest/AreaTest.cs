using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PholioObjectsTest
{
    [TestClass]
    public class AreaTest
    {
        [TestMethod]
        public void TestIsGpDeprivationDecile()
        {
            Assert.IsFalse(new Area { AreaTypeId = AreaTypeIds.GpPractice }.IsGpDeprivationDecile);
        }

        [TestMethod]
        public void TestIsShape()
        {
            Assert.IsTrue(new Area { AreaTypeId = AreaTypeIds.Shape }.IsShape);
            Assert.IsFalse(new Area { AreaTypeId = AreaTypeIds.GpPractice }.IsShape);
        }

        [TestMethod]
        public void TestIsGpPractice()
        {
            Assert.IsTrue(new Area { AreaTypeId = AreaTypeIds.GpPractice }.IsGpPractice);
            Assert.IsFalse(new Area { AreaTypeId = AreaTypeIds.County }.IsGpPractice);
        }

        [TestMethod]
        public void TestIsCountry()
        {
            Assert.IsTrue(new Area { AreaTypeId = AreaTypeIds.Country }.IsCountry);
            Assert.IsFalse(new Area { AreaTypeId = AreaTypeIds.GpPractice }.IsCountry);
        }

        [TestMethod]
        public void TestIsCcg()
        {
            Assert.IsTrue(new Area { AreaTypeId = AreaTypeIds.CcgsPostApr2019 }.IsCcg);
            Assert.IsTrue(new Area { AreaTypeId = AreaTypeIds.CcgsPostApr2018 }.IsCcg);
            Assert.IsTrue(new Area { AreaTypeId = AreaTypeIds.CcgsPostApr2017 }.IsCcg);
            Assert.IsFalse(new Area { AreaTypeId = AreaTypeIds.GpPractice }.IsCcg);
        }

        [TestMethod]
        public void TestIsPheCentre()
        {
            Assert.IsTrue(new Area { AreaTypeId = AreaTypeIds.PheCentresFrom2015 }.IsPheCentre);
            Assert.IsTrue(new Area { AreaTypeId = AreaTypeIds.PheCentresFrom2013To2015 }.IsPheCentre);
            Assert.IsFalse(new Area { AreaTypeId = AreaTypeIds.Country }.IsPheCentre);
        }

        [TestMethod]
        public void TestShouldSerializeShortName()
        {
            Assert.IsFalse(new Area { ShortName = null}.ShouldSerializeShortName());
            Assert.IsTrue(new Area { ShortName = "a" }.ShouldSerializeShortName());

            CheckSerialisation("Short", new Area { ShortName = "a" });
        }

        [TestMethod]
        public void TestShortNameSerializedToJson()
        {
            CheckSerialisation("Short", new Area { ShortName = "a" });
        }

        [TestMethod]
        public void TestIsNearestNeighbour_Returns_True_For_Nearest_Neighbour_Area_Code()
        {
            Assert.IsTrue(Area.IsNearestNeighbour("nn-1-a"));
        }

        [TestMethod]
        public void TestIsNearestNeighbour_Returns_False_For_Standard_Area_Code()
        {
            Assert.IsFalse(Area.IsNearestNeighbour(AreaCodes.England));
            Assert.IsFalse(Area.IsNearestNeighbour(AreaCodes.Gor_EastMidlands));
            Assert.IsFalse(Area.IsNearestNeighbour(AreaCodes.CountyUa_Cambridgeshire));
        }

        [TestMethod]
        public void TestIsNearestNeighbour_Returns_False_For_Null()
        {
            Assert.IsFalse(Area.IsNearestNeighbour(null));
        }

        [TestMethod]
        public void TestIsNearestNeighbour_Returns_False_For_Empty_String()
        {
            Assert.IsFalse(Area.IsNearestNeighbour(""));
        }

        [TestMethod]
        public void TestIsNearestNeighbour_Returns_False_For_Whitespace()
        {
            Assert.IsFalse(Area.IsNearestNeighbour(" "));
        }

        private static void CheckSerialisation(string propertyName, Area area)
        {
            JsonTestHelper.AssertPropertyIsSerialised(area, propertyName);
            JsonTestHelper.AssertPropertyIsNotSerialised(new Area(), propertyName);
        }
    }
}
