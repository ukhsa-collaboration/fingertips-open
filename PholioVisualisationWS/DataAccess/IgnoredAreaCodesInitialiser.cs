using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.Parsers;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess
{
    public class IgnoredAreaCodesInitialiser
    {
        private ProfileConfig profileConfig;

        public IgnoredAreaCodesInitialiser(ProfileConfig profileConfig)
        {
            this.profileConfig = profileConfig;
        }

        public IgnoredAreaCodes Initialised
        {
            get
            {
                List<string> ignoreEverywhere;
                List<string> ignoreForSpineChart;

                if (profileConfig == null)
                {
                    ignoreEverywhere = new List<string>();
                    ignoreForSpineChart = new List<string>();
                }
                else
                {
                    ignoreEverywhere = new StringListParser(profileConfig.AreaCodesIgnoredEverywhereString).StringList;
                    ignoreForSpineChart = new StringListParser(profileConfig.AreaCodesIgnoredForSpineChartString).StringList;
                    ignoreForSpineChart.AddRange(ignoreEverywhere);
                }

                return new IgnoredAreaCodes()
                    {
                        AreaCodesIgnoredEverywhere = ignoreEverywhere,
                        AreaCodesIgnoredForSpineChart = ignoreForSpineChart
                    };
            }
        }
    }
}
