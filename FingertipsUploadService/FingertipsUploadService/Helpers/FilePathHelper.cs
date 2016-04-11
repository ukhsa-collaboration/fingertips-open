using FingertipsUploadService.ProfileData.Entities.Job;
using System.IO;

namespace FingertipsUploadService.Helpers
{
    public class FilePathHelper
    {
        public static string NewExcelFilePath(string currentFilePath)
        {
            var directory = Path.GetDirectoryName(currentFilePath);
            var csvfileNameWithoutExt = Path.GetFileNameWithoutExtension(currentFilePath);
            var newFilePath = Path.Combine(directory, csvfileNameWithoutExt) + ".xls";
            return newFilePath;
        }


        public static string GetActualFilePath(UploadJob job)
        {
            var ext = Path.GetExtension(job.Filename);
            var fileOnDisk = Path.Combine(AppConfig.GetUploadFolder(), job.Guid + ext);
            return fileOnDisk;
        }

    }
}