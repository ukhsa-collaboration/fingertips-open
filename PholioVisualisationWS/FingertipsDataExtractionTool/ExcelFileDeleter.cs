using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using PholioVisualisation.DataAccess;

namespace FingertipsDataExtractionTool
{
    public interface IExcelFileDeleter
    {
        void DeleteAllExistingFiles();
    }

    public class ExcelFileDeleter : IExcelFileDeleter
    {
        private ILogger _logger;

        public ExcelFileDeleter(ILogger logger)
        {
            _logger = logger;
        }

        public void DeleteAllExistingFiles()
        {
            _logger.Info("Deleting existing files");

            var exportDirectoryInfo = new DirectoryInfo(ApplicationConfiguration.ExportFileDirectory);
            foreach (var file in exportDirectoryInfo.GetFiles())
            {
                file.Delete();
            }
        }

    }
}
