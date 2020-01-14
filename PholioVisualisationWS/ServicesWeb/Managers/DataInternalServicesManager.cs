using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Export.FileBuilder.Dtos;
using PholioVisualisation.Export.FileBuilder.SupportModels;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServicesWeb.Helpers;
using PholioVisualisation.ServicesWeb.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using PholioVisualisation.Export;
using PholioVisualisation.Export.FileBuilder.Wrappers;

namespace PholioVisualisation.ServicesWeb.Managers
{
    /// <inheritdoc />
    /// <summary>
    /// Manager of the internal services controller
    /// </summary>
    public class DataInternalServicesManager : DataManagerBase
    {
        private readonly IDataInternalServicesParameters _requestedParameters;
        private readonly IGroupDataReader _groupDataReader;
        private readonly IProfileReader _profileReader;
        private readonly IAreasReader _areasReader;

        /// <summary>
        /// DataInternalServicesManager constructor
        /// </summary>
        public DataInternalServicesManager(IDataInternalServicesParameters requestedParameters, IGroupDataReader groupDataReader, IProfileReader profileReader, IAreasReader areasReader)
        {
            _requestedParameters = requestedParameters;
            _groupDataReader = groupDataReader;
            _profileReader = profileReader;
            _areasReader = areasReader;
            InitializeParametersBuilder();
        }
        
        private void InitializeParametersBuilder()
        {
            switch (_requestedParameters.GetDataInternalServiceCalled())
            {
                case DataInternalServiceUse.LatestDataFileForGroup:
                    InitParametersForLatestDataFileForGroup();
                    break;
                case DataInternalServiceUse.LatestDataFileForIndicator:
                    InitParametersForLatestDataFileForIndicator();
                    break;
                case DataInternalServiceUse.LatestWithInequalitiesDataFileForIndicator:
                    InitParametersForLatestWithInequalitiesDataFileForIndicator();
                    break;
                case DataInternalServiceUse.AllPeriodsWithInequalitiesDataFileForIndicator:
                    InitParametersForAllPeriodsWithInequalitiesDataFileForIndicator();
                    break;
                case DataInternalServiceUse.AllPeriodDataFileByIndicator:
                    InitParametersForAllPeriodDataFileByIndicator();
                    break;
                case DataInternalServiceUse.LatestPopulationDataFile:
                    InitParametersForLatestPopulationDataFile();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("That service is not existing at the moment");
            }
        }

        /// <summary>
        /// Get ready ExportParameters and OnDemandParameters for latest data file for group
        /// </summary>
        public void InitParametersForLatestDataFileForGroup()
        {
            // Initialize child areas code
            var childAreasCodeList = _requestedParameters.GetChildAreasCodeList();
            var categoryAreaCodeArray = _requestedParameters.GetCategoryCodeArray();

            const bool allPeriods = false;

            // Get profile ID of group
            if (_requestedParameters.GetGroupId() == null)
                throw new ArgumentNullException("GroupId cannot be null");

            var profileId = GetProfileIdByGroupId(_groupDataReader, (int)_requestedParameters.GetGroupId());

            // Get indicator IDs in group
            var indicatorIdList = GetIndicatorIdListProvider(_groupDataReader, _profileReader).GetIdsForGroupAreaType((int)_requestedParameters.GetGroupId(), _requestedParameters.GetChildAreaTypeId());

            SetExportParameters(profileId, _requestedParameters.GetChildAreaTypeId(), _requestedParameters.GetParentAreaTypeId(), _requestedParameters.GetParentAreaCode());

            IDictionary<int, IList<InequalitySearch>> inequalities = new Dictionary<int, IList<InequalitySearch>>();
            inequalities = BuildInequalitiesDictionary(indicatorIdList, categoryAreaCodeArray, inequalities);

            SetOnDemandParameters(profileId, indicatorIdList, inequalities, childAreasCodeList, new List<int> { (int)_requestedParameters.GetGroupId() }, allPeriods, categoryAreaCodeArray);
        }

