using System;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServicesTest
{
    /// <summary>
    /// Tests that services can be connected to.
    /// </summary>
    [TestClass]
    public class DataTest
    {
        [TestMethod]
        public void TestGetValueNotes()
        {
            byte[] data = GetData("value_notes");

            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetAreasOfAreaType()
        {
            byte[] data = GetData("areas?" +
                "area_type_id=" + AreaTypeIds.Ccg +
                "&profile_id=" + ProfileIds.Diabetes);

            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetAreasOfParent()
        {
            byte[] data = GetData("areas?" +
                "area_type_id=" + AreaTypeIds.Ccg +
                "&parent_area_code=" + "cat-2-7" +
                "&profile_id=" + ProfileIds.Diabetes);

            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetMostRecentCategoryData()
        {
            var url = "most_recent_data_for_all_categories?"+
                "profile_id=" + ProfileIds.Phof +
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthority +
                "&area_code=" + AreaCodes.England +
                "&indicator_id=" + IndicatorIds.LifeExpectancyAtBirth +
                "&sex_id=" + SexIds.Male +
                "&age_id=" + AgeIds.AllAges;
            byte[] data = GetData(url);

            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestValueLimits()
        {
            byte[] data = GetData("value_limits?"+
                "group_id=" + GroupIds.PracticeProfiles_PracticeSummary +
                "&area_type_id=" + AreaTypeIds.GpPractice + 
                "&parent_area_code=" + AreaCodes.Ccg_Chiltern);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestNearbyAreas()
        {
            string easting = "352107", northing = "529262";
            int areaTypeId = 7;

            byte[] data = GetData("nearby_areas?easting=" + easting + "&northing=" +
                northing + "&area_type_id=" + areaTypeId);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetParentAreaOfSpecificTypeForChildAreas()
        {
            byte[] data = GetData("parent_area_of_specific_type_for_child_areas?" +
                "parent_area_code=" + AreaCodes.Ccg_Chiltern +
                "&child_area_type_id=" + AreaTypeIds.GpPractice +
                "&parent_area_type_id=" + AreaTypeIds.Shape);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetContent()
        {
            byte[] data = GetData("content?" +
                "profile_id=" + ProfileIds.PhysicalActivity +
                "&key=test-key");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetChildAreas()
        {
            byte[] data = GetData("areas?" +
                "parent_area_code=" + AreaCodes.Gor_EastMidlands +
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthority);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetAreas()
        {
            byte[] data = GetData("areas?" +
                "area_codes=" + AreaCodes.Gor_EastMidlands);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetChildAreasWithAddresses()
        {
            byte[] data = GetData("areas_with_addresses?" +
                "parent_area_code=" + AreaCodes.Ccg_AireDaleWharfdaleAndCraven +
                "&area_type_id=" + AreaTypeIds.GpPractice);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetQuinaryPopulationData()
        {
            byte[] data = GetData("quinary_population_data?"+
                "group_id=" + GroupIds.PracticeProfiles_SupportingIndicators +
                "&area_code=" + AreaCodes.Gp_MeersbrookSheffield);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestProfilesPerIndicator()
        {
            byte[] data = GetData("profiles_containing_indicators?" + 
                "area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthority +
                "&indicator_ids=" + IndicatorIds.TeenagePregnancy);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetChimatResourceId()
        {
            byte[] data = GetData("chimat_resource_id?" +
                "area_code=" + AreaCodes.CountyUa_Buckinghamshire);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetIndicatorMetadataTextProperties()
        {
            byte[] data = GetData("indicator_metadata_text_properties");
            TestHelper.IsData(data);
        }

        private byte[] GetData(string path)
        {
            var url = TestHelper.BaseUrl + "data/" + path;
            return new WebClient().DownloadData(url);
        }
    }
}
