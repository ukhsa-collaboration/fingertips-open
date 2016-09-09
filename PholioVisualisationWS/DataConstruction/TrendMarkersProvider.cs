using System.Collections.Generic;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public interface ITrendMarkersProvider
    {
        Dictionary<string, TrendMarkerResult> GetTrendResults(IList<IArea> areas, IndicatorMetadata indicatorMetadata, Grouping grouping);
    }

    public class TrendMarkersProvider : ITrendMarkersProvider
    {
        private readonly ITrendDataReader _trendReader;
        private readonly TrendMarkerCalculator _trendCalculator;

        public TrendMarkersProvider(ITrendDataReader trendReader, TrendMarkerCalculator trendCalculator)
        {
            _trendReader = trendReader;
            _trendCalculator = trendCalculator;
        }

        public Dictionary<string, TrendMarkerResult> GetTrendResults(IList<IArea> areas, IndicatorMetadata indicatorMetadata, Grouping grouping)
        {
            //var writer = new TrendMarkerWriter();

            var trendMarkers = new Dictionary<string, TrendMarkerResult>();

            foreach (var area in areas)
            {
                var areaCode = area.Code;

                var dataList = _trendReader.GetTrendData(grouping, areaCode);

                var result = GetTrendMarkerResult(indicatorMetadata, grouping, dataList);

                trendMarkers.Add(areaCode, result);

                //writer.Write(trendRequest, result);
            }

            //writer.WriteFile(groupRoot, subnationalArea);

            return trendMarkers;
        }

        public TrendMarkerResult GetTrendMarkerResult(IndicatorMetadata indicatorMetadata, Grouping grouping, IList<CoreDataSet> dataList)
        {
            var trendRequest = new TrendRequest
            {
                UnitValue = indicatorMetadata.Unit.Value,
                ValueTypeId = indicatorMetadata.ValueTypeId,
                Data = dataList,
                YearRange = grouping.YearRange,
            };

            var result = _trendCalculator.GetResults(trendRequest);
            return result;
        }
    }
}