using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class QuinaryPopulationBuilder
    {
        private static int[] sexIds = { SexIds.Male, SexIds.Female };

        // Input
        public int GroupId { get; set; }
        public string AreaCode { get; set; }
        public int DataPointOffset { get; set; }

        /// <summary>
        /// Only applies to practices. If true then other stats are calculated, e.g.
        /// QOF points, life expectancy, etc.
        /// </summary>
        public bool AreOnlyPopulationsRequired { get; set; }

        // Output
        /// <summary>
        /// Key is SexId, Value is population count
        /// </summary>
        public Dictionary<int, IEnumerable<double>> Values { get; private set; }

        // Output - Practice only 
        public string EthnicityText { get; private set; }
        public int? DeprivationDecile { get; private set; }
        public int? Shape { get; private set; }
        public ValueData ListSize { get; private set; }
        public Dictionary<string, ValueData> AdHocValues { get; private set; }

        private TimePeriod period;
        private IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();

        /// <summary>
        /// Key is SexId, value is list of values from 0-4 up to 85+.
        /// </summary>
        private PracticeDataAccess practiceReader = new PracticeDataAccess();
        private IAreasReader areasReader = ReaderFactory.GetAreasReader();
        private Area area;

        public QuinaryPopulationBuilder()
        {
            Values = new Dictionary<int, IEnumerable<double>>();
        }

        public void Build()
        {
            var metadataRepo = IndicatorMetadataProvider.Instance;

            const int indicatorId = IndicatorIds.QuinaryPopulations;

            Grouping grouping = groupDataReader.GetGroupingsByGroupIdAndIndicatorId(GroupId, indicatorId);
            area = areasReader.GetAreaFromCode(AreaCode);

            var metadata = metadataRepo.GetIndicatorMetadata(indicatorId);
            period = new DataPointOffsetCalculator(grouping, DataPointOffset, metadata.YearType).TimePeriod;

            // Get data for each sex
            int overallTotal = 0;
            foreach (var sexId in sexIds)
            {
                IEnumerable<double> vals;

                if (area.IsCcg)
                {
                    QuinaryPopulation population = practiceReader.GetCcgQuinaryPopulation(indicatorId, period, AreaCode, sexId);
                    vals = new QuinaryPopulationSorter(population.Values).SortedValues;
                }
                else
                {
                    IList<CoreDataSet> data = groupDataReader.GetCoreDataForAllAges(indicatorId, period, AreaCode, sexId);
                    vals = new QuinaryPopulationSorter(data).SortedValues;
                }

                // Add total
                int total = Convert.ToInt32(Math.Round(vals.Sum(), MidpointRounding.AwayFromZero));
                overallTotal += total;

                Values.Add(sexId, vals);
            }

            // Convert to %
            foreach (var sexId in sexIds)
            {
                Values[sexId] = Values[sexId].Select(x => Math.Round((x / overallTotal) * 100, 2, MidpointRounding.AwayFromZero));
            }

            // List size
            var val = new QofListSizeProvider(groupDataReader, area, GroupId, DataPointOffset, metadata.YearType).Value;
            if (val.HasValue)
            {
                metadata = metadataRepo.GetIndicatorMetadata(QofListSizeProvider.IndicatorId);
                ListSize = new ValueData { Value = val.Value };

                var formatter = NumericFormatterFactory.New(metadata, groupDataReader);
                formatter.Format(ListSize);
            }

            if (area.IsGpPractice && AreOnlyPopulationsRequired == false)
            {
                SetEthnicityText();

                SetDeprivationDecile();

                Shape = practiceReader.GetShape(AreaCode);

                AdHocValues = new PracticePerformanceIndicatorValues(groupDataReader, AreaCode, DataPointOffset).IndicatorToValue;
            }
        }


        public void BuildPopulation(string areaCode,int areaTypeId)
        {
            var metadataRepo = IndicatorMetadataProvider.Instance;
            area = areasReader.GetAreaFromCode(areaCode);
            const int indicatorId = IndicatorIds.QuinaryPopulations;
            var groupIds = new List<int> {GroupId};
            var indicatorIds = new List<int> {indicatorId};
            //Grouping grouping = groupDataReader.GetGroupingsByGroupIdAndAreaTypeId(GroupId, areaTypeId).FirstOrDefault(); 
            Grouping grouping = groupDataReader.GetGroupingsByGroupIdsAndIndicatorIdsAndAreaType(groupIds, indicatorIds, areaTypeId).FirstOrDefault();
            if (grouping != null)
            {
                var metadata = metadataRepo.GetIndicatorMetadata(grouping.IndicatorId);
                period = new DataPointOffsetCalculator(grouping, 0, metadata.YearType).TimePeriod;

                // Get data for each sex
                int overallTotal = 0;
                foreach (var sexId in sexIds)
                {
                    IEnumerable<double> vals;

                    if (area.IsCcg)
                    {
                        QuinaryPopulation population = practiceReader.GetCcgQuinaryPopulation(metadata.IndicatorId, period,
                            AreaCode, sexId);
                        vals = new QuinaryPopulationSorter(population.Values).SortedValues;
                    }
                    else
                    {
                        IList<CoreDataSet> data = groupDataReader.GetCoreDataForAllAges(metadata.IndicatorId, period, AreaCode,
                            sexId);
                        vals = new QuinaryPopulationSorter(data).SortedValues;
                    }

                    // Add total
                    int total = Convert.ToInt32(Math.Round(vals.Sum(), MidpointRounding.AwayFromZero));
                    overallTotal += total;

                    Values.Add(sexId, vals);
                }

                // Convert to %
                foreach (var sexId in sexIds)
                {
                    Values[sexId] =
                        Values[sexId].Select(x => Math.Round((x/overallTotal)*100, 2, MidpointRounding.AwayFromZero));
                }
                var val = new QofListSizeProvider(groupDataReader, area, GroupId, DataPointOffset, metadata.YearType).Value;
                if (val.HasValue)
                {
                    metadata = metadataRepo.GetIndicatorMetadata(QofListSizeProvider.IndicatorId);
                    ListSize = new ValueData { Value = val.Value };

                    var formatter = NumericFormatterFactory.New(metadata, groupDataReader);
                    formatter.Format(ListSize);
                }
            }
        }

        public void BuildSummary()
        {
            var metadataRepo = IndicatorMetadataProvider.Instance;

            const int indicatorId = IndicatorIds.QuinaryPopulations;

            Grouping grouping = groupDataReader.GetGroupingsByGroupIdAndIndicatorId(GroupId, indicatorId);
            area = areasReader.GetAreaFromCode(AreaCode);

            var metadata = metadataRepo.GetIndicatorMetadata(indicatorId);
            period = new DataPointOffsetCalculator(grouping, DataPointOffset, metadata.YearType).TimePeriod;
            
            if (area.IsGpPractice && AreOnlyPopulationsRequired == false)
            {
                SetEthnicityText();

                SetDeprivationDecile();

                Shape = practiceReader.GetShape(AreaCode);

                AdHocValues = new PracticePerformanceIndicatorValues(groupDataReader, AreaCode, DataPointOffset).IndicatorToValue;
            }
        }
        private void SetDeprivationDecile()
        {
            var decile = areasReader.GetCategorisedArea(AreaCode, AreaTypeIds.Country, AreaTypeIds.GpPractice,
                CategoryTypeIds.DeprivationDecileGp2015);
            if (decile != null)
            {
                DeprivationDecile = decile.CategoryId;
            }
        }

        private void SetEthnicityText()
        {
            const int categoryTypeId = EthnicityLabelBuilder.EthnicityCategoryTypeId;

            Grouping grouping = groupDataReader.GetGroupingsByGroupIdAndIndicatorId(GroupId,
                IndicatorIds.EthnicityEstimates);

            if (grouping == null)
            {
                throw new FingertipsException("Ethnicity estimates not found in practice profiles supporting indicators domain");
            }

            var dataList = groupDataReader.GetCoreDataListForAllCategoryAreasOfCategoryAreaType(
                    grouping, TimePeriod.GetDataPoint(grouping),
                    categoryTypeId, AreaCode);

            if (dataList.Any())
            {
                var categories = areasReader.GetCategories(categoryTypeId);

                EthnicityLabelBuilder builder = new EthnicityLabelBuilder(dataList, categories);
                if (builder.IsLabelRequired)
                {
                    EthnicityText = builder.Label;
                }
            }
        }
    }
}