        /// <summary>
        /// Get ready ExportParameters and OnDemandParameters for latest data file for indicator
        /// </summary>
        public void InitParametersForLatestDataFileForIndicator()
        {
            const bool allPeriods = false;
            var categoryAreaCodeArray = _requestedParameters.GetCategoryCodeArray();

            // Limit number of child areas code
            var childAreasCodeList = GetStringListAmount(_requestedParameters.GetAreasCode(), MaxChildAreasCodeList);

            // Setting indicator
            var indicatorIdList = _requestedParameters.GetListIndicatorIds();

            // Limit number of indicators
            indicatorIdList = GetListIdsAmount(indicatorIdList, MaxElementNumberForCsvDownload).ToList();

            // Setting sex
            var sexIdList = _requestedParameters.GetSexIds();
            sexIdList = GetListIdsAmount(sexIdList, MaxElementNumberForCsvDownload).ToList();

            // Setting age
            var ageIdList = _requestedParameters.GetAgeIds();
            ageIdList = GetListIdsAmount(ageIdList, MaxElementNumberForCsvDownload).ToList();

            if (_requestedParameters.GetProfileId() == null)
                throw new ArgumentNullException("ProfileId cannot be null");

            SetExportParameters((int)_requestedParameters.GetProfileId(), _requestedParameters.GetChildAreaTypeId(), _requestedParameters.GetParentAreaTypeId(), _requestedParameters.GetParentAreaCode());

            IDictionary<int, IList<InequalitySearch>> inequalities = new Dictionary<int, IList<InequalitySearch>>();

            var indicatorAreaTypeGroupIdList = GetIndicatorAreaTypeGroupIdList(indicatorIdList, _requestedParameters.GetChildAreaTypeId(), null, sexIdList, ageIdList);
            inequalities = GetDictionaryInequalities(_groupDataReader,_areasReader, indicatorAreaTypeGroupIdList, categoryAreaCodeArray, inequalities);

            SetOnDemandParameters((int)_requestedParameters.GetProfileId(), indicatorIdList, inequalities, childAreasCodeList, null, allPeriods, categoryAreaCodeArray);
        }

        /// <summary>
        /// Get ready ExportParameters and OnDemandParameters for latest with inequalities data for indicator
        /// </summary>
        public void InitParametersForLatestWithInequalitiesDataFileForIndicator()
        {
            const bool allPeriods = false;
            var categoryAreaCodeArray = _requestedParameters.GetCategoryCodeArray();

            // Limit number of child areas code
            var childAreasCodeList = GetStringListAmount(_requestedParameters.GetAreasCode(), MaxChildAreasCodeList);

            // Setting indicator
            var indicatorIdList = _requestedParameters.GetListIndicatorIds();

            // Limit number of indicators
            indicatorIdList = GetListIdsAmount(indicatorIdList, MaxElementNumberForCsvDownload).ToList();

            if (_requestedParameters.GetProfileId() == null)
                throw new ArgumentNullException("ProfileId cannot be null");

            SetExportParameters((int)_requestedParameters.GetProfileId(), _requestedParameters.GetChildAreaTypeId(), _requestedParameters.GetParentAreaTypeId(), _requestedParameters.GetParentAreaCode());

            var inequalitiesObject = JsonConvert.DeserializeObject<Dictionary<int, IList<InequalitySearchDto>>>(_requestedParameters.GetInequalities());

            IDictionary<int, IList<InequalitySearch>> inequalitiesSearch = InequalitySearchDto.MapDicInequalitiesSearchDtoToDicInequalitiesSearch(inequalitiesObject);

            SetOnDemandParameters((int)_requestedParameters.GetProfileId(), indicatorIdList, inequalitiesSearch, childAreasCodeList, null, allPeriods, categoryAreaCodeArray);
        }

