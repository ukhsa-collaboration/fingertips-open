using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class ChildAreaCounterTest
    {
        private Mock<IAreasReader> _areasReader;

        public const int ChildAreaTypeId = AreaTypeIds.County;

        [TestInitialize]
        public void TestInitialize()
        {
            _areasReader = new Mock<IAreasReader>(MockBehavior.Strict);
        }


        [TestMethod]
        public void TestCategoryAreaCount()
        {
            // Arrange
            var parentArea = new CategoryArea(AreaCodes.DeprivationDecile_Utla3);
            _areasReader.Setup(x => x.GetChildAreaCount(parentArea, ChildAreaTypeId))
                .Returns(2);

            // Act & Assert
            Assert.AreEqual(2, GetCount(parentArea));
            _areasReader.VerifyAll();
        }

        [TestMethod]
        public void TestCountryAreaCount()
        {
            // Arrange
            var parentArea = new Mock<IArea>(MockBehavior.Strict);
            parentArea.Setup(x => x.IsCountry).Returns(true);

            // Act
            _areasReader.Setup(x => x.GetAreaCountForAreaType(ChildAreaTypeId))
                .Returns(2);

            //Assert
            var count = GetCounter().GetChildAreasCount(parentArea.Object, ChildAreaTypeId);
            Assert.AreEqual(2, count);
            _areasReader.VerifyAll();
        }

        [TestMethod]
        public void TestSubnationalAreaCount()
        {
            // Arrange
            var areaCode = AreaCodes.Gor_EastMidlands;
            var parentArea = new Mock<IArea>(MockBehavior.Strict);
            parentArea.Setup(x => x.IsCountry).Returns(false);
            parentArea.Setup(x => x.Code).Returns(areaCode);

            // Act
            _areasReader.Setup(x => x.GetChildAreaCount(areaCode, ChildAreaTypeId))
                .Returns(2);

            //Assert
            var count = GetCounter().GetChildAreasCount(parentArea.Object, ChildAreaTypeId);
            Assert.AreEqual(2, count);
            _areasReader.VerifyAll();
        }

        private int GetCount(CategoryArea parentArea)
        {
            var count = GetCounter().GetChildAreasCount(parentArea, ChildAreaTypeId);
            return count;
        }

        private ChildAreaCounter GetCounter()
        {
            return new ChildAreaCounter(_areasReader.Object);
        }
    }
}
