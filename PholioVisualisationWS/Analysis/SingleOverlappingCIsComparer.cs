
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    public class SingleOverlappingCIsComparer : IndicatorComparer
    {
        public override Significance Compare(CoreDataSet data, CoreDataSet comparator, IndicatorMetadata metadata)
        {
            if (CanComparisonGoAhead(data, comparator) == false || data.AreCIsValid == false)
            {
                return Significance.None;
            }

            // Method same as empho spreadsheet (ask Paul Brand)
            if (IsBetweenConfidenceInverval(comparator.Value, data))
            {
                return Significance.Same;
            }

            if (data.Value < comparator.Value)
            {
                return AdjustForPolarity(Significance.Worse);
            }

            return AdjustForPolarity(Significance.Better);
        }

        private static bool IsBetweenConfidenceInverval(double val, CoreDataSet data)
        {
            return (val <= data.UpperCI && val >= data.LowerCI);
        }
    }
}
