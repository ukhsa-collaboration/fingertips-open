using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServicesTest
{
    /// <summary>
    /// Tests that make sure each end point returns data.
    /// </summary>
    [TestClass]
    public class DataControllerEndPointTest
    {
        [TestMethod]
        public void TestGetTrendMarkers()
        {
            var url = "recent_trends/for_child_areas?" +
                      "parent_area_code=" + AreaCodes.England +
                      "&group_id=" + GroupIds.Phof_WiderDeterminantsOfHealth +
                      "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthority +
                      "&indicator_id=" + IndicatorIds.ChildrenInLowIncomeFamilies +
                      "&sex_id=" + SexIds.Persons +
                      "&age_id=" + AgeIds.Under16;
            byte[] data = GetData(url);

            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetIndicatorStatistics_For_Group()
        {
            var url = "indicator_statistics?" +
                "group_id=" + GroupIds.Phof_HealthProtection +
                "&child_area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthority +
                "&parent_area_code=" + AreaCodes.England;
            byte[] data = GetData(url);

            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetPartitionDataForCategories()
        {
            var url = "partition_data/by_category?" +
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
        public void TestGetPartitionTrendDataForCategories()
        {
            var url = "partition_trend_data/by_category?" +
                "profile_id=" + ProfileIds.Phof +
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthority +
                "&area_code=" + AreaCodes.England +
                "&indicator_id=" + IndicatorIds.LifeExpectancyAtBirth +
                "&sex_id=" + SexIds.Male +
                "&age_id=" + AgeIds.AllAges +
                "&category_type_id=" + CategoryTypeIds.DeprivationDecileDistrictAndUA2015;
            byte[] data = GetData(url);

            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetPartitionTrendDataForSexes()
        {
            var url = "partition_trend_data/by_sex?" +
                "profile_id=" + ProfileIds.Phof +
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthority +
                "&area_code=" + AreaCodes.England +
                "&indicator_id=" + IndicatorIds.LifeExpectancyAtBirth +
                "&age_id=" + AgeIds.AllAges;
            byte[] data = GetData(url);

            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetPartitionTrendDataForAges()
        {
            var url = "partition_trend_data/by_age?" +
                "profile_id=" + ProfileIds.Phof +
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthority +
                "&area_code=" + AreaCodes.England +
                "&indicator_id=" + IndicatorIds.LifeExpectancyAtBirth +
                "&sex_id=" + SexIds.Male;
            byte[] data = GetData(url);

            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestValueLimits()
        {
            byte[] data = GetData("value_limits?" +
                "group_id=" + GroupIds.PracticeProfiles_PracticeSummary +
                "&area_type_id=" + AreaTypeIds.GpPractice +
                "&parent_area_code=" + AreaCodes.Ccg_Chiltern);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetQuinaryPopulationData()
        {
            byte[] data = GetData("quinary_population_data?" +
                "group_id=" + GroupIds.PracticeProfiles_SupportingIndicators +
                "&area_code=" + AreaCodes.Gp_MeersbrookSheffield);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetQuinaryPopulation()
        {
            byte[] data = GetData("quinary_population?" +
                "area_type_id=" + AreaTypeIds.GpPractice +
                "&area_code=" + AreaCodes.Gp_MeersbrookSheffield);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetQuinaryPopulationSummary()
        {
            byte[] data = GetData("quinary_population_summary?" +
                "area_type_id=" + AreaTypeIds.GpPractice +
                "&area_code=" + AreaCodes.Gp_MeersbrookSheffield);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetProfilesPerIndicator()
        {
            byte[] data = GetData("profiles_containing_indicators?" +
                "area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthority +
                "&indicator_ids=" + IndicatorIds.TeenagePregnancy);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetAreaValues()
        {
            byte[] data = GetData("latest_data/single_indicator_for_all_areas?" +
                "group_id=" + GroupIds.Phof_HealthcarePrematureMortality +
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthority +
                "&parent_area_code=" + AreaCodes.Gor_SouthEast +
                "&comparator_id=" + ComparatorIds.England +
                "&indicator_id=" + IndicatorIds.ExcessWinterDeaths +
                "&sex_id=" + SexIds.Persons +
                "&age_id=" + AgeIds.AllAges +
                "&profile_id=" + ProfileIds.Phof
                );
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetGroupDataAtDataPoint()
        {
            byte[] data = GetData("latest_data/all_indicators_in_profile_group_for_child_areas?" +
                "group_id=" + GroupIds.Phof_HealthcarePrematureMortality +
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthority +
                "&parent_area_code=" + AreaCodes.Gor_SouthEast +
                "&profile_id=" + ProfileIds.Phof
                );
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetGroupataByIndicatorIds()
        {
            byte[] data = GetData("latest_data/specific_indicators_for_child_areas?" +
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthority +
                "&parent_area_code=" + AreaCodes.Gor_SouthEast +
                "&restrict_to_profile_ids=" + ProfileIds.Phof +
                "&indicator_ids=" + IndicatorIds.ChildrenInLowIncomeFamilies
                );
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetTrendDataByProfileGroup()
        {
            byte[] data = GetData("trend_data/all_indicators_in_profile_group_for_child_areas?" +
                "group_id=" + GroupIds.Phof_HealthcarePrematureMortality +
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthority +
                "&parent_area_code=" + AreaCodes.Gor_SouthEast +
                "&profile_id=" + ProfileIds.Phof
                );
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetTrendDataByIndicatorIds()
        {
            byte[] data = GetData("trend_data/specific_indicators_for_child_areas?" +
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthority +
                "&parent_area_code=" + AreaCodes.Gor_SouthEast +
                "&restrict_to_profile_ids=" + ProfileIds.Phof +
                "&indicator_ids=" + IndicatorIds.ChildrenInLowIncomeFamilies
                );
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetAreaData()
        {
            byte[] data = GetData("latest_data/all_indicators_in_multiple_profile_groups_for_multiple_areas?" +
                "group_ids=" + GroupIds.Phof_HealthcarePrematureMortality +
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthority +
                "&area_codes=" + AreaCodes.Gor_SouthEast +
                "&comparator_area_codes=" + AreaCodes.England);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetProfileGroupRoots()
        {
            byte[] data = GetData("profile_group_roots?" +
                "group_id=" + GroupIds.Phof_HealthProtection +
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthority);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetIndicatorsThatMatchText()
        {
            byte[] data = GetData("indicator_search?search_text=hip");

            TestHelper.IsData(data);

            // Contains PHOF ID
            TestHelper.AssertDataContainsString(data, "\"" + ProfileIds.Phof + "\"");
        }

        public static byte[] GetData(string path)
        {
            var url = TestHelper.BaseUrl + "api/" + path;
            Debug.WriteLine(url);
            return new WebClient().DownloadData(url);
        }
    }
}
