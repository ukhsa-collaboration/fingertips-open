using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServicesWeb.Controllers;
using PholioVisualisation.PholioObjects;

namespace ServicesWebTest.Controllers
{
    [TestClass]
    public class DataControllerTest
    {
        [TestMethod]
        public void TestGetTimePeriod()
        {
            var timePeriod = new DataController().GetTimePeriod(2001,-1,-1,1,YearTypeIds.Calendar);
            Assert.AreEqual("2001", timePeriod);
        }

        [TestMethod]
        public void TestGetNhsChoicesAreaId()
        {
            var nhsChoicesId = new DataController().GetNhsChoicesAreaId(AreaCodes.Gp_Burnham);
            Assert.AreEqual("43611", nhsChoicesId);
        }

        [TestMethod]
        public void TestGetIndicatorMetadataTextProperties()
        {
            var properties = new DataController().GetIndicatorMetadataTextProperties();
            Assert.IsTrue(properties.Select(x => x.ColumnName)
                .Contains(IndicatorMetadataTextColumnNames.DataSource));
        }

        [TestMethod]
        public void TestGetQuinaryPopulationData()
        {
            var areaCode = AreaCodes.Gp_Burnham;

            var data = new DataController().GetQuinaryPopulationData(areaCode,
                GroupIds.PracticeProfiles_SupportingIndicators, 0);

            Assert.AreEqual(data["Code"], areaCode);
        }


        [TestMethod]
        public void TestGetChildAreas_Area_Ignored_For_Profile()
        {
            var areas = new DataController().GetChildAreas(AreaTypeIds.CountyAndUnitaryAuthority,
                AreaCodes.Gor_SouthWest,
                ProfileIds.LongerLives);

            Assert.IsFalse(areas.Select(x => x.Code).Contains(AreaCodes.CountyUa_IslesOfScilly),
                "Isles of Scilly should have been ignored");
        }

        [TestMethod]
        public void TestGetChildAreas_Area_Not_Ignored_When_Profile_Not_Specified()
        {
            var areas = new DataController().GetChildAreas(AreaTypeIds.CountyAndUnitaryAuthority,
                AreaCodes.Gor_SouthWest);

            Assert.IsTrue(areas.Select(x => x.Code).Contains(AreaCodes.CountyUa_IslesOfScilly));
        }

        [TestMethod]
        public void TestGetChildAreas_Area_Not_Ignored_For_Profile_That_Requires_It()
        {
            var areas = new DataController().GetChildAreas(AreaTypeIds.CountyAndUnitaryAuthority,
                AreaCodes.Gor_SouthWest, ProfileIds.Phof);

            Assert.IsTrue(areas.Select(x => x.Code).Contains(AreaCodes.CountyUa_IslesOfScilly));
        }

        [TestMethod]
        public void TestGetProfilesPerIndicator()
        {
            var response = new DataController().GetProfilesPerIndicator(
                IndicatorIds.DeprivationScoreIMD2010.ToString(), AreaTypeIds.CountyAndUnitaryAuthority);
            Assert.IsTrue(response.Count > 0);
        }
    }
}
