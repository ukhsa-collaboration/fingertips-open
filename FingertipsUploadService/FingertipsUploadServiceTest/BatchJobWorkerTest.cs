using FingertipsUploadService;
using FingertipsUploadService.Helpers;
using FingertipsUploadService.ProfileData.Entities.Job;
using FingertipsUploadService.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;

namespace FingertipsUploadServiceTest
{
    [TestClass]
    public class BatchJobWorkerTest : WorkerTestBase
    {
        private Guid _jobGuid;

        [TestInitialize]
        public void RunBeforeEachTest()
        {
            _jobGuid = Guid.NewGuid();
            AutoMapperConfig.RegisterMappings();
            JobRepository.DeleteAllJob();
        }

        [TestCleanup]
        public void RunAfterEachTest()
        {
            JobRepository.DeleteJob(_jobGuid);
            ErrorRepository.DeleteLog(_jobGuid);
        }

        [TestMethod]
        public void EndToEnd_1_ProcessBatchJobWithNewData()
        {
            var job = GetJob(UploadJobType.Batch, _jobGuid);
            job.Filename = GetTestFilePath("batch-indicator-upload-new-data.xlsx");
            job = JobRepository.SaveJob(job);

            var worker = GetWorker();
            var validator = new WorksheetNameValidator();
            var processor = new BatchWorksheetDataProcessor(CoreDataRepository, LoggingRepository);
            var fileReader = new ExcelFileReader(job.Filename);
            worker.ProcessJob(job, validator, processor, fileReader);

            Assert.AreEqual(UploadJobStatus.SuccessfulUpload, job.Status);

            // Cleanup
            CoreDataRepository.DeleteCoreData(_jobGuid);
        }


        [TestMethod]
        public void EndToEnd_2_ProcessBatchJobWithDuplicateRowInFile()
        {
            var job = GetJob(UploadJobType.Batch, _jobGuid);
            job.Filename = GetTestFilePath("batch-indicator-upload-duplicate-rows.xlsx");
            job = JobRepository.SaveJob(job);

            var worker = GetWorker();
            var validator = new WorksheetNameValidator();
            var processor = new BatchWorksheetDataProcessor(CoreDataRepository, LoggingRepository);
            var fileReader = new ExcelFileReader(job.Filename);
            worker.ProcessJob(job, validator, processor, fileReader);

            Assert.AreEqual(UploadJobStatus.SuccessfulUpload, job.Status);
            // Cleanup
            CoreDataRepository.DeleteCoreData(_jobGuid);
        }

        [TestMethod]
        public void EndToEnd_3_ProcessBatchJobWithDuplicateRowInPholio()
        {
            // First job
            var job = GetJob(UploadJobType.Batch, _jobGuid);
            job.Filename = GetTestFilePath("batch-indicator-upload-new-data.xlsx");
            job = JobRepository.SaveJob(job);

            // Process new job without any duplication
            var worker = GetWorker();
            var validator = new WorksheetNameValidator();
            var processor = new BatchWorksheetDataProcessor(CoreDataRepository, LoggingRepository);
            var excelFileReader = new ExcelFileReader(job.Filename);
            worker.ProcessJob(job, validator, processor, excelFileReader);

            // Duplicate job
            var duplicateJob = GetJob(UploadJobType.Batch, _jobGuid);
            duplicateJob.Guid = Guid.NewGuid();
            duplicateJob.Filename = GetTestFilePath("batch-indicator-upload-new-data.xlsx");
            duplicateJob = JobRepository.SaveJob(duplicateJob);

            // Process job with duplication
            excelFileReader = new ExcelFileReader(duplicateJob.Filename);
            worker.ProcessJob(duplicateJob, validator, processor, excelFileReader);

            Assert.AreEqual(UploadJobStatus.ConfirmationAwaited, duplicateJob.Status);

            // Give confirmation to override 
            duplicateJob.Status = UploadJobStatus.ConfirmationGiven;
            JobRepository.UpdateJob(duplicateJob);

            excelFileReader = new ExcelFileReader(duplicateJob.Filename);
            worker.ProcessJob(duplicateJob, validator, processor, excelFileReader);
            Assert.AreEqual(UploadJobStatus.SuccessfulUpload, duplicateJob.Status);

            // Cleanup
            CoreDataRepository.DeleteCoreData(job.Guid);
            CoreDataRepository.DeleteCoreData(duplicateJob.Guid);

            JobRepository.DeleteJob(duplicateJob.Guid);
            ErrorRepository.DeleteLog(duplicateJob.Guid);
        }


        [TestMethod]
        public void EndToEnd_4_ProcessBatchJobWithDuplicateRowInFileWithComplexGrouping()
        {
            var job = GetJob(UploadJobType.Batch, _jobGuid);
            job.Filename = GetTestFilePath("batch-indicator-upload-duplicate-rows_complex_grouping.xlsx");
            job = JobRepository.SaveJob(job);

            var worker = GetWorker();
            var validator = new WorksheetNameValidator();
            var processor = new BatchWorksheetDataProcessor(CoreDataRepository, LoggingRepository);
            var fileReader = new ExcelFileReader(job.Filename);
            worker.ProcessJob(job, validator, processor, fileReader);

            Assert.AreEqual(UploadJobStatus.SuccessfulUpload, job.Status);
            // Cleanup
            CoreDataRepository.DeleteCoreData(_jobGuid);
        }


