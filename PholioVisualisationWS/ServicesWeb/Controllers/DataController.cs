using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServiceActions;
using ServicesWeb.Common;

namespace ServicesWeb.Controllers
{
    [RoutePrefix("data")]
    public class DataController : BaseController
    {
        [HttpGet]
        [Route("value_notes")]
        public IList<ValueNote> GetValueNotes()
        {
            try
            {
                return ReaderFactory.GetPholioReader().GetValueNotes();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        ///     Get the confidence limits for a funnel plot.
        /// </summary>
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
        ///     Get a list the area types that used in a list of profiles.
        /// </summary>
        [HttpGet]
        [Route("area_types")]
        public IList<AreaType> GetAreaTypes(string profile_ids = null)
        {
            try
            {
                return new AreaTypesAction().GetResponse(
                    new IntListStringParser(profile_ids).IntList);
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
        ///     Get a list the areas covered by specific area type ID.
        /// </summary>
        [HttpGet]
        [Route("areas")]
        public IList<IArea> GetAreasOfAreaType(int area_type_id = AreaTypeIds.Undefined,
            int profile_id = ProfileIds.Undefined, string parent_area_code = null,
            int template_profile_id = ProfileIds.Undefined,
            string retrieve_ignored_areas = null, string area_codes = null)
        {
            try
            {
                if (area_codes != null)
                {
                    var codes = new StringListParser(area_codes).StringList;
                    var areaListBuilder = new AreaListBuilder(ReaderFactory.GetAreasReader());
                    areaListBuilder.CreateAreaListFromAreaCodes(codes);
                    return areaListBuilder.Areas;
                }

                if (parent_area_code != null)
                {
                    return GetChildAreas(area_type_id, parent_area_code, profile_id);
                }

                // Get all areas of an area type
                var retrieveIgnoredAreas = ServiceHelper.ParseYesOrNo(retrieve_ignored_areas, false);

                return new AreasOfAreaTypeAction().GetResponse(area_type_id, profile_id,
                    template_profile_id, retrieveIgnoredAreas);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        ///     Get a list the areas covered by specific area type ID.
        /// </summary>
        public IList<IArea> GetChildAreas(int area_type_id, string parent_area_code, 
            int profile_id = ProfileIds.Undefined)
        {
            IAreasReader reader = ReaderFactory.GetAreasReader();

            // Get child areas
            IList<IArea> areas = new ChildAreaListBuilder(reader, parent_area_code, area_type_id)
                .ChildAreas;

            // Remove areas that should be ignored for the profile
            areas = IgnoredAreasFilterFactory.New(profile_id)
                .RemoveAreasIgnoredEverywhere(areas)
                .ToList();

            return areas;
        }

        /// <summary>
        ///     Returns list of GP practices within a parent area
        /// </summary>
        [HttpGet]
        [Route("areas_with_addresses")]
        public IList<AreaAddress> GetChildAreasWithAddresses(string parent_area_code, int area_type_id)
        {
            try
            {
                return new ChildAreasWithAddressesBuilder().Build(parent_area_code, area_type_id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        ///     Get a list of min and max value limits for a group. Useful for setting limits on charts.
        /// </summary>
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
        /// Gets a formatted time period.
        /// </summary>
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

        [HttpGet]
        [Route("nhs_choices_area_id")]
        public string GetNhsChoicesAreaId(string area_code)
        {
            try
            {
                return ReaderFactory.GetAreasReader().GetNhsChoicesAreaId(area_code);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        [HttpGet]
        [Route("chimat_resource_id")]
        public int GetChimatResourceId(string area_code)
        {
            try
            {
                return ReaderFactory.GetAreasReader().GetChimatResourceId(area_code);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        [HttpGet]
        [Route("indicator_metadata_text_properties")]
        public IList<IndicatorMetadataTextProperty> GetIndicatorMetadataTextProperties()
        {
            try
            {
                return IndicatorMetadataRepository.Instance.IndicatorMetadataTextProperties
                    .OrderBy(x => x.DisplayOrder)
                    .ToList();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        ///     Gets all the most recently available category data.
        /// </summary>
        [HttpGet]
        [Route("most_recent_data_for_all_categories")]
        public MostRecentDataForAllCategories GetMostRecentDataForAllCategories(int profile_id = 0,
            string area_code = null, int indicator_id = 0, int sex_id = 0, int age_id = 0, int area_type_id = 0)
        {
            try
            {
                return new MostRecentDataForAllCategoriesAction().GetResponse(profile_id, area_code,
                    indicator_id, sex_id, age_id, area_type_id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        ///     Gets all the most recently available data for all ages.
        /// </summary>
        [HttpGet]
        [Route("most_recent_data_for_all_ages")]
        public MostRecentDataForAllAges GetMostRecentDataForAllAges(int profile_id = 0,
            string area_code = null, int indicator_id = 0, int sex_id = 0, int area_type_id = 0)
        {
            try
            {
                return new MostRecentDataForAllAgesAction().GetResponse(profile_id, area_code,
                    indicator_id, sex_id, area_type_id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        ///     Gets all the most recently available data for all sexes.
        /// </summary>
        [HttpGet]
        [Route("most_recent_data_for_all_sexes")]
        public MostRecentDataForAllSexes GetMostRecentDataForAllSexes(int profile_id = 0,
            string area_code = null, int indicator_id = 0, int age_id = 0, int area_type_id = 0)
        {
            try
            {
                return new MostRecentDataForAllSexesAction().GetResponse(profile_id, area_code,
                    indicator_id, age_id, area_type_id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        ///     Gets area details bespoke for a specific profile.
        /// </summary>
        [HttpGet]
        [Route("area_details")]
        public object GetAreaDetails(int profile_id = 0, int group_id = 0,
            int child_area_type_id = 0, string area_code = null)
        {
            try
            {
                return new AreaDetailsAction().GetResponse(profile_id, group_id, child_area_type_id, area_code);
            }
            catch (Exception ex)
            {
                Log(ex);

                throw;
            }
        }

        /// <summary>
        ///     Whether or not PDF reports can be generated for a profile.
        /// </summary>
        [HttpGet]
        [Route("are_pdfs")]
        public bool ArePdfs(int profile_id = 0, int area_type_id = 0)
        {
            try
            {
                return ReaderFactory.GetProfileReader().CanPdfBeGenerated(profile_id, area_type_id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        ///     Returns list of GP practices with in 30 miles radius
        /// </summary>
        [HttpGet]
        [Route("nearby_areas")]
        public IList<NearByAreas> GetNearbyAreas(string easting, string northing, int area_type_id)
        {
            try
            {
                return new NearbyAreasBuilder().Build(easting, northing, area_type_id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the parent area types of all the child areas of a parent area.
        /// e.g. the Shape of all the practices within a CCG
        /// </summary>
        [HttpGet]
        [Route("parent_area_of_specific_type_for_child_areas")]
        public Dictionary<string, IArea> GetParentAreasOfChildAreas(string parent_area_code,
            int child_area_type_id, int parent_area_type_id)
        {
            try
            {
                return new ParentAreasOfChildAreasBuilder().GetAreaMapping(parent_area_code,
                    child_area_type_id, parent_area_type_id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        [HttpGet]
        [Route("group_data_at_data_point_of_specific_areas")]
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

        [HttpGet]
        [Route("content")]
        public string GetContent(int profile_id, string key)
        {
            try
            {
                var reader = ReaderFactory.GetContentReader();
                ContentItem data = reader.GetContentForProfile(profile_id, key);
                return data != null ? data.Content : string.Empty;
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

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

        [HttpGet]
        [Route("profiles_containing_indicators")]
        public Dictionary<int, List<ProfilePerIndicator>> GetProfilesPerIndicator(string indicator_ids, int area_type_id)
        {
            try
            {
                var response = new ProfilePerIndicatorBuilder(ApplicationConfiguration.IsEnvironmentLive)
                    .Build(new IntListStringParser(indicator_ids).IntList, area_type_id);
                return response;
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }            
        }
    }
}