using System;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Formatting
{
    public class SuicidePreventionPlanFormatter : NumericFormatter
    {
        public override IndicatorStatsPercentilesFormatted FormatStats(IndicatorStatsPercentiles stats)
        {
            return GetNullStats();
        }

        public override void Format(ValueData data)
        {
            if (data != null && data.HasFormattedValue == false)
            {
                data.ValueFormatted = FormatValue(data.Value);
            }
        }

        public override void FormatConfidenceIntervals(ValueWithCIsData data)
        {
            if (data != null && data.HasFormattedCIs == false)
            {
                data.UpperCI95F = NoValue;
                data.LowerCI95F = NoValue;
            }
        }

        protected override void SetFormatMethod() { }

        private static string FormatValue(double val)
        {
            if (val == ValueData.NullValue)
            {
                return NoValue;
            }

            switch (Convert.ToInt32(val))
            {
                case SuicidePlanStatus.Exists:
                    return "Suicide prevention plan exists";
                case SuicidePlanStatus.InDevelopment:
                    return "Suicide prevention plan is in development";
                default:
                    return "No suicide prevention plan";
            }
        }
    }
}