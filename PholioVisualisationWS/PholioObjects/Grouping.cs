
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
            // TODO use automapper
            GroupingId = templateGrouping.GroupingId;
            GroupId = templateGrouping.GroupId;
            SexId = templateGrouping.SexId;
            Sex = templateGrouping.Sex;
            AgeId = templateGrouping.AgeId;
            Age = templateGrouping.Age;
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

        /// <summary>
        /// Entity ID
        /// </summary>
        [JsonProperty]
        public int GroupingId { get; set; }

        /// <summary>
        /// Domain ID
        /// </summary>
        [JsonProperty]
        public int GroupId { get; set; }

        [JsonProperty]
        public int SexId { get; set; }

        [JsonProperty]
        public int AgeId { get; set; }

        [JsonIgnore]
        public Sex Sex { get; set; }

        [JsonIgnore]
        public Age Age { get; set; }

        [JsonProperty]
        public int AreaTypeId { get; set; }

        [JsonProperty]
        public int ComparatorId { get; set; }

        [JsonProperty]
        public int IndicatorId { get; set; }

        [JsonProperty]
        public int PolarityId { get; set; }

        [JsonProperty(PropertyName = "ComparatorMethodId")]
        public int ComparatorMethodId { get; set; }

        [JsonProperty(PropertyName = "SigLevel")]
        public double ComparatorConfidence { get; set; }

        [JsonProperty]
        public int? ComparatorTargetId { get; set; }

        [JsonProperty]
        public int Sequence { get; set; }

        [JsonProperty]
        public int BaselineYear { get; set; }

        [JsonProperty]
        public int BaselineQuarter { get; set; }

        [JsonProperty]
        public int BaselineMonth { get; set; }

        [JsonProperty]
        public int DataPointYear { get; set; }

        [JsonProperty]
        public int DataPointQuarter { get; set; }

        [JsonProperty]
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
