namespace FingertipsUploadService.ProfileData.Entities.Profile
{
    public class Grouping
    {
        public int GroupingId { get; set; }

        public int GroupId { get; set; }

        public int SexId { get; set; }

        public int AgeId { get; set; }

        public int AreaTypeId { get; set; }

        public int IndicatorId { get; set; }

        public int Sequence { get; set; }

        public int ComparatorId { get; set; }

        public int ComparatorMethodId { get; set; }

        public double ComparatorConfidence { get; set; }

        public int YearRange { get; set; }

        public int BaselineYear { get; set; }

        public int DataPointYear { get; set; }

        public int BaselineQuarter { get; set; }

        public int DataPointQuarter { get; set; }

        public int BaselineMonth { get; set; }

        public int DataPointMonth { get; set; }

        public int TimeSeries
        {
            get
            {
                if (BaselineQuarter > 0)
                {
                    return 2;
                }

                if (BaselineMonth > 0)
                {
                    return 3;
                }

                return 1;
            }
        }

        public int ComparatorTargetId { get; set; }
        public int PolarityId { get; set; }

        public bool CanHaveSameSequence(Grouping other)
        {
            return GroupId == other.GroupId &&
                   IndicatorId == other.IndicatorId &&
                   AgeId == other.AgeId &&
                   SexId == other.SexId &&
                   AreaTypeId == other.AreaTypeId &&
                   YearRange == other.YearRange;
        }
    }
}
