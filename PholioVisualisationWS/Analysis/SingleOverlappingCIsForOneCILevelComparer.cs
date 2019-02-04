
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    public class SingleOverlappingCIsForOneCILevelComparer : IndicatorComparer
    {
        public override Significance Compare(CoreDataSet data, CoreDataSet comparator, IndicatorMetadata metadata)
        {
            if (CanComparisonGoAhead(data, comparator) == false || data.Are95CIsValid == false)
            {
                return Significance.None;
            }

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
            return (val <= data.UpperCI95 && val >= data.LowerCI95);
        }
    }
}
