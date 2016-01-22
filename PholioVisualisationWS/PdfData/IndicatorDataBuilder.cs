using System.Collections.Generic;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfData
{
    public abstract class IndicatorDataBuilder<T>
    {
        private readonly TimePeriodFormatter timeFormatter = new DataPointTimePeriodFormatter();

        private BenchmarkDataProvider benchmarkDataProvider;
        protected NumericFormatter formatter;
        protected IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();
        protected TimePeriod timePeriod;
        protected abstract IndicatorData IndicatorData { get; }
        public abstract T GetIndicatorData(GroupRoot groupRoot, IndicatorMetadata metadata, IList<Area> benchmarkAreas);

        protected void SetIndicatorData(GroupRoot groupRoot, IndicatorMetadata metadata,
            IList<Area> benchmarkAreas)
        {
            benchmarkDataProvider = new BenchmarkDataProvider(groupDataReader);
            Grouping grouping = groupRoot.FirstGrouping;
            formatter = NumericFormatterFactory.New(metadata, groupDataReader);
            timePeriod = TimePeriod.GetDataPoint(grouping);

            SetMetadata(metadata, groupRoot);
            SetTimePeriod(grouping, metadata);
            SetBenchmarkData(grouping, benchmarkAreas, metadata);
        }

        private void SetBenchmarkData(Grouping grouping, IList<Area> benchmarkAreas, IndicatorMetadata metadata)
        {
            var dataList = new Dictionary<string, CoreDataSet>();
            IndicatorData.BenchmarkData = dataList;

            AverageCalculator averageCalculator = AverageCalculatorFactory.New(null, metadata);
            var dataProcessor = new CoreDataProcessor(null);

            foreach (Area benchmarkArea in benchmarkAreas)
            {
                CoreDataSet benchmarkData = benchmarkDataProvider.GetBenchmarkData(grouping, timePeriod,
                    averageCalculator, benchmarkArea);
                formatter.Format(benchmarkData);
                dataProcessor.Truncate(benchmarkData);
                ModifyDataForSpecificProfile(benchmarkData);
                dataList.Add(benchmarkArea.Code, benchmarkData);
            }
        }

        private void SetMetadata(IndicatorMetadata metadata, GroupRoot groupRoot)
        {
            IDictionary<string, string> text = metadata.Descriptive;

            IndicatorData.IndicatorId = metadata.IndicatorId;

            string indicatorName = text[IndicatorMetadataTextColumnNames.Name];
            IndicatorData.ShortName = SexTextAppender.GetIndicatorName(indicatorName,
                groupRoot.SexId, groupRoot.StateSex);

            if (text.ContainsKey(IndicatorMetadataTextColumnNames.IndicatorNumber))
            {
                IndicatorData.IndicatorNumber = text[IndicatorMetadataTextColumnNames.IndicatorNumber];
            }

            IndicatorData.SexId = groupRoot.SexId;
            IndicatorData.AgeId = groupRoot.AgeId;
            IndicatorData.PolarityId = groupRoot.PolarityId;
        }

        private void SetTimePeriod(Grouping grouping, IndicatorMetadata metadata)
        {
            timeFormatter.Format(grouping, metadata);
            IndicatorData.Period = grouping.TimePeriodText;
        }

        protected virtual void ModifyDataForSpecificProfile(CoreDataSet data)
        {
        }
    }
}