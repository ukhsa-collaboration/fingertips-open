using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export.FileBuilder.SupportModels;
using PholioVisualisation.Export.FileBuilder.Wrappers;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServiceActions;
using Remotion.Linq.Utilities;
using PholioVisualisation.DataSorting;
using PholioVisualisation.UserData;
using PholioVisualisation.UserData.Repositories;

namespace PholioVisualisation.Export.FileBuilder.Containers
{
    public class BodyPeriodSearchContainer
    {
        public OnDemandQueryParametersWrapper OnDemandQueryParameters { set; get; }
        public ExportAreaHelper AreaHelper { set; get; }
        public CsvBuilderAttributesForPeriodsWrapper AttributesForPeriods { set; get; }
        private readonly IGroupDataReader _groupDataReader;
        private readonly IAreasReader _areasReader;
        private readonly Grouping _grouping;
        private readonly IndicatorExportParameters _generalParameters;
        private BenchmarkDataProvider _benchmarkDataProvider;

        public BodyPeriodSearchContainer(ExportAreaHelper areaHelper, IndicatorExportParameters generalParameters, OnDemandQueryParametersWrapper onDemandQueryParameters,
            CsvBuilderAttributesForPeriodsWrapper attributesForPeriods, IGroupDataReader groupDataReader, IAreasReader areasReader, Grouping grouping)
        {
            OnDemandQueryParameters = onDemandQueryParameters;
            AreaHelper = areaHelper;
            AttributesForPeriods = attributesForPeriods;
            _groupDataReader = groupDataReader;
            _areasReader = areasReader;
            _grouping = grouping;
            _generalParameters = generalParameters;
            _benchmarkDataProvider = new BenchmarkDataProvider(_groupDataReader);
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

            // Filter parent core by chosen child area code
            if (isFiltered)
            {
                parentCoreData = parentCoreData.Where(core => OnDemandQueryParameters.ChildAreaCodeList.Any(areaCode => AreaHelper.ParentAreaCodeToChildAreaCodesMap.Where(parentChild =>
                    core.AreaCode == parentChild.Key).Select(childCode => childCode.Value).Any(childCodeList =>
                    childCodeList.Any(childCode =>
                        string.Equals(areaCode, childCode, StringComparison.OrdinalIgnoreCase))))).ToList();
            }

            return isFiltered;
        }

        public IList<CoreDataSet> FilterCoreDataByInequalitiesNational(int indicatorId, TimePeriod timePeriod, params string[] areaCode)
        {
            var dataIncludingInequalities = _groupDataReader.GetDataIncludingInequalities(indicatorId, timePeriod,
                AttributesForPeriods.ExcludedCategoryTypeIds, areaCode);

            if (this.OnDemandQueryParameters.AreQuinaryPopulations)
            {
                dataIncludingInequalities = ExportPopulationHelper.FilterQuinaryPopulations(dataIncludingInequalities);
            }

            if (HasNotInequalitiesFilter(indicatorId, dataIncludingInequalities))
            {
                return dataIncludingInequalities;
            }

            var inequalitiesForNational = OnDemandQueryParameters.Inequalities[indicatorId];

            if (OnDemandQueryParameters.CategoryAreaCode == null)
            {
                inequalitiesForNational = inequalitiesForNational.Where(x => x.CategoryInequalitySearch.CategoryTypeId == CategoryTypeIds.Undefined &&
                                                                      x.CategoryInequalitySearch.CategoryId == CategoryTypeIds.Undefined).ToList();
            }

            return GetDataFilteredByInequalities(inequalitiesForNational, dataIncludingInequalities);
        }

        private static IList<InequalitySearch> GetFilteredInequalitiesByCategoryType(IList<InequalitySearch> inequalities)
        {
            inequalities = inequalities.Where(inequality =>
                inequality != null &&
                inequality.CategoryInequalitySearch.CategoryTypeId != CategoryTypeIds.Undefined &&
                inequality.CategoryInequalitySearch.CategoryId != CategoryIds.Undefined).ToList();
            return inequalities;
        }

