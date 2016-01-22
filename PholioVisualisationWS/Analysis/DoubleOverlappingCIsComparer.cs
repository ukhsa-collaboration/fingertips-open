
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
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
                data.AreCIsValid == false ||
                comparator.AreCIsValid == false)
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
            return data1.Value < data2.Value ? data2.LowerCI <= data1.UpperCI : data1.LowerCI <= data2.UpperCI;
        }
    }
}
