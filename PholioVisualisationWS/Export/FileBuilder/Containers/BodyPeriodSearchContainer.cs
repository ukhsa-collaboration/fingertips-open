using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Export.FileBuilder.Wrappers;
using PholioVisualisation.PholioObjects;
using Remotion.Linq.Utilities;

namespace PholioVisualisation.Export.FileBuilder.Containers
{
    public class BodyPeriodSearchContainer
    {
        public OnDemandQueryParametersWrapper OnDemandQueryParameters { set; get; }
        public ExportAreaHelper AreaHelper { set; get; }
        public CsvBuilderAttributesForPeriodsWrapper AttributesForPeriods { set; get; }
        private readonly IGroupDataReader _groupDataReader;
        private readonly Grouping _grouping;

        public BodyPeriodSearchContainer(ExportAreaHelper areaHelper, OnDemandQueryParametersWrapper onDemandQueryParameters, CsvBuilderAttributesForPeriodsWrapper attributesForPeriods, IGroupDataReader groupDataReader,
                                            Grouping grouping)
        {
            OnDemandQueryParameters = onDemandQueryParameters;
            AreaHelper = areaHelper;
            AttributesForPeriods = attributesForPeriods;
            _groupDataReader = groupDataReader;
            _grouping = grouping;
        }

        /// <summary>
        /// Finds the matching CoreDataSet object in the list
        /// </summary>
        public static CoreDataSet FindMatchingCoreDataSet(IEnumerable<CoreDataSet> dataList, CoreDataSet coreData, string areaCode = null)
        {
            return areaCode == null ?
                dataList.FirstOrDefault(x => x.AgeId == coreData.AgeId && x.SexId == coreData.SexId) :
                dataList.FirstOrDefault(x => x.AreaCode == areaCode && x.AgeId == coreData.AgeId && x.SexId == coreData.SexId);
        }

        public bool FilterChildCoreDataByChildAreaCodeList(ref IList<CoreDataSet> coreDataList)
        {
            var isFiltered = OnDemandQueryParameters.ChildAreaCodeList != null;
            if (isFiltered)
            {
                coreDataList = coreDataList.Where(childCore => OnDemandQueryParameters.ChildAreaCodeList.Any(currentArea =>
                    string.Equals(currentArea, childCore.AreaCode, StringComparison.OrdinalIgnoreCase))).ToList();
            }

            return isFiltered;
        }

        public bool FilterParentCoreDataByChildAreaCodeList(ref IList<CoreDataSet> parentCoreData)
        {
            var isFiltered = OnDemandQueryParameters.ChildAreaCodeList != null;

            // Filter parent core by choosen child area code
            if (isFiltered)
            {
                parentCoreData = parentCoreData.Where(core => OnDemandQueryParameters.ChildAreaCodeList.Any(areaCode => AreaHelper.ParentAreaCodeToChildAreaCodesMap.Where(parentChild =>
                    core.AreaCode == parentChild.Key).Select(childCode => childCode.Value).Any(childCodeList =>
                    childCodeList.Any(childCode =>
                        string.Equals(areaCode, childCode, StringComparison.OrdinalIgnoreCase))))).ToList();
            }

            return isFiltered;
        }

        public IList<CoreDataSet> FilterCoreDataByInequalities(int indicatorId, TimePeriod timePeriod, params string[] areaCode)
        {
            var resultCoreDataSetList = new List<CoreDataSet>();
            var dataIncludingInequalities = _groupDataReader.GetDataIncludingInequalities(indicatorId, timePeriod, AttributesForPeriods.ExcludedCategoryTypeIds, areaCode);


            if (OnDemandQueryParameters.Inequalities[indicatorId] == null) return dataIncludingInequalities;

            foreach (var inequality in OnDemandQueryParameters.Inequalities[indicatorId])
            {
                InitializeGeneralInequalitiesAttributes(inequality);
                var filteredDataByInequality = dataIncludingInequalities.Where(childCore => childCore.CategoryTypeId == inequality.CategoryTypeId
                                                                                         && childCore.CategoryId == inequality.CategoryId
                                                                                         && childCore.AgeId == inequality.AgeId
                                                                                         && childCore.SexId == inequality.SexId).ToList();
                resultCoreDataSetList.AddRange(filteredDataByInequality);
            }

            return resultCoreDataSetList;
        }

        private void InitializeGeneralInequalitiesAttributes(Inequality inequality)
        {
            if (inequality.SexId == null && inequality.AgeId == null && inequality.CategoryTypeId == 0 && inequality.CategoryId == 0)
            {
                throw new ArgumentEmptyException("The attributes for inequality received are empty");
            }

            if (inequality.SexId == null && inequality.AgeId == null)
            {
                inequality.SexId = _grouping.SexId;
                inequality.AgeId = _grouping.AgeId;
            }

            if (inequality.CategoryTypeId != 0 || inequality.CategoryId != 0) return;
            inequality.CategoryTypeId = -1;
            inequality.CategoryId = -1;
        }
    }
}
