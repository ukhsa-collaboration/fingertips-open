using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataSorting;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class QuinaryPopulationBuilder
    {
        private static readonly int[] SexIds = { PholioObjects.SexIds.Male, PholioObjects.SexIds.Female };

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
        /// Key is SexId, Value is population 
        /// </summary>
        public Dictionary<int, IEnumerable<double>> Values { get; private set; }
        public IList<string> Labels { get; private set; }
        public string PopulationIndicatorName { get; set; }
        public string Period { get; set; }

        // Output - Practice only 
        public string EthnicityText { get; private set; }
        public int? DeprivationDecile { get; private set; }
        public int? Shape { get; private set; }
        public ValueData ListSize { get; private set; }
        public Dictionary<string, ValueData> AdHocValues { get; private set; }

        // Data readers
        private readonly IGroupDataReader _groupDataReader = ReaderFactory.GetGroupDataReader();
        private readonly PracticeDataAccess _practiceReader = new PracticeDataAccess();
        private readonly IAreasReader _areasReader = ReaderFactory.GetAreasReader();
        private readonly IProfileReader _profileReader = ReaderFactory.GetProfileReader();

        // Private variables
        private TimePeriod _period;
        private IArea _area;

        public QuinaryPopulationBuilder()
        {
            Values = new Dictionary<int, IEnumerable<double>>();
            Labels = null;
        }

        /// <summary>
        /// LEGACY: required for practice profiles only
        /// </summary>
        public void BuildPopulationAndSummary()
        {
            // Grouping: Specific indicator ID from practice profiles supporting indicator
            const int indicatorId = IndicatorIds.QuinaryPopulations;
            Grouping grouping = _groupDataReader.GetGroupingsByGroupIdAndIndicatorId(GroupId, indicatorId);

            _area = _areasReader.GetAreaFromCode(AreaCode);
            var metadataRepo = IndicatorMetadataProvider.Instance;

            var metadata = metadataRepo.GetIndicatorMetadata(indicatorId);
            _period = new DataPointOffsetCalculator(grouping, DataPointOffset, metadata.YearType).TimePeriod;

            SetPopulationValuesAndLabels(indicatorId);
            SetListSize(metadata, metadataRepo);

            SetSummaryValues();
        }

        public void BuildPopulationOnly(string areaCode, int areaTypeId, int profileId, int indicatorId)
        {
            var groupIds = _profileReader.GetGroupIdsFromSpecificProfiles(new List<int> { profileId });
            var grouping = _groupDataReader.GetGroupingsByGroupIdsAndIndicatorIdsAndAreaType(groupIds, 
                new List<int> {indicatorId}, areaTypeId).FirstOrDefault();
            AssignPopulation(areaCode, grouping);
        }

        public void BuildPopulationOnly(string areaCode, int areaTypeId)
        {
            // Grouping: Assume first grouping in Populations profile is the quinary population
            var groupings = _groupDataReader
                .GetGroupingsByGroupIdAndAreaTypeId(GroupId, areaTypeId)
                .OrderBy(x => x.Sequence);
            Grouping grouping = groupings.FirstOrDefault();
            AssignPopulation(areaCode, grouping);
        }

        private void AssignPopulation(string areaCode, Grouping grouping)
        {
            if (grouping != null)
            {
                GroupId = grouping.GroupId;
                var metadataRepo = IndicatorMetadataProvider.Instance;
                _area = AreaFactory.NewArea(_areasReader, areaCode);

                var metadata = metadataRepo.GetIndicatorMetadata(grouping.IndicatorId);
                _period = new DataPointOffsetCalculator(grouping, 0, metadata.YearType).TimePeriod;

                SetPopulationValuesAndLabels(metadata.IndicatorId);
                SetListSize(metadata, metadataRepo);
                PopulationIndicatorName = metadata.Name;
                Period = new TimePeriodTextFormatter(metadata).Format(_period);
            }
        }

        public void BuildSummaryOnly()
        {
            var metadataRepo = IndicatorMetadataProvider.Instance;

            const int indicatorId = IndicatorIds.QuinaryPopulations;

            Grouping grouping = _groupDataReader.GetGroupingsByGroupIdAndIndicatorId(GroupId, indicatorId);
            _area = AreaFactory.NewArea(_areasReader, AreaCode);

            var metadata = metadataRepo.GetIndicatorMetadata(indicatorId);
            _period = new DataPointOffsetCalculator(grouping, DataPointOffset, metadata.YearType).TimePeriod;

            SetSummaryValues();
        }

        private void SetSummaryValues()
        {
            if (_area.IsGpPractice && AreOnlyPopulationsRequired == false)
            {
                SetEthnicityText();

                SetDeprivationDecile();

                Shape = _practiceReader.GetShape(AreaCode);

                AdHocValues =
                    new PracticePerformanceIndicatorValues(_groupDataReader, AreaCode, DataPointOffset).IndicatorToValue;
            }
        }

        private void SetListSize(IndicatorMetadata metadata, IndicatorMetadataProvider metadataRepo)
        {
            var val = new QofListSizeProvider(_groupDataReader, _area, GroupId, DataPointOffset, metadata.YearType).Value;
            if (val.HasValue)
            {
                metadata = metadataRepo.GetIndicatorMetadata(QofListSizeProvider.IndicatorId);
                ListSize = new ValueData { Value = val.Value };

                var formatter = NumericFormatterFactory.New(metadata, _groupDataReader);
                formatter.Format(ListSize);
            }
        }

        private void SetPopulationValuesAndLabels(int indicatorId)
        {
            // Get data for each sex
            int overallTotal = 0;
            foreach (var sexId in SexIds)
            {
                IEnumerable<double> vals;
                QuinaryPopulationSorter quinaryPopulationSorter;
                if (_area.IsCcg)
                {
                    QuinaryPopulation population = _practiceReader.GetCcgQuinaryPopulation(indicatorId, _period, AreaCode, sexId);
                    quinaryPopulationSorter = new QuinaryPopulationSorter(population.Values);
                }
                else
                {
                    IList<CoreDataSet> data = _groupDataReader.GetCoreDataForAllAges(indicatorId, _period, AreaCode, sexId);
                    quinaryPopulationSorter = new QuinaryPopulationSorter(data);
                }
                vals = quinaryPopulationSorter.SortedValues;

                // Add total
                int total = Convert.ToInt32(Math.Round(vals.Sum(), MidpointRounding.AwayFromZero));
                overallTotal += total;

                Values.Add(sexId, vals);

                if (Labels == null)
                {
                    Labels = ReaderFactory.GetPholioReader().GetQuinaryPopulationLabels(quinaryPopulationSorter.SortedAgeIds);
                }
            }

            // Convert to %
            foreach (var sexId in SexIds)
            {
                Values[sexId] = Values[sexId].Select(x => Math.Round((x / overallTotal) * 100, 2, MidpointRounding.AwayFromZero));
            }
        }

        private void SetDeprivationDecile()
        {
            var decile = _areasReader.GetCategorisedArea(AreaCode, AreaTypeIds.Country, AreaTypeIds.GpPractice,
                CategoryTypeIds.DeprivationDecileGp2015);
            if (decile != null)
            {
                DeprivationDecile = decile.CategoryId;
            }
        }

        private void SetEthnicityText()
        {
            const int categoryTypeId = EthnicityLabelBuilder.EthnicityCategoryTypeId;

            Grouping grouping = _groupDataReader.GetGroupingsByGroupIdAndIndicatorId(GroupId,
                IndicatorIds.EthnicityEstimates);

            if (grouping == null)
            {
                throw new FingertipsException("Ethnicity estimates not found in practice profiles supporting indicators domain");
            }

            var dataList = _groupDataReader.GetCoreDataListForAllCategoryAreasOfCategoryAreaType(
                    grouping, TimePeriod.GetDataPoint(grouping),
                    categoryTypeId, AreaCode);

            if (dataList.Any())
            {
                var categories = _areasReader.GetCategories(categoryTypeId);

                EthnicityLabelBuilder builder = new EthnicityLabelBuilder(dataList, categories);
                if (builder.IsLabelRequired)
                {
                    EthnicityText = builder.Label;
                }
            }
        }
    }
}
