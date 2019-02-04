using FingertipsUploadService.ProfileData.Entities.Job;
using FingertipsUploadService.ProfileData.Repositories;
using NLog;
using System;
using System.IO;
using FingertipsUploadService.Helpers;

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
                CopyFileToUploadFolder(files[0]);

        }

        private void CopyFileToUploadFolder(string filePath)
        {
            var guid = Guid.NewGuid();

            var fileNameHelper = new FileNameHelper(filePath, guid);

            try
            {
                // Copy the file to archive directory
                File.Copy(filePath, AppConfig.GetAutoUploadArchiveFolder() + "\\" + fileNameHelper.GetFileNameForArchiveFolder());

                // Move the file to upload directory
                File.Move(filePath, AppConfig.GetUploadFolder() + "\\" + fileNameHelper.GetFileNameForUploadFolder());

                var newUploadJob = new UploadJob
                {
                    DateCreated = DateTime.Now,
                    Guid = guid,
                    Filename = fileNameHelper.GetFileName(),
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
