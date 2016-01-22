using FingertipsUploadService;
using FingertipsUploadService.Entities.Job;
using Fpm.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;

namespace FingertipsUploadServiceTest
{
    [TestClass]
    public class UploadWorkerTest
    {
        [TestMethod]
        public void ProcessJobTest()
        {
            var job = new UploadJob
            {
                Id = Guid.NewGuid(),
                Status = UploadJobStatus.NotStart,
                DateCreated = DateTime.Now,
                JobType = UploadJobType.Simple
            };

            var worker = new UploadWorker();
            worker.ProcessJob(job);

            Assert.AreEqual(UploadJobStatus.InProgress, job.Status);
        }

        private static DataTable GetSimpleUploadDataTable()
        {
            var table = new DataTable();
            table.Columns.Add(UploadColumnNames.IndicatorId);
            table.Columns.Add(UploadColumnNames.AreaCode);
            table.Columns.Add(UploadColumnNames.AreaName);
            table.Columns.Add(UploadColumnNames.Count);
            table.Columns.Add(UploadColumnNames.Value);
            table.Columns.Add(UploadColumnNames.LowerCI);
            table.Columns.Add(UploadColumnNames.UpperCI);
            table.Columns.Add(UploadColumnNames.Denominator);
            table.Columns.Add(UploadColumnNames.ValueNoteId);
            return table;
        }
    }
}
