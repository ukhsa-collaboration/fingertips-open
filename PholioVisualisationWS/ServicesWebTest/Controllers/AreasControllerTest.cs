using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServicesWeb.Controllers;

namespace PholioVisualisation.ServicesWebTest.Controllers
{
    [TestClass]
    public class AreasControllerTest
    {
        private const string ParentCode = "al-ZY6zmuVONE";

        [TestMethod]
        public void TestGetChildAreas_Area_Ignored_For_Profile()
        {
            var areas = new AreasController().GetChildAreas(AreaTypeIds.CountyAndUnitaryAuthority,
                AreaCodes.Gor_SouthWest,
                ProfileIds.LongerLives);

            Assert.IsFalse(areas.Select(x => x.Code).Contains(AreaCodes.CountyUa_IslesOfScilly),
                "Isles of Scilly should have been ignored");
        }

        [TestMethod]
        public void TestGetChildAreas_Area_Not_Ignored_When_Profile_Not_Specified()
        {
            var areas = new AreasController().GetChildAreas(AreaTypeIds.CountyAndUnitaryAuthority,
                AreaCodes.Gor_SouthWest);

            Assert.IsTrue(areas.Select(x => x.Code).Contains(AreaCodes.CountyUa_IslesOfScilly));
        }

        [TestMethod]
        public void TestGetChildAreas_Area_Not_Ignored_For_Profile_That_Requires_It()
        {
            var areas = new AreasController().GetChildAreas(AreaTypeIds.CountyAndUnitaryAuthority,
                AreaCodes.Gor_SouthWest, ProfileIds.Phof);

            Assert.IsTrue(areas.Select(x => x.Code).Contains(AreaCodes.CountyUa_IslesOfScilly));
        }

        [TestMethod]
        public void TestGetAreasOfAreaType_WhenAreaCodesListSubmitted()
        {
            var codes = new List<string> { AreaCodes.CountyUa_Cumbria, AreaCodes.CountyUa_Leicestershire };

            var areas = new AreasController().GetAreasOfAreaType(string.Join(",", codes));

            // Assert
            Assert.AreEqual(2, areas.Count);
            Assert.IsNotNull(areas.FirstOrDefault(x => x.Code == AreaCodes.CountyUa_Cumbria));
        }

        [TestMethod]
        public void TestGetChildAreas_WithAddressesAsCsv()
        {
            var addresses = new AreasController().GetChildAreasWithAddressesAsCsv(
                AreaCodes.Ccg_Barnet, AreaTypeIds.GpPractice);

            // Assert
            var text = addresses.Content.ReadAsStringAsync().Result;
            Assert.IsTrue(text.Contains("Barnet"));
        }

        [TestMethod]
        public void TestGetArea_ByAreaType_And_ParentCode()
        {
            var area = new AreasController().GetAreaByAreaTypeAndParentCode(AreaTypeIds.AreaList,
                ParentCode, ProfileIds.Phof);

            Assert.AreEqual(area.Code, ParentCode);
        }

        [TestMethod]
        public void TestGetParentToChildAreaMapping_ByParentCode()
        {
            var mappings = new AreasController().GetParentToChildAreaMappingByParentCode(-1, AreaTypeIds.AreaList,
                ProfileIds.Phof, null, ParentCode, null);

            Assert.IsTrue(mappings.Any());
        }
    }
}
