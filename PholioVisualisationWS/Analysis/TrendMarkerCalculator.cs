using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    public class TrendResponse
    {
        public double? Slope { get; set; }
        public double? Intercept { get; set; }
        public TrendMarker Marker { get; set; }
        public int NumberOfPointsUsedInCalculation { get; set; }
        public double ChiSquare { get; set; }
        public bool IsSignificant { get; set; }
        public string Message { get; set; }
    }


    public class TrendRequest
    {
        public int ValueTypeId { get; set; }
        public double ComparatorConfidence { get; set; }
        public IEnumerable<CoreDataSet> Data { get; set; }
        public int YearRange { get; set; }

        public bool IsValid(ref string validationMessage)
        {
            var valid = (this.ValueTypeId == ValueTypeIds.Proportion
                    || this.ValueTypeId == ValueTypeIds.CrudeRate
                    || this.ValueTypeId == ValueTypeIds.DirectlyStandardisedRate);

            if (!valid)
            {
                validationMessage = string.Format("Value Type {0} is not relevant for the calculation", this.ValueTypeId);
                return false;
            }

            if (this.YearRange != 1)
            {
                validationMessage = string.Format("Year Range {0} is not relevant for the calculation", this.YearRange);
                return false;
            }

            if (this.Data == null)
            {
                validationMessage = "No data points found ";
                return false;
            }

            if (this.Data.Count() < 5)
            {
                validationMessage = "Not enough data points to do the calculation";
                return false;
            }

            if (this.Data.Count(d => d.IsCountValid == false || d.IsDenominatorValid == false) > 0)
            {
                validationMessage = "One or more data points have invalid values";
            }

            return true;
        }
    }

    public class TrendMarkerCalculator
    {
        public TrendResponse GetResults(TrendRequest trendRequest)
        {
            return CalculateTrend(trendRequest);
        }

        private TrendResponse CalculateTrend(TrendRequest trendRequest, int pointsUsed = 0)
        {
            var message = string.Empty;
            var chiSquare = 0.0;

            if (!IsSignificant(trendRequest, ref pointsUsed, ref chiSquare, ref message))
            {
                return new TrendResponse()
                {
                    Slope = null,
                    Intercept = null,
                    NumberOfPointsUsedInCalculation = pointsUsed,
                    Marker = pointsUsed > 0 ? TrendMarker.NoChange : TrendMarker.CannotBeCalculated,
                    IsSignificant = false,
                    ChiSquare = chiSquare,
                    Message = message
                };
            }

            var sumOfx = 0.0;
            var sumOfxLog = 0.0;
            var sumOft = 0.0;
            var sumOftSq = 0.0;
            var sumOfxLogAndt = 0.0;

            var n = pointsUsed;

            foreach (var dataItem in trendRequest.Data.OrderByDescending(t => t.Year).Take(n).Reverse())
            {
                var x = dataItem.Value / 100;

                var xLog = trendRequest.ValueTypeId == ValueTypeIds.Proportion ?
                    Math.Log(x / (1 - x), Math.E)
                    : Math.Log(x, Math.E);

                var t = dataItem.Year;

                // do sums
                sumOfx += x;
                sumOfxLog += xLog;
                sumOft += t;

                sumOftSq += t * t;
                sumOfxLogAndt += xLog * t;
            }

            var slopeBeta = ((n * sumOfxLogAndt) - (sumOfxLog * sumOft)) / (n * sumOftSq - (sumOft * sumOft));

            // not used yet
            var interceptAlpha = (sumOfx - (slopeBeta * sumOft)) / n;

            return new TrendResponse()
            {
                Slope = slopeBeta,
                Intercept = interceptAlpha,
                NumberOfPointsUsedInCalculation = pointsUsed,
                Marker = slopeBeta > 0 ? TrendMarker.Increasing : TrendMarker.Decreasing,
                IsSignificant = true,
                ChiSquare = chiSquare

            };
        }


        private bool IsSignificant(TrendRequest trendRequest, ref int pointsUsed, ref double chiSquare, ref string message)
        {
            chiSquare = 0.0;

            if (trendRequest.IsValid(ref message) == false)
            {
                pointsUsed = 0;

                return false;
            }

            const double chiSquared95 = 3.84145882069412;
            const double chiSquared98 = 9.54953570608324;

            pointsUsed = 1;

            var sumOfr = 0.0;
            var sumOfn = 0.0;
            var sumOfrAndt = 0.0;
            var sumOfnAndt = 0.0;
            var sumOfnAndtSq = 0.0;

            var chiSquaredConfidence = trendRequest.ComparatorConfidence <= 95 ? chiSquared95 : chiSquared98;

            foreach (var dataItem in trendRequest.Data.OrderByDescending(t => t.Year).Reverse())
            {

                var r = dataItem.Count.Value;
                var n = dataItem.Denominator;
                var t = dataItem.Year;

                sumOfr += r;
                sumOfn += n;
                sumOfrAndt += r * t;
                sumOfnAndt += n * t;
                sumOfnAndtSq += n * (t * t);

                var sumPart1 = (sumOfn * sumOfrAndt - sumOfr * sumOfnAndt);

                chiSquare = (sumOfn * (sumPart1 * sumPart1)) /
                                (sumOfr * (sumOfn - sumOfr) * (sumOfn * sumOfnAndtSq - (sumOfnAndt * sumOfnAndt)));

                if (pointsUsed > 4 && chiSquare > chiSquaredConfidence)
                {
                    return true;
                }

                pointsUsed++;

            }

            pointsUsed--;
            return false;
        }
    }
}
