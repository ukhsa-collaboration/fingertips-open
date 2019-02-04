using System.Collections.Generic;
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

        public TrendMarkerResult GetTrendMarker(IndicatorMetadata indicatorMetadata, CoreDataSet coreData, ExportAreaHelper.GeographicalCategory geographicalCategory)
        {
            IList<CoreDataSet> trendDataList = null;

            if (geographicalCategory == ExportAreaHelper.GeographicalCategory.National)
                trendDataList = _coreDataCollector.GetDataListForEngland(coreData);
            else if (geographicalCategory == ExportAreaHelper.GeographicalCategory.SubNational)
                trendDataList = _coreDataCollector.GetDataListForParentArea(coreData);
            else if (geographicalCategory == ExportAreaHelper.GeographicalCategory.Local)
                trendDataList = _coreDataCollector.GetDataListForChildArea(coreData);

            trendDataList.Add(coreData);
            var trendMarkerResult = _attributesForPeriods.TrendMarkersProvider.GetTrendMarkerResult(indicatorMetadata, coreData.YearRange, trendDataList);

            return trendMarkerResult;
        }
    }
}
