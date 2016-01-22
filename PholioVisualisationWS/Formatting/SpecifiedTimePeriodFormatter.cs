
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Formatting
{
    public class SpecifiedTimePeriodFormatter : TimePeriodFormatter
    {
        public TimePeriod TimePeriod { get; set; }

        public override void Format(Grouping grouping, IndicatorMetadata indicatorMetadata)
        {
            grouping.TimePeriodText = GetTimePeriodString(TimePeriod, indicatorMetadata.YearTypeId);
        }

        public string Format(IndicatorMetadata indicatorMetadata)
        {
            return GetTimePeriodString(TimePeriod, indicatorMetadata.YearTypeId);
        }
    }
}
