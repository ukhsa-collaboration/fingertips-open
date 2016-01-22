using System.Collections.Generic;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    /// <summary>
    ///     Provides a list of CoreDataSet objects.
    /// </summary>
    public class CoreDataSetListProvider
    {
        private IGroupDataReader groupDataReader;

        /// <summary>
        ///     Parameterless constructor required for mocking.
        /// </summary>
        protected CoreDataSetListProvider()
        {
        }

        public CoreDataSetListProvider(IGroupDataReader groupDataReader)
        {
            this.groupDataReader = groupDataReader;
        }

        public virtual IList<CoreDataSet> GetChildAreaData(Grouping grouping, IArea parentArea, TimePeriod period)
        {
            var categoryArea = parentArea as CategoryArea;
            if (categoryArea != null)
            {
                return groupDataReader.GetCoreDataListForChildrenOfCategoryArea(
                    grouping, period, categoryArea);
            }

            return parentArea.IsCountry
                ? groupDataReader.GetCoreDataForAllAreasOfType(grouping, period)
                : groupDataReader.GetCoreDataListForChildrenOfArea(grouping, period, parentArea.Code);
        }
    }
}