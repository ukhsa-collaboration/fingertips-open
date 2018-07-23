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

        public static bool CalculateAveragesWhetherOrNotIndicatorsHaveChanged
        {
            get { return ParseBool("CalculateAveragesWhetherOrNotIndicatorsHaveChanged"); }
        }

        private static int ParseInt(string s)
        {
            return int.Parse(ConfigurationManager.AppSettings[s]);
        }

        private static bool ParseBool(string s)
        {
            return bool.Parse(ConfigurationManager.AppSettings[s]);
        }
    }
}