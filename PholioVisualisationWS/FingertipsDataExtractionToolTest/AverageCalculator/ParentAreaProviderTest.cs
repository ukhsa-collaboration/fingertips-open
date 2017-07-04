using System;
using System.Collections.Generic;
using System.Linq;
using FingertipsDataExtractionTool.AverageCalculator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.FingertipsDataExtractionToolTest.AverageCalculator
{
    [TestClass]
    public class ParentAreaProviderTest
    {
        private const int ChildAreaTypeId = AreaTypeIds.Ccg;
        private const int ParentAreaTypeId = AreaTypeIds.GoRegion;
        private const int CategoryTypeId = CategoryTypeIds.EthnicGroups5;
        private const int CategoryId = CategoryIds.EthnicityAsian;

        private Mock<IAreasReader> _areasReader;
        private Mock<IAreaTypeListProvider> _areaTypeListProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            // Arrange dependencies
            _areasReader = new Mock<IAreasReader>();
            _areaTypeListProvider = new Mock<IAreaTypeListProvider>();

            _areaTypeListProvider.Setup(x => x.GetParentAreaTypeIdsUsedForChildAreaType(ChildAreaTypeId))
                .Returns(new List<int> { ParentAreaTypeId });

            _areaTypeListProvider.Setup(x => x.GetCategoryTypeIdsUsedForChildAreaType(ChildAreaTypeId))
                .Returns(new List<int> { CategoryTypeId });

            _areasReader.Setup(x => x.GetAreasByAreaTypeId(ParentAreaTypeId))
                .Returns(new List<IArea> { new Area { Code = "a" } });

            _areasReader.Setup(x => x.GetCategories(CategoryTypeId))
                .Returns(new List<Category> { new Category { CategoryTypeId = CategoryTypeId, Id = CategoryIds.EthnicityAsian } });

            _areasReader.Setup(x => x.GetAreaFromCode(AreaCodes.England))
            .Returns(new Area { Code = AreaCodes.England });
        }

        [TestMethod]
        public void Test_Category_Area_Is_Provided()
        {
            var areas = GetAreas();

            // Assert
            Assert.AreEqual(CategoryArea.CreateAreaCode(CategoryTypeId, CategoryId), areas[1].Code);
            Verify();
        }

        [TestMethod]
        public void Test_Area_Is_Provided()
        {
            var areas = GetAreas();

            // Assert
            Assert.AreEqual(3, areas.Count);
            Assert.AreEqual("a", areas[0].Code);
            Verify();
        }

        [TestMethod]
        public void Test_England_Is_Always_Provided()
        {
            var areas = GetAreas();

            // Assert
            Assert.AreEqual(AreaCodes.England, areas[2].Code);
            Verify();
        }

        private List<IArea> GetAreas()
        {
            var areas = new ParentAreaProvider(_areasReader.Object, _areaTypeListProvider.Object)
                .GetParentAreas(ChildAreaTypeId);
            return areas;
        }

        private void Verify()
        {
            _areasReader.VerifyAll();
            _areaTypeListProvider.VerifyAll();
        }
    }
}

