using System.Collections.Generic;
using System.Linq;
using IndicatorsUI.DataAccess;

namespace IndicatorsUI.MainUI.Helpers
{
    public static class QuartilesLabelHelper
    {
        public static IList<string> GetLabels()
        {
            if (AppConfig.Instance.IsFeatureActive("useLongerLivesBestWorstLabels"))
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

            // High / low
            return new List<string>
            {
                "Substantially<br>above average",
                "Above average",
                "Below average",
                "Substantially<br>below average"
            };
        }

        public static IList<string> GetLabelsWithoutLineBreaks()
        {
            return GetLabels().Select(x => x.Replace("<br>"," ")).ToList();
        }
    }
}