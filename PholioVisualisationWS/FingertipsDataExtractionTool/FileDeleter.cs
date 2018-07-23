using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using PholioVisualisation.DataAccess;

namespace FingertipsDataExtractionTool
{
    public interface IFileDeleter
    {
        void DeleteAllExistingFiles();
    }

    public class FileDeleter : IFileDeleter
    {
        private ILogger _logger;

        public FileDeleter(ILogger logger)
        {
            _logger = logger;
        }

        public void DeleteAllExistingFiles()
        {
            _logger.Info("Deleting existing files");

            var exportDirectoryInfo = new DirectoryInfo(ApplicationConfiguration.Instance.ExportFileDirectory);
            foreach (var file in exportDirectoryInfo.GetFiles())
            {
                file.Delete();
            }
        }

    }
}
