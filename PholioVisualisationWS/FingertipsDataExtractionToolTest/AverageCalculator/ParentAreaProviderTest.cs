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
            _areasReader = new Mock<IAreasReader>();
            _areaTypeListProvider = new Mock<IAreaTypeListProvider>();
        }

        [TestMethod]
        public void Test()
        {
            _areaTypeListProvider.Setup(x => x.GetParentAreaTypeIdsUsedForChildAreaType(ChildAreaTypeId))
                .Returns(new List<int> {ParentAreaTypeId});

            _areaTypeListProvider.Setup(x => x.GetCategoryTypeIdsUsedForChildAreaType(ChildAreaTypeId))
                .Returns(new List<int> { CategoryTypeId });

            _areasReader.Setup(x => x.GetAreasByAreaTypeId(ParentAreaTypeId))
                .Returns(new List<IArea> { new Area {Code = "a"}});

            _areasReader.Setup(x => x.GetCategories(CategoryTypeId))
                .Returns(new List<Category> { new Category {CategoryTypeId = CategoryTypeId, Id = CategoryIds.EthnicityAsian} });

            // Act
            var areas = new ParentAreaProvider(_areasReader.Object, _areaTypeListProvider.Object)
                .GetParentAreas(ChildAreaTypeId);

            // Assert
            Assert.AreEqual(2, areas.Count);
            Assert.AreEqual("a", areas[0].Code);
            Assert.AreEqual(CategoryArea.CreateAreaCode(CategoryTypeId, CategoryId), areas[1].Code);

            _areasReader.VerifyAll();
            _areaTypeListProvider.VerifyAll();
        }
    }
}

