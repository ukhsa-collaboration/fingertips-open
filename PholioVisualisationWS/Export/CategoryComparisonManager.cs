using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export
{
    public class CategoryComparisonManager
    {
        private Dictionary<CategoryComparisonDifferentiator, ICategoryComparer> _comparatorMap =
            new Dictionary<CategoryComparisonDifferentiator, ICategoryComparer>();

        private IList<CoreDataSet> _childAreaCoreDatas;
        private ICategoryComparer _templateComparer;

        public CategoryComparisonManager(ICategoryComparer templateComparer, IList<CoreDataSet> childAreaCoreDatas)
        {
            _templateComparer = templateComparer;

            // Remove category data from child areas
            _childAreaCoreDatas = childAreaCoreDatas
                .Where(x => x.CategoryTypeId == CategoryTypeIds.Undefined)
                .ToList();
        }

        public void InitNationalData(IList<CoreDataSet> englandCoreDatasForComparison)
        {
            // Init the comparers
            foreach (var coreData in englandCoreDatasForComparison)
            {
                var differentiator = new CategoryComparisonDifferentiator(coreData, AreaCodes.England);

                // Get data for all child areas
                var differentiatedData = _childAreaCoreDatas.Where(x => x.AgeId == differentiator.AgeId &&
                                                         x.SexId == differentiator.SexId);

                AddComparer(differentiatedData, differentiator);
            }
        }

        public void InitSubnationalParentData(IDictionary<string, List<string>> parentAreaCodeToChildAreasMap, 
            IList<CoreDataSet> parentCoreDatasForComparison)
        {
            foreach (var parentAreaCode in parentAreaCodeToChildAreasMap.Keys)
            {
                var parentDataList = parentCoreDatasForComparison.Where(x => x.AreaCode == parentAreaCode);
                foreach (var parentData in parentDataList)
                {
                    var differentiator = new CategoryComparisonDifferentiator(parentData, parentAreaCode);
 
                    // Get child data
                    var childAreaCodes = parentAreaCodeToChildAreasMap[parentAreaCode];
                    var differentiatedData = _childAreaCoreDatas.Where(x => childAreaCodes.Contains(x.AreaCode) &&
                        x.AgeId == differentiator.AgeId && x.SexId == differentiator.SexId);

                    AddComparer(differentiatedData, differentiator);
                }
            }
        }

        /// <summary>
        /// Determines the appropriate category for the data
        /// </summary>
        public Significance GetCategory(CoreDataSet coreData, string parentCode)
        {
            if (coreData != null)
            {
                var differentiator = new CategoryComparisonDifferentiator(coreData, parentCode);
                if (_comparatorMap.ContainsKey(differentiator))
                {
                    return (Significance)_comparatorMap[differentiator].GetCategory(coreData);
                }
            }

            return Significance.None;
        }

        private void AddComparer(IEnumerable<CoreDataSet> differentiatedData, CategoryComparisonDifferentiator differentiator)
        {
            // Create new comparer
            var comparer = _templateComparer.NewInstance();

            // Init comparer with values for comparison
            var values = new CoreDataSetFilter(differentiatedData).SelectValidValues().ToList();
            comparer.SetDataForCategories(values);

            if (_comparatorMap.ContainsKey(differentiator))
            {
                _comparatorMap.Add(differentiator, comparer);
            }
        }
    }
}