using System.Collections.Generic;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public abstract class MostRecentDataActionBase
    {
        // Data readers
        protected IAreasReader areasReader = ReaderFactory.GetAreasReader();
        protected IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();
        protected IProfileReader profileReader = ReaderFactory.GetProfileReader();
        protected PholioReader pholioReader = ReaderFactory.GetPholioReader();

        // Instance variables
        protected Grouping grouping;
        protected IndicatorMetadata indicatorMetadata;
        protected TimePeriod timePeriod;

        protected void ValidateParameters(int profileId, int areaTypeId, int indicatorId, string areaCode)
        {
            ParameterCheck.GreaterThanZero("Profile ID", profileId);
            ParameterCheck.GreaterThanZero("Indicator ID", indicatorId);
            ParameterCheck.GreaterThanZero("Area Type ID", areaTypeId);
            ParameterCheck.ValidAreaCode(areaCode);
            // NOTE: sex and age IDs can be -1
        }

        protected void InitMetadataAndTimePeriod(Grouping grouping)
        {
            indicatorMetadata = IndicatorMetadataRepository.Instance.GetIndicatorMetadata(grouping);
            timePeriod = TimePeriod.GetDataPoint(grouping);
        }

        protected void FormatData(IList<CoreDataSet> dataList)
        {
            NumericFormatter formatter = NumericFormatterFactory.New(indicatorMetadata, groupDataReader);
            new CoreDataProcessor(formatter).FormatAndTruncateList(dataList);
        }

        protected virtual void CalculateSignificances(string areaCode, IList<CoreDataSet> categoryDataList)
        {
            var area = AreaFactory.NewArea(areasReader, areaCode);
            var nationalArea = GetNationalArea(area);

            var indicatorComparisonHelper = new IndicatorComparisonHelper(indicatorMetadata,
                grouping, groupDataReader, pholioReader, nationalArea);

            // Set benchmark data
            var benchmarkDataProvider = new BenchmarkDataProvider(groupDataReader);
            AverageCalculator averageCalculator = null; // Assume parent value is in database
            CoreDataSet benchmarkData = benchmarkDataProvider.GetBenchmarkData(grouping, timePeriod,
                averageCalculator, area);

            foreach (CoreDataSet coreDataSet in categoryDataList)
            {
                coreDataSet.SignificanceAgainstOneBenchmark =
                    (int) indicatorComparisonHelper.GetSignificance(coreDataSet, benchmarkData);
            }
        }

        protected IArea GetNationalArea(IArea area)
        {
            IArea nationalArea = area.IsCountry
                ? area
                : AreaFactory.NewArea(areasReader, AreaCodes.England);
            return nationalArea;
        }
    }
}