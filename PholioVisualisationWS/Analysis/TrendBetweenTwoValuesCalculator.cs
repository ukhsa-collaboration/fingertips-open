using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    /// <summary>
    /// Calculate the trend between a value and the value of the previous time period
    /// </summary>
    public class TrendBetweenTwoValuesCalculator
    {
        private readonly DoubleOverlappingCIsComparer _comparer = new DoubleOverlappingCIsComparer();

        public void SetTrendMarkerFromDataList(IList<CoreDataSet> dataList, TrendMarkerResult trendMarkerResult)
        {
            if (dataList.Count >= 2)
            {
                // Sort so most recent first
                var sortedData = new CoreDataSetSorter(dataList).SortByDescendingYear().ToList();
                SetTrendMarker(sortedData[0], sortedData[1], trendMarkerResult);
            }
        }

        public void SetTrendMarker(CoreDataSet mostRecentData, CoreDataSet previousData,
            TrendMarkerResult trendMarkerResult)
        {
            TrendMarker trendMarker;

            if (IsDataValid(mostRecentData) && IsDataValid(previousData))
            {
                trendMarker = GetTrendMarker(mostRecentData, previousData);
            }
            else
            {
                trendMarker = TrendMarker.CannotBeCalculated;
            }

            trendMarkerResult.MarkerForMostRecentValueComparedWithPreviousValue = trendMarker;
        }

        private bool IsDataValid(CoreDataSet coreDataSet)
        {
            return coreDataSet != null &&
                   coreDataSet.IsValueValid && coreDataSet.Are95CIsValid;
        }

        private TrendMarker GetTrendMarker(CoreDataSet mostRecentData, CoreDataSet previousData)
        {
            TrendMarker trendMarker;
            Significance significance = _comparer.Compare(mostRecentData, previousData, null);

            switch (significance)
            {
                case Significance.Better:
                    // Better because more recent value is lower
                    trendMarker = TrendMarker.Decreasing;
                    break;
                case Significance.Same:
                    trendMarker = TrendMarker.NoChange;
                    break;
                case Significance.Worse:
                    // Worse because more recent value is higher
                    trendMarker = TrendMarker.Increasing;
                    break;
                default:
                    trendMarker = TrendMarker.CannotBeCalculated;
                    break;
            }
            return trendMarker;
        }
    }
}