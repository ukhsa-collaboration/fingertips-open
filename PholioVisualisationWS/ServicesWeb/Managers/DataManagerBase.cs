using System;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export;
using PholioVisualisation.Export.FileBuilder.Wrappers;
using PholioVisualisation.Parsers;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.Export.FileBuilder.SupportModels;

namespace PholioVisualisation.ServicesWeb.Managers
{
    /// <summary>
    /// Manager of the internal services controller
    /// </summary>
    public class DataManagerBase
    {
        /// <summary>
        /// Parameters related with the indicator 
        /// </summary>
        public IndicatorExportParameters ExportParameters { get; private set; }

        /// <summary>
        /// Parameters on demand for searching
        /// </summary>
        public OnDemandQueryParametersWrapper OnDemandParameters { get; private set; }

        /// <summary>
        /// Maximum number of indicators in a list
        /// </summary>
        protected const int MaxElementNumberForCsvDownload = 100;
        /// <summary>
        /// Maximum number of child areas code in a list
        /// </summary>
        protected const int MaxChildAreasCodeList = 80;
        
        /// <summary>
        /// Set the export parameters for the class
        /// </summary>
        public void SetExportParameters(int profileId, int childAreaTypeId, int parentAreaTypeId, string parentAreaCode)
        {
            ExportParameters = new IndicatorExportParameters
            {
                ProfileId = profileId,
                ChildAreaTypeId = childAreaTypeId,
                ParentAreaTypeId = parentAreaTypeId,
                ParentAreaCode = parentAreaCode
            };
        }

        /// <summary>
        /// Set the on demand parameters for the class
        /// </summary>
        public void SetOnDemandParameters(int profileId, IList<int> indicatorIdList, IDictionary<int,IList<InequalitySearch>> inequalities,IList<string> childAreasCodeList, IList<int> groupIdList, bool allPeriods,
            string[] categoryAreaCodeArray)
        {
            OnDemandParameters = new OnDemandQueryParametersWrapper(profileId, indicatorIdList, inequalities, childAreasCodeList, groupIdList, allPeriods, categoryAreaCodeArray);
        }

        /// <summary>
        /// Get a dictionary of inequalities from a list of indicators, areasTypes and GroupIds
        /// </summary>
        public static IDictionary<int, IList<InequalitySearch>> GetDictionaryInequalities(IGroupDataReader groupDataReader, IAreasReader areasReader, IList<GroupingInequality> indicatorAreaTypeGroupIdList, 
            string[] categoryAreaCodesArray, IDictionary<int, IList<InequalitySearch>> dictionaryTarget)
        {
            foreach (var indicatorAreaTypeGroupId in indicatorAreaTypeGroupIdList)
            {
                dictionaryTarget = InequalitySearch.AddInequalitiesToDictionary(groupDataReader, areasReader, indicatorAreaTypeGroupId, categoryAreaCodesArray, dictionaryTarget);
            }

            return dictionaryTarget;
        }

        /// <summary>
        /// Find the first profile found by group id
        /// </summary>
        public static int GetProfileIdByGroupId(IGroupDataReader groupDataReader, int groupId)
        {
            // Initialize lists
            var groupIdList = new List<int> { groupId };

            // Get profile ID of group
            var profileId = groupDataReader.GetGroupingMetadataList(groupIdList).First().ProfileId;

            return profileId;
        }

        /// <summary>
        /// Remove when have DI framework
        /// </summary>
        public static IIndicatorIdListProvider GetIndicatorIdListProvider(IGroupDataReader groupDataReader, IProfileReader profileReader)
        {
            var groupIdProvider = new GroupIdProvider(profileReader);
            return new IndicatorIdListProvider(groupDataReader, groupIdProvider);
        }

        /// <summary>
        /// Get a list of IndicatorAreaTypeGroupId
        /// </summary>
        public static IList<GroupingInequality> GetIndicatorAreaTypeGroupIdList(IList<int> indicatorIdList, int areaTypeId,
            int? groupId, IList<SexAgeInequalitySearch> listGroupingSexAgeInequalities)
        {
            if (!listGroupingSexAgeInequalities.Any() || !indicatorIdList.Any())
                throw new ArgumentOutOfRangeException("There have to be categories, sexes and indicators");

            var indicatorAreaTypeGroupIdList = new List<GroupingInequality>();

            foreach (var indicator in indicatorIdList)
            {
                indicatorAreaTypeGroupIdList.AddRange(listGroupingSexAgeInequalities.Select(sexAgeInequality => 
                    new GroupingInequality(indicator, areaTypeId, groupId, sexAgeInequality.SexId, sexAgeInequality.AgeId)));
            }
            

            return indicatorAreaTypeGroupIdList.ToList();
        }

        /// <summary>
        /// Get a list of IndicatorAreaTypeGroupId
        /// </summary>
        public static IList<GroupingInequality> GetIndicatorAreaTypeGroupIdList(int indicatorId, int? groupId, IList<AreaTypeIdSexAgeInequalitySearch> listGroupingAreaTypeSexAgeInequalities)
        {
            var indicatorAreaTypeGroupIdList = new List<GroupingInequality>();

            indicatorAreaTypeGroupIdList.AddRange(listGroupingAreaTypeSexAgeInequalities.Select(areaTypeIdSexAgeInequality =>
                new GroupingInequality(indicatorId, areaTypeIdSexAgeInequality.AreaTypeId, groupId, areaTypeIdSexAgeInequality.SexId, areaTypeIdSexAgeInequality.AgeId)));
            
            return indicatorAreaTypeGroupIdList.ToList();
        }

        /// <summary>
        /// Get a list of IndicatorAreaTypeGroupId
        /// </summary>
        public static IList<GroupingInequality> GetIndicatorAreaTypeGroupIdList(IList<int> indicatorIdList, int areaTypeId, int? groupId, IList<int> sexIds, IList<int> ageIds)
        {
            if (indicatorIdList.Count != sexIds.Count || indicatorIdList.Count != ageIds.Count)
                throw new ArgumentOutOfRangeException("The amount of sexes and ages have to match for indicatorIds");

            var indicatorAreaTypeGroupIdList = indicatorIdList.Select((t, i) => new GroupingInequality(t, areaTypeId, groupId, sexIds[i], ageIds[i])).ToList();

            return indicatorAreaTypeGroupIdList.ToList();
        }

        /// <summary>
        /// Transform a string area codes in list with a maximum number of elements
        /// </summary>
        protected static IList<string> GetStringListAmount(string stringElements, int maxChildAreasCodeList)
        {
            if (stringElements == null) return null;

            var childAreasCodeList = new StringListParser(stringElements).StringList;

            // Limit number of child areas code
            if (childAreasCodeList.Count > maxChildAreasCodeList)
            {
                childAreasCodeList = childAreasCodeList.Take(maxChildAreasCodeList).ToList();
            }

            return childAreasCodeList;
        }

        /// <summary>
        /// Get a list with the maximum number of elements specified
        /// </summary>
        protected static IList<int> GetListIdsAmount(IList<int> listIds, int maxIndicatorsForCsvDownload)
        {
            return listIds.Count <= maxIndicatorsForCsvDownload
                ? listIds
                : listIds.Take(maxIndicatorsForCsvDownload).ToList();
        }
    }
}