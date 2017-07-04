using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export
{
    public class SignificanceFormatter
    {
        private readonly int _polarityId;
        private readonly int _comparatorMethodId;

        public SignificanceFormatter(int polarityId, int comparatorMethodId)
        {
            _polarityId = polarityId;
            _comparatorMethodId = comparatorMethodId;
        }

        public string GetLabel(Significance significance)
        {
            // Comparator method first because it overrides polarity if quintiles
            if (_comparatorMethodId == ComparatorMethodIds.Quintiles)
            {
                switch ((int)significance)
                {
                    case 1:
                        return "Lowest quintile";
                    case 2:
                        return "2nd lowest quintile";
                    case 3:
                        return "Middle quintile";
                    case 4:
                        return "2nd highest quintile";
                    case 5:
                        return "Highest quintile";
                }
            }

            if (_polarityId == PolarityIds.BlueOrangeBlue)
            {
                switch (significance)
                {
                    case Significance.Worse:
                        return "Lower";
                    case Significance.Same:
                        return "Same";
                    case Significance.Better:
                        return "Higher";
                }
            }
            else if (_polarityId == PolarityIds.RagHighIsGood ||
                     _polarityId == PolarityIds.RagLowIsGood)
            {
                switch (significance)
                {
                    case Significance.Worse:
                        return "Worse";
                    case Significance.Same:
                        return "Same";
                    case Significance.Better:
                        return "Better";
                }
            }

            return "Not compared";
        }
    }
}