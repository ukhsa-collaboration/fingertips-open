
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class Grouping
    {
        public Grouping()
        {
            BaselineMonth = TimePoint.Undefined;
            BaselineQuarter = TimePoint.Undefined;
            DataPointMonth = TimePoint.Undefined;
            DataPointQuarter = TimePoint.Undefined;
        }

        public Grouping(Grouping templateGrouping)
        {
            GroupingId = templateGrouping.GroupingId;
            GroupId = templateGrouping.GroupId;
            SexId = templateGrouping.SexId;
            AgeId = templateGrouping.AgeId;
            AreaTypeId = templateGrouping.AreaTypeId;
            IndicatorId = templateGrouping.IndicatorId;
            PolarityId = templateGrouping.PolarityId;
            ComparatorMethodId = templateGrouping.ComparatorMethodId;
            ComparatorTargetId = templateGrouping.ComparatorTargetId;
            ComparatorConfidence = templateGrouping.ComparatorConfidence;
            Sequence = templateGrouping.Sequence;
            BaselineYear = templateGrouping.BaselineYear;
            BaselineMonth = templateGrouping.BaselineMonth;
            BaselineQuarter = templateGrouping.BaselineQuarter;
            DataPointYear = templateGrouping.DataPointYear;
            DataPointQuarter = templateGrouping.DataPointQuarter;
            DataPointMonth = templateGrouping.DataPointMonth;
            YearRange = templateGrouping.YearRange;
            TimePeriodText = templateGrouping.TimePeriodText;
            ComparatorData = templateGrouping.ComparatorData;
        }

        [JsonIgnore]
        public int GroupingId { get; set; }

        [JsonIgnore]
        public int GroupId { get; set; }

        [JsonIgnore]
        public int SexId { get; set; }

        [JsonIgnore]
        public int AgeId { get; set; }

        [JsonIgnore]
        public int AreaTypeId { get; set; }

        [JsonProperty]
        public int ComparatorId { get; set; }

        [JsonIgnore]
        public int IndicatorId { get; set; }

        [JsonIgnore]
        public int PolarityId { get; set; }

        [JsonProperty(PropertyName = "MethodId")]
        public int ComparatorMethodId { get; set; }

        [JsonProperty(PropertyName = "SigLevel")]
        public double ComparatorConfidence { get; set; }

        [JsonIgnore]
        public int? ComparatorTargetId { get; set; }

        [JsonIgnore]
        public int Sequence { get; set; }

        [JsonIgnore]
        public int BaselineYear { get; set; }

        [JsonIgnore]
        public int BaselineQuarter { get; set; }

        [JsonIgnore]
        public int BaselineMonth { get; set; }

        [JsonIgnore]
        public int DataPointYear { get; set; }

        [JsonIgnore]
        public int DataPointQuarter { get; set; }

        [JsonIgnore]
        public int DataPointMonth { get; set; }

        [JsonProperty]
        public int YearRange { get; set; }

        [JsonProperty(PropertyName = "Period")]
        public string TimePeriodText { get; set; }

        [JsonProperty]
        public CoreDataSet ComparatorData { get; set; }

        public bool IsDataQuarterly()
        {
            return BaselineQuarter > 0;
        }

        public bool IsDataMonthly()
        {
            return BaselineMonth > 0;
        }

        public bool IsComparatorDefined()
        {
            return ComparatorId > -1;
        }

        public TimePeriodIterator GetTimePeriodIterator(YearType yearType)
        {
            return new TimePeriodIterator(TimePeriod.GetBaseline(this),
                TimePeriod.GetDataPoint(this), yearType);
        }
    }
}
