
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Formatting
{
    public class ProportionFormatter : NumericFormatter
    {
        internal ProportionFormatter(IndicatorMetadata metadata, Limits limits) :
            base(metadata, limits)
        {
        }

        public override void Format(ValueData data)
        {
            if (data != null)
            {
                data.ValueFormatted = formatMethod(data.Value);
            }
        }

        public override void FormatConfidenceIntervals(ValueWithCIsData data)
        {
            if (data != null)
            {
                data.UpperCIF = formatMethod(data.UpperCI);
                data.LowerCIF = formatMethod(data.LowerCI);
            }
        }

        public override IndicatorStatsPercentilesFormatted FormatStats(IndicatorStatsPercentiles stats)
        {
            if (stats == null)
            {
                return GetNullStats();
            }
            return new IndicatorStatsPercentilesFormatted
            {
                Min = formatMethod(stats.Min),
                Max = formatMethod(stats.Max),
                Percentile25 = formatMethod(stats.Percentile25),
                Percentile75 = formatMethod(stats.Percentile75)
            };
        }

        private static string FormatOnValue(double val)
        {
            if (val == ValueData.NullValue)
            {
                return NoValue;
            }

            if (val >= 99.95)
            {
                return "100";
            }

            return Format1DP(val);
        }

        protected override void SetFormatMethod()
        {
            if (limits == null)
            {
                formatMethod = FormatOnValue;
                return;
            }

            double max = limits.Max;

            if (max < 1)
            {
                formatMethod = Format3DP;
            }
            else if (max < 10)
            {
                formatMethod = Format2DP;
            }
            else
            {
                formatMethod = FormatOnValue;
            }
        }
    }
}
