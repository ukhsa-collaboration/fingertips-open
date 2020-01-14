using System.Collections.Generic;
using System.Linq;
using IndicatorsUI.DataAccess;

namespace IndicatorsUI.MainUI.Helpers
{
    public static class QuartilesLabelHelper
    {
        public static IList<string> GetLabels()
        {
            // Best / worst
            return new List<string>
                {
                    "Best",
                    "Better than average rank",
                    "Worse than average rank",
                    "Worst"
                };
        }

        public static IList<string> GetLabelsWithoutLineBreaks()
        {
            return GetLabels().Select(x => x.Replace("<br>", " ")).ToList();
        }
    }
}