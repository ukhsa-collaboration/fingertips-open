namespace Fpm.Upload
{
    public class DuplicateRowInDatabaseError
    {
        public int RowNumber { get; set; }
        public int IndicatorId { get; set; }
        public int AgeId { get; set; }
        public int SexId { get; set; }
        public int Uid { get; set; }
        public string AreaCode { get; set; }
        public string ErrorMessage { get; set; }
        public double DbValue { get; set; }
        public double ExcelValue { get; set; }
    }
}