
using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Formatting
{
    public class DefaultFormatter : NumericFormatter
    {
        /// <summary>
        /// For Mock object creation
        /// </summary>
        public DefaultFormatter() { }

        public DefaultFormatter(IndicatorMetadata metadata, Limits limits) :
            base(metadata, limits)
        {
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
                data.UpperCI95F = FormatNullableValue(data.UpperCI95);
                data.LowerCI95F = FormatNullableValue(data.LowerCI95);
                data.UpperCI99_8F = FormatNullableValue(data.UpperCI99_8);
                data.LowerCI99_8F = FormatNullableValue(data.LowerCI99_8);
            }
        }

        private string FormatValue(double val)
        {
            if (val == ValueData.NullValue)
            {
                return NoValue;
            }
            return formatMethod(val);
        }

        /// <summary>
        /// Nullables are never -1
        /// </summary>
        private string FormatNullableValue(double? val)
        {
            return val.HasValue ? formatMethod(val.Value) : NoValue;
        }

        private string FormatOnValue(double val)
        {
            if (val > 1000)
            {
                return FormatZeroDP(val);
            }
            if (val > 10)
            {
                return Format1DP(val);
            }
            if (val < 1)
            {
                return Format3DP(val);
            }
            return Format2DP(val);
        }

        private string FormatNullableOnValue(double? val)
        {
            if (val.HasValue)
            {
                return FormatOnValue(val.Value);
            }
            return NoValue;
        }

        protected override void SetFormatMethod()
        {
            if (limits == null)
            {
                formatMethod = FormatOnValue;                                                                                                                                                          
                formatNullableMethod = FormatNullableOnValue;
                return;
            }                                                                                                                                          

            double max = limits.Max;

            if (max > 1000)
            {
                formatMethod = FormatZeroDP;
            }
            else if (max > 10)
            {
                formatMethod = Format1DP;
            }
            else if (max < 1)
            {
                formatMethod = Format3DP;
            }
            else
            {
                formatMethod = Format2DP;
            }
        }

    }
}