        /// <summary>
        /// Get ready ExportParameters and OnDemandParameters for all periods with inequalities data for indicator
        /// </summary>
        public void InitParametersForAllPeriodsWithInequalitiesDataFileForIndicator()
        {
            const bool allPeriods = true;
            var categoryAreaCodeArray = _requestedParameters.GetCategoryCodeArray();

            // Limit number of child areas code
            var childAreasCodeList = GetStringListAmount(_requestedParameters.GetAreasCode(), MaxChildAreasCodeList);

            // Setting indicator
            var indicatorIdList = _requestedParameters.GetListIndicatorIds();

            // Limit number of indicators
            indicatorIdList = GetListIdsAmount(indicatorIdList, MaxElementNumberForCsvDownload).ToList();

            if (_requestedParameters.GetProfileId() == null)
                throw new ArgumentNullException("ProfileId cannot be null");

            SetExportParameters((int)_requestedParameters.GetProfileId(), _requestedParameters.GetChildAreaTypeId(), _requestedParameters.GetParentAreaTypeId(), _requestedParameters.GetParentAreaCode());

            var inequalitiesObject = JsonConvert.DeserializeObject<Dictionary<int, IList<InequalitySearchDto>>>(_requestedParameters.GetInequalities());

            IDictionary<int, IList<InequalitySearch>> inequalitiesSearch = InequalitySearchDto.MapDicInequalitiesSearchDtoToDicInequalitiesSearch(inequalitiesObject);

            SetOnDemandParameters((int)_requestedParameters.GetProfileId(), indicatorIdList, inequalitiesSearch, childAreasCodeList, null, allPeriods, categoryAreaCodeArray);
        }

        /// <summary>
        /// Get ready ExportParameters and OnDemandParameters for all periods by indicator
        /// </summary>
        public void InitParametersForAllPeriodDataFileByIndicator()
        {
            const bool allPeriods = true;
            var categoryAreaCodeArray = _requestedParameters.GetCategoryCodeArray();

            // Limit number of child areas code
            var childAreasCodeList = GetStringListAmount(_requestedParameters.GetAreasCode(), MaxChildAreasCodeList);

            // Setting indicator
            var indicatorIdList = _requestedParameters.GetListIndicatorIds();

            // Limit number of indicators
            indicatorIdList = GetListIdsAmount(indicatorIdList, MaxElementNumberForCsvDownload).ToList();

            // Setting sex
            var sexIdList = _requestedParameters.GetSexIds();
            sexIdList = GetListIdsAmount(sexIdList, MaxElementNumberForCsvDownload).ToList();

            // Setting age
            var ageIdList = _requestedParameters.GetAgeIds();
            ageIdList = GetListIdsAmount(ageIdList, MaxElementNumberForCsvDownload).ToList();

            if (_requestedParameters.GetProfileId() == null)
                throw new ArgumentNullException("ProfileId cannot be null");

            SetExportParameters((int)_requestedParameters.GetProfileId(), _requestedParameters.GetChildAreaTypeId(), _requestedParameters.GetParentAreaTypeId(), _requestedParameters.GetParentAreaCode());

            // Setting inequalities
            IDictionary<int, IList<InequalitySearch>> inequalities = new Dictionary<int, IList<InequalitySearch>>();

            var indicatorAreaTypeGroupIdList = GetIndicatorAreaTypeGroupIdList(indicatorIdList, _requestedParameters.GetChildAreaTypeId(), null, sexIdList, ageIdList);
            inequalities = GetDictionaryInequalities(_groupDataReader, _areasReader, indicatorAreaTypeGroupIdList, categoryAreaCodeArray, inequalities);

            SetOnDemandParameters((int)_requestedParameters.GetProfileId(), indicatorIdList, inequalities, childAreasCodeList, null, allPeriods, categoryAreaCodeArray);
        }