        private IList<CoreDataSet> GetDataFilteredByInequalities(IList<InequalitySearch> inequalities, IList<CoreDataSet> dataIncludingInequalities)
        {
            var resultCoreDataSetList = new List<CoreDataSet>();

            foreach (var inequality in inequalities)
            {
                if (inequality == null)
                {
                    return dataIncludingInequalities;
                }

                InitializeGeneralInequalitiesAttributes(inequality);

                var filteredDataByInequality = dataIncludingInequalities.Where(childCore => isFoundCoreDataSet(childCore, inequality)).ToList();

                resultCoreDataSetList.AddRange(filteredDataByInequality);
            }

            if (OnDemandQueryParameters.GroupIds.FirstOrDefault() != GroupIds.Population) return resultCoreDataSetList;

            resultCoreDataSetList = ExportPopulationHelper.FilterQuinaryPopulations(resultCoreDataSetList);
            return resultCoreDataSetList;
        }

        private bool isFoundCoreDataSet(CoreDataSet coreData, InequalitySearch inequality)
        {
            return coreData.CategoryTypeId == inequality.CategoryInequalitySearch.CategoryTypeId
                        && coreData.CategoryId == inequality.CategoryInequalitySearch.CategoryId
                        && coreData.AgeId == inequality.SexAgeInequalitySearch.AgeId
                        && coreData.SexId == inequality.SexAgeInequalitySearch.SexId;
        }

        public IList<CoreDataSet> GetCoreDataByCalculation(IList<CoreDataSet> coreDataSetList, string areaCode, int areaTypeId)
        {
            var coreDataSetClonedList = coreDataSetList.Select(coreDataSet => (CoreDataSet)coreDataSet.Clone()).ToList();

            var calculatedPopulation = new QuinaryPopulationDataCsvAction().GetPopulationOnly(areaCode, areaTypeId, 0);

            object catchedValues;
            calculatedPopulation.TryGetValue("PopulationTotals", out catchedValues);
            var valueDic = (Dictionary<int, IEnumerable<double>>)catchedValues;

            IEnumerable<double> calculatedFemaleValues;
            IEnumerable<double> calculatedMaleValues;
            valueDic.TryGetValue(SexIds.Female, out calculatedFemaleValues);
            valueDic.TryGetValue(SexIds.Male, out calculatedMaleValues);

            if (calculatedFemaleValues == null) throw new ArgumentEmptyException("It is expected a calculated value");
            if (calculatedMaleValues == null) throw new ArgumentEmptyException("It is expected a calculated value");

            var calculatedValues = new List<double>();
            calculatedValues.AddRange(calculatedFemaleValues);
            calculatedValues.AddRange(calculatedMaleValues);

            coreDataSetClonedList = (List<CoreDataSet>)SetCalculatedValues(areaCode, calculatedValues, coreDataSetClonedList);

            return coreDataSetClonedList;
        }

        private static IList<CoreDataSet> SetCalculatedValues(string areaCode, IEnumerable<double> calculatedValues, IList<CoreDataSet> coreDataSetClonedList)
        {
            if (!calculatedValues.Any() || coreDataSetClonedList.Count == 0) return new List<CoreDataSet>();

            var index = 0;
            foreach (var value in calculatedValues)
            {
                coreDataSetClonedList[index].AreaCode = areaCode;
                coreDataSetClonedList[index].Value = value;
                coreDataSetClonedList[index].Count = value;
                index++;
            }

            return coreDataSetClonedList;
        }

        public IList<CoreDataSet> FilterCoreDataByInequalitiesSubNational(int indicatorId, TimePeriod timePeriod, params string[] areaCode)
        {
            var dataIncludingInequalities = _groupDataReader.GetDataIncludingInequalities(indicatorId, timePeriod,
                AttributesForPeriods.ExcludedCategoryTypeIds, areaCode);

            if (HasNotInequalitiesFilter(indicatorId, dataIncludingInequalities))
            {
                return dataIncludingInequalities;
            }

            var inequalities = OnDemandQueryParameters.Inequalities[indicatorId];

            if (ExportAreaHelper.IsCategoryAreaTypeId(_generalParameters.ParentAreaTypeId))
            {
                inequalities = GetFilteredInequalitiesByCategoryAreaTypeForSubNational(inequalities);
            }

            return GetDataFilteredByInequalitiesSubNational(inequalities, dataIncludingInequalities);
        }

