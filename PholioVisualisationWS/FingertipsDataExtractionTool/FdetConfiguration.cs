using System.Configuration;

namespace FingertipsDataExtractionTool
{
    public static class FdetConfiguration
    {
        public static int DaysToCheckForChangedIndicators
        {
            get { return ParseInt("DaysToCheckForChangedIndicators"); }
        }

        public static int IndicatorIdToStartFrom
        {
            get { return ParseInt("IndicatorIdToStartFrom"); }
        }

        private static int ParseInt(string s)
        {
            return int.Parse(ConfigurationManager.AppSettings[s]);
        }
    }
}