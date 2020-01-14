using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.Analysis;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export.FileBuilder.Containers
{
    public class BodyPeriodComparisonContainer
    {
        private readonly ExportAreaHelper _areaHelper;
        private readonly IndicatorComparer _comparer;

        public BodyPeriodComparisonContainer(ExportAreaHelper areaHelper, IndicatorComparer comparer)
        {
            _areaHelper = areaHelper;
            _comparer = comparer;
        }

        public static IList<CoreDataSet> CloneCoreDataForComparison(IList<CoreDataSet> coreDataForComparison)
        {
            return new List<CoreDataSet>(coreDataForComparison.Select(x => (CoreDataSet)x.Clone()).ToList());
        }

        public CategoryComparisonManager GetCategoryComparisonManager(IList<CoreDataSet> englandCoreDataForComparison, IList<CoreDataSet> parentCoreDataForComparison, IList<CoreDataSet> childCoreDataList)
        {
            // Set the data for category comparators
            CategoryComparisonManager categoryComparisonManager = null;

            // Category comparers (e.g. quintiles)
            var categoryComparer = _comparer as ICategoryComparer;
            if (categoryComparer != null)
            {
                categoryComparisonManager = new CategoryComparisonManager(categoryComparer, childCoreDataList);
                categoryComparisonManager.InitNationalData(englandCoreDataForComparison);
                categoryComparisonManager.InitSubnationalParentData(_areaHelper.ParentAreaCodeToChildAreaCodesMap, parentCoreDataForComparison);
            }

            return categoryComparisonManager;
        }

        public static IEnumerable<CoreDataSet> GetCoreDataForComparisonToWrite(ref IList<CoreDataSet> coreDataForComparison, IEnumerable<CoreDataSet> coreDataList, int categoryTypeId)
        {
            var coreDataForComparisonAux = BodyPeriodSignificanceContainer.GetCoreDataForComparison(coreDataList, categoryTypeId);

            IList<CoreDataSet> coreDataForComparisonToWrite = null;

            coreDataForComparisonToWrite = coreDataForComparison != null ? coreDataForComparison : coreDataForComparisonAux;

            coreDataForComparison = coreDataForComparisonAux;
            return coreDataForComparisonToWrite;
        }
    }
}
