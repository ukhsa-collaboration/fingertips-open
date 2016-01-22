using System.Linq;
using PholioVisualisation.DataConstruction;

namespace PholioVisualisation.ServiceActions
{
    public class MostRecentDataForAllAgesAction : MostRecentDataActionBase
    {
        public MostRecentDataForAllAges GetResponse(int profileId = 0,
            string areaCode = null, int indicatorId = 0, int sexId = 0, int areaTypeId = 0)
        {
            ValidateParameters(profileId, areaTypeId, indicatorId, areaCode);    
            InitGrouping(profileId, areaTypeId, indicatorId, sexId); 
            InitMetadataAndTimePeriod(grouping);

            // Get Data
            var dataList = groupDataReader.GetCoreDataForAllAges(indicatorId, 
                timePeriod, areaCode, sexId);

            // Define and order agess
            var ageIds = new CoreDataSetFilter(dataList).SelectDistinctAgeIds().ToList();
            var ages = pholioReader.GetAgesByIds(ageIds);
            ages = new AgeSorter().SortByAge(ages);

            // Process data list
            dataList = new CoreDataSetSorter(dataList).SortByAgeId(ages);
            CalculateSignificances(areaCode, dataList);
            FormatData(dataList);

            return new MostRecentDataForAllAges
            {
                AreaCode = areaCode,
                IndicatorId = indicatorId,
                SexId = sexId,
                Ages = ages,
                Data = dataList
            };
        }

        private void InitGrouping(int profileId, int areaTypeId, int indicatorId, int sexId)
        {
            var groupingProvider = new SingleGroupingProvider(groupDataReader, new GroupIdProvider(profileReader));
            grouping = groupingProvider.GetGroupingByIndicatorIdAndSexId(profileId, areaTypeId, indicatorId, sexId);
        }

    }
}