        /// <summary>
        /// Get ready ExportParameters and OnDemandParameters for all periods by indicator
        /// </summary>
        public void InitParametersForLatestPopulationDataFile()
        {
            const int groupId = GroupIds.Population;
            const bool allPeriods = false;
            var categoryAreaCodeArray = _requestedParameters.GetCategoryCodeArray();

            // Initialize child areas code
            var childAreasCodeList = ServiceHelper.StringListStringParser(_requestedParameters.GetAreasCode());

            // Initialize lists
            var groupIdList = new List<int> {groupId};

            // Get profile ID of group
            if (_requestedParameters.GetGroupId() == null)
                throw new ArgumentNullException("GroupId cannot be null");

            var profileId = _groupDataReader
                .GetGroupingMetadataList(groupIdList)
                .First()
                .ProfileId;

            // Get indicator IDs in group
            var indicatorIdList = GetIndicatorIdListProvider(_groupDataReader, _profileReader).GetIdsForGroupAreaType(groupId, _requestedParameters.GetChildAreaTypeId());

            SetExportParameters(profileId, _requestedParameters.GetChildAreaTypeId(), _requestedParameters.GetParentAreaTypeId(), _requestedParameters.GetParentAreaCode());

            // Setting inequalities
            var categoryInequalitySearchList = new List<CategoryInequalitySearch>
            {
                new CategoryInequalitySearch(CategoryTypeIds.Undefined,CategoryIds.Undefined),
            };

            if (ExportAreaHelper.IsCategoryAreaCode(_requestedParameters.GetParentAreaCode()))
            {
                categoryInequalitySearchList.Add(ExportAreaHelper.GetCategoryInequalityFromAreaCode(_requestedParameters.GetParentAreaCode(), _areasReader));
            }

            var sexesList = new List<int>{ SexIds.Female, SexIds.Male };
            var timePeriod = ExportPeriodHelper.GetPopulationTimePeriod();
            var agesTargetList = _groupDataReader.GetDataIncludingInequalities(indicatorIdList[0], timePeriod, new CsvBuilderAttributesForPeriodsWrapper().ExcludedCategoryTypeIds,
                AreaCodes.England).Where(x => x.AgeId != -1 ).Select(x => x.AgeId).Distinct();

            var sexAgeInequalitySearchList = SexAgeInequalitySearch.CombineSexAndAgeInSexAgeInequalitySearchList(sexesList, agesTargetList.ToList());
            var populationInequalitiesSearchList = InequalitySearch.CombineTwoListsIntoInequalitySearchList(categoryInequalitySearchList, sexAgeInequalitySearchList);
            var inequalities = indicatorIdList.ToDictionary(indicatorId => indicatorId, indicatorId => populationInequalitiesSearchList);

            SetOnDemandParameters(profileId, indicatorIdList, inequalities, childAreasCodeList, groupIdList, allPeriods, categoryAreaCodeArray);
        }

        private IDictionary<int, IList<InequalitySearch>> BuildInequalitiesDictionary(IList<int> indicatorIdList, string[] categoryAreaCodeArray, IDictionary<int, IList<InequalitySearch>> inequalities)
        {
            foreach (var indicator in indicatorIdList)
            {
                var groupingInequalityInitial = new GroupingInequality(indicator, _requestedParameters.GetChildAreaTypeId(), _requestedParameters.GetGroupId(), -1, -1);
                var sexAndAgeForIndicator = InequalitySearch.GetGroupingSexAgeInequalityByIndicatorGroupAreaType(groupingInequalityInitial, _groupDataReader);

                var indicatorAreaTypeGroupIdList = GetIndicatorAreaTypeGroupIdList(indicatorIdList, _requestedParameters.GetChildAreaTypeId(), _requestedParameters.GetGroupId(), sexAndAgeForIndicator);
                inequalities = GetDictionaryInequalities(_groupDataReader, _areasReader, indicatorAreaTypeGroupIdList, categoryAreaCodeArray, inequalities);
            }

            return inequalities;
        }
    }
}