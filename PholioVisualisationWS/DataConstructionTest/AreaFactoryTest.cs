using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class AreaFactoryTest
    {
        [TestMethod]
        [ExpectedException(typeof(FingertipsException),"Area code was null")]
        public void TestNewAreaThrowMeaningfulExceptionIfAreaCodeIsNull()
        {
            AreaFactory.NewArea(ReaderFactory.GetAreasReader(), null);
        }

        [TestMethod]
        public void TestNewArea()
        {
            var code = AreaCodes.CountyUa_Bexley;
            var area = AreaFactory.NewArea(ReaderFactory.GetAreasReader(), code);
            Assert.AreEqual(code.ToLower(), area.Code.ToLower());
            Assert.IsNotNull(area as Area);
        }

        [TestMethod]
        public void TestNewCategoryArea()
        {
            var categoryArea = CategoryArea.New(CategoryTypeIds.DeprivationDecileCountyAndUnitaryAuthority, 5);
            var area = AreaFactory.NewArea(ReaderFactory.GetAreasReader(), categoryArea.Code);

            Assert.IsNotNull(area as CategoryArea);
            Assert.IsFalse(string.IsNullOrEmpty(area.Name));
            Assert.IsFalse(string.IsNullOrEmpty(area.ShortName));
        }

        [TestMethod]
        public void TestNewNearestNeighbourArea()
        {
            var nearestNeighbourArea = NearestNeighbourArea.New(NearestNeighbourTypeIds.Cipfa, AreaCodes.CountyUa_Cambridgeshire);
            var area = AreaFactory.NewArea(ReaderFactory.GetAreasReader(), nearestNeighbourArea.Code);

            var newNearestNeighbourArea = area as NearestNeighbourArea;
            Assert.IsNotNull(newNearestNeighbourArea);
            Assert.AreEqual(15, newNearestNeighbourArea.Neighbours.Count);
            Assert.AreEqual(1, newNearestNeighbourArea.NeighbourTypeId);

            // Names do not need to be defined
            Assert.IsTrue(string.IsNullOrEmpty(area.Name));
            Assert.IsTrue(string.IsNullOrEmpty(area.ShortName));
        }
    }
}