        public IList<CoreDataSet> GetFilteredCoreDataSetsForSubNational(int indicatorId, TimePeriod timePeriod, IArea parentArea,
            string[] areaCodes, IndicatorMetadata indicatorMetadata,
            IList<CoreDataSet> coreDataPreList)
        {
            if (OnDemandQueryParameters.GroupIds.FirstOrDefault() == GroupIds.Population)
            {
                var coreData = FilterCoreDataByInequalitiesNational(indicatorId, timePeriod, AreaCodes.England);
                coreData = ExportPopulationHelper.FilterQuinaryPopulations(coreData);

                if (coreDataPreList.Count == coreData.Count)
                {
                    return coreDataPreList;
                }

                var areaCode = AreaHelper.GetPopulationAreaCode(parentArea, OnDemandQueryParameters, areaCodes[0]);

                var areaTypeId = AreaHelper.GetPopulationAreaTypeId(parentArea, OnDemandQueryParameters);

                var data = GetCoreDataByCalculation(coreData, areaCode, areaTypeId);
                coreDataPreList.Clear();
                foreach (var coreDataSet in data)
                {
                    coreDataPreList.Add(coreDataSet);
                }

                return coreDataPreList;
            }

            // Area list
            if (Area.IsAreaListArea(parentArea))
            {
                var data = CalculateAverageForAreaListCode(indicatorMetadata, timePeriod, parentArea, coreDataPreList);
                coreDataPreList.Clear();
                coreDataPreList.Add(data);

                return coreDataPreList;
            }

            // Nearest neighbours
            if (NearestNeighbourArea.IsNearestNeighbourAreaCode(parentArea.Code))
            {
                var data = CalculateAverageForAreaListCode(indicatorMetadata, timePeriod, parentArea, coreDataPreList);
                coreDataPreList.Clear();
                coreDataPreList.Add(data);

                return coreDataPreList;
            }

            // Filter coreData for subNational
            FilterParentCoreDataByChildAreaCodeList(ref coreDataPreList);

            return coreDataPreList;
        }

        private IList<InequalitySearch> GetFilteredInequalitiesByCategoryAreaTypeForSubNational(IList<InequalitySearch> inequalities)
        {
            if (OnDemandQueryParameters.CategoryAreaCode != null)
            {
                var inequalitiesFiltered = new List<InequalitySearch>();
                foreach (var categoryAreaCode in OnDemandQueryParameters.CategoryAreaCode)
                {
                    var currentCategories = ExportAreaHelper.GetCategoryInequalityFromAreaCode(categoryAreaCode, _areasReader);
                    inequalitiesFiltered.AddRange(inequalities.Where(inequality =>
                        inequality != null &&
                        inequality.CategoryInequalitySearch.CategoryTypeId == currentCategories.CategoryTypeId &&
                        inequality.CategoryInequalitySearch.CategoryId == currentCategories.CategoryId
                        ).ToList());
                }

                inequalities = inequalitiesFiltered;
            }
            else
            {
                // Get defined inequalities
                inequalities = GetFilteredInequalitiesByCategoryType(inequalities);
            }

            return inequalities;
        }

        private IList<CoreDataSet> GetDataFilteredByInequalitiesSubNational(IList<InequalitySearch> inequalities, IList<CoreDataSet> dataIncludingInequalities)
        {
            var resultCoreDataSetList = new List<CoreDataSet>();

            foreach (var inequality in inequalities)
            {
                if (inequality == null)
                {
                    return dataIncludingInequalities;
                }

                InitializeGeneralInequalitiesAttributes(inequality);
                var filteredDataByInequality = dataIncludingInequalities.Where(childCore =>
                    childCore.CategoryTypeId == inequality.CategoryInequalitySearch.CategoryTypeId
                    && childCore.CategoryId == inequality.CategoryInequalitySearch.CategoryId
                    && childCore.AgeId == inequality.SexAgeInequalitySearch.AgeId
                    && childCore.SexId == inequality.SexAgeInequalitySearch.SexId).ToList();

                var isCategoryTypeId = ExportAreaHelper.IsCategoryAreaTypeId(_generalParameters.ParentAreaTypeId);
                var dataExcludingNationalData = GetCoreDataSetExcludingNationalData(filteredDataByInequality, isCategoryTypeId).ToList();
                resultCoreDataSetList.AddRange(dataExcludingNationalData);
            }

            return resultCoreDataSetList;
        }

