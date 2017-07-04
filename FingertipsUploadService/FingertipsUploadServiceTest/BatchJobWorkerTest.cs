using FingertipsUploadService;
using FingertipsUploadService.FpmFileReader;
using FingertipsUploadService.ProfileData.Entities.Job;
using FingertipsUploadService.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using System;
using System.Collections.Generic;
using FingertipsUploadService.ProfileData;

namespace FingertipsUploadServiceTest
{
    [TestClass]
    public class BatchJobWorkerTest : WorkerTestBase
    {
        private Guid _jobGuid;
        private Logger _logger = null;

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
        public void EndToEnd_1_ProcessBatchJobWithNewDataExcelFile()
        {
            ProcessBatchJobWithNewData("batch-indicator-upload-new-data.xlsx");
        }

        [TestMethod]
        public void EndToEnd_1_ProcessBatchJobWithNewDataWithCsvFile()
        {
            ProcessBatchJobWithNewData("batch-indicator-upload-new-data.csv");
        }


        [TestMethod]
        public void EndToEnd_1_ProcessBatchJobWithNewDataWithExcelFileWithNull()
        {
            ProcessBatchJobWithNewData("batch-indicator-upload-new-data-null-value.xlsx");
        }


        private void ProcessBatchJobWithNewData(string filename)
        {
            var job = GetJob(UploadJobType.Batch, _jobGuid);
            job.Filename = GetTestFilePath(filename);
            job = JobRepository.SaveJob(job);

            var worker = GetWorker();
            var validator = new WorksheetNameValidator();
            var processor = new BatchWorksheetDataProcessor(CoreDataRepository, LoggingRepository, _logger);
            var fileReader = new FileReaderFactory().Get(job.Filename, job.JobType);
            worker.ProcessJob(job, validator, processor, fileReader);

            Assert.AreEqual(UploadJobStatus.SuccessfulUpload, job.Status);

            // Cleanup
            CoreDataRepository.DeleteCoreData(_jobGuid);
        }

        [TestMethod]
        public void EndToEnd_2_ProcessBatchJobWithDuplicateRowInFile()
        {
            ProcessBatchJobWithDuplicateRowInFile("batch-indicator-upload-duplicate-rows.xlsx");
        }

        [TestMethod]
        public void EndToEnd_2_ProcessBatchJobWithDuplicateRowInFileWithCsvFile()
        {
            ProcessBatchJobWithDuplicateRowInFile("batch-indicator-upload-duplicate-rows.csv");
        }

        public void ProcessBatchJobWithDuplicateRowInFile(string filename)
        {
            var job = GetJob(UploadJobType.Batch, _jobGuid);
            job.Filename = GetTestFilePath(filename);
            job = JobRepository.SaveJob(job);

            var worker = GetWorker();
            var validator = new WorksheetNameValidator();
            var processor = new BatchWorksheetDataProcessor(CoreDataRepository, LoggingRepository, _logger);
            var fileReader = new FileReaderFactory().Get(job.Filename, job.JobType);
            worker.ProcessJob(job, validator, processor, fileReader);

            Assert.AreEqual(UploadJobStatus.SuccessfulUpload, job.Status);
            // Cleanup
            CoreDataRepository.DeleteCoreData(_jobGuid);
        }

        [TestMethod]
        public void EndToEnd_3_ProcessBatchJobWithDuplicateRowInPholio()
        {
            ProcessBatchJobWithDuplicateRowInPholioWithCsvFile("batch-indicator-upload-new-data.xlsx");
        }

        [TestMethod]
        public void EndToEnd_3_ProcessBatchJobWithDuplicateRowInPholioWithCsvFile()
        {
            ProcessBatchJobWithDuplicateRowInPholioWithCsvFile("batch-indicator-upload-new-data.csv");
        }

