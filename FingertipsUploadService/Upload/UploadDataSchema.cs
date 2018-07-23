using FingertipsUploadService.ProfileData.Entities.Core;
using System.Collections.Generic;
using System.Data;

namespace FingertipsUploadService.Upload
{
    public static class UploadDataSchema
    {
        public static DataTable CreateEmptyTable()
        {
            var table = new DataTable();

            var columnNames = UploadColumnNames.GetColumnNames();
            foreach (var columnName in columnNames)
            {
                table.Columns.Add(columnName);
            }

            SetColumnDataTypes(table);
            return table;
        }

        public static void SetColumnDataTypes(DataTable table)
        {
            foreach (DataColumn column in table.Columns)
            {
                var lowerColumnName = column.ColumnName.ToLower();
                if (lowerColumnName == UploadColumnNames.AreaCode)
                {
                    column.DataType = typeof(string);
                }
                else
                {
                    // All numeric columns are doubles because this is the type given when they are read from the spreadsheet
                    column.DataType = typeof(double);
                }
            }
        }

        public static DataTable CreateDataTable(List<CoreDataSet> dataRows)
        {
            var table = CreateEmptyTable();
            foreach (var row in dataRows)
            {
                table.Rows.Add(row.IndicatorId,
                    row.Year,
                    row.YearRange,
                    row.Quarter,
                    row.Month,
                    row.AgeId,
                    row.SexId,
                    row.AreaCode,
                    row.Count,
                    row.Value,
                    row.LowerCI95,
                    row.UpperCI95,
                    row.LowerCI99_8,
                    row.UpperCI99_8,
                    row.Denominator,
                    row.Denominator_2,
                    row.ValueNoteId,
                    row.CategoryTypeId,
                    row.CategoryId);
            }
            return null;
        }
    }
}
