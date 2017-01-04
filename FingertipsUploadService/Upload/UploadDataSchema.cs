using FingertipsUploadService.ProfileData.Entities.Core;
using System.Collections.Generic;
using System.Data;

namespace FingertipsUploadService.Upload
{
    public class UploadDataSchema
    {
        public DataTable CreateEmptyTable()
        {
            var table = new DataTable();
            table.Columns.Add(UploadColumnNames.IndicatorId);
            table.Columns.Add(UploadColumnNames.Year);
            table.Columns.Add(UploadColumnNames.YearRange);
            table.Columns.Add(UploadColumnNames.Quarter);
            table.Columns.Add(UploadColumnNames.Month);
            table.Columns.Add(UploadColumnNames.AgeId);
            table.Columns.Add(UploadColumnNames.SexId);
            table.Columns.Add(UploadColumnNames.AreaCode);
            table.Columns.Add(UploadColumnNames.Count);
            table.Columns.Add(UploadColumnNames.Value);
            table.Columns.Add(UploadColumnNames.LowerCI);
            table.Columns.Add(UploadColumnNames.UpperCI);
            table.Columns.Add(UploadColumnNames.Denominator);
            table.Columns.Add(UploadColumnNames.Denominator2);
            table.Columns.Add(UploadColumnNames.ValueNoteId);
            table.Columns.Add(UploadColumnNames.CategoryTypeId);
            table.Columns.Add(UploadColumnNames.CategoryId);

            SetColumnDataTypes(table);
            return table;
        }

        public void SetColumnDataTypes(DataTable table)
        {
            table.Columns[UploadColumnIndexes.IndicatorId].DataType = typeof(double);
            table.Columns[UploadColumnIndexes.Year].DataType = typeof(double);
            table.Columns[UploadColumnIndexes.YearRange].DataType = typeof(double);
            table.Columns[UploadColumnIndexes.Quarter].DataType = typeof(double);
            table.Columns[UploadColumnIndexes.Month].DataType = typeof(double);
            table.Columns[UploadColumnIndexes.AgeId].DataType = typeof(double);
            table.Columns[UploadColumnIndexes.SexId].DataType = typeof(double);
            table.Columns[UploadColumnIndexes.AreaCode].DataType = typeof(string);
            table.Columns[UploadColumnIndexes.Count].DataType = typeof(double);
            table.Columns[UploadColumnIndexes.Value].DataType = typeof(double);
            table.Columns[UploadColumnIndexes.LowerCI].DataType = typeof(double);
            table.Columns[UploadColumnIndexes.UpperCI].DataType = typeof(double);
            table.Columns[UploadColumnIndexes.Denominator].DataType = typeof(double);
            table.Columns[UploadColumnIndexes.Denominator2].DataType = typeof(double);
            table.Columns[UploadColumnIndexes.ValueNoteId].DataType = typeof(double);
            table.Columns[UploadColumnIndexes.CategoryTypeId].DataType = typeof(double);
            table.Columns[UploadColumnIndexes.CategoryId].DataType = typeof(double);
        }

        public DataTable CreateDataTable(List<CoreDataSet> dataRows)
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
                    row.LowerCi,
                    row.UpperCi,
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
