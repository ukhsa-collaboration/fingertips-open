
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
            if (data != null && data.HasFormattedValue == false)
            {
                data.ValueFormatted = formatMethod(data.Value);
            }
        }

        public override void FormatConfidenceIntervals(ValueWithCIsData data)
        {
            if (data != null && data.HasFormattedCIs == false)
            {
                data.UpperCI95F = formatNullableMethod(data.UpperCI95);
                data.LowerCI95F = formatNullableMethod(data.LowerCI95);
                data.UpperCI99_8F = formatNullableMethod(data.UpperCI99_8);
                data.LowerCI99_8F = formatNullableMethod(data.LowerCI99_8);
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
                Median = formatMethod(stats.Median),
                Percentile5 = formatMethod(stats.Percentile5),
                Percentile25 = formatMethod(stats.Percentile25),
                Percentile75 = formatMethod(stats.Percentile75),
                Percentile95 = formatMethod(stats.Percentile95)
            };
        }

        private static string FormatOnValue(double val)
        {
            if (val == ValueData.NullValue)
            {
                return NoValue;
            }

            return FormatValidValue(val);
        }

        private static string FormatOnNullableValue(double? val)
        {
            if (val.HasValue == false)
            {
                return NoValue;
            }

            return FormatValidValue(val.Value);
        }

        private static string FormatValidValue(double val)
        {
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
                formatNullableMethod = FormatOnNullableValue;
                return;
            }

            double max = limits.Max;

            if (max < 1)
            {
                formatMethod = Format3DP;
                formatNullableMethod = FormatNullable3DP;
            }
            else if (max < 10)
            {
                formatMethod = Format2DP;
                formatNullableMethod = FormatNullable2DP;
            }
            else
            {
                formatMethod = FormatOnValue;
                formatNullableMethod = FormatOnNullableValue;
            }
        }
    }
}
