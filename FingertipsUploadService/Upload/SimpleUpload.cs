
namespace Fpm.Upload
{
    public class SimpleUpload : BaseUpload 
    {
        public int IndicatorId { get; set; }
        public int Year { get; set; }
        public int YearTypeId { get; set; }
        public string YearTypeDescription { get; set; }
        public int YearRange { get; set; }
        public int Quarter { get; set; }
        public int Month { get; set; }
        public int AgeId { get; set; }
        public int SexId { get; set; }
        public int ValueNoteId { get; set; }

        public string UploadPeriod { set; get; }

        public override string RequiredWorksheetText()
        {
            return "\"IndicatorDetails\" and \"PholioData\" worksheets";
        }
    }
}