using System.Collections.Generic;
using System.Linq;
using Ckan.Model;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataSorting;
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
            var timePeriods = TimePeriodIterator.TimePeriodsFromGrouping(grouping, yearType);

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

                        IList<CoreDataSet> dataList;
                        IEnumerable<CkanCoreDataSet> ckanDataList;

                        // Area types
                        foreach (var areaTypeId in areaTypeIds)
                        {
                            grouping.AreaTypeId = areaTypeId;
                            dataList = GroupDataReader
                                .GetCoreDataForAllAreasOfType(grouping, timePeriod);

                            ckanDataList = FilterAndConvert(areaCodesToIgnore, dataList, periodString);

                            allDataList.AddRange(ckanDataList);
                        }

                        // Category types
                        dataList = GroupDataReader.GetCoreDataForCategoryTypeIds(
                            grouping, timePeriod, categoryTypeIds.ToArray());

                        ckanDataList = FilterAndConvert(areaCodesToIgnore, dataList, periodString);

                        allDataList.AddRange(ckanDataList);
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