using CsvHelper;
using FingertipsUploadService.ProfileData;
using FingertipsUploadService.Upload;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace FingertipsUploadService.Helpers
{
    public class CsvStreamReader
    {
        private readonly DataTable _dataTable = new DataTable();
        private CsvReader _csvReader;

        public const string InvalidNumericText = "#DIV/0!";

        public DataTable Read(StreamReader stream)
        {
            _csvReader = new CsvReader(stream);
            _csvReader.Configuration.IgnoreBlankLines = true;
            _csvReader.Configuration.SkipEmptyRecords = true;

            ReadHeader();
            UploadDataSchema.SetColumnDataTypes(_dataTable);
            ReadDataRows();
            return _dataTable;
        }

        private void ReadHeader()
        {
            _csvReader.Configuration.HasHeaderRecord = true;
            _csvReader.ReadHeader();
            foreach (var header in _csvReader.FieldHeaders)
            {
                _dataTable.Columns.Add(header);
            }
        }

        private void ReadDataRows()
        {
            var columnNames = GetColumnNames();

            var count = 1; // Count is 1 because we validate only the data not the header
            while (_csvReader.Read())
            {
                var row = _dataTable.NewRow();
                count++;

                foreach (var columnName in columnNames)
                {
                    var fieldValue = _csvReader.GetField(columnName);

                    try
                    {

                        var columnNameLower = columnName.ToLower();
                        if (columnNameLower == UploadColumnNames.AreaCode)
                        {
                            // Area code 
                            row[columnName] = fieldValue;
                        }

                        else if (string.IsNullOrWhiteSpace(fieldValue) == false)
                        {
                            row[columnName] = ParseNumericCell(fieldValue, columnName);
                        }
                        else
                        {
                            // Empty numeric field
                            row[columnName] = EmptyCellToAcceptableValue(columnNameLower);
                        }
                    }
                    catch (Exception ex)
                    {
                        var errorMessage = "Invalid value \"" + fieldValue + "\" in column \"" + 
                                           columnName + "\" at row number " + count;
                        throw new FpmException(errorMessage, ex);
                    }
                }
                _dataTable.Rows.Add(row);
            }
        }

        private IList<string> GetColumnNames()
        {
            var columnNames = UploadColumnNames.GetColumnNames(_dataTable);

            // Ignore any extra columns
            var expectedColumnNamesCount = UploadColumnNames.GetColumnNames().Count;
            if (columnNames.Count > expectedColumnNamesCount)
            {
                columnNames = columnNames.Take(expectedColumnNamesCount).ToList();
            }

            return columnNames;
        }

        private static double ParseNumericCell(string fieldValue, string columnName)
        {
            if (fieldValue == InvalidNumericText)
            {
                throw new FpmException(string.Format(@"Invalid text found in {0} column: {1}",
                    columnName, InvalidNumericText));
            }

            // Numeric field with value
            return double.Parse(fieldValue);
        }

        // Apart from value note all fields should have -1 value
        private int EmptyCellToAcceptableValue(string columnNameLower)
        {
            return columnNameLower == UploadColumnNames.ValueNoteId
                ? ValueNoteIds.NoNote
                : -1 /* -1 is default for all other numeric columns */;
        }
    }
}
