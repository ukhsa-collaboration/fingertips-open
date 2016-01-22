using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SpreadsheetGear;

namespace PholioVisualisation.Export
{
    public interface IExcelFileWriter
    {
        /// <summary>
        /// Files are only written if this is true.
        /// </summary>
        bool UseFileCache { get; set; }

        byte[] Write(BaseExcelFileInfo fileInfo, IWorkbook workbook);
    }

    public class ExcelFileWriter : IExcelFileWriter
    {
        /// <summary>
        /// Files are only written if this is true.
        /// </summary>
        public bool UseFileCache { get; set; }

        public byte[] Write(BaseExcelFileInfo fileInfo, IWorkbook workbook)
        {
            // Select first sheet
            workbook.Worksheets[0].Select();

            var filePath = fileInfo.FilePath;

            byte[] outData = workbook.SaveToMemory(fileInfo.FileFormat);
            if (UseFileCache && filePath != null)
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        writer.Write(outData);
                        writer.Close();
                    }
                }
            }
            return outData;
        }
    }
}
