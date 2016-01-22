using System.Collections.Generic;
using System.Linq;
using Ckan.Model;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace Ckan.DataTransformation
{
    public class CoreDataListBuilder
    {
        public IGroupDataReader GroupDataReader { get; set; }

        public List<CkanCoreDataSet> GetData(YearType yearType, Grouping grouping,
            IList<int> sexIds, IList<int> ageIds, IList<int> areaTypeIds,
            IList<int> categoryTypeIds, IList<string> areaCodesToIgnore)
        {
            var timePeriods = new TimePeriodIterator(
                TimePeriod.GetBaseline(grouping),
                TimePeriod.GetDataPoint(grouping), yearType).TimePeriods;

            var allDataList = new List<CkanCoreDataSet>();
            foreach (var sexId in sexIds)
            {
                foreach (var ageId in ageIds)
                {
                    foreach (var timePeriod in timePeriods)
                    {
                        var periodString = TimePeriodFormatter.GetTimePeriodString(timePeriod, yearType.Id);

                        grouping.SexId = sexId;
                        grouping.AgeId = ageId;

                        foreach (var areaTypeId in areaTypeIds)
                        {
                            grouping.AreaTypeId = areaTypeId;
                            var dataList = GroupDataReader
                                .GetCoreDataForAllAreasOfType(grouping, timePeriod);

                            var ckanDataList = FilterAndConvert(areaCodesToIgnore, dataList, periodString);

                            allDataList.AddRange(ckanDataList);
                        }

                        foreach (var categoryTypeId in categoryTypeIds)
                        {
                            var dataList = GroupDataReader.GetCoreDataForCategoryTypeId(
                                grouping, timePeriod, categoryTypeId);

                            var ckanDataList = FilterAndConvert(areaCodesToIgnore, dataList, periodString);

                            allDataList.AddRange(ckanDataList);
                        }
                    }
                }
            }
            return allDataList;
        }

        private static IEnumerable<CkanCoreDataSet> FilterAndConvert(IList<string> areaCodesToIgnore,
            IList<CoreDataSet> dataList, string periodString)
        {
            dataList = new CoreDataSetFilter(dataList)
                .RemoveWithAreaCode(areaCodesToIgnore).ToList();

            var ckanDataList = dataList
                .Select(x => new CkanCoreDataSet(x, periodString));

            return ckanDataList;
        }
    }
}