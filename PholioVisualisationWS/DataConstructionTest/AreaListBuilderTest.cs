using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class AreaListBuilderTest
    {
        private int areaTypeId = 1;
        private int profileId = 2;

        [TestMethod]
        public void TestGetCategoryTypeIdFromAreaTypeId()
        {
            Assert.AreEqual(2, CategoryAreaType.GetCategoryTypeIdFromAreaTypeId(10002));
        }

        [TestMethod]
        public void CreateAreaListFromAreaTypeIdWhereCategoryAreaType()
        {
            var categoryAreaType = CategoryAreaType.New(new CategoryType
            {
                Id = CategoryTypeIds.DeprivationDecileCountyAndUA2010
            });
            var builder = new AreaListProvider(ReaderFactory.GetAreasReader());
            builder.CreateAreaListFromAreaTypeId(profileId, categoryAreaType.Id);
            var areaList = builder.Areas;
            Assert.IsTrue(areaList.Any());

            // Check names are defined
            Assert.IsFalse(string.IsNullOrEmpty(areaList.First().Name));
            Assert.IsFalse(string.IsNullOrEmpty(areaList.First().ShortName));
        }

        [TestMethod]
        public void TestCreateAreaListFromAreaTypeIdReturningNoAreas()
        {
            var builder = CreateAreaListFromAreaTypeId(new List<IArea>());
            Assert.AreEqual(0, builder.Areas.Count);
        }

        [TestMethod]
        public void CreateAreaListFromAreaTypeIdReturnsParentAreas()
        {
            IList<string> codes = new List<string> { "a", "b" };
            var areas = AreaList();

            var mockAreasReader = new Mock<AreasReader>();
            ParentAreaCodesAreFound(codes, mockAreasReader);
            AreasGotFromCodes(codes, areas, mockAreasReader);

            var builder = new AreaListProvider(mockAreasReader.Object);
            builder.CreateAreaListFromAreaTypeId(profileId, areaTypeId);
            var areaList = builder.Areas;
            Assert.AreEqual(3, areaList.Count);
        }

        [TestMethod]
        public void TestRemoveAreasIgnoredEverywhere()
        {
            var areas = AreaList();
            var builder = CreateAreaListFromAreaTypeId(areas);

            var ignoredAreasFilter = new IgnoredAreasFilter(new List<string> { "a" });
            builder.RemoveAreasIgnoredEverywhere(ignoredAreasFilter);

            var filteredAreas = builder.Areas;
            AssertName("c", filteredAreas[0]);
            AssertName("b", filteredAreas[1]);
        }

        [TestMethod]
        public void TestSortByName()
        {
            var areas = AreaList();
            var builder = CreateAreaListFromAreaTypeId(areas);

            builder.SortByOrderOrName();

            var filteredAreas = builder.Areas;
            AssertName("a", filteredAreas[0]);
            AssertName("b", filteredAreas[1]);
            AssertName("c", filteredAreas[2]);
        }

        [TestMethod]
        public void TestSortByOrder()
        {
            var areas = new List<IArea>
            {
                new Area {Code = "c", Name = "c", Sequence = 2},
                new Area {Code = "a", Name = "a", Sequence = 3},
                new Area {Code = "b", Name = "b", Sequence = 1},
            };
            var builder = CreateAreaListFromAreaTypeId(areas);

            builder.SortByOrderOrName();

            var filteredAreas = builder.Areas;
            for (int i = 0; i < filteredAreas.Count; i++)
            {
                Assert.AreEqual(i + 1, filteredAreas[i].Sequence);
            }
        }

        [TestMethod]
        public void TestGetAreaListFromAreaCodesReturnEmptyListForIfAreaCodes()
        {
            var codes = new string[] { };

            var mock = new Mock<AreasReader>();
            mock.Setup(x => x
                .GetAreasFromCodes(codes))
                .Returns(new List<IArea>());

            var builder = new AreaListProvider(mock.Object);

            builder.CreateAreaListFromAreaCodes(codes);
            Assert.AreEqual(0, builder.Areas.Count);
        }

        [TestMethod]
        public void TestGetAreaListFromAreaCodes()
        {
            const string code = "a";

            var codes = new[] { code };

            var mock = new Mock<AreasReader>();
            mock.Setup(x => x
                .GetAreaFromCode(code))
                .Returns(new Area { Code = code });
            var builder = new AreaListProvider(mock.Object);

            builder.CreateAreaListFromAreaCodes(codes);
            var areas = builder.Areas;
            Assert.AreEqual(1, areas.Count);
            Assert.AreEqual(code, areas.First().Code);
        }

        [TestMethod]
        public void TestCreateAreaListFromNearestNeighbourAreaCode()
        {
            const string code = AreaCodes.NearestNeighbours_Derby;
            var nn = new NearestNeighbourArea(code);
            IAreasReader areasReader = ReaderFactory.GetAreasReader();

            var builder = new AreaListProvider(areasReader);
            builder.CreateAreaListFromNearestNeighbourAreaCode(
                ProfileIds.ChildrenAndYoungPeoplesBenchmarkingTool, code);

            // Check areas
            var areas = builder.Areas;
            Assert.IsNotNull(areas);

            // Check order of Nearest Neighbours
            var sortedNearestNeighboursByRank = areasReader.GetNearestNeighbours(nn.Code, 1);
            for (var i = 0; i < sortedNearestNeighboursByRank.Count; i++)
            {
                Assert.AreEqual(sortedNearestNeighboursByRank[i].NeighbourAreaCode, areas[i].Code);
            }
        }

        private AreaListProvider CreateAreaListFromAreaTypeId(List<IArea> areas)
        {
            var mockAreasReader = new Mock<AreasReader>();
            GetAreasByAreaTypeIdReturnsAreas(areas, mockAreasReader);
            NoParentAreaCodes(mockAreasReader);

            var builder = new AreaListProvider(mockAreasReader.Object);
            builder.CreateAreaListFromAreaTypeId(profileId, areaTypeId);
            return builder;
        }

        private List<IArea> AreaList()
        {
            var areas = new List<IArea>
            {
                Area("c"),
                Area("a"),
                Area("b")
            };
            return areas;
        }

        public Area Area(string nameAndCode)
        {
            return new Area { Name = nameAndCode, Code = nameAndCode };
        }

        public void AssertName(string name, IArea area)
        {
            Assert.AreEqual(name, area.Name);
        }

        private void GetAreasByAreaTypeIdReturnsAreas(List<IArea> areas, Mock<AreasReader> mockAreasReader)
        {
            mockAreasReader.Setup(x => x
                .GetAreasByAreaTypeId(areaTypeId))
                .Returns(areas);
        }

        private void NoParentAreaCodes(Mock<AreasReader> mock)
        {
            mock.Setup(x => x
                .GetProfileParentAreaCodes(profileId, areaTypeId))
                .Returns(new List<string>());
        }

        private void ParentAreaCodesAreFound(IList<string> areaCodes, Mock<AreasReader> mock)
        {
            mock.Setup(x => x
                .GetProfileParentAreaCodes(profileId, areaTypeId))
                .Returns(areaCodes);
        }

        private static void AreasGotFromCodes(IList<string> codes, List<IArea> areas, Mock<AreasReader> mockAreasReader)
        {
            mockAreasReader.Setup(x => x
                .GetAreasFromCodes(codes))
                .Returns(areas);
        }
    }
}
