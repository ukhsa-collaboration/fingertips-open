
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
                Percentile25 = formatMethod(stats.Percentile25),
                Percentile75 = formatMethod(stats.Percentile75)
            };
        }

        public override void Format(ValueData data)
        {
            if (data != null)
            {
                data.ValueFormatted = FormatValue(data.Value);
            }
        }

        public override void FormatConfidenceIntervals(ValueWithCIsData data)
        {
            if (data != null)
            {
                data.UpperCIF = FormatValue(data.UpperCI);
                data.LowerCIF = FormatValue(data.LowerCI);
            }
        }

        private string FormatValue(double val)
        {
            if (val == -1)
            {
                return NoValue;
            }
            return formatMethod(val);
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

        protected override void SetFormatMethod()
        {
            if (limits == null)
            {
                formatMethod = FormatOnValue;
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
