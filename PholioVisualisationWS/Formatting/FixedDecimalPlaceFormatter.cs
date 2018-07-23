using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Formatting
{
    public class FixedDecimalPlaceFormatter : NumericFormatter
    {
        private string formatString;
        private int decimalPlacesToDisplay;

        public FixedDecimalPlaceFormatter(int decimalPlacesToDisplay) :
            base(null, null)
        {
            this.decimalPlacesToDisplay = decimalPlacesToDisplay;
            SetFormatString();
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

        public override IndicatorStatsPercentilesFormatted FormatStats(IndicatorStatsPercentiles stats)
        {
            if (stats == null)
            {
                return GetNullStats();
            }
            return new IndicatorStatsPercentilesFormatted
            {
                Min = FormatValue(stats.Min),
                Max = FormatValue(stats.Max),
                Median = FormatValue(stats.Median),
                Percentile5 = FormatValue(stats.Percentile5),
                Percentile25 = FormatValue(stats.Percentile25),
                Percentile75 = FormatValue(stats.Percentile75),
                Percentile95 = FormatValue(stats.Percentile95)
            };
        }

        private void SetFormatString()
        {
            StringBuilder sb = new StringBuilder("{0:0.");
            for (int i = 0; i < decimalPlacesToDisplay; i++)
            {
                sb.Append("0");
            }
            sb.Append("}");
            formatString = sb.ToString();
        }

        private string FormatValue(double val)
        {
            if (val.Equals(ValueData.NullValue))
            {
                return NoValue;
            }

            return string.Format(formatString, val);
        }

        private string FormatNullableValue(double? val)
        {
            if (val.HasValue)
            {
                return string.Format(formatString, val);
            }
            return NoValue;
        }

        protected override void SetFormatMethod() { }
    }
}
