using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.Analysis;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export.FileBuilder.Containers
{
    public class BodyPeriodSignificanceContainer
    {
        public IndicatorComparer Comparer;
        private readonly ExportAreaHelper _areaHelper;

        public BodyPeriodSignificanceContainer(ExportAreaHelper areaHelper, IndicatorComparer comparer)
        {
            _areaHelper = areaHelper;
            Comparer = comparer;
        }

        public Significance GetSignificance(IndicatorMetadata indicatorMetadata, CoreDataSet coreData, IEnumerable<CoreDataSet> coreDataForComparison, IArea parentArea)
        {
            Significance significance;
            if (IsSignificanceNone(coreData, parentArea, out significance)) return significance;

            var matchingCoreDataSet = BodyPeriodSearchContainer.FindMatchingCoreDataSet(coreDataForComparison, coreData);
            significance = TakeSignificance(coreData, matchingCoreDataSet, indicatorMetadata);

            return significance;
        }

        public Significance GetSignificance(CategoryComparisonManager categoryComparisonManager, IEnumerable<CoreDataSet> englandCoreDataForComparison, IndicatorMetadata indicatorMetadata, CoreDataSet coreData)
        {
            // England significances
            Significance toEnglandSignificance;
            if (categoryComparisonManager != null)
            {
                // Find category
                toEnglandSignificance = categoryComparisonManager.GetCategory(coreData, AreaCodes.England);
            }
            else
            {
                // Benchmark comparison
                var englandData = BodyPeriodSearchContainer.FindMatchingCoreDataSet(englandCoreDataForComparison, coreData);
                toEnglandSignificance = TakeSignificance(coreData, englandData, indicatorMetadata);
            }

            return toEnglandSignificance;
        }


        public static List<CoreDataSet> GetCoreDataForComparison(IEnumerable<CoreDataSet> coreData, int categoryTypeId)
        {
            return coreData.Where(x => x.CategoryTypeId == categoryTypeId).ToList();
        }

        public Significance GetSubNationalParentSignificance(CategoryComparisonManager categoryComparisonManager,
            IEnumerable<CoreDataSet> parentCoreDataForComparison, IndicatorMetadata indicatorMetadata, CoreDataSet coreData, bool isParentEngland, out IArea parentArea)
        {
            // SubNational significances
            var toSubNationalParentSignificance = Significance.None;
            parentArea = null;
            if (isParentEngland)
            {
                parentArea = _areaHelper.England;
            }
            else
            {
                if (!_areaHelper.ChildAreaCodeToParentAreaMap.ContainsKey(coreData.AreaCode)) return toSubNationalParentSignificance;

                parentArea = _areaHelper.ChildAreaCodeToParentAreaMap[coreData.AreaCode];

                if (categoryComparisonManager != null)
                {
                    // Find category
                    toSubNationalParentSignificance = categoryComparisonManager.GetCategory(coreData, parentArea.Code);
                }
                else
                {
                    // Benchmark comparison
                    var parentData = BodyPeriodSearchContainer.FindMatchingCoreDataSet(parentCoreDataForComparison, coreData, parentArea.Code);
                    toSubNationalParentSignificance = TakeSignificance(coreData, parentData, indicatorMetadata);
                }
            }

            return toSubNationalParentSignificance;
        }

        public Significance TakeSignificance(CoreDataSet coreData, CoreDataSet englandData, IndicatorMetadata indicatorMetadata)
        {
            if (Comparer != null)
            {
                return Comparer.Compare(coreData, englandData, indicatorMetadata);
            }
            return Significance.None;
        }

        private bool IsSignificanceNone(CoreDataSet coreData, IArea parentArea, out Significance significance)
        {
            // England significance
            significance = Significance.None;

            // Category comparers (e.g. quintiles)
            var categoryComparer = Comparer as ICategoryComparer;

            // If category type is undefined then would be comparing same value
            if (categoryComparer != null)
            {
                return true;
            }

            return parentArea == null && coreData.CategoryTypeId == CategoryTypeIds.Undefined;
        }
    }
}
