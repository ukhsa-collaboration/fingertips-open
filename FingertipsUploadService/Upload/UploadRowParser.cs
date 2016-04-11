using System.Data;

namespace FingertipsUploadService.Upload
{
    public class UploadRowParser
    {
        public const double UndefinedDouble = -1;
        public const int UndefinedInt = -1;
        public const int DefaultValueNoteId = 0;

        protected DataRow Row;

        public UploadRowParser(DataRow row)
        {
            Row = row;
        }

        public string AreaCode
        {
            get { return Row.Field<string>(UploadColumnNames.AreaCode); }
        }

        public double? Count
        {
            get { return Row.Field<double?>(UploadColumnNames.Count); }
        }
    }
}