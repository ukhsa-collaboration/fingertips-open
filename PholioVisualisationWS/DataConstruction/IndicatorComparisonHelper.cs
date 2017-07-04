using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class IndicatorComparisonHelper
    {
        private IndicatorMetadata indicatorMetadata;
        private IndicatorComparer comparer;
        private TargetComparer targetComparer;
        private Grouping grouping;
        private IGroupDataReader groupDataReader;

        public IndicatorComparisonHelper(IndicatorMetadata indicatorMetadata, Grouping grouping,
            IGroupDataReader groupDataReader, PholioReader pholioReader, TargetComparerProvider targetComparerProvider)
        {
            // Assign constructor parameter to instance variables
            this.indicatorMetadata = indicatorMetadata;
            this.grouping = grouping;
            this.groupDataReader = groupDataReader;

            InitComparer(pholioReader, targetComparerProvider);
        }

        private void InitComparer(PholioReader pholioReader, TargetComparerProvider targetComparerProvider)
        {
            if (targetComparerProvider != null && indicatorMetadata.HasTarget)
            {
                targetComparer = targetComparerProvider.GetTargetComparer(indicatorMetadata, grouping, TimePeriod.GetDataPoint(grouping));
            }
            else
            {
                comparer = new IndicatorComparerFactory {PholioReader = pholioReader}.New(grouping);
            }
        }

        /// <summary>
        /// Gets the significance of the data value against the benchmark 
        /// or against a target 
        /// or the category a value falls in, e.g. quintile
        /// </summary>
        /// <param name="data">The data that the comparison will be made against</param>
        /// <param name="benchmarkData">Is not required if the comparison is against a target or to determine the category a value falls in</param>
        public int GetSignificance(CoreDataSet data, CoreDataSet benchmarkData)
        {
            if (targetComparer != null)
            {
                return (int)targetComparer.CompareAgainstTarget(data);
            }

            ICategoryComparer categoryComparer = comparer as ICategoryComparer;
            if (categoryComparer != null)
            {
                return categoryComparer.GetCategory(data);
            }

            return (int)comparer.Compare(data, benchmarkData, indicatorMetadata);
        }

        /// <summary>
        /// Assigns required data if the comparer calculate categories.
        /// </summary>
        /// <param name="subnationalDataList">Use null if you know the categories will be national</param>
        public void AssignCategoryDataIfRequired(IList<CoreDataSet> subnationalDataList)
        {
            AssignCategoryDataIfRequired(comparer, grouping, groupDataReader, subnationalDataList);
        }

        /// <summary>
        /// Assigns required data if the comparer calculate categories.
        /// </summary>
        /// <param name="subnationalDataList">Use null if you know the categories will be national</param>
        public static void AssignCategoryDataIfRequired(IndicatorComparer comparer, Grouping grouping, 
            IGroupDataReader groupDataReader, IList<CoreDataSet> subnationalDataList)
        {
            ICategoryComparer categoryComparer = comparer as ICategoryComparer;
            if (categoryComparer != null)
            {
                IList<CoreDataSet> coreDataList = null;

                if (grouping.ComparatorId == ComparatorIds.England)
                {
                    coreDataList = groupDataReader.GetCoreDataForAllAreasOfType(
                    grouping, TimePeriod.GetDataPoint(grouping));
                }
                else
                {
                    if (subnationalDataList != null)
                    {
                        coreDataList = subnationalDataList;
                    }
                }

                if (coreDataList != null)
                {
                    var values = new CoreDataSetFilter(coreDataList).SelectValidValues().ToList();
                    categoryComparer.SetDataForCategories(values);
                }
            }
        }
    }
}
