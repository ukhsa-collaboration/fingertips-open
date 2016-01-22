using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class BenchmarkDataProvider
    {
        private IGroupDataReader groupDataReader;

        public BenchmarkDataProvider(IGroupDataReader groupDataReader)
        {
            this.groupDataReader = groupDataReader;
        }

        public CoreDataSet GetBenchmarkData(Grouping grouping, TimePeriod timePeriod,
            AverageCalculator averageCalculator, IArea parentArea)
        {
            var benchmarkData = GetDataFromDatabase(grouping, timePeriod, parentArea);

            if (benchmarkData == null && averageCalculator != null)
            {
                benchmarkData = averageCalculator.Average;
                if (benchmarkData != null)
                {
                   benchmarkData.AreaCode = parentArea.Code;
                }
            }

            if (benchmarkData == null)
            {
                benchmarkData = CoreDataSet.GetNullObject(parentArea.Code);
            }

            return benchmarkData;
        }

        private CoreDataSet GetDataFromDatabase(Grouping grouping, TimePeriod timePeriod, IArea parentArea)
        {
            CoreDataSet benchmarkData;

            var categoryArea = parentArea as CategoryArea;
            if (categoryArea != null)
            {
                benchmarkData = groupDataReader.GetCoreDataForCategoryArea(grouping, timePeriod, categoryArea);
            }
            else
            {
                IList<CoreDataSet> data = groupDataReader.GetCoreData(grouping, timePeriod, parentArea.Code);
                CheckOnlyOneCoreDataSetFound(grouping, parentArea, data);
                benchmarkData = data.FirstOrDefault();
            }
            return benchmarkData;
        }

        private static void CheckOnlyOneCoreDataSetFound(Grouping grouping, IArea area,
            IList<CoreDataSet> dataList)
        {
            if (dataList.Count > 1)
            {
                throw new FingertipsException(string.Format("More than one CoreDataSet row for: IndicatorId={0} Area={1}",
                    grouping.IndicatorId, area.Code));
            }
        }
    }
}