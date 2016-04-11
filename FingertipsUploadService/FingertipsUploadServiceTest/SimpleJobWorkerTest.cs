using FingertipsUploadService;
using FingertipsUploadService.Helpers;
using FingertipsUploadService.ProfileData.Entities.Job;
using FingertipsUploadService.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;


namespace FingertipsUploadServiceTest
{
    [TestClass]
    public class SimpleJobWorkerTest : WorkerTestBase
    {
        private Guid _jobGuid;

        [TestInitialize]
        public void RunBeforeEachTest()
        {
            _jobGuid = Guid.NewGuid();
            AutoMapperConfig.RegisterMappings();
        }

        [TestCleanup]
        public void RunAfterEachTest()
        {
            // JobRepository.DeleteJob(_jobGuid);
            ErrorRepository.DeleteLog(_jobGuid);
        }

        [TestMethod]
        public void EndToEnd_1_ProcessJobWithNewData()
        {
            var job = GetJob(UploadJobType.Simple, _jobGuid);
            job.Filename = GetTestFilePath("upload-simple-new-data.xlsx");
            job = JobRepository.SaveJob(job);

            var worker = GetWorker();
            var validator = new WorksheetNameValidator();
            var processor = new SimpleWorksheetDataProcessor(CoreDataRepository, LoggingRepository);
            var excelFileReader = new ExcelFileReader(job.Filename);
            worker.ProcessJob(job, validator, processor, excelFileReader);

            Assert.AreEqual(UploadJobStatus.SuccessfulUpload, job.Status);
            // Clean up
            CoreDataRepository.DeleteCoreData(_jobGuid);
        }

        [TestMethod]
        public void EndToEnd_2_ProcessJobWithDuplicateRowInFile()
        {
            var job = GetJob(UploadJobType.Simple, _jobGuid);
            job.Filename = GetTestFilePath("upload-simple-duplicate-row-in-file.xlsx");
            job = JobRepository.SaveJob(job);

            var worker = GetWorker();
            var validator = new WorksheetNameValidator();
            var processor = new SimpleWorksheetDataProcessor(CoreDataRepository, LoggingRepository);
            var excelFileReader = new ExcelFileReader(job.Filename);
            worker.ProcessJob(job, validator, processor, excelFileReader);

            Assert.AreEqual(UploadJobStatus.ConfirmationAwaited, job.Status);
            // Clean up
            CoreDataRepository.DeleteCoreData(_jobGuid);
        }

        [TestMethod]
        public void EndToEnd_3_ProcessJobWithDuplicateRowInPholio()
        {
            // First job
            var job = GetJob(UploadJobType.Simple, _jobGuid);
            job.Filename = GetTestFilePath("upload-simple-duplicate-row-in-pholio.xlsx");
            job = JobRepository.SaveJob(job);

            var duplicateJob = GetJob(UploadJobType.Simple, _jobGuid);
            duplicateJob.Guid = Guid.NewGuid();
            duplicateJob.Filename = GetTestFilePath("upload-simple-duplicate-row-in-pholio.xlsx");
            duplicateJob = JobRepository.SaveJob(duplicateJob);

            // process new job without any duplication
            var worker = GetWorker();
            var validator = new WorksheetNameValidator();
            var processor = new SimpleWorksheetDataProcessor(CoreDataRepository, LoggingRepository);
            var excelFileReader = new ExcelFileReader(job.Filename);
            worker.ProcessJob(job, validator, processor, excelFileReader);

            // Process job with duplication
            excelFileReader = new ExcelFileReader(duplicateJob.Filename);
            worker.ProcessJob(duplicateJob, validator, processor, excelFileReader);

            Assert.AreEqual(UploadJobStatus.ConfirmationAwaited, duplicateJob.Status);

            // Clean up
            CoreDataRepository.DeleteCoreData(job.Guid);
            CoreDataRepository.DeleteCoreData(duplicateJob.Guid);

            JobRepository.DeleteJob(duplicateJob.Guid);
            ErrorRepository.DeleteLog(duplicateJob.Guid);
        }

