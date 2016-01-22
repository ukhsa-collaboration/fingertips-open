namespace PholioVisualisation.PholioObjects
{
    public class TimeRange
    {
        public TimePeriod FirstTimePeriod { get; set; }
        public TimePeriod LastTimePeriod { get; set; }

        public TimeRange(Grouping grouping)
        {
            FirstTimePeriod = TimePeriod.GetBaseline(grouping);
            LastTimePeriod = TimePeriod.GetDataPoint(grouping);
        }
    }
}