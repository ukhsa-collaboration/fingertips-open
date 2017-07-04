using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SpreadsheetGear;

namespace PholioVisualisation.Export.File
{
    public abstract class BaseExcelFileInfo
    {
        /// <summary>
        /// Is virtual to allow mocking for testing
        /// </summary>
        public virtual string FilePath { get; protected set; }

        public bool DoesFileExist
        {
            get { return new FileInfo(FilePath).Exists; }
        }

        public virtual FileFormat FileFormat
        {
            get
            {
                return FileFormat.OpenXMLWorkbook;
            }
        }

        public virtual string FileExtension
        {
            get
            {
                return "xlsx";
            }
        }
    }

}
