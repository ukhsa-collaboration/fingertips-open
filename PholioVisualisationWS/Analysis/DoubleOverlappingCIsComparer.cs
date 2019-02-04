
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    /// <summary>
    /// Value confidence intervals overlapping with benchmark confidence intervals
    /// </summary>
    public class DoubleOverlappingCIsComparer : IndicatorComparer
    {
        public override Significance Compare(CoreDataSet data, CoreDataSet comparator, IndicatorMetadata metadata)
        {
            return GetSignificance(data, comparator);
        }

        public Significance Compare(ValueWithCIsData data, ValueWithCIsData comparator, IndicatorMetadata metadata)
        {
            return GetSignificance(data, comparator);
        }

        private Significance GetSignificance(ValueWithCIsData data, ValueWithCIsData comparator)
        {
            if (CanComparisonGoAhead(data, comparator) == false ||
                data.Are95CIsValid == false ||
                comparator.Are95CIsValid == false)
            {
                return Significance.None;
            }

            if (DoCIsOverlap(comparator, data))
            {
                return Significance.Same;
            }

            if (data.Value < comparator.Value)
            {
                return AdjustForPolarity(Significance.Worse);
            }

            return AdjustForPolarity(Significance.Better);
        }

        private static bool DoCIsOverlap(ValueWithCIsData data1, ValueWithCIsData data2)
        {
            return data1.Value < data2.Value ? data2.LowerCI95 <= data1.UpperCI95 : data1.LowerCI95 <= data2.UpperCI95;
        }
    }
}
