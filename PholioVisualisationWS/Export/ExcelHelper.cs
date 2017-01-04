namespace PholioVisualisation.Export
{
    public class ExcelHelper
    {
        /// <summary>
        /// Maximum number of rows found to work. Any more and 
        /// SpreadsheetGear gives I/O errors.
        /// </summary>
        public const int MaximumNumberOfRowsInXlsxFile = 750000;
    }
}