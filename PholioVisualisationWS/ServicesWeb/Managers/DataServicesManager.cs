using PholioVisualisation.DataAccess;
using PholioVisualisation.Export.FileBuilder.SupportModels;
using PholioVisualisation.Export.FileBuilder.Wrappers;
using PholioVisualisation.ServicesWeb.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServicesWeb.Managers
{
    /// <inheritdoc />
    /// <summary>
    /// Class to provide management to DataController
    /// </summary>
    public class DataServicesManager : DataManagerBase
    {
        private readonly IDataServicesParameters _parameters;
        private readonly IGroupDataReader _groupDataReader;
        private readonly IProfileReader _profileReader;
        private readonly IAreasReader _areasReader;

        /// <summary>
        /// DataInternalServicesManager constructor
        /// </summary>
        public DataServicesManager(IDataServicesParameters requestedParameters, IGroupDataReader groupDataReader, IProfileReader profileReader, IAreasReader areasReader)
        {
            _parameters = requestedParameters;
            _groupDataReader = groupDataReader;
            _profileReader = profileReader;
            _areasReader = areasReader;
            InitializeParametersBuilder();
        }

        private void InitializeParametersBuilder()
        {
            switch (_parameters.GetDataServiceCalled())
            {
                case DataServiceUse.AllDataFileForIndicatorList:
                    InitParametersDataFileForIndicatorList();
                    break;
                case DataServiceUse.AllDataFileForGroup:
                    InitParametersForAllDataFileForGroup();
                    break;
                case DataServiceUse.AllDataFileForProfile:
                    InitParametersForAllDataFileForProfile();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("That service is not existing at the moment");
            }
        }

        /// <summary>
        /// Get ready ExportParameters and OnDemandParameters for all data file for profile
        /// </summary>
        public void InitParametersForAllDataFileForProfile()
        {
            var categoryAreaCodeArray = _parameters.GetCategoryCodeArray();

            if (_parameters.GetProfileId() == null)
                throw new ArgumentNullException("ProfileId cannot be null");

            SetExportParameters((int)_parameters.GetProfileId(), _parameters.GetChildAreaTypeId(), _parameters.GetParentAreaTypeId(), _parameters.GetParentAreaCode());

            // Get indicatorIDs in profile
            var indicatorIds = GetIndicatorIdListProvider(_groupDataReader, _profileReader).GetIdsForProfile((int)_parameters.GetProfileId());

            IDictionary<int, IList<InequalitySearch>> inequalities = new Dictionary<int, IList<InequalitySearch>>();

            SetOnDemandParameters((int)_parameters.GetProfileId(), indicatorIds, inequalities, null, null, true, categoryAreaCodeArray);
        }

        /// <summary>
        /// Get ready ExportParameters and OnDemandParameters for all data file for group
        /// </summary>
        public void InitParametersForAllDataFileForGroup()
        {
            var categoryAreaCodeArray = _parameters.GetCategoryCodeArray();

            // Get profile ID of group
            if (_parameters.GetGroupId() == null)
                throw new ArgumentNullException("GroupId cannot be null");

            var profileId = _groupDataReader
                .GetGroupingMetadataList(new List<int> { (int)_parameters.GetGroupId() })
                .First()
                .ProfileId;

            // Get indicator IDs in group
            var indicatorIds = GetIndicatorIdListProvider(_groupDataReader, _profileReader).GetIdsForGroupAreaType((int)_parameters.GetGroupId(), _parameters.GetChildAreaTypeId());

            SetExportParameters(profileId, _parameters.GetChildAreaTypeId(), _parameters.GetParentAreaTypeId(), _parameters.GetParentAreaCode());

            IDictionary<int, IList<InequalitySearch>> inequalities = new Dictionary<int, IList<InequalitySearch>>();

            SetOnDemandParameters(profileId, indicatorIds, inequalities, null, null, true, categoryAreaCodeArray);
        }

        /// <summary>
        /// Get ready ExportParameters and OnDemandParameters for data file for indicator list
        /// </summary>
        public void InitParametersDataFileForIndicatorList()
        {
            var categoryAreaCodeArray = _parameters.GetCategoryCodeArray();
            var indicatorIdList = _parameters.GetListIndicatorIds();

            var profileId = _parameters.GetProfileId();
            var ageIds = _parameters.GetAgeIds();
            var sexIds = _parameters.GetSexIds();

            // Limit number of indicators
            indicatorIdList = GetListIdsAmount(indicatorIdList, MaxElementNumberForCsvDownload).ToList();

            if (profileId.HasValue == false)
            {
                throw new ArgumentNullException("ProfileId cannot be null");
            }

            SetExportParameters(profileId.Value, _parameters.GetChildAreaTypeId(), _parameters.GetParentAreaTypeId(),
                _parameters.GetParentAreaCode());

            IDictionary<int, IList<InequalitySearch>> inequalities = new Dictionary<int, IList<InequalitySearch>>();
            if (ageIds != null && sexIds != null)
            {
                if (categoryAreaCodeArray == null)
                {
                    categoryAreaCodeArray = InequalitySearch.GetCategoriesByIndicatorId(indicatorIdList[0], sexIds[0],
                        ageIds[0], _groupDataReader);
                }

                inequalities = BuildInequalitiesDictionary(indicatorIdList, sexIds[0], ageIds[0], categoryAreaCodeArray,
                    inequalities);
            }

            SetOnDemandParameters(profileId.Value, indicatorIdList, inequalities, null, null, true, categoryAreaCodeArray);
        }

        private IDictionary<int, IList<InequalitySearch>> BuildInequalitiesDictionary(IList<int> indicatorIdList, int sexId, int ageId, string[] categoryAreaCodeArray, IDictionary<int, IList<InequalitySearch>> inequalities)
        {
            foreach (var indicatorId in indicatorIdList)
            {
                var areaTypeIdSexAndAgeForIndicatorList = InequalitySearch.GetGroupingSexAgeInequalityByIndicatorId(indicatorId, _groupDataReader).Where(x => x.SexId == sexId && x.AgeId == ageId).ToList();

                var indicatorAreaTypeGroupIdList = GetIndicatorAreaTypeGroupIdList(indicatorId, _parameters.GetGroupId(), areaTypeIdSexAndAgeForIndicatorList);
                inequalities = GetDictionaryInequalities(_groupDataReader, _areasReader, indicatorAreaTypeGroupIdList, categoryAreaCodeArray, inequalities);
            }

            return inequalities;
        }
    }
}