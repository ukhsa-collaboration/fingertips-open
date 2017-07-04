namespace FingertipsUploadService.Upload
{
    public class SmallNumberWarning
    {
        public SmallNumberWarning()
        {
        }

        public SmallNumberWarning(int rowNumber, double countValue)
        {
            RowNumber = rowNumber;
            SmallCountValue = countValue;
        }

        public int? RowNumber { get; set; }
        public double SmallCountValue { get; set; }
    }
}