using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Export.FileBuilder.Wrappers;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export.FileBuilder.Containers
{
    public class BodyPeriodTrendContainer
    {
        private readonly MultipleCoreDataCollector _coreDataCollector;
        private readonly CsvBuilderAttributesForPeriodsWrapper _attributesForPeriods;

        public BodyPeriodTrendContainer(MultipleCoreDataCollector coreDataCollector, CsvBuilderAttributesForPeriodsWrapper attributesForPeriods)
        {
            _coreDataCollector = coreDataCollector;
            _attributesForPeriods = attributesForPeriods;
        }

        public TrendMarkerResult GetTrendMarker(IndicatorMetadata indicatorMetadata, CoreDataSet coreData,
            ExportAreaHelper.GeographicalCategory geographicalCategory, Grouping grouping)
        {
            IList<CoreDataSet> trendDataList = null;

            if (geographicalCategory == ExportAreaHelper.GeographicalCategory.National)
                trendDataList = _coreDataCollector.GetDataListForEngland(coreData);
            else if (geographicalCategory == ExportAreaHelper.GeographicalCategory.SubNational)
                trendDataList = _coreDataCollector.GetDataListForParentArea(coreData);
            else if (geographicalCategory == ExportAreaHelper.GeographicalCategory.Local)
                trendDataList = _coreDataCollector.GetDataListForChildArea(coreData);

            // CsvBuilderIndicatorDataBodyPeriodWriter.WriteSinglePeriodInFile
            // In this method when collecting core data for child, the most recent time period
            // does not get added and this results in wrong calculation of recent trends for csv download.
            // This is a workaround to add the core data to the trend data list for most recent time period if it is not already included.
            if (trendDataList != null)
            {
                var trendDataForMostRecentTimePeriod = trendDataList.FirstOrDefault(x => x.Year == coreData.Year);
                if (trendDataForMostRecentTimePeriod == null)
                {
                    trendDataList.Add(coreData);
                }
            }

            var trendMarkerResult = _attributesForPeriods.TrendMarkersProvider.GetTrendMarkerResult(indicatorMetadata, coreData.YearRange, trendDataList, grouping);

            return trendMarkerResult;
        }
    }
}