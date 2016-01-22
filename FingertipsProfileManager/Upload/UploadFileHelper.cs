using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;

namespace Fpm.Upload
{
    public class UploadFileHelper
    {
        public const int MaximumFileSize = 10000000;

        public static bool IsFileTooLarge(int fileSize)
        {
            return fileSize > MaximumFileSize;
        }

        public static string GetFilePath(string filePath, Guid uploadBatchId)
        {
            var uploadDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uploads");

            return Path.Combine(uploadDirectory, uploadBatchId + "_" + Path.GetFileName(filePath));
        }

        public static OleDbConnection OpenSpreadsheet(string excelFile, bool firstRowHeader)
        {
            OleDbConnection connExcel;

            var hdr = firstRowHeader
                ? "Yes"
                : "No";

            connExcel =
                   new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + excelFile +
                       @";Extended Properties=""Excel 8.0;HDR=" + hdr + @";""");

            new OleDbCommand { Connection = connExcel };
            connExcel.Open();
            return connExcel;
        }


        public static DataTable ReadWorksheet(string excelFile, string selectedWorksheet)
        {
            OleDbConnection connExcel = OpenSpreadsheet(excelFile, true);

            var adapter = new OleDbDataAdapter(String.Format("SELECT * FROM [{0}]", selectedWorksheet), connExcel);

            var ds = new DataSet();
            adapter.Fill(ds, "results");
            DataTable data = ds.Tables["results"];

            // Delete non-valid rows
            foreach (var row in data.Select("IndicatorId is null"))
            {
                row.Delete();
            }
            data.AcceptChanges();

            
            adapter.Dispose();
            connExcel.Close();
            return data;
        }

        public static List<UploadExcelSheet> GetAvailableWorksheets(OleDbConnection connExcel)
        {
            DataTable worksheets = connExcel.GetSchema("Tables");
            return (from DataRow row in worksheets.Rows select new UploadExcelSheet { SheetName = row["TABLE_NAME"].ToString() }).ToList();
        }
    }
}