using SpreadsheetGear;

namespace ExportTest
{
    public class ExportTestHelper
    {
        public static bool IsCellEmpty(IRange cell)
        {
            return string.IsNullOrEmpty(cell.Value.ToString());
        }
    }
}