namespace FingertipsDataExtractionTool.AverageCalculator
{
    public class AverageCalculationConfig
    {
        public int DaysToCheckForChangedIndicators { get; set; }

        /// <summary>
        /// Indicator ID to start processing from
        /// </summary>
        public int IndicatorIdToStartFrom { get; set; }

        public AverageCalculationConfig()
        {
            DaysToCheckForChangedIndicators = FdetConfiguration.DaysToCheckForChangedIndicators;
            IndicatorIdToStartFrom = FdetConfiguration.IndicatorIdToStartFrom;
        }
    }
}