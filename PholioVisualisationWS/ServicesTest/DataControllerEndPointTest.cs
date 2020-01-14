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
        /// <summary>
        /// Try and get population data as this is compiled differently to other data
        /// </summary>
        [TestMethod]
        public void TestGetAllDataAsCsvByProfileId_Populations()
        {
            var url = "all_data/csv/by_profile_id?" +
                      "parent_area_type_id=" + AreaTypeIds.GoRegion +
                      "&child_area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019 +
                      "&profile_id=" + ProfileIds.Populations;
            byte[] data = GetData(url);

            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetAllDataAsCsvByProfileId()
        {
            var url = "all_data/csv/by_profile_id?" +
                      "parent_area_type_id=" + AreaTypeIds.GoRegion +
                      "&child_area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019 +
                      "&profile_id=" + ProfileIds.HealthProfiles;
            byte[] data = GetData(url);

            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetAllDataAsCsvByGroupId()
        {
            var url = "all_data/csv/by_group_id?" +
                      "parent_area_type_id=" + AreaTypeIds.GoRegion +
                      "&child_area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019 +
                      "&group_id=" + GroupIds.Phof_HealthProtection;
            byte[] data = GetData(url);

            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetAllDataAsCsvByIndicatorId()
        {
            var url = "all_data/csv/by_indicator_id?" +
                      "parent_area_type_id=" + AreaTypeIds.GoRegion +
                      "&child_area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019 +
                      "&indicator_ids=" + IndicatorIds.AgedOver85Years;
            byte[] data = GetData(url);

            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetTrendMarkers()
        {
            var url = "recent_trends/for_child_areas?" +
                      "parent_area_code=" + AreaCodes.England +
                      "&group_id=" + GroupIds.Phof_WiderDeterminantsOfHealth +
                      "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019 +
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
                "&child_area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019 +
                "&parent_area_code=" + AreaCodes.England;
            byte[] data = GetData(url);

            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetPartitionDataForCategories()
        {
            var url = "partition_data/by_category?" +
                "profile_id=" + ProfileIds.Phof +
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019 +
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
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019 +
                "&area_code=" + AreaCodes.England +
                "&indicator_id=" + IndicatorIds.LifeExpectancyAtBirth +
                "&sex_id=" + SexIds.Male +
                "&age_id=" + AgeIds.AllAges +
                "&category_type_id=" + CategoryTypeIds.DeprivationDecileDistrictAndUA2015;
            byte[] data = GetData(url);

            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetGroupRootSummariesByProfileId()
        {
            var url = "grouproot_summaries/by_profile_id?" +
                      "profile_id=" + ProfileIds.Phof +
                      "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019;
            byte[] data = GetData(url);

            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetGroupRootSummariesByIndicatorId()
        {
            var url = "grouproot_summaries/by_indicator_id?" +
                      "indicator_ids=" + IndicatorIds.LifeExpectancyAtBirth +
                      "&profile_id=" + ProfileIds.Phof;
            byte[] data = GetData(url);

            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetPartitionTrendDataForSexes()
        {
            var url = "partition_trend_data/by_sex?" +
                "profile_id=" + ProfileIds.Phof +
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019 +
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
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019 +
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
        public void TestGetQuinaryPopulation()
        {
            byte[] data = GetData("quinary_population?" +
                "area_type_id=" + AreaTypeIds.GpPractice +
                "&area_code=" + AreaCodes.Gp_MeersbrookSheffield);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetQuinaryPopulationByIndicatorId()
        {
            byte[] data = GetData("quinary_population?" +
                "area_type_id=" + AreaTypeIds.GpPractice +
                "&area_code=" + AreaCodes.Gp_MeersbrookSheffield +
                "&profile_id=" + ProfileIds.HealthProfilesSupportingIndicators +
                "&indicator_id" + IndicatorIds.PopulationProjection);
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
                "area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019 +
                "&indicator_ids=" + IndicatorIds.AdultSmokingPrevalence2);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetAreaValues()
        {
            byte[] data = GetData("latest_data/single_indicator_for_all_areas?" +
                "group_id=" + GroupIds.Phof_HealthcarePrematureMortality +
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019 +
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
        public void TestGetGroupDataAtDataPointForSpecificArea()
        {
            byte[] data = GetData("latest_data/specific_indicators_for_single_area?" +
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019 +
                "&area_code=" + AreaCodes.CountyUa_Cumbria +
                "&indicator_ids=" + IndicatorIds.ExcessWinterDeaths
                );
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetGroupDataAtDataPoint()
        {
            byte[] data = GetData("latest_data/all_indicators_in_profile_group_for_child_areas?" +
                "group_id=" + GroupIds.Phof_HealthcarePrematureMortality +
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019 +
                "&parent_area_code=" + AreaCodes.Gor_SouthEast +
                "&profile_id=" + ProfileIds.Phof
                );
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetGroupDataByIndicatorIds()
        {
            byte[] data = GetData("latest_data/specific_indicators_for_child_areas?" +
                "&profile_id=" + ProfileIds.Phof +
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019 +
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
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019 +
                "&parent_area_code=" + AreaCodes.Gor_SouthEast +
                "&profile_id=" + ProfileIds.Phof
                );
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetTrendDataByIndicatorIds()
        {
            byte[] data = GetData("trend_data/specific_indicators_for_child_areas?" +
                "&profile_id=" + ProfileIds.Phof +
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019 +
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
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019 +
                "&area_codes=" + AreaCodes.Gor_SouthEast +
                "&comparator_area_codes=" + AreaCodes.England);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetProfileGroupRoots()
        {
            byte[] data = GetData("profile_group_roots?" +
                "group_id=" + GroupIds.Phof_HealthProtection +
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetIndicatorsThatMatchText()
        {
            byte[] data = GetData("indicator_search?search_text=hip");

            TestHelper.IsData(data);

            // Contains PHOF ID
            TestHelper.AssertDataContainsString(data, "\"" + AreaTypeIds.DistrictAndUnitaryAuthorityPreApr2019 + "\"");
        }

        [TestMethod]
        public void TestGetAvailableDataForGrouping()
        {
            byte[] data = GetData("available_data");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetDataChanges()
        {
            byte[] data = GetData("data_changes?indicator_id=" + IndicatorIds.AdultExcessWeight);
            TestHelper.IsData(data);
        }

        public static byte[] GetData(string path)
        {
            return EndPointTestHelper.GetData(path);
        }
    }
}
