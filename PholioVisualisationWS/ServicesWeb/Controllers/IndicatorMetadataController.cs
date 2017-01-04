using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;
using PholioVisualisation.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Http;

namespace ServicesWeb.Controllers
{
    [RoutePrefix("api")]
    public class IndicatorMetadataController : BaseController
    {
        /// <summary>
        /// Gets the indicator metadata for a list of indicator IDs
        /// </summary>
        /// <param name="indicator_ids">Comma separated list of indicator IDs</param>
        /// <param name="restrict_to_profile_ids">Comma separated list of profile IDs</param>
        /// <param name="include_definition">Whether to include the definition in response [yes/no - no is default]</param>
        /// <param name="include_system_content">Whether to include system content in response [yes/no - no is default]</param>
        [HttpGet]
        [Route("indicator_metadata/by_indicator_id")]
        public Dictionary<int, IndicatorMetadata> GetIndicatorMetadata(string indicator_ids = "",
            string restrict_to_profile_ids = "", string include_definition = "", string include_system_content = "")
        {
            try
            {
                NameValueCollection nameValues = new NameValueCollection();
                nameValues.Add(DataParameters.ParameterIndicatorIds, indicator_ids);
                nameValues.Add(ParameterNames.RestrictToProfileId, restrict_to_profile_ids);
                nameValues.Add(IndicatorMetadataParameters.ParameterIncludeDefinition, include_definition);
                nameValues.Add(IndicatorMetadataParameters.ParameterIncludeSystemContent, include_system_content);

                // Redundant parameter
                nameValues.Add(ParameterNames.GroupIds, string.Empty);

                var parameters = new IndicatorMetadataParameters(nameValues);
                return new JsonBuilderIndicatorMetadata(parameters).GetIndicatorMetadatas();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets indicator metadata for one or more profile groups
        /// </summary>
        /// <param name="group_ids">Comma separated list of profile group IDs</param>
        /// <param name="include_definition">Whether to include the indicator definition in response [yes/no - no is default]</param>
        /// <param name="include_system_content">Whether to include system content in response [yes/no - no is default]</param>
        [HttpGet]
        [Route("indicator_metadata/by_group_id")]
        public Dictionary<int, IndicatorMetadata> GetIndicatorMetadata(string group_ids,
            string include_definition = "", string include_system_content = "")
        {
            try
            {
                NameValueCollection nameValues = new NameValueCollection();
                nameValues.Add(ParameterNames.GroupIds, group_ids);
                nameValues.Add(IndicatorMetadataParameters.ParameterIncludeDefinition, include_definition);
                nameValues.Add(IndicatorMetadataParameters.ParameterIncludeSystemContent, include_system_content);

                // Redundant parameters
                nameValues.Add(DataParameters.ParameterIndicatorIds, string.Empty);
                nameValues.Add(ParameterNames.RestrictToProfileId, string.Empty);

                var parameters = new IndicatorMetadataParameters(nameValues);
                return new JsonBuilderIndicatorMetadata(parameters).GetIndicatorMetadatas();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get a list of the properties that are available for indicator metadata
        /// </summary>
        [HttpGet]
        [Route("indicator_metadata_text_properties")]
        public IList<IndicatorMetadataTextProperty> GetIndicatorMetadataTextProperties()
        {
            try
            {
                return IndicatorMetadataProvider.Instance.IndicatorMetadataTextProperties
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
        /// Finds indicators for each area type that match specific words
        /// </summary>
        /// <remarks>
        /// Words can be combined with AND / OR
        /// 
        /// Returns a hash of area type IDs that each map to a list of IDs of indicators
        /// for which the metadata matches the search text.
        /// </remarks>
        /// <param name="search_text">Text to search indicator metadata with</param>
        /// <param name="restrict_to_profile_ids">Comma separated list of profile IDs (optional)</param>
        [HttpGet]
        [Route("indicator_search")]
        public IDictionary<int, List<int>> GetIndicatorsThatMatchText(string search_text,
            string restrict_to_profile_ids = "")
        {
            try
            {
                NameValueCollection nameValues = new NameValueCollection();
                nameValues.Add(SearchIndicatorsParameters.ParameterSearchText, search_text);
                nameValues.Add(ParameterNames.RestrictToProfileId, restrict_to_profile_ids);

                var parameters = new SearchIndicatorsParameters(nameValues);
                return new JsonBuilderSearchIndicators(parameters).GetAreaTypeIdToIndicatorIdsWithData();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }
    }
}