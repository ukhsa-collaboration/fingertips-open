using FingertipsUploadService.ProfileData.Entities.Job;
using FingertipsUploadService.ProfileData.Repositories;
using NLog;
using System;
using System.IO;

namespace FingertipsUploadService
{
    public class AutomatedUpload
    {
        private IFusUploadRepository _uploadRepository;
        private Logger _logger;


        public AutomatedUpload(IFusUploadRepository fusUploadRepository, Logger logger)
        {
            _uploadRepository = fusUploadRepository;
            _logger = logger;
        }

        public void Process()
        {
            var files = Directory.GetFiles(AppConfig.GetAutoUploadFolder());
            if (files.Length > 0)
                _copyFileToUploadFolder(files[0]);

        }

        private void _copyFileToUploadFolder(string filePath)
        {
            var guid = Guid.NewGuid();
            var actualFilename = Path.GetFileName(filePath);
            var uniqueFilename = string.Format("{0}.csv", guid);
            try
            {
                // Copy the file to archive directory
                File.Copy(filePath, AppConfig.GetAutoUploadArchiveFolder() + "\\" + actualFilename);
                // Move the file to upload directory
                File.Move(filePath, AppConfig.GetUploadFolder() + "\\" + uniqueFilename);
                var newUploadJob = new UploadJob
                {
                    DateCreated = DateTime.Now,
                    Guid = guid,
                    Filename = actualFilename,
                    Status = UploadJobStatus.NotStarted,
                    UserId = AppConfig.GetAutoUploadUserId(),
                };

                _uploadRepository.CreateNewJob(newUploadJob);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to copy the data file");
            }

        }
    }
}
