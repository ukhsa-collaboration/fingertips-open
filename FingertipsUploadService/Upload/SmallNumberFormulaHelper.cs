using System.Data;

namespace FingertipsUploadService.Upload
{
    public class SmallNumberFormulaHelper
    {
        public bool IsSmallNumber(double count, string expression)
        {
            var newExpress = expression.Replace("$", count.ToString("R"));
            var table = new DataTable();
            table.Columns.Add("formula", typeof(string), newExpress);
            var row = table.NewRow();
            table.Rows.Add(row);
            return bool.Parse((string)row["formula"]);
        }
    }
}
