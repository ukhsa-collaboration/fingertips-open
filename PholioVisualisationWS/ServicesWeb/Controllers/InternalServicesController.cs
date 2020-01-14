using PholioVisualisation.DataAccess;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServiceActions;
using PholioVisualisation.Services;
using PholioVisualisation.UserData.IndicatorLists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace PholioVisualisation.ServicesWeb.Controllers
{
    /// <summary>
    /// Services that are used by Fingertips or Longer Lives but should not be included
    /// in the public API.
    /// </summary>
    [RoutePrefix("api")]
    public class InternalServicesController : BaseController
    {
        /// <summary>
        /// Gets the blue / green status of all deployment components
        /// </summary>
        [HttpGet]
        [Route("internal/blue-green-status")]
        public DeploymentStatus GetBlueGreenStatus()
        {
            try
            {
                if (ApplicationConfiguration.Instance.IsEnvironmentLive)
                {
                    return new DeploymentStatusProvider(ApplicationConfiguration.Instance,
                        new ConnectionStringsWrapper(), new DatabaseLogRepository()).GetDeploymentStatus();
                }
                return null;
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        ///     Gets Longer Lives area details for a specific profile
        /// </summary>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="group_id">Group ID</param>
        /// <param name="child_area_type_id">Child area type ID</param>
        /// <param name="area_code">Area code</param>
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

        #region 
        /// <summary>
        /// Gets all the indicator IDs of an indicator list for each area type
        /// </summary>
        /// <param name="indicator_list_id">Indicator list ID</param>
        [HttpGet]
        [Route("indicator-list/indicators-for-each-area-type")]
        public object GetIndicatorsByAreaTypeForIndicatorList(string indicator_list_id)
        {
            try
            {
                // Init dependencies
                var groupDataReader = ReaderFactory.GetGroupDataReader();
                var profileReader = ReaderFactory.GetProfileReader();
                var areaTypeListProvider =
                    new AreaTypeListProvider(new GroupIdProvider(profileReader), ReaderFactory.GetAreasReader(), groupDataReader);
                var groupingListProvider = new GroupingListProvider(groupDataReader, profileReader);
                var indicatorsWithDataByAreaTypeBuilder = new IndicatorsWithDataByAreaTypeBuilder(groupDataReader, groupingListProvider);

                // Get data
                var indicatorIds = new IndicatorListRepository().GetIndicatorIdsInIndicatorList(indicator_list_id);
                var profileIds = profileReader.GetAllProfileIds();
                var areaTypes = areaTypeListProvider.GetSupportedAreaTypes();

                return indicatorsWithDataByAreaTypeBuilder.GetDictionaryOfAreaTypeToIndicatorIds(areaTypes, indicatorIds, profileIds);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets an indicator list
        /// </summary>
        /// <param name="id">Indicator list ID</param>
        [HttpGet]
        [Route("indicator-list")]
        public object GetIndicatorList(string id)
        {
            try
            {
                var pholioReader = ReaderFactory.GetPholioReader();

                var listVm = new IndicatorListProvider(new IndicatorListRepository(),
                    pholioReader, new IndicatorMetadataRepository())
                    .GetIndicatorList(id);
                return listVm;
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the most recent data for an indicator list
        /// </summary>
        /// <param name="indicator_list_id">Indicator list ID</param>
        /// <param name="area_type_id">Area type ID</param>
        /// <param name="parent_area_code">Parent area code</param>
        [HttpGet]
        [System.Web.Http.Route("latest_data/indicator_list_for_child_areas")]
        public IList<GroupRoot> GetGroupDataAtDataPointForIndicatorList(string indicator_list_id,
            int area_type_id, string parent_area_code)
        {
            try
            {
                var listId = indicator_list_id;

                var comparatorMap = DataController.GetComparatorMapForParentArea(area_type_id, parent_area_code);

                var singleGroupingProvider = new SingleGroupingProvider(ReaderFactory.GetGroupDataReader(), null);
                var builder = new GroupDataBuilderByIndicatorList(new IndicatorListRepository(), singleGroupingProvider)
                {
                    ProfileId = ProfileIds.Search,
                    IndicatorListPublicId = listId,
                    ComparatorMap = comparatorMap,
                    ChildAreaTypeId = area_type_id
                };

                var roots = new JsonBuilderGroupDataAtDataPointBySearch(builder).GetGroupRoots();

                // User may have selected from multiple age/sex categories so need to display 
                foreach (var groupRoot in roots)
                {
                    groupRoot.StateSex = true;
                    groupRoot.StateAge = true;
                }

                return roots;
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        #endregion

        /// <summary>
        /// Get a list of the properties that are available for indicator metadata
        /// </summary>
        /// <param name="include_internal_metadata">A boolean flag to state whether to include internal metadata</param>
        /// <returns></returns>
        [HttpGet]
        [Route("internal/indicator_metadata_text_properties")]
        public IList<IndicatorMetadataTextProperty> GetIndicatorMetadataTextProperties(bool include_internal_metadata)
        {
            try
            {
                var indicatorMetadataTextProperties = include_internal_metadata
                    ? IndicatorMetadataProvider.Instance.IndicatorMetadataTextPropertiesIncludingInternalMetadata
                    : IndicatorMetadataProvider.Instance.IndicatorMetadataTextProperties;

                return indicatorMetadataTextProperties.OrderBy(x => x.DisplayOrder).ToList();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

    }
}