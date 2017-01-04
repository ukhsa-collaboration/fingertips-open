
using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Job;
using FingertipsUploadService.ProfileData.Repositories;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;

namespace FingertipsUploadServiceTest
{
    public class WorkerTestBase
    {
        public CoreDataRepository CoreDataRepository;
        public LoggingRepository LoggingRepository;
        public UploadJobRepository JobRepository;
        public UploadJobErrorRepository ErrorRepository;

        public WorkerTestBase()
        {
            CoreDataRepository = new CoreDataRepository();
            LoggingRepository = new LoggingRepository();
            JobRepository = new UploadJobRepository();
            ErrorRepository = new UploadJobErrorRepository();
        }

        protected string GetTestFilePath(string filename)
        {
            var startupPath = AppDomain.CurrentDomain.BaseDirectory;
            var pathItems = startupPath.Split(Path.DirectorySeparatorChar);
            var projectPath = string.Join(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture), pathItems.Take(pathItems.Length - 2));
            var baseFolder = ConfigurationManager.AppSettings["TestFiles"];
            return Path.Combine(projectPath, baseFolder, filename);
        }

        protected UploadJob GetJob(UploadJobType jobType, Guid jobGuid)
        {
            var job = new UploadJob
            {
                Guid = jobGuid,
                Status = UploadJobStatus.NotStarted,
                DateCreated = DateTime.Now,
                JobType = jobType,
                Filename = @"fake.xls",
                UserId = 11,
                Username = @"phe\doris.hain"
            };
            return job;
        }
    }
}
