
using SpreadsheetGear;

namespace PholioVisualisation.Export
{
    /// <summary>
    /// Instructs an ExcelFileWriter instance what data to write to the Excel file being created.
    /// </summary>
    public abstract class ExcelFileBuilder
    {
        public abstract IWorkbook BuildWorkbook();
    }
}
