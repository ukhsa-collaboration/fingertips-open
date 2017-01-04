using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;
using PholioVisualisation.ServiceActions;
using PholioVisualisation.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Http;

namespace ServicesWeb.Controllers
{
    [RoutePrefix("api")]
    public class DataController : BaseController
    {
        /// <summary>
        /// Gets the most recent data for a profile group
        /// </summary>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="group_id">Profile group ID</param>
        /// <param name="parent_area_code">Parent area code</param>
        /// <param name="area_type_id">Area type ID</param>
        [HttpGet]
        [Route("latest_data/all_indicators_in_profile_group_for_child_areas")]
        public IList<GroupRoot> GetGroupDataAtDataPoint(int profile_id, int group_id, int area_type_id, string parent_area_code)
        {
            try
            {
                NameValueCollection nameValues = new NameValueCollection();
                nameValues.Add(ParameterNames.GroupIds, group_id.ToString());
                nameValues.Add(ParameterNames.ProfileId, profile_id.ToString());
                nameValues.Add(ParameterNames.AreaTypeId, area_type_id.ToString());
                nameValues.Add(ParameterNames.ParentAreaCode, parent_area_code);

                var parameters = new GroupDataAtDataPointParameters(nameValues);
                return new JsonBuilderGroupDataAtDataPoint(parameters).GetGroupRoots();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the most recent data for a list of indicator IDs
        /// </summary>
        /// <param name="area_type_id">Area type ID</param>
        /// <param name="indicator_ids">Comma separated list of indicator IDs</param>
        /// <param name="parent_area_code">Parent area code</param>
        /// <param name="restrict_to_profile_ids">Comma separated list of profile IDs</param>
        [HttpGet]
        [Route("latest_data/specific_indicators_for_child_areas")]
        public IList<GroupRoot> GetGroupDataAtDataPoint(int area_type_id, string parent_area_code, string indicator_ids, string restrict_to_profile_ids)
        {
            try
            {
                NameValueCollection nameValues = new NameValueCollection();
                nameValues.Add(DataParameters.ParameterIndicatorIds, indicator_ids);
                nameValues.Add(ParameterNames.RestrictToProfileId, restrict_to_profile_ids);
                nameValues.Add(ParameterNames.AreaTypeId, area_type_id.ToString());
                nameValues.Add(ParameterNames.ParentAreaCode, parent_area_code);

                var parameters = new GroupDataAtDataPointBySearchParameters(nameValues);
                return new JsonBuilderGroupDataAtDataPointBySearch(parameters).GetGroupRoots();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets data values for a group for one specific area 
        /// </summary>
        /// <remarks>
        /// Gets CoreDataSet objects for every group root in a domain
        /// </remarks>
        /// <param name="area_code">Area code</param>
        /// <param name="area_type_id">Area type ID</param>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="group_id">Group ID</param>
        [HttpGet]
        [Route("latest_data/all_indicators_in_profile_group_for_single_area")]
        public List<CoreDataSet> GetGroupDataAtDataPointOfSpecificAreas(string area_code,
            int area_type_id, int profile_id, int group_id)
        {
            IAreasReader areasReader = ReaderFactory.GetAreasReader();

            try
            {
                IArea parentArea = (area_code == AreaCodes.England)
                    ? AreaFactory.NewArea(areasReader, area_code)
                    : areasReader.GetParentAreas(area_code).First();

                GroupData data = new GroupDataAtDataPointRepository().GetGroupDataProcessed(parentArea.Code,
                    area_type_id,
                    profile_id, group_id);
                IList<GroupRoot> roots = data.GroupRoots;
                var dataForArea = new List<CoreDataSet>();

                foreach (GroupRoot groupRoot in roots)
                {
                    CoreDataSet coreData;

                    if (parentArea.IsCountry)
                    {
                        coreData = groupRoot.GetNationalGrouping().ComparatorData;
                    }
                    else
                    {
                        IList<CoreDataSet> dataList = groupRoot.Data;
                        coreData = dataList.FirstOrDefault(x => x.AreaCode == area_code);
                    }

                    dataForArea.Add(coreData);
                }
                return dataForArea;
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets a list of the core data for all the areas within a parent area
        /// </summary>
        /// <param name="group_id">Profile group ID</param>
        /// <param name="area_type_id">Area type ID</param>
        /// <param name="parent_area_code">Parent area code</param>
        /// <param name="comparator_id">Comparator ID</param>
        /// <param name="indicator_id">Indicator ID</param>
        /// <param name="sex_id">Sex ID</param>
        /// <param name="age_id">Age ID</param>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="template_profile_id">ID of the profile to use as template if accessing search results</param>
        /// <param name="data_point_offset">Offset in years, quarters or months from the most recent time point [default is 0]</param>
        [HttpGet]
        [Route("latest_data/single_indicator_for_all_areas")]
        public IList<CoreDataSet> GetAreaValues(int group_id, int area_type_id, string parent_area_code,
            int comparator_id, int indicator_id, int sex_id, int age_id,
            int profile_id = ProfileIds.Undefined, int template_profile_id = ProfileIds.Undefined,
            int data_point_offset = 0)
        {
            try
            {
                NameValueCollection nameValues = new NameValueCollection();
                nameValues.Add(ParameterNames.GroupIds, group_id.ToString());
                nameValues.Add(ParameterNames.AreaTypeId, area_type_id.ToString());
                nameValues.Add(ParameterNames.ParentAreaCode, parent_area_code);
                nameValues.Add(ParameterNames.ComparatorId, comparator_id.ToString());
                nameValues.Add(ParameterNames.IndicatorId, indicator_id.ToString());
                nameValues.Add(ParameterNames.SexId, sex_id.ToString());
                nameValues.Add(ParameterNames.AgeId, age_id.ToString());
                nameValues.Add(ParameterNames.ProfileId, profile_id.ToString());
                nameValues.Add(ParameterNames.TemplateProfileId, template_profile_id.ToString());
                nameValues.Add(IndicatorStatsParameters.ParameterDataPointOffset, data_point_offset.ToString());

                var parameters = new AreaValuesParameters(nameValues);
                return new JsonBuilderAreaValues(parameters).GetValues();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get an ordered list of indicators with core data for requested profiles groups and areas
        /// </summary>
        /// <param name="group_ids">Comma separated list of profile group IDs</param>
        /// <param name="area_type_id">Area type ID</param>
        /// <param name="area_codes">Comma separated list of area codes</param>
        /// <param name="comparator_area_codes">Comma separated list of comparator area codes</param>
        /// <param name="include_time_periods">Whether to include time periods in response [yes/no - no is default]</param>
        /// <param name="latest_data_only">Whether to include only the latest data [yes/no - no is default]</param>
        [HttpGet]
        [Route("latest_data/all_indicators_in_multiple_profile_groups_for_multiple_areas")]
        public Dictionary<int, Dictionary<string, IList<SimpleAreaData>>> GetAreaData(string group_ids, int area_type_id, string area_codes,
            string comparator_area_codes = null, string include_time_periods = null, string latest_data_only = null)
        {
            try
            {
                NameValueCollection nameValues = new NameValueCollection();
                nameValues.Add(ParameterNames.GroupIds, group_ids);
                nameValues.Add(ParameterNames.AreaTypeId, area_type_id.ToString());
                nameValues.Add(ParameterNames.AreaCode, area_codes);
                nameValues.Add(AreaDataParameters.ParameterComparatorAreaCodes, comparator_area_codes);
                nameValues.Add(AreaDataParameters.ParameterIncludeTimePeriods, include_time_periods);
                nameValues.Add(AreaDataParameters.ParameterLatestDataOnly, latest_data_only);

                var parameters = new AreaDataParameters(nameValues);
                return new JsonBuilderAreaData(parameters).GetAreaData();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the recent trends for every area under a parent area.
        /// </summary>
        /// <param name="parent_area_code">Parent area code</param>
        /// <param name="group_id">Profile group ID</param>
        /// <param name="area_type_id">Area type ID</param>
        /// <param name="indicator_id">Indicator ID</param>
        /// <param name="sex_id">Sex ID</param>
        /// <param name="age_id">Age ID</param>
        [HttpGet]
        [Route("recent_trends/for_child_areas")]
        public Dictionary<string, TrendMarkerResult> GetTrendMarkers(string parent_area_code, int group_id,
            int area_type_id, int indicator_id, int sex_id, int age_id)
        {
            try
            {
                // Create dependencies
                var groupDataReader = ReaderFactory.GetGroupDataReader();
                var trendMarkersProvider = new TrendMarkersProvider(ReaderFactory.GetTrendDataReader(), new TrendMarkerCalculator());
                var areaListProvider = new FilteredChildAreaListProvider(ReaderFactory.GetAreasReader());
                var groupIdProvider = new GroupIdProvider(ReaderFactory.GetProfileReader());
                var singleGroupingProvider = new SingleGroupingProvider(groupDataReader, groupIdProvider);
                var groupMetadataList = groupDataReader.GetGroupMetadataList(new List<int> { group_id });
                var profileId = groupMetadataList.First().ProfileId;

                return new TrendMarkersAction(areaListProvider, trendMarkersProvider, singleGroupingProvider)
                    .GetTrendMarkers(parent_area_code, profileId, group_id, area_type_id, indicator_id, sex_id, age_id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the trend data for a profile group
        /// </summary>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="group_id">Profile group ID</param>
        /// <param name="parent_area_code">Parent area code</param>
        /// <param name="area_type_id">Area type ID</param>
        [HttpGet]
        [Route("trend_data/all_indicators_in_profile_group_for_child_areas")]
        public IList<TrendRoot> GetTrendData(int profile_id, int group_id, int area_type_id, string parent_area_code)
        {
            try
            {
                NameValueCollection nameValues = new NameValueCollection();
                nameValues.Add(ParameterNames.GroupIds, group_id.ToString());
                nameValues.Add(ParameterNames.ProfileId, profile_id.ToString());
                nameValues.Add(ParameterNames.AreaTypeId, area_type_id.ToString());
                nameValues.Add(ParameterNames.ParentAreaCode, parent_area_code);

                var parameters = new TrendDataParameters(nameValues);
                return new JsonBuilderTrendData(parameters).GetTrendData();

            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the trend data for a list of indicator IDs
        /// </summary>
        /// <param name="area_type_id">Area type ID</param>
        /// <param name="indicator_ids">Comma separated list of indicator IDs</param>
        /// <param name="parent_area_code">Parent area code</param>
        /// <param name="restrict_to_profile_ids">Comma separated list of profile IDs</param>
        [HttpGet]
        [Route("trend_data/specific_indicators_for_child_areas")]
        public IList<TrendRoot> GetTrendData(int area_type_id, string parent_area_code, string indicator_ids, string restrict_to_profile_ids)
        {
            try
            {
                NameValueCollection nameValues = new NameValueCollection();
                nameValues.Add(DataParameters.ParameterIndicatorIds, indicator_ids);
                nameValues.Add(ParameterNames.RestrictToProfileId, restrict_to_profile_ids);
                nameValues.Add(ParameterNames.AreaTypeId, area_type_id.ToString());
                nameValues.Add(ParameterNames.ParentAreaCode, parent_area_code);

                var parameters = new TrendDataBySearchParameters(nameValues);
                return new JsonBuilderTrendDataBySearch(parameters).GetTrendData();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        ///     Gets all the most recently available category data
        /// </summary>
        /// <remarks>
        /// Used in the inequality tab of Fingertips
        /// </remarks>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="area_code">Area code</param>
        /// <param name="indicator_id">Indicator ID</param>
        /// <param name="sex_id">Sex ID</param>
        /// <param name="age_id">Age ID</param>
        /// <param name="area_type_id">Area type ID</param>
        [HttpGet]
        [Route("partition_data/by_category")]
        public PartitionDataForAllCategories GetMostRecentDataForAllCategories(int profile_id,
            string area_code, int indicator_id, int sex_id, int age_id, int area_type_id)
        {
            try
            {
                return new PartitionDataForAllCategoriesBuilder().GetPartitionData(profile_id, area_code,
                    indicator_id, sex_id, age_id, area_type_id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        ///     Gets all the most recently available data for all ages
        /// </summary>
        /// <remarks>
        /// Used in the inequality tab of Fingertips
        /// </remarks>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="area_code">Area code</param>
        /// <param name="indicator_id">Indicator ID</param>
        /// <param name="sex_id">Sex ID</param>
        /// <param name="area_type_id">Area type ID</param>
        [HttpGet]
        [Route("partition_data/by_age")]
        public PartitionDataForAllAges GetMostRecentDataForAllAges(int profile_id,
            string area_code, int indicator_id, int sex_id, int area_type_id)
        {
            try
            {
                return new PartitionDataForAllAgesBuilder().GetPartitionData(profile_id, area_code,
                    indicator_id, sex_id, area_type_id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        ///     Gets all the most recently available data for all sexes
        /// </summary>
        /// <remarks>
        /// Used in the inequality tab of Fingertips
        /// </remarks>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="area_code">Area code</param>
        /// <param name="indicator_id">Indicator ID</param>
        /// <param name="age_id">Age ID</param>
        /// <param name="area_type_id">Area type ID</param>
        [HttpGet]
        [Route("partition_data/by_sex")]
        public PartitionDataForAllSexes GetMostRecentDataForAllSexes(int profile_id,
            string area_code, int indicator_id, int age_id, int area_type_id)
        {
            try
            {
                return new PartitionDataForAllSexesBuilder().GetPartitionData(profile_id, area_code,
                    indicator_id, age_id, area_type_id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get trend data partitioned by age
        /// </summary>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="area_code">Area code</param>
        /// <param name="indicator_id">Indicator ID</param>
        /// <param name="sex_id">Sex ID</param>
        /// <param name="area_type_id">Area type ID</param>
        [HttpGet]
        [Route("partition_trend_data/by_age")]
        public PartitionTrendData TrendDataForInequalitiesByAge(int profile_id,
            string area_code, int indicator_id, int sex_id, int area_type_id)
        {
            try
            {
                return new PartitionDataForAllAgesBuilder().GetPartitionTrendData(
                    profile_id, area_code, indicator_id, sex_id, area_type_id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get trend data partitioned by sex
        /// </summary>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="area_code">Area code</param>
        /// <param name="indicator_id">Indicator ID</param>
        /// <param name="age_id">Age ID</param>
        /// <param name="area_type_id">Area type ID</param>
        [HttpGet]
        [Route("partition_trend_data/by_sex")]
        public PartitionTrendData TrendDataForInequalitiesBySex(int profile_id,
            string area_code, int indicator_id, int age_id, int area_type_id)
        {
            try
            {
                return new PartitionDataForAllSexesBuilder().GetPartitionTrendData(
                    profile_id, area_code, indicator_id, age_id, area_type_id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get trend data partitioned by category
        /// </summary>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="area_code">Area code</param>
        /// <param name="indicator_id">Indicator ID</param>
        /// <param name="age_id">Age ID</param>
        /// <param name="sex_id">Sex ID</param>
        /// <param name="category_type_id">Category type ID</param>
        /// <param name="area_type_id">Area type ID</param>
        [HttpGet]
        [Route("partition_trend_data/by_category")]
        public PartitionTrendData TrendDataForInequalitiesByCategory(int profile_id,
            string area_code, int indicator_id, int age_id, int sex_id, int category_type_id, int area_type_id)
        {
            try
            {
                return new PartitionDataForAllCategoriesBuilder().GetPartitionTrendData(profile_id,
                    area_code, indicator_id, age_id, sex_id, category_type_id, area_type_id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets a list of all the indicators within a profile
        /// </summary>
        /// <remarks>
        /// The indicators are differentiated by age and sex where appropriate. Returns a list of group roots.
        /// </remarks>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="area_type_id">Area type ID</param>
        [HttpGet]
        [Route("grouproot_summaries")]
        public IList<GroupRootSummary> GetGroupDataForProfile(int profile_id, int area_type_id)
        {
            try
            {
                var summaries = new GroupRootSummaryBuilder().Build(profile_id, area_type_id)
                    .ToList();
                summaries.Sort();
                return summaries;
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        ///     Get the confidence limits for a funnel plot
        /// </summary>
        /// <param name="comparator_value">Comparator value</param>
        /// <param name="population_min">Minimum population value</param>
        /// <param name="population_max">Maximum population value</param>
        /// <param name="unit_value">Unit value</param>
        /// <param name="year_range">Year range</param>
        [HttpGet]
        [Route("spc_for_dsr_limits")]
        public SpcForDsrLimitsResponseObject GetSpcForDsrLimits(double comparator_value = 0,
            double population_min = 0, double population_max = 0,
            double unit_value = 0, int year_range = 0)
        {
            try
            {
                return new SpcForDsrLimitsAction().GetResponse(comparator_value,
                    population_min, population_max, unit_value, year_range);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets population data used in the Practice Profiles
        /// </summary>
        /// <param name="area_code">Area code</param>
        /// <param name="group_id">Group ID</param>
        /// <param name="data_point_offset">Time period offset from the data point (i.e. latest available time period)</param>
        [HttpGet]
        [Route("quinary_population_data")]
        public Dictionary<string, object> GetQuinaryPopulationData(string area_code,
            int group_id, int data_point_offset = 0)
        {
            try
            {
                return new QuinaryPopulationDataAction().GetResponse(area_code, group_id, data_point_offset);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets population data used in Profiles
        /// </summary>
        /// <param name="area_code">Area code</param>
        /// <param name="area_type_id">AraeType ID</param>
        /// <param name="data_point_offset">Time period offset from the data point (i.e. latest available time period)</param>
        [HttpGet]
        [Route("quinary_population")]
        public Dictionary<string, object> GetQuinaryPopulation(string area_code,int area_type_id, int data_point_offset = 0)
        {
            try
            {
                return new QuinaryPopulationDataAction().GetPopulationResponse(area_code, area_type_id, data_point_offset);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }
        [HttpGet]
        [Route("quinary_population_summary")]
        public Dictionary<string, object> GetQuinaryPopulationSummary(string area_code, int area_type_id, int data_point_offset = 0)
        {
            try
            {
                return new QuinaryPopulationDataAction().GetPopulationSummaryResponse(area_code, area_type_id, data_point_offset);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }
        /// <summary>
        /// Gets a list of minimum and maximum value limits for a group
        /// </summary>
        /// <remarks>
        ///  Useful for setting limits on charts.
        /// </remarks>
        /// <param name="group_id">Group ID</param>
        /// <param name="area_type_id">Area type ID</param>
        /// <param name="parent_area_code">Parent area code</param>
        [HttpGet]
        [Route("value_limits")]
        public IList<Limits> GetValueLimits(int group_id = 0, int area_type_id = 0,
            string parent_area_code = null)
        {
            try
            {
                return new ValueLimitsAction().GetResponse(group_id, area_type_id, parent_area_code);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets a formatted time period
        /// </summary>
        /// <param name="year">Year</param>
        /// <param name="quarter">Quarter</param>
        /// <param name="month">Month</param>
        /// <param name="year_range">Year range</param>
        /// <param name="year_type_id">Year type ID</param>
        [HttpGet]
        [Route("time_period")]
        public string GetTimePeriod(int year, int quarter, int month, int year_range, int year_type_id)
        {
            try
            {
                return TimePeriodFormatter.GetTimePeriodString(new TimePeriod
                {
                    Month = month,
                    Quarter = quarter,
                    Year = year,
                    YearRange = year_range
                }, year_type_id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets an ordered list of profile group roots for a profile
        /// </summary>
        /// <remarks>
        /// Group roots are returned without data. This service is used for find out the order of indicators within a group.
        /// </remarks>
        /// <param name="group_id">Profile group ID</param>
        /// <param name="area_type_id">Area type ID</param>
        [HttpGet]
        [Route("profile_group_roots")]
        public IList<GroupRoot> GetGroupRoots(int group_id, int area_type_id)
        {
            try
            {
                NameValueCollection nameValues = new NameValueCollection();
                nameValues.Add(ParameterNames.GroupIds, group_id.ToString());
                nameValues.Add(ParameterNames.AreaTypeId, area_type_id.ToString());

                var parameters = new GroupRootsParameters(nameValues);
                return new JsonBuilderGroupRoots(parameters).GetGroupRoots();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets statistics for all the indicators in a profile group
        /// </summary>
        /// <param name="group_id">Profile group ID</param>
        /// <param name="child_area_type_id">Child area type ID</param>
        /// <param name="parent_area_code">Parent area code</param>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="indicator_ids">Comma separated list of indicator IDs</param>
        /// <param name="restrict_to_profile_ids">Comma separated list of profile IDs</param>
        /// <param name="data_point_offset">Offset in years, quarters or months from the most recent time point [default is 0]</param>
        [HttpGet]
        [Route("indicator_statistics")]
        public Dictionary<int, IndicatorStatsResponse> GetIndicatorStatistics(int group_id, int child_area_type_id, string parent_area_code,
            int? profile_id = null, string indicator_ids = null, string restrict_to_profile_ids = null, int data_point_offset = 0)
        {
            try
            {
                NameValueCollection nameValues = new NameValueCollection();
                nameValues.Add(ParameterNames.GroupIds, group_id.ToString());
                nameValues.Add(ParameterNames.ProfileId, profile_id.ToString());
                nameValues.Add(ParameterNames.AreaTypeId, child_area_type_id.ToString());
                nameValues.Add(ParameterNames.ParentAreaCode, parent_area_code);
                nameValues.Add(DataParameters.ParameterIndicatorIds, indicator_ids);
                nameValues.Add(ParameterNames.RestrictToProfileId, restrict_to_profile_ids);
                nameValues.Add(IndicatorStatsParameters.ParameterDataPointOffset, data_point_offset.ToString());

                var parameters = new IndicatorStatsParameters(nameValues);
                return new JsonBuilderIndicatorStats(parameters).GetIndicatorStats();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }
    }
}