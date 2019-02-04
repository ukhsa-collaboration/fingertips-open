
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    public abstract class IndicatorComparer
    {
        public double ConfidenceVariable { get; set; }
        public int PolarityId { get; set; }

        public abstract Significance Compare(CoreDataSet data, CoreDataSet comparator, IndicatorMetadata metadata);

        protected virtual bool CanComparisonGoAhead(ValueWithCIsData data, ValueWithCIsData comparator)
        {
            return IsPolarityValid() && IsDataValid(data) && IsDataValid(comparator);
        }

        protected bool IsPolarityValid()
        {
            return PolarityId != PolarityIds.NotApplicable;
        }

        protected bool IsDataValid(ValueWithCIsData data)
        {
            return data != null && data.IsValueValid;
        }

        protected Significance AdjustForPolarity(Significance significance)
        {
            if (PolarityId == PolarityIds.RagLowIsGood)
            {
                switch (significance)
                {
                    case Significance.Worse:
                        return Significance.Better;
                    case Significance.Worst:
                        return Significance.Best;
                    case Significance.Better:
                        return Significance.Worse;
                    case Significance.Best:
                        return Significance.Worst;
                }
            }

            return significance;
        }

        protected Significance GetSignificanceFromSDs(double numerator, double lowerSD, double upperSD)
        {
            if (upperSD < numerator)
            {
                return AdjustForPolarity(Significance.Better);
            }
            if (lowerSD > numerator)
            {
                return AdjustForPolarity(Significance.Worse);
            }
            return Significance.Same;
        }
    }
}
