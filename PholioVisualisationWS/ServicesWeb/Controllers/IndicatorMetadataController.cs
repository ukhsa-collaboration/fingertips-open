using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;
using PholioVisualisation.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.Export;
using PholioVisualisation.Export.File;
using PholioVisualisation.Parsers;
using PholioVisualisation.SearchQuerying;
using PholioVisualisation.ServiceActions;
using PholioVisualisation.ServicesWeb.Helpers;

namespace PholioVisualisation.ServicesWeb.Controllers
{
    [RoutePrefix("api")]
    public class IndicatorMetadataController : BaseController
    {
        /// <summary>
        /// Gets the indicator metadata for a list of indicator IDs
        /// </summary>
        /// <param name="indicator_ids">Comma separated list of indicator IDs</param>
        /// <param name="restrict_to_profile_ids">Comma separated list of profile IDs [optional]</param>
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
                nameValues.Add(IndicatorMetadataParameters.ParameterIncludeDefinition, include_definition);
                nameValues.Add(IndicatorMetadataParameters.ParameterIncludeSystemContent, include_system_content);

                // Redundant Parameter
                nameValues.Add(ParameterNames.GroupIds, string.Empty);

                var parameters = new IndicatorMetadataParameters(nameValues);
                parameters.RestrictResultsToProfileIds = GetProfileIds(restrict_to_profile_ids);

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
        /// Gets indicator metadata for all the indicators available
        /// </summary>
        /// <param name="include_definition">Whether to include the indicator definition in response [yes/no - no is default]</param>
        /// <param name="include_system_content">Whether to include system content in response [yes/no - no is default]</param>
        [HttpGet]
        [Route("indicator_metadata/all")]
        public Dictionary<int, IndicatorMetadata> GetAllIndicatorMetadata(string include_definition = "",
            string include_system_content = "")
        {
            try
            {
                NameValueCollection nameValues = new NameValueCollection();
                nameValues.Add(IndicatorMetadataParameters.ParameterIncludeDefinition, include_definition);
                nameValues.Add(IndicatorMetadataParameters.ParameterIncludeSystemContent, include_system_content);

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
        /// <param name="restrict_to_profile_ids">Comma separated list of profile IDs [optional]</param>
        [HttpGet]
        [Route("indicator_search")]
        public IDictionary<int, List<int>> GetIndicatorsThatMatchText(string search_text,
            string restrict_to_profile_ids = "")
        {
            try
            {
                var profileIds = GetProfileIds(restrict_to_profile_ids);

                // Init dependencies
                var groupDataReader = ReaderFactory.GetGroupDataReader();
                var profileReader = ReaderFactory.GetProfileReader();
                var areaTypeListProvider =
                    new AreaTypeListProvider(new GroupIdProvider(profileReader), ReaderFactory.GetAreasReader(), groupDataReader);
                var groupingListProvider = new GroupingListProvider(groupDataReader, profileReader);
                var indicatorsWithDataByAreaTypeBuilder = new IndicatorsWithDataByAreaTypeBuilder(groupDataReader, groupingListProvider);
                var indicatorFilter = new IndicatorKnowledgeFilter(new IndicatorMetadataRepository());

                return new IndicatorSearchAction(areaTypeListProvider, groupDataReader, indicatorsWithDataByAreaTypeBuilder,
                    indicatorFilter).GetAreaTypeIdToIndicatorIdsWithData(search_text, profileIds);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets a CSV file of metadata for the specified indicators.
        /// [Note: this services returns data in CSV not JSON format so the response will not be viewable on this page]
        /// </summary>
        /// <param name="indicator_ids">Comma-separated list of indicator IDs</param>
        /// <param name="profile_id">Profile ID [optional]</param>
        [HttpGet]
        [Route("indicator_metadata/csv/by_indicator_id")]
        public HttpResponseMessage GetIndicatorMetadataFileForIndicatorList(string indicator_ids, int? profile_id = null)
        {
            try
            {
                var indicatorIds = new IntListStringParser(indicator_ids).IntList;

                var fileBuilder = GetIndicatorMetadataFileBuilder();
                var fileContent = fileBuilder.GetFileForSpecifiedIndicators(indicatorIds, profile_id);

                if (fileContent != null)
                {
                    var filename = SingleEntityFileNamer.IndicatorMetadataFilenameForUser;
                    return FileResponseBuilder.NewMessage(fileContent, filename);
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
        /// Gets a CSV file of metadata for all the indicators in the specified profile group    
        /// [Note: this services returns data in CSV not JSON format so the response will not be viewable on this page]
        /// </summary>
        /// <param name="group_id">Profile group ID</param>
        [HttpGet]
        [Route("indicator_metadata/csv/by_group_id")]
        public HttpResponseMessage GetIndicatorMetadataFileByProfileGroup(int group_id)
        {
            try
            {
                var groupingMetadata = ReaderFactory.GetGroupDataReader()
                    .GetGroupingMetadataList(new List<int> { group_id })
                    .FirstOrDefault();

                if (groupingMetadata != null)
                {
                    var fileContent = GetIndicatorMetadataFileBuilder()
                        .GetFileForGroups(new List<int> { group_id });
                    if (fileContent != null)
                    {
                        var filename = new SingleEntityFileNamer(groupingMetadata.Name).MetadataFileName;
                        return FileResponseBuilder.NewMessage(fileContent, filename);
                    }
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
        /// Gets a CSV file of metadata for all the indicators in the specified profile    
        /// [Note: this services returns data in CSV not JSON format so the response will not be viewable on this page]
        /// </summary>
        /// <param name="profile_id">Profile ID</param>
        [HttpGet]
        [Route("indicator_metadata/csv/by_profile_id")]
        public HttpResponseMessage GetIndicatorMetadataFileByProfile(int profile_id)
        {
            try
            {
                var filename = SingleEntityFileNamer.GetProfileMetadataFileNameForUser(profile_id);
                var fileManager = new ExportFileManager(filename);

                // Check whether file is already cached
                byte[] content = fileManager.TryGetFile();
                if (content != null)
                {
                    return FileResponseBuilder.NewMessage(content, filename);
                }

                // Create new file
                var groupIds = new GroupIdProvider(ReaderFactory.GetProfileReader()).GetGroupIds(profile_id);
                var fileContent = GetIndicatorMetadataFileBuilder()
                    .GetFileForGroups(groupIds);
                if (fileContent != null)
                {
                    // Save file to cache
                    fileManager.SaveFile(fileContent);
                    return FileResponseBuilder.NewMessage(fileContent, filename);
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
        /// Gets a CSV file of metadata for all the indicators available
        /// [Note: this services returns data in CSV not JSON format so the response will not be viewable on this page]
        /// </summary>
        [HttpGet]
        [Route("indicator_metadata/csv/all")]
        public HttpResponseMessage GetIndicatorMetadataFileForAllIndicators()
        {
            try
            {
                var filename = SingleEntityFileNamer.GetAllMetadataFileNameForUser();
                var fileManager = new ExportFileManager(filename);

                // Check whether file is already cached
                byte[] content = fileManager.TryGetFile();
                if (content != null)
                {
                    return FileResponseBuilder.NewMessage(content, filename);
                }

                var indicatorIds = ReaderFactory.GetGroupDataReader().GetAllIndicatorIds();

                var fileBuilder = GetIndicatorMetadataFileBuilder();
                var fileContent = fileBuilder.GetFileForSpecifiedIndicators(indicatorIds, null);

                if (fileContent != null)
                {
                    // Save file to cache
                    fileManager.SaveFile(fileContent);
                    return FileResponseBuilder.NewMessage(fileContent, filename);
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
        /// Get a list of the indicators in the specified profile groups
        /// </summary>
        /// <param name="group_ids">Comma separated list of group ids</param>
        [HttpGet]
        [Route("indicator_names/by_group_id")]
        public IList<DomainIndicatorName> GetIndicatorNamesByGroupId(string group_ids)
        {
            try
            {
                var groupIds = ParseIntList(group_ids);

                var domainIndicatorNames = new List<DomainIndicatorName>();
                foreach (var groupId in groupIds)
                {
                    var groupings = ReaderFactory.GetGroupDataReader().GetGroupingsByGroupId(groupId);

                    var metadataCollection = IndicatorMetadataProvider.Instance.GetIndicatorMetadataCollection(groupings);

                    foreach (var indicatorMetadata in metadataCollection.IndicatorMetadata)
                    {
                        var domainIndicatorName = new DomainIndicatorName()
                        {
                            GroupId = groupId,
                            IndicatorId = indicatorMetadata.IndicatorId,
                            IndicatorName = indicatorMetadata.Name
                        };

                        domainIndicatorNames.Add(domainIndicatorName);
                    }
                }

                return domainIndicatorNames;
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Build this way until have DI framework
        /// </summary>
        private IndicatorMetadataFileBuilder GetIndicatorMetadataFileBuilder()
        {
            var groupingProvider = new GroupingListProvider(ReaderFactory.GetGroupDataReader(),
                ReaderFactory.GetProfileReader());
            var polarities = ReaderFactory.GetPholioReader().GetAllPolarities();
            return new IndicatorMetadataFileBuilder(IndicatorMetadataProvider.Instance, groupingProvider, polarities);
        }
    }
}