        private static IEnumerable<CoreDataSet> GetCoreDataSetExcludingNationalData(IEnumerable<CoreDataSet> filteredDataByInequality, bool isCategoryTypeId)
        {
            return filteredDataByInequality.Where(element => element.AreaCode != AreaCodes.England || element.AreaCode == AreaCodes.England && isCategoryTypeId);
        }

        public IList<CoreDataSet> GetFilterCoreDataForChildArea(int indicatorId, TimePeriod timePeriod)
        {
            var isNearestNeighbours = false;

            // Is nearest neighbours?
            var childAreaCodeList = OnDemandQueryParameters.ChildAreaCodeList;
            if (childAreaCodeList != null)
            {
                isNearestNeighbours = NearestNeighbourArea.IsNearestNeighbourAreaCode(childAreaCodeList[0]);
            }

            var childAreaCodes = GetChildAreaCodes(isNearestNeighbours);

            // It doesn't make any filter if inequalities variable is null
            var childCoreDataList = FilterCoreDataByInequalitiesLocal(indicatorId, timePeriod, childAreaCodes);

            if (!isNearestNeighbours)
            {
                FilterChildCoreDataByChildAreaCodeList(ref childCoreDataList);
            }

            if (OnDemandQueryParameters.AreQuinaryPopulations)
            {
                childCoreDataList = ExportPopulationHelper.FilterQuinaryPopulations(childCoreDataList);
            }

            return childCoreDataList;
        }

        private string[] GetChildAreaCodes(bool isNearestNeighbours)
        {
            var childAreaCodeList = OnDemandQueryParameters.ChildAreaCodeList;

            string[] childAreaCodes;
            if (isNearestNeighbours)
            {
                childAreaCodes = new[] {NearestNeighbourArea.GetAreaCodeFromNeighbourAreaCode(childAreaCodeList[0])};
            }
            else if (childAreaCodeList != null && childAreaCodeList.Any())
            {
                childAreaCodes = childAreaCodeList.ToArray();
            }
            else
            {
                childAreaCodes = AreaHelper.ChildAreaCodes;
            }

            return childAreaCodes;
        }

        public IList<CoreDataSet> FilterCoreDataByInequalitiesLocal(int indicatorId, TimePeriod timePeriod, params string[] areaCode)
        {
            var dataIncludingInequalities = _groupDataReader.GetDataIncludingInequalities(indicatorId, timePeriod,
                AttributesForPeriods.ExcludedCategoryTypeIds, areaCode);

            if (HasNotInequalitiesFilter(indicatorId, dataIncludingInequalities))
            {
                return dataIncludingInequalities;
            }

            var inequalities = OnDemandQueryParameters.Inequalities[indicatorId];

            return GetDataFilteredByInequalities(inequalities, dataIncludingInequalities);
        }

        private bool HasNotInequalitiesFilter(int indicatorId, IList<CoreDataSet> dataIncludingInequalities)
        {
            return dataIncludingInequalities.Count == 0 ||
                   OnDemandQueryParameters.Inequalities.Count == 0 ||
                   OnDemandQueryParameters.Inequalities.ContainsKey(indicatorId) == false ||
                   OnDemandQueryParameters.Inequalities[indicatorId] == null ||
                   OnDemandQueryParameters.Inequalities[indicatorId].Count == 0;
        }

        public CoreDataSet CalculateAverageForAreaListCode(IndicatorMetadata indicatorMetadata, TimePeriod timePeriod,
            IArea parentArea, IList<CoreDataSet> coreDataList)
        {
            var averageCalculator = AverageCalculatorFactory.New(coreDataList, indicatorMetadata);
            return _benchmarkDataProvider.CalculateBenchmarkDataAverage(_grouping, timePeriod, averageCalculator, parentArea);
        }

        private void InitializeGeneralInequalitiesAttributes(InequalitySearch inequality)
        {
            var search = inequality.CategoryInequalitySearch;

            if (inequality.SexAgeInequalitySearch == null || search == null)
            {
                throw new ArgumentEmptyException(string.Format("{0}", "The attributes for inequalitySearch received are empty"));
            }

            if (search.CategoryTypeId != 0 || search.CategoryId != 0) return;

            search.CategoryTypeId = CategoryTypeIds.Undefined;
            search.CategoryId = CategoryIds.Undefined;
        }
    }
}
