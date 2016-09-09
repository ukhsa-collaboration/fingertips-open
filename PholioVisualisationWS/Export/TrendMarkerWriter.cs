using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export
{
    public interface ITrendMarkerWriter
    {
        void WriteChildTrendMarkers(WorksheetInfo worksheetInfo,
            Dictionary<string, TrendMarkerResult> trendMarkerResults, IList<string> childAreaCodes);

        void WriteNationalTrendMarkers(WorksheetInfo worksheetInfo,
            Dictionary<string, TrendMarkerResult> trendMarkerResults, string areaCode);

        void WriteMultipleSubnationalTrendMarkers(WorksheetInfo worksheetInfo, Grouping grouping,
            IndicatorMetadata indicatorMetadata, CoreDataCollector coreDataCollector, IList<CategoryIdAndAreaCode> categoryIdAndAreaCodes);

        void WriteSingleSubnationalTrendMarker(WorksheetInfo worksheetInfo, Grouping grouping,
            IndicatorMetadata indicatorMetadata, CoreDataCollector coreDataCollector);
    }

    public static class TrendMarkerWriterFactory
    {
        public static ITrendMarkerWriter New(ProfileDataWriter profileDataWriter, int polarityId, IList<TimePeriod> timePeriods, bool hasTrendMarkers)
        {
            if (hasTrendMarkers && timePeriods.Count >= TrendMarkerCalculator.MinimumNumberOfPoints)
            {
                // Use null for TrendReader as all data will be provided to the provider
                var trendMarkerLabelProvider = new TrendMarkerLabelProvider(polarityId);
                var trendMarkerProvider = new TrendMarkersProvider(null, new TrendMarkerCalculator());
                return new TrendMarkerWriter(profileDataWriter, trendMarkerProvider, trendMarkerLabelProvider);
            }

            return new NullTrendMarkerWriter();
        }
    }

    public class NullTrendMarkerWriter : ITrendMarkerWriter
    {
        public void WriteChildTrendMarkers(WorksheetInfo worksheetInfo,
            Dictionary<string, TrendMarkerResult> trendMarkerResults, IList<string> childAreaCodes)
        { }

        public void WriteNationalTrendMarkers(WorksheetInfo worksheetInfo,
            Dictionary<string, TrendMarkerResult> trendMarkerResults, string areaCode)
        { }

        public void WriteMultipleSubnationalTrendMarkers(WorksheetInfo worksheetInfo, Grouping grouping,
            IndicatorMetadata indicatorMetadata, CoreDataCollector coreDataCollector, IList<CategoryIdAndAreaCode> categoryIdAndAreaCodes)
        { }

        public void WriteSingleSubnationalTrendMarker(WorksheetInfo worksheetInfo, Grouping grouping,
            IndicatorMetadata indicatorMetadata, CoreDataCollector coreDataCollector)
        { }
    }

    public class TrendMarkerWriter : ITrendMarkerWriter
    {
        private const int RowOffsetForOneArea = 1;

        private readonly ProfileDataWriter _profileDataWriter;
        private readonly TrendMarkersProvider _trendMarkersProvider;
        private readonly TrendMarkerLabelProvider _trendMarkerLabelProvider;

        public TrendMarkerWriter(ProfileDataWriter profileDataWriter, TrendMarkersProvider trendMarkersProvider, 
            TrendMarkerLabelProvider trendMarkerLabelProvider)
        {
            _profileDataWriter = profileDataWriter;
            _trendMarkersProvider = trendMarkersProvider;
            _trendMarkerLabelProvider = trendMarkerLabelProvider;
        }

        public void WriteChildTrendMarkers(WorksheetInfo worksheetInfo,
            Dictionary<string, TrendMarkerResult> trendMarkerResults, IList<string> childAreaCodes)
        {
            int rowOffset = childAreaCodes.Count;
            foreach (var childAreaCode in childAreaCodes.OrderBy(x => x))
            {
                var trendLabel = GetLabel(trendMarkerResults[childAreaCode]);
                _profileDataWriter.AddTrendMarker(trendLabel, rowOffset, worksheetInfo);
                rowOffset--;
            }
        }

        public void WriteMultipleSubnationalTrendMarkers(WorksheetInfo worksheetInfo, Grouping grouping,
            IndicatorMetadata indicatorMetadata, CoreDataCollector coreDataCollector,
            IList<CategoryIdAndAreaCode> categoryIdAndAreaCodes)
        {
            int rowOffset = categoryIdAndAreaCodes.Count;

            foreach (var area in categoryIdAndAreaCodes)
            {
                var dataList = coreDataCollector.GetDataListForArea(area);
                var result = _trendMarkersProvider.GetTrendMarkerResult(indicatorMetadata, grouping, dataList);
                _profileDataWriter.AddTrendMarker(GetLabel(result), rowOffset, worksheetInfo);
                rowOffset--;
            }
        }

        public void WriteSingleSubnationalTrendMarker(WorksheetInfo worksheetInfo, Grouping grouping,
            IndicatorMetadata indicatorMetadata, CoreDataCollector coreDataCollector)
        {
            List<CoreDataSet> dataList = coreDataCollector.GetDataList();
            var result = _trendMarkersProvider.GetTrendMarkerResult(indicatorMetadata, grouping, dataList);
            _profileDataWriter.AddTrendMarker(GetLabel(result), RowOffsetForOneArea, worksheetInfo);
        }

        public void WriteNationalTrendMarkers(WorksheetInfo worksheetInfo,
            Dictionary<string, TrendMarkerResult> trendMarkerResults, string areaCode)
        {
            var label = GetLabel(trendMarkerResults[areaCode]);
            _profileDataWriter.AddTrendMarker(label, RowOffsetForOneArea, worksheetInfo);
        }

        private TrendMarkerLabel GetLabel(TrendMarkerResult trendMarkerResult)
        {
            return _trendMarkerLabelProvider.GetLabel(trendMarkerResult.Marker);
        }
    }
}