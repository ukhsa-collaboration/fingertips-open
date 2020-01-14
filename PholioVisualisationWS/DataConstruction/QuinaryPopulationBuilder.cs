using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.DataSorting;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class QuinaryPopulationBuilder
    {
        public const int NumberOfQuinaryPopulationBands = 20;

        private static readonly int[] SexIds = { PholioObjects.SexIds.Male, PholioObjects.SexIds.Female };

        // Input
        public int GroupId { get; set; }
        public string AreaCode { get; set; }
        public int AreaTypeId { get; set; }
        public int DataPointOffset { get; set; }

        /// <summary>
        /// Only applies to practices. If true then other stats are calculated, e.g.
        /// QOF points, life expectancy, etc.
        /// </summary>
        public bool AreOnlyPopulationsRequired { get; set; }

        // Output
        /// <summary>
        /// Key is SexId, Value is population %
        /// </summary>
        public Dictionary<int, IEnumerable<double>> PopulationPercentages { get; private set; }

        /// <summary>
        /// Key is SexId, Counts is population value
        /// </summary>
        public Dictionary<int, IEnumerable<double>> PopulationTotals { get; private set; }

        public IList<string> Labels { get; private set; }
        public string PopulationIndicatorName { get; set; }
        public string Period { get; set; }
        public int ChildAreaCount { get; set; }

        // Output - Practice only 
        public string EthnicityText { get; private set; }
        public int? DeprivationDecile { get; private set; }
        public ValueData ListSize { get; private set; }
        public Dictionary<string, ValueData> AdHocValues { get; private set; }

        // Data readers
        private readonly IGroupDataReader _groupDataReader = ReaderFactory.GetGroupDataReader();
        private readonly PracticeDataAccess _practiceReader = new PracticeDataAccess();
        private readonly IAreasReader _areasReader = ReaderFactory.GetAreasReader();

        // Private variables
        private TimePeriod _period;
        private IArea _area;


        public QuinaryPopulationBuilder()
        {
            PopulationPercentages = new Dictionary<int, IEnumerable<double>>();
            PopulationTotals = new Dictionary<int, IEnumerable<double>>();
            Labels = null;
        }

        public void BuildPopulationSummary()
        {
            var metadataRepo = IndicatorMetadataProvider.Instance;

            const int indicatorId = IndicatorIds.QuinaryPopulations;

            Grouping grouping = _groupDataReader.GetGroupingsByGroupIdAndIndicatorId(GroupId, indicatorId);
            _area = AreaFactory.NewArea(_areasReader, AreaCode);

            var metadata = metadataRepo.GetIndicatorMetadata(indicatorId);
            _period = new DataPointOffsetCalculator(grouping, DataPointOffset, metadata.YearType).TimePeriod;

            SetSummaryValues();
        }

        public void BuildPopulationTotalsAndPercentages(string areaCode)
        {
            var grouping = GetGrouping(AreaTypeId);
            AssignPopulation(areaCode, grouping);
        }

        public void BuildPopulationTotalsWithoutPercentages(string areaCode)
        {
            var grouping = GetGrouping(AreaTypeId);

            AssignPopulationWithOutPercentage(areaCode, grouping);
            RoundPopulationTotalsToZero();
        }

        private void RoundPopulationTotalsToZero()
        {
            foreach (var sexId in SexIds)
            {
                var values = PopulationTotals[sexId].ToList();

                for (int i = 0; i < values.Count; i++)
                {
                    values[i] = Math.Round(values[i], 0, MidpointRounding.AwayFromZero);
                }

                PopulationTotals[sexId] = values;
            }
        }

        private Grouping GetGrouping(int areaTypeId)
        {
            // Grouping: Assume first grouping in Populations profile is the quinary population
            var groupings = _groupDataReader
                .GetGroupingsByGroupIdAndAreaTypeId(GroupId, areaTypeId)
                .OrderBy(x => x.Sequence);
            Grouping grouping = groupings.FirstOrDefault();
            return grouping;
        }

        private void AssignPopulation(string areaCode, Grouping grouping)
        {
            if (grouping == null) return;

            var metadata = SetDescriptiveProperties(areaCode, grouping);
            SetPopulationValuesAndLabels(metadata.IndicatorId);
        }

        private void AssignPopulationWithOutPercentage(string areaCode, Grouping grouping)
        {
            if (grouping == null) return;

            var metadata = SetDescriptiveProperties(areaCode, grouping);

            CalculatePopulationTotals(metadata.IndicatorId);
        }

        private IndicatorMetadata SetDescriptiveProperties(string areaCode, Grouping grouping)
        {
            GroupId = grouping.GroupId;
            _area = AreaFactory.NewArea(_areasReader, areaCode);

            var metadataRepo = IndicatorMetadataProvider.Instance;
            var metadata = GetMetadata(grouping, metadataRepo);

            SetListSize(metadata, metadataRepo);
            PopulationIndicatorName = metadata.Name;
            Period = new TimePeriodTextFormatter(metadata).Format(_period);
            return metadata;
        }

        private void SetSummaryValues()
        {
            if (_area.IsGpPractice && AreOnlyPopulationsRequired == false)
            {
                var summary =new PracticeSummary(_groupDataReader, _areasReader);

                EthnicityText = summary.GetEthnicityText(AreaCode, GroupId);
                DeprivationDecile = summary.GetDeprivationDecile(AreaCode);

                AdHocValues = new PracticePerformanceIndicatorValues(_groupDataReader,
                    AreaCode, DataPointOffset).IndicatorToValue;
            }
        }

        private void SetListSize(IndicatorMetadata metadata, IndicatorMetadataProvider metadataRepo)
        {
            if (AreaTypeId == AreaTypeIds.GpPractice)
            {
                var val = new QofListSizeProvider(_groupDataReader, _area, GroupId, DataPointOffset, metadata.YearType).Value;

                if (val.HasValue && val > 0)
                {
                    ListSize = new ValueData {Value = val.Value};

                    // Format list size
                    metadata = metadataRepo.GetIndicatorMetadata(QofListSizeProvider.IndicatorId);
                    var formatter = new NumericFormatterFactory(_groupDataReader).New(metadata);
                    formatter.Format(ListSize);
                }
            }
        }

        private void SetPopulationValuesAndLabels(int indicatorId)
        {
            // Get data for each sex
            double totalPopulation = 0;
            foreach (var sexId in SexIds)
            {
                // Get population values
                QuinaryPopulation population;
                if (_area.IsCcg)
                {
                    population = _practiceReader.GetCcgQuinaryPopulation(indicatorId, _period, AreaCode, sexId);
                }
                else
                {
                    population = GetQuinaryPopulation(indicatorId, sexId);
                }

                // Set supporting info
                SetLabels(population.Values);
                SetChildAreaCount(population);

                // Set population percentages
                var values = GetValues(population.Values);
                PopulationPercentages.Add(sexId, values);

                // Increment total population
                var total = values.Sum();
                totalPopulation += total;
            }

            ConvertValuesToPercentage(totalPopulation);
        }

        private void SetChildAreaCount(QuinaryPopulation population)
        {
            if (population.ChildAreaCount > 0)
            {
                // Population was calculated for child area count known
                ChildAreaCount = population.ChildAreaCount;
            }
            else
            {
                // Population was retrieved from DB so child area count unknown

                // Is this area parent area?
                var areaTypeComponents = new AreaTypeComponentRepository().GetAreaTypeComponents(AreaTypeId);
                if (areaTypeComponents.Select(x => x.ComponentAreaTypeId).Contains(_area.AreaTypeId) == false)
                {
                    // Assume all child areas have population data
                    var childAreasList = new ChildAreaListBuilder(_areasReader).GetChildAreas(_area.Code, AreaTypeId);
                    ChildAreaCount = childAreasList.Count;
                }
            }
        }

        private void CalculatePopulationTotals(int indicatorId)
        {
            // Get data for each sex
            foreach (var sexId in SexIds)
            {
                var population = CalculateParentPopulations(indicatorId, sexId);

                SetLabels(population.Values);

                var values = GetValues(population.Values);
                PopulationTotals.Add(sexId, values);
            }
        }

        private QuinaryPopulation CalculateParentPopulations(int indicatorId, int sexId)
        {
            var calculator = new QuinaryPopulationCalculator(_groupDataReader, _areasReader);
            return calculator.CalculateParentPopulationValues(AreaCode,
                AreaTypeId, indicatorId, sexId, _period);
        }

        private static IList<double> GetValues(IList<QuinaryPopulationValue> values)
        {
            var quinaryPopulationSorter = new QuinaryPopulationSorter(values);
            var sortedValues = quinaryPopulationSorter.SortedValues;
            return sortedValues;
        }

        private void SetLabels(IList<QuinaryPopulationValue> values)
        {
            if (Labels == null)
            {
                var ageIds = new QuinaryPopulationSorter(values).SortedAgeIds;
                Labels = ReaderFactory.GetPholioReader().GetQuinaryPopulationLabels(ageIds);
            }
        }

        private void ConvertValuesToPercentage(double totalPopulation)
        {
            foreach (var sexId in SexIds)
            {
                PopulationTotals[sexId] = PopulationPercentages[sexId].Select(x => x);
                PopulationPercentages[sexId] = PopulationPercentages[sexId].Select(x => GetPopulationPercentage(x, totalPopulation));
            }
        }

        public static double GetPopulationPercentage(double population, double totalPopulation)
        {
            var percent = population / totalPopulation * 100;
            return Math.Round(percent, 2, MidpointRounding.AwayFromZero);
        }

        private QuinaryPopulation GetQuinaryPopulation(int indicatorId, int sexId)
        {
            var allDataForAllAges = _groupDataReader.GetCoreDataForAllAges(indicatorId, _period, AreaCode, sexId);

            var population = allDataForAllAges.Count < NumberOfQuinaryPopulationBands
                ? CalculateParentPopulations(indicatorId, sexId)
                : QuinaryPopulation.New(allDataForAllAges);

            return population;
        }

        private IndicatorMetadata GetMetadata(Grouping grouping, IndicatorMetadataProvider metadataRepo)
        {
            var metadata = metadataRepo.GetIndicatorMetadata(grouping.IndicatorId);
            _period = new DataPointOffsetCalculator(grouping, 0, metadata.YearType).TimePeriod;
            return metadata;
        }
    }
}
