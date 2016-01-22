
using System;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    public class SpcForDsrComparer : IndicatorComparer
    {
        public override Significance Compare(CoreDataSet data, CoreDataSet comparator, IndicatorMetadata metadata)
        {
            if (CanComparisonGoAhead(data, comparator) == false || !data.IsDenominator2Valid)
            {
                return Significance.None;
            }

            double unit = metadata.Unit.Value;

            double denominator = data.Denominator2;

            double E1 = comparator.Value * (denominator / unit);

            double l2SD = (StatisticalMethods.PoisLow(E1, ConfidenceVariable) / denominator) * unit;
            double u2SD = (StatisticalMethods.PoisHigh(E1, ConfidenceVariable) / denominator) * unit;

            return GetSignificanceFromSDs(data.Value, l2SD, u2SD);
        }
    }
}
