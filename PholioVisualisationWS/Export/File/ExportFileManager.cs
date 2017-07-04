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
            if (ApplicationConfiguration.UseFileCache)
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

        public byte[] SaveFile(byte[] content)
        {
            // Check whether file is already cached
            if (ApplicationConfiguration.UseFileCache)
            {
                var filePath = GetFilePath();
                System.IO.File.WriteAllBytes(filePath, content);
            }

            return null;
        }

        private string GetFilePath()
        {
            var filePath = Path.Combine(ApplicationConfiguration.ExportFileDirectory, _fileName);
            return filePath;
        }
    }
}