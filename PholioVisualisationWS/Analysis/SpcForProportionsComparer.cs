
using System;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    public class SpcForProportionsComparer : IndicatorComparer
    {
        public override Significance Compare(CoreDataSet data, CoreDataSet comparator, IndicatorMetadata metadata)
        {
            if (CanComparisonGoAhead(data, comparator) == false || !data.IsDenominatorValid)
            {
                return Significance.None;
            }

            double unitValue = metadata.Unit.Value;

            double P1 = data.Value / unitValue;
            double numerator = P1;

            double confidenceVariableSquared = Math.Pow(ConfidenceVariable, 2.0);
            double P0 = comparator.Value / unitValue;
            double N1 = data.Denominator;
            double E1 = N1 * P0;

            // B
            double Bs1 = 1.0 - (E1 / N1);
            double Bs2 = 4.0 * E1 * Bs1;
            double Bs3 = confidenceVariableSquared + Bs2;
            double B = ConfidenceVariable * Math.Sqrt(Bs3);

            // Control limits
            double A = (2.0 * E1) + confidenceVariableSquared;
            double C = 2.0 * (N1 + confidenceVariableSquared);
            double L3SD = (A - B) / C;
            double U3SD = (A + B) / C;

            return GetSignificanceFromSDs(numerator, L3SD, U3SD);
        }
    }
}
