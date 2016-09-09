using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfData
{
    public class SpineChartTableRowDataBuilder : IndicatorDataBuilder<SpineChartTableRowData>
    {
        private readonly IList<string> areaCodes;
        private readonly IList<string> areaCodesToIgnore;
        private int profileId;
        private readonly Dictionary<string, CoreDataSetProvider> coreDataSetProviders =
            new Dictionary<string, CoreDataSetProvider>();

        private readonly PholioReader pholioReader = ReaderFactory.GetPholioReader();
        private readonly IProfileReader profileReader = ReaderFactory.GetProfileReader();
        private readonly IAreasReader areasReader = ReaderFactory.GetAreasReader();

        private IArea nationalArea;

        private SpineChartTableRowData currentRow;

        public SpineChartTableRowDataBuilder(int profileId, IList<string> areaCodes)
        {
            this.areaCodes = areaCodes;
            this.profileId = profileId;
            InitCoreDataSetProviders();
            areaCodesToIgnore = profileReader.GetAreaCodesToIgnore(profileId).AreaCodesIgnoredForSpineChart;
            nationalArea = areasReader.GetAreaFromCode(AreaCodes.England);
        }

        protected override IndicatorData IndicatorData
        {
            get { return currentRow; }
        }

        public override SpineChartTableRowData GetIndicatorData(GroupRoot groupRoot, IndicatorMetadata metadata,
            IList<Area> benchmarkAreas)
        {
            currentRow = new SpineChartTableRowData();

            // Subnational spine charts not supported
            Grouping grouping = groupRoot.GetNationalGrouping();

            SetIndicatorData(groupRoot, metadata, benchmarkAreas);

            string longName = metadata.Descriptive[IndicatorMetadataTextColumnNames.NameLong];
            currentRow.LongName = SexTextAppender.GetIndicatorName(longName,
                groupRoot.SexId, groupRoot.StateSex);
            currentRow.ComparatorMethodId = grouping.ComparatorMethodId;
            SetStats(grouping, timePeriod);

            AssignChildAreaData(grouping, timePeriod, metadata);

            return currentRow;
        }

        private void InitCoreDataSetProviders()
        {
            var providerfactory = new CoreDataSetProviderFactory();
            foreach (string areaCode in areaCodes)
            {
                Area area = areasReader.GetAreaFromCode(areaCode);
                coreDataSetProviders.Add(areaCode, providerfactory.New(area));
            }
        }

        private void SetStats(Grouping grouping, TimePeriod timePeriod)
        {
            IList<double> values = groupDataReader.GetOrderedCoreDataValidValuesForAllAreasOfType(grouping,
                timePeriod, areaCodesToIgnore);
            IndicatorStatsPercentiles stats = new IndicatorStatsCalculator(values).GetStats();
            if (stats != null)
            {
                currentRow.Min = Truncate(stats.Min);
                currentRow.Percentile25 = Truncate(stats.Percentile25);
                currentRow.Percentile75 = Truncate(stats.Percentile75);
                currentRow.Max = Truncate(stats.Max);

                IndicatorStatsPercentilesFormatted formattedStats = formatter.FormatStats(stats);
                currentRow.MaxF = formattedStats.Max;
                currentRow.MinF = formattedStats.Min;
            }
        }

        private double Truncate(double d)
        {
            return DataProcessor<ValueData>.Round(d);
        }

        private void AssignChildAreaData(Grouping grouping, TimePeriod timePeriod, IndicatorMetadata metadata)
        {
            var indicatorComparisonHelper = new IndicatorComparisonHelper(metadata, grouping, groupDataReader,
                pholioReader, nationalArea);
            var dataProcessor = new CoreDataProcessor(formatter);

            foreach (string areaCode in areaCodes)
            {
                CoreDataSet data = coreDataSetProviders[areaCode].GetData(grouping, timePeriod, metadata);
                if (data != null)
                {
                    indicatorComparisonHelper.AssignCategoryDataIfRequired(null/*data assumed to always be national*/);

                    data.SignificanceAgainstOneBenchmark =
                        indicatorComparisonHelper.GetSignificance(data, grouping.ComparatorData);

                    // Truncate after significance has been calculated
                    dataProcessor.FormatAndTruncate(data);
                }
                currentRow.AreaData.Add(areaCode, data);
            }
        }

    }
}