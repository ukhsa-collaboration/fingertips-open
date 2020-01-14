using System.IO;
using PholioVisualisation.DataAccess;

namespace PholioVisualisation.Export.File
{
    public class ExportFileManager
    {
        private readonly string _fileName;

        public ExportFileManager(string fileName)
        {
            _fileName = fileName;
        }

        public byte[] TryGetFile()
        {
            // Check whether file is already cached
            if (ApplicationConfiguration.Instance.UseFileCache)
            {
                var filePath = GetFilePath();
                if (System.IO.File.Exists(filePath))
                {
                    var bytes = System.IO.File.ReadAllBytes(filePath);
                    return bytes;
                }
            }

            return null;
        }

        public void SaveFile(byte[] content)
        {
            // Check whether file is already cached
            if (ApplicationConfiguration.Instance.UseFileCache)
            {
                var filePath = GetFilePath();

                // Only write if file does not exist
                if (System.IO.File.Exists(filePath) == false)
                {
                    System.IO.File.WriteAllBytes(filePath, content);
                }
            }
        }

        private string GetFilePath()
        {
            var filePath = Path.Combine(ApplicationConfiguration.Instance.ExportFileDirectory, _fileName);
            return filePath;
        }
    }
}