        private void ProcessBatchJobWithDuplicateRowInPholioWithCsvFile(string filename)
        {
            // First job
            var job = GetJob(UploadJobType.Batch, _jobGuid);
            job.Filename = GetTestFilePath(filename);
            job = JobRepository.SaveJob(job);

            // Process new job without any duplication
            var worker = GetWorker();
            var validator = new WorksheetNameValidator();
            var processor = new BatchWorksheetDataProcessor(CoreDataRepository, LoggingRepository, _logger);
            var fileReader = new FileReaderFactory().Get(job.Filename, job.JobType);
            worker.ProcessJob(job, validator, processor, fileReader);

            // Duplicate job
            var duplicateJob = GetJob(UploadJobType.Batch, _jobGuid);
            duplicateJob.Guid = Guid.NewGuid();
            duplicateJob.Filename = GetTestFilePath(filename);
            duplicateJob = JobRepository.SaveJob(duplicateJob);

            // Process job with duplication
            fileReader = new FileReaderFactory().Get(job.Filename, job.JobType);
            worker.ProcessJob(duplicateJob, validator, processor, fileReader);

            Assert.AreEqual(UploadJobStatus.ConfirmationAwaited, duplicateJob.Status);

            // Give confirmation to override 
            duplicateJob.Status = UploadJobStatus.ConfirmationGiven;
            JobRepository.UpdateJob(duplicateJob);

            fileReader = new FileReaderFactory().Get(job.Filename, job.JobType);
            worker.ProcessJob(duplicateJob, validator, processor, fileReader);
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
            ProcessBatchJobWithDuplicateRowInFileWithComplexGrouping(
                "batch-indicator-upload-duplicate-rows_complex_grouping.xlsx");
        }

        [TestMethod]
        public void EndToEnd_4_ProcessBatchJobWithDuplicateRowInFileWithComplexGroupingWithCsvFile()
        {
            ProcessBatchJobWithDuplicateRowInFileWithComplexGrouping(
                "batch-indicator-upload-duplicate-rows_complex_grouping.csv");
        }

        private void ProcessBatchJobWithDuplicateRowInFileWithComplexGrouping(string filename)
        {
            var job = GetJob(UploadJobType.Batch, _jobGuid);
            job.Filename = GetTestFilePath(filename);
            job = JobRepository.SaveJob(job);

            var worker = GetWorker();
            var validator = new WorksheetNameValidator();
            var processor = new BatchWorksheetDataProcessor(CoreDataRepository, LoggingRepository, _logger);
            var fileReader = new FileReaderFactory().Get(job.Filename, job.JobType);
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
            var processor = new BatchWorksheetDataProcessor(CoreDataRepository, LoggingRepository, _logger);

            var wrongWorksheets = new List<string> { "Fus", "FPM" };
            var fileReader = new Moq.Mock<IUploadFileReader>();
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
            var processor = new BatchWorksheetDataProcessor(CoreDataRepository, LoggingRepository, _logger);

            var correctWorksheet = new List<string> { WorksheetNames.BatchPholio };

            var batchDataTable = new UploadDataSchema().CreateEmptyTable();
            batchDataTable.Rows.Add(91491, 2030, 1, -1, -1, 44, 1, "E92000001", -1, 35.3, 34.9, 35.7, 56704, -1, 0, -1, -1);

            var fileReader = new Moq.Mock<IUploadFileReader>();
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
            var processor = new BatchWorksheetDataProcessor(CoreDataRepository, LoggingRepository, _logger);

            var correctWorksheet = new List<string> { WorksheetNames.BatchPholio };
            // Add new Data
            var batchDataTable = new UploadDataSchema().CreateEmptyTable();
            batchDataTable.Rows.Add(91491, 2030, 1, -1, -1, 44, 1, "E92000001", -1, 35.3, 34.9, 35.7, 56704, -1, 0, -1, -1);

            var fileReader = new Moq.Mock<IUploadFileReader>();
            fileReader.Setup(f => f.GetWorksheets()).Returns(correctWorksheet);
            fileReader.Setup(f => f.GetBatchData()).Returns(batchDataTable);

            worker.ProcessJob(job, validator, processor, fileReader.Object);

            // Try adding duplicate data
            worker.ProcessJob(job, validator, processor, fileReader.Object);

            Assert.AreEqual(UploadJobStatus.ConfirmationAwaited, job.Status);

            // Cleanup
            CoreDataRepository.DeleteCoreData(job.Guid);
        }


        private BatchJobWorker GetWorker()
        {
            var work = new BatchJobWorker();
            return work;
        }
    }
}
