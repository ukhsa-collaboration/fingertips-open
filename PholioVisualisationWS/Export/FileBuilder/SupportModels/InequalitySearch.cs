using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.Export.FileBuilder.SupportModels
{
    public class InequalitySearch
    {
        public SexAgeInequalitySearch SexAgeInequalitySearch { get; set; }
        public CategoryInequalitySearch CategoryInequalitySearch { get; set; }
        
        public InequalitySearch(int categoryTypeId, int categoryId, int sexId, int ageId)
        {
            SexAgeInequalitySearch = new SexAgeInequalitySearch(sexId, ageId);
            CategoryInequalitySearch = new CategoryInequalitySearch(categoryTypeId, categoryId);
        }

        public InequalitySearch(int categoryTypeId, int categoryId)
        {
            SexAgeInequalitySearch = new SexAgeInequalitySearch();
            CategoryInequalitySearch = new CategoryInequalitySearch(categoryTypeId, categoryId);
        }

        public InequalitySearch(CategoryInequalitySearch categoryInequalitySearch, SexAgeInequalitySearch sexAgeInequalitySearch)
        {
            SexAgeInequalitySearch = sexAgeInequalitySearch;
            CategoryInequalitySearch = categoryInequalitySearch;
        }

        public InequalitySearch()
        {
        }

        public static IList<InequalitySearch> CombineTwoListsIntoInequalitySearchList(IList<CategoryInequalitySearch> categoryInequalitySearchList,
            IList<SexAgeInequalitySearch> sexAgeInequalitySearchList)
        {
            return (from categoryInequalitySearch in categoryInequalitySearchList
                    from sexAgeInequalitySearch in sexAgeInequalitySearchList
                    select new InequalitySearch(categoryInequalitySearch, sexAgeInequalitySearch)).ToList();
        }

        public static IDictionary<int, IList<InequalitySearch>> AddInequalitiesToDictionary(IGroupDataReader groupDataReader, IAreasReader areasReader, 
            GroupingInequality groupingInequality, string[] categoryAreaCodesArray, IDictionary<int, IList<InequalitySearch>> dictionaryTarget)
        {
            dictionaryTarget = InitializeDictionary(dictionaryTarget);

            var listCategoryInequalities = GetListCategoryInequalities(categoryAreaCodesArray, areasReader);

            foreach (var categoryInequality in listCategoryInequalities)
            {
                var groupingSexAgeInequality = new SexAgeInequalitySearch(groupingInequality.Sex, groupingInequality.Age);
                var inequalitySearchTarget = new InequalitySearch(categoryInequality, groupingSexAgeInequality);

                if (dictionaryTarget.ContainsKey(groupingInequality.IndicatorId))
                {
                    IList<InequalitySearch> listInequalitySearch;
                    dictionaryTarget.TryGetValue(groupingInequality.IndicatorId, out listInequalitySearch);

                    if (listInequalitySearch == null)
                    {
                        listInequalitySearch = new List<InequalitySearch>();
                        dictionaryTarget[groupingInequality.IndicatorId] = listInequalitySearch;
                    }

                    if (ContainsInequality(listInequalitySearch, inequalitySearchTarget)) continue;

                    var cloneInequalitySearch = CloneInequalitySearch(inequalitySearchTarget);
                    listInequalitySearch.Add(cloneInequalitySearch);
                }
                else
                {
                    var inequalitySearch = new List<InequalitySearch> { inequalitySearchTarget };
                    dictionaryTarget.Add(groupingInequality.IndicatorId, inequalitySearch);
                }
            }

            return dictionaryTarget;
        }

        public static IList<CategoryInequalitySearch> GetListCategoryInequalities(string[] categoryAreaCodesArray, IAreasReader areasReader)
        {
            var listCategoryInequalities = new List<CategoryInequalitySearch>
            {
                CategoryInequalitySearch.GetUndefinedCategoryInequality()
            };

            if (categoryAreaCodesArray == null || categoryAreaCodesArray.Length == 0)
            {
                return listCategoryInequalities;
            }

            foreach (var categoryAreaCode in categoryAreaCodesArray)
            {
                var categoryInequality = GetCategoryInequalitySearch(categoryAreaCode, areasReader);
                listCategoryInequalities.Add(categoryInequality);
            }

            return listCategoryInequalities;
        }

        public static CategoryInequalitySearch GetCategoryInequalitySearch(string areaCode, IAreasReader areasReader)
        {
            if (string.IsNullOrEmpty(areaCode))
            {
                throw new ArgumentException("The areaCode must contains a value");
            }

            if (!ExportAreaHelper.IsCategoryAreaCode(areaCode))
            {
                throw new ArgumentException("The areaCode must be a categoryAreaCode");
            }

            var categoryInequality = ExportAreaHelper.GetCategoryInequalityFromAreaCode(areaCode, areasReader);

            return categoryInequality;
        }

        public static IList<AreaTypeIdSexAgeInequalitySearch> GetGroupingSexAgeInequalityByIndicatorId(int indicatorId, IGroupDataReader groupDataReader)
        {
            var groupingList = groupDataReader.GetGroupingsByIndicatorId(indicatorId);

            var groupingAreaTypeSexAgeInequalities = groupingList
                .GroupBy(x => new { x.IndicatorId, x.AreaTypeId, x.AgeId, x.SexId })
                .Select(x => new AreaTypeIdSexAgeInequalitySearch(x.Key.AreaTypeId, x.Key.SexId, x.Key.AgeId))
                .Where(x => x.SexId != 0 || x.AgeId != 0)
                .ToList();

            return groupingAreaTypeSexAgeInequalities;
        }

        public static string[] GetCategoriesByIndicatorId(int indicatorId, int sexId, int ageId, IGroupDataReader groupDataReader)
        {
            var categories = groupDataReader.GetCoreDataForIndicatorId(indicatorId)
                .Where(x => x.CategoryTypeId != CategoryTypeIds.Undefined && x.CategoryId != CategoryIds.Undefined && x.SexId == sexId && x.AgeId == ageId)
                .GroupBy(x => new {x.CategoryTypeId, x.CategoryId})
                .Distinct()
                .Select(x => new CategoryInequalitySearch(x.Key.CategoryTypeId, x.Key.CategoryId).ConvertIntoCategoryAreaCode())
                .ToArray();

            return categories;
        }

        public static IList<SexAgeInequalitySearch> GetGroupingSexAgeInequalityByIndicatorGroupAreaType(GroupingInequality groupingInequality, IGroupDataReader groupDataReader)
        {
            var groupingList = groupingInequality.GroupId != null ?
                groupDataReader.GetGroupingListByGroupIdIndicatorIdAreaType((int)groupingInequality.GroupId, groupingInequality.IndicatorId, groupingInequality.AreaTypeId) :
                groupDataReader.GetGroupingListByIndicatorIdAreaType(groupingInequality.IndicatorId, groupingInequality.AreaTypeId);

            IList<SexAgeInequalitySearch> groupingSexAgeInequalities;
            if (groupingList != null)
            {
                groupingSexAgeInequalities = groupingList
                    .GroupBy(x => new {x.IndicatorId, x.AreaTypeId, x.AgeId, x.SexId})
                    .Select(x => new SexAgeInequalitySearch(x.Key.SexId, x.Key.AgeId))
                    .ToList();
            }
            else
            {
                groupingSexAgeInequalities = new List<SexAgeInequalitySearch>();
            }

            var groupingSexAgeInequalitiesPeopleFilter = groupingSexAgeInequalities.Where(x => x.SexId == SexIds.Persons && x.AgeId == AgeIds.AllAges).ToList();

            return groupingSexAgeInequalitiesPeopleFilter.Count != 0 ? groupingSexAgeInequalitiesPeopleFilter : groupingSexAgeInequalities;
        }

        /// <summary>
        /// Check if inequality is contained in the list
        /// </summary>
        public static bool ContainsInequality(IList<InequalitySearch> listTarget, InequalitySearch inequalityComparator)
        {
            return listTarget.Any(inequality => inequality.SexAgeInequalitySearch.AgeId == inequalityComparator.SexAgeInequalitySearch.AgeId &&
                                                inequality.SexAgeInequalitySearch.SexId == inequalityComparator.SexAgeInequalitySearch.SexId &&
                                                inequality.CategoryInequalitySearch.CategoryTypeId == inequalityComparator.CategoryInequalitySearch.CategoryTypeId &&
                                                inequality.CategoryInequalitySearch.CategoryId == inequalityComparator.CategoryInequalitySearch.CategoryId);
        }

        public static InequalitySearch CloneInequalitySearch(InequalitySearch inequalityOrigin)
        {
            SexAgeInequalitySearch sexAgeInequalitySearch = null;
            if (inequalityOrigin.SexAgeInequalitySearch != null)
            {
                sexAgeInequalitySearch = new SexAgeInequalitySearch(inequalityOrigin.SexAgeInequalitySearch.SexId, inequalityOrigin.SexAgeInequalitySearch.AgeId);
            }

            if (sexAgeInequalitySearch == null)
            {
                sexAgeInequalitySearch = new SexAgeInequalitySearch();
            }

            return new InequalitySearch
            {
                SexAgeInequalitySearch = sexAgeInequalitySearch,
                CategoryInequalitySearch = new CategoryInequalitySearch(inequalityOrigin.CategoryInequalitySearch.CategoryTypeId, inequalityOrigin.CategoryInequalitySearch.CategoryId)
            };
        }

        private static IDictionary<int, IList<InequalitySearch>> InitializeDictionary(IDictionary<int, IList<InequalitySearch>> dictionaryTarget)
        {
            var listOfElementsInDictionaryTarget = dictionaryTarget.ToList();

            foreach (var element in listOfElementsInDictionaryTarget)
            {
                if (element.Value != null) continue;

                IList<InequalitySearch> listInequalitySearch;
                dictionaryTarget.TryGetValue(element.Key, out listInequalitySearch);

                if (listInequalitySearch != null) continue;

                listInequalitySearch = new List<InequalitySearch>();
                dictionaryTarget[element.Key] = listInequalitySearch;
            }

            return dictionaryTarget;
        }
    }
}
