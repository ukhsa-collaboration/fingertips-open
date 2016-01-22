using System;

namespace Fpm.ProfileData.Entities.Core
{
    public class CoreDataSetArchive
    {
        public int IndicatorId { get; set; }
        public int Year { get; set; }
        public int YearRange { get; set; }
        public int Quarter { get; set; }
        public int Month { get; set; }
        public int AgeId { get; set; }
        public int SexId { get; set; }

        public string AreaCode { get; set; }

        public double? Count { get; set; }
        public double Value { get; set; }
        public double LowerCi { get; set; }
        public double UpperCi { get; set; }
        public double Denominator { get; set; }
        public double Denominator_2 { get; set; }
        public int ValueNoteId { get; set; }
        public int Uid { get; set; }
        public Guid UploadBatchId { get; set; }
        public Guid ReplacedByUploadBatchId { get; set; }
    }
}
