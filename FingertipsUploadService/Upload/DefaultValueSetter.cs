using System;
using System.Data;
using FingertipsUploadService.ProfileData;

namespace FingertipsUploadService.Upload
{
    /// <summary>
    /// Add default where values are missing. CSV data is sanitised when it is read.
    /// </summary>
    public class DefaultValueSetter
    {
        // Default value
        private const double MinusOne = -1;

        private DataRow _row;

        private readonly string[] _columnsWithMinusOneDefaults =
        {
            UploadColumnNames.Quarter,
            UploadColumnNames.Month,
            UploadColumnNames.Count,
            UploadColumnNames.Value,
            UploadColumnNames.LowerCI95,
            UploadColumnNames.UpperCI95,
            UploadColumnNames.LowerCI99_8,
            UploadColumnNames.UpperCI99_8,
            UploadColumnNames.Denominator,
            UploadColumnNames.Denominator2,
            UploadColumnNames.CategoryTypeId,
            UploadColumnNames.CategoryId
        };

        public void ReplaceNullsWithDefaultValues(DataTable table)
        {
            var rows = table.Rows;

            for (var i = 0; i < rows.Count; i++)
            {
                _row = rows[i];

                foreach (var columnName in _columnsWithMinusOneDefaults)
                {
                    SetMinusOneIfNullOrEmptyOrWhitespace(columnName);
                }

                SanitizeValueNoteId();
            }
        }

        private void SetMinusOneIfNullOrEmptyOrWhitespace(string columnName)
        {
            object val = _row[columnName];
            if (val == DBNull.Value || string.IsNullOrWhiteSpace(val.ToString()))
            {
                _row[columnName] = MinusOne;
            }
        }

        private void SanitizeValueNoteId()
        {
            if (_row[UploadColumnNames.ValueNoteId] == DBNull.Value)
            {
                _row[UploadColumnNames.ValueNoteId] = ValueNoteIds.NoNote;
            }
        }
    }
}
