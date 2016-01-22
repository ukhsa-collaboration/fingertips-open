using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class MostRecentDataForAllSexesAction : MostRecentDataActionBase
    {
        public MostRecentDataForAllSexes GetResponse(int profileId = 0,
            string areaCode = null, int indicatorId = 0, int ageId = 0, int areaTypeId = 0)
        {
            ValidateParameters(profileId, areaTypeId, indicatorId, areaCode);
            InitGrouping(profileId, areaTypeId, indicatorId, ageId);
            InitMetadataAndTimePeriod(grouping);

            // Get Data
            IList<CoreDataSet> dataList = groupDataReader.GetCoreDataForAllSexes(indicatorId,
                timePeriod, areaCode, ageId);

            // Define and order sexes
            List<int> sexIds = new CoreDataSetFilter(dataList).SelectDistinctSexIds().ToList();
            IList<Sex> sexes = pholioReader.GetSexesByIds(sexIds);

            // Process data list
            dataList = new CoreDataSetSorter(dataList).SortBySexId(sexes);
            CalculateSignificances(areaCode, dataList);
            FormatData(dataList);

            return new MostRecentDataForAllSexes
            {
                AreaCode = areaCode,
                IndicatorId = indicatorId,
                AgeId = ageId,
                Sexes = sexes,
                Data = dataList
            };
        }

        private void InitGrouping(int profileId, int areaTypeId, int indicatorId, int ageId)
        {
            var groupingProvider = new SingleGroupingProvider(groupDataReader, new GroupIdProvider(profileReader));
            grouping = groupingProvider.GetGroupingByIndicatorIdAndAgeId(profileId, areaTypeId, indicatorId, ageId);
        }

        protected override void CalculateSignificances(string areaCode, IList<CoreDataSet> categoryDataList)
        {
            var area = AreaFactory.NewArea(areasReader, areaCode);
            var nationalArea = GetNationalArea(area);

            var personsData = categoryDataList.FirstOrDefault(x => x.SexId == SexIds.Persons);

            if (personsData == null && indicatorMetadata.HasTarget == false)
            {
                // Do not calculate significance
                foreach (var coreDataSet in categoryDataList)
                {
                    coreDataSet.SignificanceAgainstOneBenchmark = (int)Significance.None;
                }
            }
            else
            {
                // Calculate significance (is data for persons or there is a target)
                var indicatorComparisonHelper = new IndicatorComparisonHelper(indicatorMetadata,
                    grouping, groupDataReader, pholioReader, nationalArea);
                foreach (var coreDataSet in categoryDataList)
                {
                    coreDataSet.SignificanceAgainstOneBenchmark =
                        indicatorComparisonHelper.GetSignificance(coreDataSet, personsData);
                }
            }
        }
    }
}