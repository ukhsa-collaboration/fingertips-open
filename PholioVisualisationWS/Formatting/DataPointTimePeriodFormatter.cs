
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Formatting
{
    public class DataPointTimePeriodFormatter : TimePeriodFormatter
    {
        public override void Format(Grouping grouping, IndicatorMetadata indicatorMetadata)
        {
            grouping.TimePeriodText = GetTimePeriodString(TimePeriod.GetDataPoint(grouping), indicatorMetadata.YearTypeId);
        }

    }
}
