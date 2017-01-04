using PholioVisualisation.DataHelpers;
using PholioVisualisation.PholioObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.Analysis
{
    public class TrendRequest
    {
        public int ValueTypeId { get; set; }
        public double UnitValue { get; set; }
        public IEnumerable<CoreDataSet> Data { get; set; }
        public int YearRange { get; set; }

        public bool IsValid(ref string validationMessage)
        {
            var valid = ValueTypeId == ValueTypeIds.Proportion
                    || ValueTypeId == ValueTypeIds.CrudeRate;

            if (!valid)
            {
                validationMessage = "The recent trend cannot be calculated for this value type";
                return false;
            }

            if (YearRange != 1)
            {
                validationMessage = "The recent trend cannot be calculated for this year range";
                return false;
            }

            if (this.Data == null)
            {
                validationMessage = "No data points found ";
                return false;
            }

            if (this.Data.Count() < TrendMarkerCalculator.MinimumNumberOfPoints)
            {
                validationMessage = "Not enough data points to calculate recent trend";
                return false;
            }

            var Data = GetValidDataList();
            if (Data.Count < TrendMarkerCalculator.MinimumNumberOfPoints)
            {
                validationMessage = "Not enough data points with valid values to calculate recent trend";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get the data (starting with the most recent) until an invalid data point is reached.
        /// </summary>
        private List<CoreDataSet> GetValidDataList()
        {
            var orderedData = new CoreDataSetSorter(Data.ToList()).SortByDescendingYear();
            var validData = new List<CoreDataSet>();
            foreach (var data in orderedData)
            {
                if (IsDataValid(data))
                {
                    validData.Add(data);
                }
                else
                {
                    break;
                }
            }
            return validData;
        }

        /// <summary>
        /// Only Count and denominator are required for the calculation. Check Value too because 
        /// a missing value suggests it has been suppressed.
        /// </summary>
        private static bool IsDataValid(CoreDataSet data)
        {
            return data.IsValueValid && data.IsCountValid && data.IsDenominatorValid;
        }
    }

    public class TrendLine
    {
        public double SlopeBeta { get; set; }
        public double InterceptAlpha { get; set; }
    }

    public class TrendMarkerCalculator
    {
        public const int MinimumNumberOfPoints = 5;

        public TrendMarkerResult GetResults(TrendRequest trendRequest)
        {
            return CalculateTrend(trendRequest);
        }

        private TrendMarkerResult CalculateTrend(TrendRequest trendRequest, int pointsUsed = 0)
        {
            var message = string.Empty;
            var chiSquare = 0.0;

            // Is there a significant trend
            var isSignificant = IsSignificant(trendRequest, ref pointsUsed, ref chiSquare, ref message);

            // Trend line
            TrendLine trendLine;
            if (isSignificant)
            {
                var values = GetMostRecentDataOrderedByIncreasingYear(trendRequest.Data, pointsUsed)
                    .Select(x => x.Value)
                    .ToList();
                trendLine = GetSlopeAndIntercept(values, trendRequest.ValueTypeId, trendRequest.UnitValue);
            }
            else
            {
                trendLine = new TrendLine();
            }

            // Trend marker
            TrendMarker marker;
            if (isSignificant)
            {
                marker = trendLine.SlopeBeta > 0 ? TrendMarker.Increasing : TrendMarker.Decreasing;
            }
            else
            {
                marker = pointsUsed > 0 ? TrendMarker.NoChange : TrendMarker.CannotBeCalculated;
            }

            // Return results
            return new TrendMarkerResult()
            {
                Slope = trendLine.SlopeBeta,
                Intercept = trendLine.InterceptAlpha,
                NumberOfPointsUsedInCalculation = pointsUsed,
                Marker = marker,
                IsSignificant = isSignificant,
                ChiSquare = chiSquare,
                Message = message
            };
        }

        public static TrendLine GetSlopeAndIntercept(IList<double> values, int valueTypeId, double unitValue)
        {
            double sumOfxLog = 0D;
            double sumOft = 0D;
            double sumOftSq = 0D;
            double sumOfxLogAndt = 0D;

            double n = values.Count;
            double t = n;

            foreach (var val in values)
            {
                // Don't know why 100??
                double x = (val / 100D);

                x = x / unitValue;

                double xLog;
                if (valueTypeId == ValueTypeIds.Proportion)
                {
                    double xTimeoneminusx = x / (1D - x);
                    xLog = Math.Log(xTimeoneminusx);
                }
                else
                {
                    xLog = Math.Log(x); ;
                }

                sumOfxLog += xLog;
                sumOft += t;

                sumOftSq += t * t;
                sumOfxLogAndt += xLog * t;

                t--;
            }

            // This needs a minus in front to be consistent with Excel sheet results (don't know why?!)
            double slopeBeta = -((n * sumOfxLogAndt) - (sumOfxLog * sumOft)) / (n * sumOftSq - (sumOft * sumOft));
            double interceptAlpha = (sumOfxLog - (slopeBeta * sumOft)) / n;

            return new TrendLine
            {
                SlopeBeta = slopeBeta,
                InterceptAlpha = interceptAlpha
            };
        }

        private IEnumerable<CoreDataSet> GetMostRecentDataOrderedByIncreasingYear(IEnumerable<CoreDataSet> dataList, int numberOfPoints)
        {
            return dataList
                .OrderByDescending(t => t.Year)
                .Take(numberOfPoints)
                .Reverse();
        }

        private bool IsSignificant(TrendRequest trendRequest, ref int pointsUsed, ref double chiSquare, ref string message)
        {
            chiSquare = 0.0;

            if (trendRequest.IsValid(ref message) == false)
            {
                pointsUsed = 0;

                return false;
            }

            // Chi squared confidence 
            const double chiSquared99Point8 = 9.54953570608324; // inExcel =CHIINV(0.002,1)
            double chiSquaredConfidence = chiSquared99Point8;

            var fullDataList = trendRequest.Data.ToList();
            for (int pointsToUse = MinimumNumberOfPoints; pointsToUse <= fullDataList.Count; pointsToUse++)
            {
                pointsUsed = pointsToUse;

                double sumOfr = 0;
                double sumOfn = 0;
                double sumOfrAndt = 0;
                double sumOfnAndt = 0;
                double sumOfnAndtSq = 0;
                double t = pointsUsed;

                var dataList = GetMostRecentDataOrderedByIncreasingYear(fullDataList, pointsToUse);
                foreach (var coreDataSet in dataList)
                {
                    var r = coreDataSet.Count.Value;
                    var n = coreDataSet.Denominator;

                    sumOfr += r;
                    sumOfn += n;
                    sumOfrAndt += r * t;
                    sumOfnAndt += n * t;
                    sumOfnAndtSq += n * (t * t);

                    t--;
                }

                var sumPart1 = ((sumOfn * sumOfrAndt) - (sumOfr * sumOfnAndt));

                chiSquare = (sumOfn * (sumPart1 * sumPart1)) /
                            (sumOfr * (sumOfn - sumOfr) * (sumOfn * sumOfnAndtSq - (sumOfnAndt * sumOfnAndt)));

                if (chiSquare > chiSquaredConfidence)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
