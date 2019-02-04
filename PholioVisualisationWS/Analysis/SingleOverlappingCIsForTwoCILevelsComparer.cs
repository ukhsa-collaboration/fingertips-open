using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    public class SingleOverlappingCIsForTwoCILevelsComparer : IndicatorComparer
    {
        public override Significance Compare(CoreDataSet data, CoreDataSet comparator, IndicatorMetadata metadata)
        {
            if (CanComparisonGoAhead(data, comparator) == false || data.Are95CIsValid == false || data.Are99_8CIsValid == false)
            {
                return Significance.None;
            }

            if (IsBetweenConfidenceInterval(comparator.Value, data))
            {
                return Significance.Same;
            }

            if (IsBetweenLowerConfidenceInterval95And98(comparator.Value, data))
            {
                return AdjustForPolarity(Significance.Worse);
            }

            if (data.Value < comparator.Value)
            {
                return AdjustForPolarity(Significance.Worst);
            }

            if (IsBetweenHigherConfidenceInterval95And98(comparator.Value, data))
            {
                return AdjustForPolarity(Significance.Better);
            }

            return AdjustForPolarity(Significance.Best);
        }

        private static bool IsBetweenConfidenceInterval(double val, CoreDataSet data)
        {
            return (val <= data.UpperCI95 && val >= data.LowerCI95);
        }

        private static bool IsBetweenLowerConfidenceInterval95And98(double val, CoreDataSet data)
        {
            return (val <= data.LowerCI95 && val >= data.LowerCI99_8);
        }

        private static bool IsBetweenHigherConfidenceInterval95And98(double val, CoreDataSet data)
        {
            return (val <= data.UpperCI99_8 && val >= data.UpperCI95);
        }
    }
}
