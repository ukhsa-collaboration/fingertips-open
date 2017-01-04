using CsvHelper;
using FingertipsUploadService.ProfileData;
using System.Data;
using System.IO;
using FingertipsUploadService.Upload;

namespace FingertipsUploadService.Helpers
{
    public class CsvRowParser
    {
        public DataTable Parse(StreamReader stream)
        {
            var coreData = new DataTable();
            var csv = new CsvReader(stream);
            csv.Configuration.HasHeaderRecord = true;

            csv.Configuration.HasHeaderRecord = true;
            csv.ReadHeader();

            // Create Datatable columns from header row
            foreach (var header in csv.FieldHeaders)
            {
                coreData.Columns.Add(header);
            }

            // Define datatypes for data table
            new UploadDataSchema().SetColumnDataTypes(coreData);

            // Read the rest of the file and create fill datatable
            while (csv.Read())
            {
                var row = coreData.NewRow();
                var columns = coreData.Columns;

                for (var i = 0; i < columns.Count; i++)
                {
                    var fieldValue = csv.GetField(i);
                    if (i == UploadColumnIndexes.AreaCode)
                    {
                        // Area code 
                        row[i] = fieldValue;
                    }
                    else if (string.IsNullOrWhiteSpace(fieldValue) == false)
                    {
                        row[i] = fieldValue;
                    }
                    else
                    {
                        row[i] = EmptyCellToAcceptableValue(i);
                    }
                }
                coreData.Rows.Add(row);
            }
            return coreData;
        }

        // Apart from value note all fields should have -1 value
        private int EmptyCellToAcceptableValue(int index)
        {
            return index == UploadColumnIndexes.ValueNoteId
                ? ValueNoteIds.NoNote
                : -1 /* -1 is default for all other numeric columns */;
        }
    }
}
