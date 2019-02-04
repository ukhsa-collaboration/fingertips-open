using System;
using System.IO;

namespace FingertipsUploadService.Helpers
{
    public class FileNameHelper
    {
        private string _filePath;
        private Guid _guid;

        public FileNameHelper(string filePath, Guid guid)
        {
            _filePath = filePath;
            _guid = guid;
        }

        public string GetFileNameForArchiveFolder()
        {
            var fileName = Path.GetFileNameWithoutExtension(_filePath);
            return string.Format("{0}-{1}{2}", fileName, _guid, GetExtension()); ;
        }

        public string GetFileName()
        {
            return Path.GetFileName(_filePath);
        }

        public string GetFileNameForUploadFolder()
        {
            return string.Format("{0}{1}", _guid, GetExtension());
        }

        /// <summary>
        /// e.g. ".csv"
        /// </summary>
        private string GetExtension()
        {
            return Path.GetExtension(_filePath);
        }
    }
}