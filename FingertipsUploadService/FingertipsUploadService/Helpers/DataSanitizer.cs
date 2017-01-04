using System;
using System.Data;
using FingertipsUploadService.ProfileData;
using FingertipsUploadService.Upload;

namespace FingertipsUploadService.Helpers
{
    public class DataSanitizer
    {
        // Default value
        private readonly double minusOne = -1;

        private DataRow row;

        private readonly int[] _columnIndexesWithMinusOneDefaults =
        {
            UploadColumnIndexes.Quarter,
            UploadColumnIndexes.Month,
            UploadColumnIndexes.Count,
            UploadColumnIndexes.Value,
            UploadColumnIndexes.LowerCI,
            UploadColumnIndexes.UpperCI,
            UploadColumnIndexes.Denominator,
            UploadColumnIndexes.Denominator2,
            UploadColumnIndexes.CategoryTypeId,
            UploadColumnIndexes.CategoryId
        };

        public void SanitizeExcelData(DataTable table)
        {
            var rows = table.Rows;

            for (var i = 0; i < rows.Count; i++)
            {
                row = rows[i];

                foreach (var columnIndex in _columnIndexesWithMinusOneDefaults)
                {
                    SetMinusOneIfNull(columnIndex);
                }

                SanitizeValueNoteId();
            }
        }

        private void SetMinusOneIfNull(int index)
        {
            if (row[index] == DBNull.Value)
            {
                row[index] = minusOne;
            }
        }

        private void SanitizeValueNoteId()
        {
            if (row[UploadColumnIndexes.ValueNoteId] == DBNull.Value)
            {
                row[UploadColumnIndexes.ValueNoteId] = ValueNoteIds.NoNote;
            }
        }
    }
}
