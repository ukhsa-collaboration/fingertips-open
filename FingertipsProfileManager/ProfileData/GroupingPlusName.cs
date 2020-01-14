namespace Fpm.ProfileData
{
    public class GroupingPlusName
    {
        public int ComparatorId { get; set; }
        public int IndicatorId { get; set; }
        public string IndicatorName { get; set; }
        public int SexId { get; set; }
        public string Sex { get; set; }
        public string Age { get; set; }
        public int AgeId { get; set; }
        public int Sequence { get; set; }
        public int YearRange { get; set; }
        public int BaselineYear { get; set; }
        public int BaselineQuarter { get; set; }
        public int BaselineMonth { get; set; }
        public int DatapointYear { get; set; }
        public int DatapointQuarter { get; set; }
        public int DatapointMonth { get; set; }
        public int AreaTypeId { get; set; }
        public string AreaType { get; set; }
        public string ComparatorConfidence { get; set; }
        public int ComparatorMethodId { get; set; }
        public string ComparatorMethod { get; set; }
        public int YearTypeId { get; set; }
        public bool UsedElsewhere { get; set; }
        public int? ProfileId { get; set; }
        public int? GroupId { get; set; }
        public string IndicatorNumber { get; set; }
    }
}