        [TestMethod]
        public void ProcessBatchJobWithWrongWorksheet()
        {
            var job = GetJob(UploadJobType.Batch, _jobGuid);
            job = JobRepository.SaveJob(job);

            var worker = GetWorker();
            var validator = new WorksheetNameValidator();
            var processor = new BatchWorksheetDataProcessor(CoreDataRepository, LoggingRepository);

            var wrongWorksheets = new List<string> { "Fus", "FPM" };
            var fileReader = new Moq.Mock<IExcelFileReader>();
            fileReader.Setup(f => f.GetWorksheets()).Returns(wrongWorksheets);

            worker.ProcessJob(job, validator, processor, fileReader.Object);
            Assert.AreEqual(UploadJobStatus.FailedValidation, job.Status);

            // Cleanup
            CoreDataRepository.DeleteCoreData(job.Guid);
        }

        [TestMethod]
        public void ProcessBatchJobWithCorrectWorksheet()
        {
            var job = GetJob(UploadJobType.Batch, _jobGuid);
            job = JobRepository.SaveJob(job);

            var worker = GetWorker();
            var validator = new WorksheetNameValidator();
            var processor = new BatchWorksheetDataProcessor(CoreDataRepository, LoggingRepository);

            var correctWorksheet = new List<string> { WorksheetNames.BatchPholio };

            var batchDataTable = GetEmptyDataTable();
            batchDataTable.Rows.Add(91491, 2030, 1, -1, -1, 44, 1, "E92000001", -1, 35.3, 34.9, 35.7, 56704, -1, 0, -1, -1);

            var fileReader = new Moq.Mock<IExcelFileReader>();
            fileReader.Setup(f => f.GetWorksheets()).Returns(correctWorksheet);
            fileReader.Setup(f => f.GetBatchData()).Returns(batchDataTable);

            worker.ProcessJob(job, validator, processor, fileReader.Object);
            Assert.AreEqual(UploadJobStatus.SuccessfulUpload, job.Status);

            // Cleanup
            CoreDataRepository.DeleteCoreData(job.Guid);
        }


        [TestMethod]
        public void ProcessBatchJobWithDuplicateRows()
        {
            var job = GetJob(UploadJobType.Batch, _jobGuid);
            job = JobRepository.SaveJob(job);

            var worker = GetWorker();
            var validator = new WorksheetNameValidator();
            var processor = new BatchWorksheetDataProcessor(CoreDataRepository, LoggingRepository);

            var correctWorksheet = new List<string> { WorksheetNames.BatchPholio };
            // Add new Data
            var batchDataTable = GetEmptyDataTable();
            batchDataTable.Rows.Add(91491, 2030, 1, -1, -1, 44, 1, "E92000001", -1, 35.3, 34.9, 35.7, 56704, -1, 0, -1, -1);

            var fileReader = new Moq.Mock<IExcelFileReader>();
            fileReader.Setup(f => f.GetWorksheets()).Returns(correctWorksheet);
            fileReader.Setup(f => f.GetBatchData()).Returns(batchDataTable);

            worker.ProcessJob(job, validator, processor, fileReader.Object);

            // Try adding duplicate data
            worker.ProcessJob(job, validator, processor, fileReader.Object);

            Assert.AreEqual(UploadJobStatus.ConfirmationAwaited, job.Status);

            // Cleanup
            CoreDataRepository.DeleteCoreData(job.Guid);
        }


        public DataTable GetEmptyDataTable()
        {
            var table = new DataTable();
            table.Columns.Add("IndicatorID", typeof(double));
            table.Columns.Add("Year", typeof(double));
            table.Columns.Add("YearRange", typeof(double));
            table.Columns.Add("Quarter", typeof(double));
            table.Columns.Add("Month", typeof(double));
            table.Columns.Add("AgeID", typeof(double));
            table.Columns.Add("SexID", typeof(double));
            table.Columns.Add("AreaCode", typeof(string));
            table.Columns.Add("Count", typeof(double));
            table.Columns.Add("Value", typeof(double));
            table.Columns.Add("LowerCI", typeof(double));
            table.Columns.Add("UpperCI", typeof(double));
            table.Columns.Add("Denominator", typeof(double));
            table.Columns.Add("Denominator_2", typeof(double));
            table.Columns.Add("ValueNoteId", typeof(double));
            table.Columns.Add("CategoryTypeId", typeof(double));
            table.Columns.Add("CategoryId", typeof(double));

            return table;
        }

        private BatchJobWorker GetWorker()
        {
            var work = new BatchJobWorker();
            return work;
        }
    }
}
