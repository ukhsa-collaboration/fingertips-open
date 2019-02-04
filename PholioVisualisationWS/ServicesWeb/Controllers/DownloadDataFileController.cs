using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export;
using PholioVisualisation.Export.File;
using PholioVisualisation.Export.FileBuilder;
using PholioVisualisation.Export.FileBuilder.Wrappers;
using PholioVisualisation.Parsers;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServicesWeb.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;

namespace PholioVisualisation.ServicesWeb.Controllers
{
    /// <summary>
    /// Endpoints related with the download data file
    /// </summary>
    [RoutePrefix("api")]
    public class DownloadDataFileController : BaseController
    {
        /// <summary>
        /// The maximum number of indicators a user is allowed to download at once.
        /// </summary>
        public const int MaxIndicatorsForCsvDownload = 100;

        /// <summary>
        /// Get data for specific indicators in CSV format.
        /// [Note: this services returns data in CSV not JSON format so the response will not be viewable on this page]
        /// </summary>
        /// <param name="indicator_ids">Comma separated list of indicator IDs [Maximum 100]</param>
        /// <param name="child_area_type_id">Child area type ID</param>
        /// <param name="parent_area_type_id">Parent area type ID</param>
        /// <param name="profile_id">Profile ID [optional]</param>
        /// <param name="parent_area_code">The parent area code [default is England]</param>
        [HttpGet]
        [Route("all_data/csv/by_indicator_id")]
        public HttpResponseMessage GetDataFileForIndicatorList(string indicator_ids, int child_area_type_id,
            int parent_area_type_id, int profile_id = ProfileIds.Undefined, string parent_area_code = AreaCodes.England)
        {
            try
            {
                var indicatorIds = new IntListStringParser(indicator_ids).IntList;

                // Limit number of indicators
                if (indicatorIds.Count > MaxIndicatorsForCsvDownload)
                {
                    indicatorIds = indicatorIds.Take(MaxIndicatorsForCsvDownload).ToList();
                }

                var exportParameters = new IndicatorExportParameters
                {
                    ProfileId = profile_id,
                    ChildAreaTypeId = child_area_type_id,
                    ParentAreaTypeId = parent_area_type_id,
                    ParentAreaCode = parent_area_code
                };

                var inequalities = indicatorIds.ToDictionary<int, int, IList<Inequality>>(iid => iid, iid => null);

                var onDemandParameters = new OnDemandQueryParametersWrapper(profile_id, indicatorIds, inequalities, null, null, true);
                return GetOnDemandIndicatorDataResponse(exportParameters, onDemandParameters);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get data for all the indicators in a profile group in CSV format
        /// [Note: this services returns data in CSV not JSON format so the response will not be viewable on this page]
        /// </summary>
        /// <param name="child_area_type_id">Child area type ID</param>
        /// <param name="parent_area_type_id">Parent area type ID</param>
        /// <param name="group_id">Profile group ID</param>
        /// <param name="parent_area_code">The parent area code [default is England]</param>
        [HttpGet]
        [Route("all_data/csv/by_group_id")]
        public HttpResponseMessage GetDataFileForGroup(int child_area_type_id, int parent_area_type_id, int group_id, string parent_area_code = AreaCodes.England)
        {
            try
            {
                // Get profile ID of group
                var groupDataReader = ReaderFactory.GetGroupDataReader();
                var profileId = groupDataReader
                    .GetGroupingMetadataList(new List<int> { group_id })
                    .First()
                    .ProfileId;

                // Get indicator IDs in group
                var indicatorIds = GetIndicatorIdListProvider().GetIdsForGroup(group_id);

                var exportParameters = new IndicatorExportParameters
                {
                    ProfileId = profileId,
                    ChildAreaTypeId = child_area_type_id,
                    ParentAreaTypeId = parent_area_type_id,
                    ParentAreaCode = parent_area_code
                };

                var inequalities = indicatorIds.ToDictionary<int, int, IList<Inequality>>(iid => iid, iid => null);

                var onDemandParameters = new OnDemandQueryParametersWrapper(profileId, indicatorIds, inequalities, null, null, true);
                return GetOnDemandIndicatorDataResponse(exportParameters, onDemandParameters);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get data for all the indicators in a profile in CSV format
        /// [Note: this services returns data in CSV not JSON format so the response will not be viewable on this page]
        /// </summary>
        /// <param name="child_area_type_id">Child area type ID</param>
        /// <param name="parent_area_type_id">Parent area type ID</param>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="parent_area_code">The parent area code [default is England]</param>
        [HttpGet]
        [Route("all_data/csv/by_profile_id")]
        public HttpResponseMessage GetDataFileForProfile(int child_area_type_id, int parent_area_type_id, int profile_id, string parent_area_code = AreaCodes.England)
        {
            try
            {
                var filename = SingleEntityFileNamer.GetDataForUserbyProfileAndAreaType(profile_id, child_area_type_id);
                var fileManager = new ExportFileManager(filename);

                // Parameters
                var exportParameters = new IndicatorExportParameters
                {
                    ProfileId = profile_id,
                    ChildAreaTypeId = child_area_type_id,
                    ParentAreaTypeId = parent_area_type_id,
                    ParentAreaCode = parent_area_code
                };

                // Check whether file is already cached
                byte[] content = fileManager.TryGetFile();
                if (content == null)
                {
                    // Get indicatorIDs in profile
                    var indicatorIds = GetIndicatorIdListProvider().GetIdsForProfile(profile_id);

                    var inequalities = indicatorIds.ToDictionary<int, int, IList<Inequality>>(iid => iid, iid => null);

                    var onDemandParameters = new OnDemandQueryParametersWrapper(profile_id, indicatorIds, inequalities, null, null, true);
                    return GetOnDemandIndicatorDataResponse(exportParameters, onDemandParameters);
                }

                return FileResponseBuilder.NewMessage(content, filename);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get the latest data for all the indicators without inequalities in a profile group in CSV format
        /// [Note: this services returns data in CSV not JSON format so the response will not be viewable on this page]
        /// </summary>
        /// <param name="child_area_type_id">Child area type ID</param>
        /// <param name="parent_area_type_id">Parent area type ID</param>
        /// <param name="group_id">Profile group ID</param>
        /// <param name="areas_code">Area code list selected</param>
        /// <param name="parent_area_code">The parent area code [default is England]</param>
        [HttpGet]
        [Route("latest/no_inequalities_data/csv/by_group_id")]
        public HttpResponseMessage GetLatestDataFileForGroup(int child_area_type_id, int parent_area_type_id, int group_id, string areas_code, string parent_area_code = AreaCodes.England)
        {
            try
            {
                // Initialize child areas code
                List<string> childAreasCodeList = null;
                
                if (areas_code != null)
                {
                    childAreasCodeList = new StringListParser(areas_code).StringList;
                }
                
                // Initialize lists
                var groupIdList = new List<int> { group_id };

                // Get profile ID of group
                var groupDataReader = ReaderFactory.GetGroupDataReader();

                var profileId = groupDataReader.GetGroupingMetadataList(groupIdList).First().ProfileId;

                // Get indicator IDs in group
                var indicatorIds = GetIndicatorIdListProvider().GetIdsForGroup(group_id);

                const int categoryTypeId = -1;
                const int categoryId = -1;

                var inequalities = indicatorIds.ToDictionary<int, int, IList<Inequality>>(iid => iid, iid => new List<Inequality> {new Inequality(categoryTypeId, categoryId)});

                var onDemandParameters = new OnDemandQueryParametersWrapper(profileId, indicatorIds, inequalities, childAreasCodeList, groupIdList);

                var exportParameters = new IndicatorExportParameters
                {
                    ProfileId = profileId,
                    ChildAreaTypeId = child_area_type_id,
                    ParentAreaTypeId = parent_area_type_id,
                    ParentAreaCode = parent_area_code
                };

                return GetOnDemandIndicatorDataResponse(exportParameters, onDemandParameters);
            }
            catch (Exception ex)
            {
                Log(ex);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Get the latest data for specific indicators without inequalities in CSV format.
        /// [Note: this services returns data in CSV not JSON format so the response will not be viewable on this page]
        /// </summary>
        /// <param name="indicator_ids">Comma separated list of indicator IDs [Maximum 20]</param>
        /// <param name="child_area_type_id">Child area type ID</param>
        /// <param name="parent_area_type_id">Parent area type ID</param>
        /// <param name="areas_code">Area code list selected [Maximum 80]</param>
        /// <param name="profile_id">Profile ID [optional]</param>
        /// <param name="parent_area_code">The parent area code [default is England]</param>
        [HttpGet]
        [Route("latest/no_inequalities_data/csv/by_indicator_id")]
        public HttpResponseMessage GetLatestDataFileForIndicator(string indicator_ids, int child_area_type_id, int parent_area_type_id, string areas_code, 
                                                                 int profile_id = ProfileIds.Undefined, string parent_area_code = AreaCodes.England)
        {
            const int maxIndicatorsForCsvDownload = 20;
            const int maxChildAreasCodeList = 80;

            try
            {
                // Limit number of child areas code
                var childAreasCodeList = GetAreasCodeListAmount(areas_code, maxChildAreasCodeList);

                // Setting indicator
                var indicatorIds = new IntListStringParser(indicator_ids).IntList;

                // Limit number of indicators
                indicatorIds = GetIndicatorIdsAmount(indicatorIds, maxIndicatorsForCsvDownload);

                var exportParameters = new IndicatorExportParameters
                {
                    ProfileId = profile_id,
                    ChildAreaTypeId = child_area_type_id,
                    ParentAreaTypeId = parent_area_type_id,
                    ParentAreaCode = parent_area_code
                };

                const int categoryTypeId = -1;
                const int categoryId = -1;

                // Setting inequalities
                var inequalities = indicatorIds.ToDictionary<int, int, IList<Inequality>>(iid => iid, iid => new List<Inequality> { new Inequality(categoryTypeId, categoryId) });

                var onDemandParameters = new OnDemandQueryParametersWrapper(profile_id, indicatorIds, inequalities, childAreasCodeList);

                return GetOnDemandIndicatorDataResponse(exportParameters, onDemandParameters);
            }
            catch (Exception ex)
            {
                Log(ex);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Gets the latest data with inequalities by indicator
        /// </summary>
        /// <param name="indicator_ids">Comma separated list of indicator IDs [Maximum 20]</param>
        /// <param name="child_area_type_id">Child area type ID</param>
        /// <param name="parent_area_type_id">Parent area type ID</param>
        /// <param name="areas_code">Area code list selected [Maximum 80]</param>
        /// <param name="inequalities">Inequalities to be filtered by</param>
        /// <param name="profile_id">Profile ID [optional]</param>
        /// <param name="parent_area_code">The parent area code [default is England]</param>
        [HttpGet]
        [Route("latest/with_inequalities_data/csv/by_indicator_id")]
        public HttpResponseMessage GetLatestWithInequalitiesDataFileForIndicator(string indicator_ids, int child_area_type_id, int parent_area_type_id, string areas_code, string inequalities,
                                                                                    int profile_id = ProfileIds.Undefined, string parent_area_code = AreaCodes.England)
        {
            const int maxIndicatorsForCsvDownload = 20;
            const int maxChildAreasCodeList = 80;

            try
            {
                // Limit number of child areas code
                var childAreasCodeList = GetAreasCodeListAmount(areas_code, maxChildAreasCodeList);

                // Setting indicator
                var indicatorIds = new IntListStringParser(indicator_ids).IntList;

                // Limit number of indicators
                indicatorIds = GetIndicatorIdsAmount(indicatorIds, maxIndicatorsForCsvDownload);

                var exportParameters = new IndicatorExportParameters
                {
                    ProfileId = profile_id,
                    ChildAreaTypeId = child_area_type_id,
                    ParentAreaTypeId = parent_area_type_id,
                    ParentAreaCode = parent_area_code
                };

                var inequalitiesObject = JsonConvert.DeserializeObject<Dictionary<int, IList<Inequality>>>(inequalities);
                var onDemandParameters = new OnDemandQueryParametersWrapper(profile_id, indicatorIds, inequalitiesObject, childAreasCodeList);

                return GetOnDemandIndicatorDataResponse(exportParameters, onDemandParameters);
            }
            catch (Exception ex)
            {
                Log(ex);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Gets all periods data with inequalities by indicator
        /// </summary>
        /// <param name="indicator_ids">Comma separated list of indicator IDs [Maximum 20]</param>
        /// <param name="child_area_type_id">Child area type ID</param>
        /// <param name="parent_area_type_id">Parent area type ID</param>
        /// <param name="areas_code">Area code list selected [Maximum 80]</param>
        /// <param name="inequalities">Inequalities to be filtered by</param>
        /// <param name="profile_id">Profile ID [optional]</param>
        /// <param name="parent_area_code">The parent area code [default is England]</param>
        [HttpGet]
        [Route("allPeriods/with_inequalities_data/csv/by_indicator_id")]
        public HttpResponseMessage GetAllPeriodsWithInequalitiesDataFileForIndicator(string indicator_ids, int child_area_type_id, int parent_area_type_id, string areas_code, string inequalities,
                                                                                    int profile_id = ProfileIds.Undefined, string parent_area_code = AreaCodes.England)
        {
            const int maxIndicatorsForCsvDownload = 20;
            const int maxChildAreasCodeList = 80;

            try
            {
                // Limit number of child areas code
                var childAreasCodeList = GetAreasCodeListAmount(areas_code, maxChildAreasCodeList);

                // Setting indicator
                var indicatorIds = new IntListStringParser(indicator_ids).IntList;

                // Limit number of indicators
                indicatorIds = GetIndicatorIdsAmount(indicatorIds, maxIndicatorsForCsvDownload);

                var exportParameters = new IndicatorExportParameters
                {
                    ProfileId = profile_id,
                    ChildAreaTypeId = child_area_type_id,
                    ParentAreaTypeId = parent_area_type_id,
                    ParentAreaCode = parent_area_code
                };

                var inequalitiesObject = JsonConvert.DeserializeObject<Dictionary<int, IList<Inequality>>>(inequalities);
                var onDemandParameters = new OnDemandQueryParametersWrapper(profile_id, indicatorIds, inequalitiesObject, childAreasCodeList, null, true);

                return GetOnDemandIndicatorDataResponse(exportParameters, onDemandParameters);
            }
            catch (Exception ex)
            {
                Log(ex);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Get the latest data for population in a profile group in CSV format
        /// [Note: this services returns data in CSV not JSON format so the response will not be viewable on this page]
        /// </summary>
        /// <param name="child_area_type_id">Child area type ID</param>
        /// <param name="parent_area_type_id">Parent area type ID</param>
        /// <param name="areas_code">Area code list selected</param>
        /// <param name="parent_area_code">The parent area code [default is England]</param>
        [HttpGet]
        [Route("latest/population/csv")]
        public HttpResponseMessage GetLatestPopulationDataFile(int child_area_type_id, int parent_area_type_id, string areas_code, string parent_area_code = AreaCodes.England)
        {
            const int groupId = GroupIds.Population;
            try
            {
                // Initialize child areas code
                List<string> childAreasCodeList = null;

                if (areas_code != null)
                {
                    childAreasCodeList = new StringListParser(areas_code).StringList;
                }

                // Initialize lists
                var groupIdList = new List<int> { groupId };

                // Get profile ID of group
                var groupDataReader = ReaderFactory.GetGroupDataReader();
                var profileId = groupDataReader
                    .GetGroupingMetadataList(groupIdList)
                    .First()
                    .ProfileId;

                // Get indicator IDs in group
                var indicatorIds = GetIndicatorIdListProvider().GetIdsForGroup(groupId);

                var exportParameters = new IndicatorExportParameters
                {
                    ProfileId = profileId,
                    ChildAreaTypeId = child_area_type_id,
                    ParentAreaTypeId = parent_area_type_id,
                    ParentAreaCode = parent_area_code
                };

                var inequalities = indicatorIds.ToDictionary<int, int, IList<Inequality>>(iid => iid, iid => null);

                var onDemandParameters = new OnDemandQueryParametersWrapper(profileId, indicatorIds, inequalities, childAreasCodeList, groupIdList);

                return GetOnDemandIndicatorDataResponse(exportParameters, onDemandParameters);
            }
            catch (Exception ex)
            {
                Log(ex);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Get the all periods data for specific indicators without inequalities in CSV format.
        /// [Note: this services returns data in CSV not JSON format so the response will not be viewable on this page]
        /// </summary>
        /// <param name="indicator_ids">Comma separated list of indicator IDs [Maximum 20]</param>
        /// <param name="child_area_type_id">Child area type ID</param>
        /// <param name="parent_area_type_id">Parent area type ID</param>
        /// <param name="areas_code">Area code list selected [Maximum 80]</param>
        /// <param name="profile_id">Profile ID [optional]</param>
        /// <param name="parent_area_code">The parent area code [default is England]</param>
        [HttpGet]
        [Route("allPeriods/no_inequalities_data/csv/by_indicator_id")]
        public HttpResponseMessage GetAllPeriodDataFileByIndicator(string indicator_ids, int child_area_type_id, int parent_area_type_id, string areas_code, int profile_id = ProfileIds.Undefined,
                                                                    string parent_area_code = AreaCodes.England)
        {
            const int maxIndicatorsForCsvDownload = 20;
            const int maxChildAreasCodeList = 80;

            try
            {
                // Limit number of child areas code
                var childAreasCodeList = GetAreasCodeListAmount(areas_code, maxChildAreasCodeList);

                // Setting indicator
                var indicatorIds = new IntListStringParser(indicator_ids).IntList;

                // Limit number of indicators
                indicatorIds = GetIndicatorIdsAmount(indicatorIds, maxIndicatorsForCsvDownload);

                var exportParameters = new IndicatorExportParameters
                {
                    ProfileId = profile_id,
                    ChildAreaTypeId = child_area_type_id,
                    ParentAreaTypeId = parent_area_type_id,
                    ParentAreaCode = parent_area_code
                };

                const int categoryTypeId = -1;
                const int categoryId = -1;

                // Setting inequalities
                var inequalities = indicatorIds.ToDictionary<int, int, IList<Inequality>>(iid => iid, iid => new List<Inequality> { new Inequality(categoryTypeId, categoryId) });

                var onDemandParameters = new OnDemandQueryParametersWrapper(profile_id, indicatorIds, inequalities, childAreasCodeList, null, true);

                return GetOnDemandIndicatorDataResponse(exportParameters, onDemandParameters);
            }
            catch (Exception ex)
            {
                Log(ex);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        private static List<int> GetIndicatorIdsAmount(List<int> indicatorIds, int maxIndicatorsForCsvDownload)
        {
            return indicatorIds.Count <= maxIndicatorsForCsvDownload
                    ? indicatorIds
                    : indicatorIds.Take(maxIndicatorsForCsvDownload).ToList();
        }

        private static List<string> GetAreasCodeListAmount(string areasCode, int maxChildAreasCodeList)
        {
            if (areasCode == null) return null;

            var childAreasCodeList = new StringListParser(areasCode).StringList;

            // Limit number of child areas code
            if (childAreasCodeList.Count > maxChildAreasCodeList)
            {
                childAreasCodeList = childAreasCodeList.Take(maxChildAreasCodeList).ToList();
            }

            return childAreasCodeList;
        }

        /// <summary>
        /// Remove when have DI framework
        /// </summary>
        private static IIndicatorIdListProvider GetIndicatorIdListProvider()
        {
            var groupDataReader = ReaderFactory.GetGroupDataReader();
            var groupIdProvider = new GroupIdProvider(ReaderFactory.GetProfileReader());
            return new IndicatorIdListProvider(groupDataReader, groupIdProvider);
        }

        private static IndicatorDataCsvFileBuilder GetOnDemandDataDataFileBuilder(IndicatorExportParameters exportParameters, OnDemandQueryParametersWrapper onDemandParameters)
        {
            var areasReader = ReaderFactory.GetAreasReader();

            var areaHelper = new ExportAreaHelper(areasReader, exportParameters, new AreaFactory(areasReader));
            var fileBuilder = new IndicatorDataCsvFileBuilder(IndicatorMetadataProvider.Instance, areaHelper, areasReader, exportParameters, onDemandParameters);
            return fileBuilder;
        }

        private static HttpResponseMessage GetOnDemandIndicatorDataResponse(IndicatorExportParameters exportParameters, OnDemandQueryParametersWrapper onDemandParameters)
        {
            var fileContent = GetOnDemandDataDataFileBuilder(exportParameters, onDemandParameters).GetDataFile();
            return GetFileResponseMessage(exportParameters, fileContent);
        }

        private static HttpResponseMessage GetFileResponseMessage(IndicatorExportParameters exportParameters, byte[] fileContent)
        {
            if (fileContent == null) return null;

            var filename = SingleEntityFileNamer.GetDataForUserbyIndicatorAndAreaType(exportParameters.ChildAreaTypeId);
            return FileResponseBuilder.NewMessage(fileContent, filename);
        }
    }
}