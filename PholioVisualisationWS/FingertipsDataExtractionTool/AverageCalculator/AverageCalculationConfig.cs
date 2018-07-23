namespace FingertipsDataExtractionTool.AverageCalculator
{
    public class AverageCalculationConfig
    {
        public int DaysToCheckForChangedIndicators { get; set; }

        /// <summary>
        /// Indicator ID to start processing from
        /// </summary>
        public int IndicatorIdToStartFrom { get; set; }

        /// <summary>
        /// Just calculate averages for all indicators
        /// </summary>
        public bool CalculateAveragesWhetherOrNotIndicatorsHaveChanged { get; set; }

        public AverageCalculationConfig()
        {
            DaysToCheckForChangedIndicators = FdetConfiguration.DaysToCheckForChangedIndicators;
            IndicatorIdToStartFrom = FdetConfiguration.IndicatorIdToStartFrom;
            CalculateAveragesWhetherOrNotIndicatorsHaveChanged = FdetConfiguration.CalculateAveragesWhetherOrNotIndicatorsHaveChanged;
        }
    }
}