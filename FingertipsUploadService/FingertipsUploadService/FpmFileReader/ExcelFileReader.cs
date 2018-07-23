using FingertipsUploadService.Upload;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace FingertipsUploadService.FpmFileReader
{
    public class ExcelFileReader : IUploadFileReader, IDisposable
    {
        private readonly Logger _logger = LogManager.GetLogger("ExcelFileReader");
        private string _filePath;

        public ExcelFileReader(string filePath)
        {
            _filePath = filePath;
        }

        /// <summary>
        /// Returns oleDbConnection on given file
        /// </summary>
        /// <param name="isFirstRowHeader"></param>
        /// <returns>OleDbConnection</returns>
        private OleDbConnection GetConnection(bool isFirstRowHeader)
        {
            _logger.Info("Opening file '{0}'", _filePath);

            var hdr = isFirstRowHeader ? "Yes" : "No";
            var connection = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + _filePath +
                       @";Extended Properties=""Excel 8.0;HDR=" + hdr + @";""");
            new OleDbCommand { Connection = connection };
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Returns the list of worksheet name
        /// </summary>
        /// <returns></returns>
        public List<string> GetWorksheets()
        {
            // Read worksheets
            var worksheetNames = new List<string>();

            var connection = GetConnection(false);
            var schema = connection.GetSchema("Tables");
            foreach (DataRow row in schema.Rows)
            {
                string sheetName = row["TABLE_NAME"].ToString();
                if (!sheetName.EndsWith("$") && !sheetName.EndsWith("$'"))
                    continue;
                worksheetNames.Add(sheetName);
            }

            connection.Dispose();

            return worksheetNames;
        }



        /// <summary>
        /// Reads indicator worksheet from batch upload file and  
        /// return it as DataTable
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable ReadData()
        {
            var batchData = GetDataTableFromWorksheet(WorksheetNames.Pholio, true, "results");
            return batchData;
        }


        /// <summary>
        /// Reads worksheet into datatable
        /// </summary>
        /// <param name="worksheetName"></param>
        /// <param name="isFirstRowHeader"></param>
        /// <param name="newDataTableName"></param>
        /// <returns>DataTable</returns>
        private DataTable GetDataTableFromWorksheet(string worksheetName, bool isFirstRowHeader, string newDataTableName)
        {
            OleDbConnection connection = GetConnection(isFirstRowHeader);
            OleDbDataAdapter adapter = new OleDbDataAdapter(string.Format(
                "SELECT * FROM [{0}]", worksheetName), connection);

            // Create table
            var table = new DataTable();
            adapter.FillSchema(table, SchemaType.Source);

            // Specify data types for numeric columns
            foreach (var columnName in UploadColumnNames.GetNumericColumnNames())
            {
                SetColumnNumeric(table, columnName);
            }

            adapter.Fill(table);

            // Tidy up
            adapter.Dispose();
            connection.Dispose();
            connection.Close();

            return table;
        }

        private void SetColumnNumeric(DataTable table, string columnName)
        {
            if (table.Columns.Contains(columnName))
            {
                var column = table.Columns[columnName];
                column.MaxLength = -1;
                column.DataType = typeof(double);
            }
        }

        public void Dispose()
        {

        }
    }
}