        [TestMethod]
        public void ProcessJobWithWrongWorksheet()
        {
            var job = GetJob(UploadJobType.Simple, _jobGuid);
            // Save to db
            job = JobRepository.SaveJob(job);

            var worker = GetWorker();
            var validator = new WorksheetNameValidator();
            var processor = new SimpleWorksheetDataProcessor(CoreDataRepository, LoggingRepository);

            var wrongWorksheets = new List<string> { "Fus", "FPM" };
            var fileReader = new Mock<IExcelFileReader>();
            fileReader.Setup(f => f.GetWorksheets()).Returns(wrongWorksheets);

            worker.ProcessJob(job, validator, processor, fileReader.Object);

            Assert.AreEqual(UploadJobStatus.FailedValidation, job.Status);

            // Clean up
            CoreDataRepository.DeleteCoreData(_jobGuid);
        }

        [TestMethod]
        public void ProcessJobWithCorrectWorksheet()
        {
            var job = GetJob(UploadJobType.Simple, _jobGuid);
            job = JobRepository.SaveJob(job);

            var worker = GetWorker();
            var validator = new WorksheetNameValidator();
            var processor = new SimpleWorksheetDataProcessor(CoreDataRepository, LoggingRepository);

            var correctWorksheet = new List<string> { WorksheetNames.SimpleIndicator, WorksheetNames.SimplePholio };

            var indicatorDetailsTable = GetIndicatorDetailsTable();
            var pholioTable = GetPholioTable();

            var fileReader = new Mock<IExcelFileReader>();
            fileReader.Setup(x => x.GetWorksheets()).Returns(correctWorksheet);
            fileReader.Setup(x => x.GetIndicatorDetails()).Returns(indicatorDetailsTable);
            fileReader.Setup(x => x.GetPholioData()).Returns(pholioTable);

            worker.ProcessJob(job, validator, processor, fileReader.Object);

            Assert.AreEqual(UploadJobStatus.SuccessfulUpload, job.Status);

            // Cleanup
            CoreDataRepository.DeleteCoreData(_jobGuid);
        }

        [TestMethod]
        public void ProcessJobWithDuplicateRows()
        {
            var job = GetJob(UploadJobType.Simple, _jobGuid);
            job = JobRepository.SaveJob(job);
            var worker = GetWorker();
            var validator = new WorksheetNameValidator();
            var processor = new SimpleWorksheetDataProcessor(CoreDataRepository, LoggingRepository);
            var correctWorksheet = new List<string> { WorksheetNames.SimpleIndicator, WorksheetNames.SimplePholio };
            var indicatorDetailsTable = GetIndicatorDetailsTable();
            var pholioTable = GetPholioTable();

            var fileReader = new Mock<IExcelFileReader>();
            fileReader.Setup(x => x.GetWorksheets()).Returns(correctWorksheet);
            fileReader.Setup(x => x.GetIndicatorDetails()).Returns(indicatorDetailsTable);
            fileReader.Setup(x => x.GetPholioData()).Returns(pholioTable);
            // Add new data
            worker.ProcessJob(job, validator, processor, fileReader.Object);
            // Add duplicate data
            worker.ProcessJob(job, validator, processor, fileReader.Object);

            Assert.AreEqual(UploadJobStatus.ConfirmationAwaited, job.Status);

            // Cleanup
            CoreDataRepository.DeleteCoreData(_jobGuid);
        }

        private SimpleJobWorker GetWorker()
        {
            var work = new SimpleJobWorker();
            return work;
        }

        private DataTable GetIndicatorDetailsTable()
        {
            var table = new DataTable();
            table.Columns.Add("Key", typeof(string));
            table.Columns.Add("Value", typeof(double));

            table.Rows.Add("", 10101);
            table.Rows.Add("", 2020);
            table.Rows.Add("", 1);
            table.Rows.Add("", 1);
            table.Rows.Add("", 1);
            table.Rows.Add("", 1);
            table.Rows.Add("", 1);

            return table;
        }

        private DataTable GetPholioTable()
        {
            var table = new DataTable();
            table.Columns.Add("AreaCode", typeof(string));
            table.Columns.Add("AreaCodeOld", typeof(string));
            table.Columns.Add("AreaName", typeof(string));
            table.Columns.Add("Count", typeof(double));
            table.Columns.Add("Value", typeof(double));
            table.Columns.Add("LowerCI", typeof(double));
            table.Columns.Add("UpperCI", typeof(double));
            table.Columns.Add("Denominator", typeof(double));
            table.Columns.Add("ValueNoteId", typeof(double));

            table.Rows.Add("E92000001", "", "", 1, 1, 123, 25, 3, 101);

            return table;
        }
    }
}