using FingertipsUploadService;
using FingertipsUploadService.FpmFileReader;
using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Core;
using FingertipsUploadService.ProfileData.Entities.Job;
using FingertipsUploadService.ProfileData.Repositories;
using FingertipsUploadService.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;

namespace FingertipsUploadServiceTest
{
    [TestClass]
    public class UploadJobWorkerTest
    {
        private const string TempCsvFileName = "temp.csv";
        private Guid _jobGuid;
        private Logger _logger = LogManager.GetLogger("BatchJobWorker");

        public CoreDataRepository CoreDataRepository;
        public LoggingRepository LoggingRepository;
        public UploadJobRepository JobRepository;
        public UploadJobErrorRepository ErrorRepository;

        [TestInitialize]
        public void RunBeforeEachTest()
        {
            CoreDataRepository = new CoreDataRepository();
            LoggingRepository = new LoggingRepository();
            JobRepository = new UploadJobRepository();
            ErrorRepository = new UploadJobErrorRepository();

            _jobGuid = Guid.NewGuid();
            AutoMapperConfig.RegisterMappings();
        }

        [TestCleanup]
        public void RunAfterEachTest()
        {
            JobRepository.DeleteJob(_jobGuid);
            ErrorRepository.DeleteLog(_jobGuid);
            CoreDataRepository.DeleteCoreData(_jobGuid);
            File.Delete(GetTestFilePath(TempCsvFileName));
        }

        [TestMethod]
        public void EndToEnd_Check_Non_Numeric_Field_Is_Caught_For_Csv_File()
        {
            var job = ProcessUploadJob("upload-non-numeric-data.csv");

            Assert.AreEqual(UploadJobStatus.UnexpectedError, job.Status);

            // Assert: Check error message contains invalid string
            var error = ErrorRepository.FindJobErrorsByJobGuid(job.Guid);
            Assert.IsTrue(error.First().ErrorText.Contains("#DIV/0!"));
        }

        [TestMethod]
        public void EndToEnd_Check_Empty_Quarter_Month_Are_Uploaded_As_Minus_One_For_Excel_File()
        {
            ProcessSuccessfulUploadJob("upload-new-data-empty-time-columns.xlsx");
            var data = GetUploadedData();
            Assert.AreEqual(-1, data.Month);
            Assert.AreEqual(-1, data.Quarter);
        }

        [TestMethod]
        public void EndToEnd_Check_Empty_Numeric_Columns_Are_Uploaded_As_Minus_One_For_Excel_File()
        {
            ProcessSuccessfulUploadJob("upload-new-data_empty_value_denominator_count.xlsx");
            AssertAreCIsAreNull();
            var data = GetUploadedData();
            Assert.AreEqual(-1, data.Value);
            Assert.AreEqual(-1, data.Denominator);
            Assert.AreEqual(-1, data.Denominator_2);
            Assert.AreEqual(-1, data.Count);
        }

        [TestMethod]
        public void EndToEnd_Check_Empty_CIs_Are_Uploaded_As_Nulls_For_Excel_File()
        {
            ProcessSuccessfulUploadJob("upload-new-data_empty_cis.xlsx");
            AssertAreCIsAreNull();
        }

        [TestMethod]
        public void EndToEnd_Check_Empty_CIs_Are_Uploaded_As_Nulls_For_Csv_File()
        {
            var header = string.Join(",", UploadColumnNames.GetColumnNames());
            string row = GetDataRow(new CoreDataSet());

            WriteTempFile(header, row);
            ProcessSuccessfulUploadJob(TempCsvFileName);

            // Assert: check data uploaded
            AssertAreCIsAreNull();
        }

        [TestMethod]
        public void EndToEnd_Check_Minus_One_CIs_Are_Uploaded_As_Nulls_For_Excel_File()
        {
            ProcessSuccessfulUploadJob("upload-new-data_minus_one_cis.xlsx");
            AssertAreCIsAreNull();
        }

        [TestMethod]
        public void EndToEnd_Csv_With_Old_CI_Column_Names()
        {
            var job = ProcessUploadJob("upload-new-data-old-ci-names.csv");
            Assert.IsTrue(job.Status == UploadJobStatus.SuccessfulUpload ||
                job.Status == UploadJobStatus.ConfirmationAwaited);
        }

        [TestMethod]
        public void EndToEnd_Check_Minus_One_CIs_Are_Uploaded_As_Nulls_For_Csv_File()
        {
            var header = string.Join(",", UploadColumnNames.GetColumnNames());
            string row = GetDataRow(new CoreDataSet
            {
                LowerCI95 = -1,
                UpperCI95 = -1,
                LowerCI99_8 = -1,
                UpperCI99_8 = -1
            });

            WriteTempFile(header, row);
            ProcessSuccessfulUploadJob(TempCsvFileName);

            // Assert: check data uploaded
            AssertAreCIsAreNull();
        }

        [TestMethod]
        public void EndToEnd_Check_Valid_CIs_Are_Uploaded_For_Excel_File()
        {
            ProcessSuccessfulUploadJob("upload-new-data_valid_cis.xlsx");
            AssertValidCIs();
        }

        [TestMethod]
        public void EndToEnd_Check_Valid_CIs_Are_Uploaded_For_Csv_File()
        {
            var header = string.Join(",", UploadColumnNames.GetColumnNames());
            string row = GetDataRow(new CoreDataSet
            {
                LowerCI95 = 4,
                UpperCI95 = 5,
                LowerCI99_8 = 2,
                UpperCI99_8 = 3
            });

            WriteTempFile(header, row);
            ProcessSuccessfulUploadJob(TempCsvFileName);

            // Assert: check data uploaded
            AssertValidCIs();
        }

        [TestMethod]
        public void EndToEnd_With_New_Data_For_Excel_File()
        {
            ProcessSuccessfulUploadJob("upload-new-data.xlsx");
        }

        [TestMethod]
        public void EndToEnd_With_New_Data_For_Csv_File()
        {
            ProcessSuccessfulUploadJob("upload-new-data.csv");
        }

        [TestMethod]
        public void EndToEnd_With_Columns_In_Different_Order_For_Csv_File()
        {
            ProcessSuccessfulUploadJob("upload-new-data-columns-different-order.csv");
        }

        [TestMethod]
        public void EndToEnd_With_Columns_With_Different_Cases_For_Csv_File()
        {
            ProcessSuccessfulUploadJob("upload-new-data-columns-different-case.csv");
        }

        [TestMethod]
        public void EndToEnd_With_Columns_With_Missing_Columns_For_Csv_File()
        {
            ProcessSuccessfulUploadJob("upload-new-data-missing-columns.csv");
        }

        [TestMethod]
        public void EndToEnd_With_New_Data_Null_Value_For_Excel_File()
        {
            ProcessSuccessfulUploadJob("upload-new-data-null-value.xlsx");
        }

        [TestMethod]
        public void EndToEnd_With_Duplicate_Rows_In_File_For_Excel_File()
        {
            ProcessSuccessfulUploadJob("upload-duplicate-rows.xlsx");
        }

        [TestMethod]
        public void EndToEnd_With_Duplicate_Rows_In_File_For_Csv_File()
        {
            ProcessSuccessfulUploadJob("upload-duplicate-rows.csv");
        }

        [TestMethod]
        public void EndToEnd_With_Duplicate_Row_In_Pholio_For_Excel_File()
        {
            ProcessUploadJobWithDuplicateRowInPholio("upload-new-data.xlsx");
        }

        [TestMethod]
        public void EndToEnd_With_Duplicate_Row_In_Pholio_For_Csv_File()
        {
            ProcessUploadJobWithDuplicateRowInPholio("upload-new-data.csv");
        }

        [TestMethod]
        public void EndToEnd_With_Duplicate_Row_In_File_With_Complex_Grouping_For_Excel_File()
        {
            ProcessSuccessfulUploadJob(
                "upload-duplicate-rows_complex_grouping.xlsx");
        }

        [TestMethod]
        public void EndToEnd_With_Duplicate_Row_In_File_With_Complex_Grouping_For_Csv_File()
        {
            ProcessSuccessfulUploadJob(
                "upload-duplicate-rows_complex_grouping.csv");
        }

        [TestMethod]
        public void EndToEnd_With_Worksheet_Names_Wrong()
        {
            var job = GetUploadJob(_jobGuid);
            job = JobRepository.SaveJob(job);

            var worker = new UploadJobWorker();
            var validator = new WorksheetNameValidator();
            var uploader = new DataUploader(CoreDataRepository, _logger);
            var processor = new DataValidator(CoreDataRepository, _logger);

            var wrongWorksheets = new List<string> { "Fus", "FPM" };
            var fileReader = new Moq.Mock<IUploadFileReader>();
            fileReader.Setup(f => f.GetWorksheets()).Returns(wrongWorksheets);

            worker.ProcessJob(job, validator, processor, fileReader.Object, uploader);
            Assert.AreEqual(UploadJobStatus.FailedValidation, job.Status);
        }

        [TestMethod]
        public void EndToEnd_With_Worksheet_Names_Correct()
        {
            var job = GetUploadJob(_jobGuid);
            job = JobRepository.SaveJob(job);

            var worker = new UploadJobWorker();
            var uploader = new DataUploader(CoreDataRepository, _logger);
            var validator = new WorksheetNameValidator();
            var processor = new DataValidator(CoreDataRepository, _logger);

            var correctWorksheet = new List<string> { WorksheetNames.Pholio };

            var batchDataTable = UploadDataSchema.CreateEmptyTable();
            batchDataTable.Rows.Add(IndicatorIds.GeneralHealthExcellent, 2030, 1, -1, -1,
                AgeIds.Aged15, SexIds.Persons, AreaCodes.England,
                -1, 35.3, 34.9, 35.7, 1, 1, 56704, -1, 0, -1, -1);

            var fileReader = new Moq.Mock<IUploadFileReader>();
            fileReader.Setup(f => f.GetWorksheets()).Returns(correctWorksheet);
            fileReader.Setup(f => f.ReadData()).Returns(batchDataTable);

            worker.ProcessJob(job, validator, processor, fileReader.Object, uploader);

            Assert.AreEqual(UploadJobStatus.SuccessfulUpload, job.Status);
        }

        private void WriteTempFile(string header, string row)
        {
            var path = GetTestFilePath(TempCsvFileName);
            File.WriteAllLines(path, new List<string>
            {
                header,
                row
            });
        }

        private string GetTestFilePath(string filename)
        {
            var startupPath = AppDomain.CurrentDomain.BaseDirectory;
            var pathItems = startupPath.Split(Path.DirectorySeparatorChar);
            var projectPath = string.Join(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture),
                pathItems.Take(pathItems.Length - 2));
            var baseFolder = ConfigurationManager.AppSettings["TestFiles"];
            return Path.Combine(projectPath, baseFolder, filename);
        }

        private void ProcessUploadJobWithDuplicateRowInPholio(string filename)
        {
            // First job
            var job = GetUploadJob(_jobGuid);
            job.Filename = GetTestFilePath(filename);
            job = JobRepository.SaveJob(job);

            // Process new job without any duplication
            var worker = new UploadJobWorker();
            var validator = new WorksheetNameValidator();
            var uploader = new DataUploader(CoreDataRepository, _logger);
            var processor = new DataValidator(CoreDataRepository, _logger);
            var fileReader = new FileReaderFactory().Get(job.Filename);
            worker.ProcessJob(job, validator, processor, fileReader, uploader);
            Assert.AreEqual(UploadJobStatus.SuccessfulUpload, job.Status);

            // Duplicate job
            var duplicateJob = GetUploadJob(_jobGuid);
            duplicateJob.Guid = Guid.NewGuid();
            duplicateJob.Filename = GetTestFilePath(filename);
            duplicateJob = JobRepository.SaveJob(duplicateJob);

            // Process job with duplication
            fileReader = new FileReaderFactory().Get(job.Filename);
            worker.ProcessJob(duplicateJob, validator, processor, fileReader, uploader);
            Assert.AreEqual(UploadJobStatus.ConfirmationAwaited, duplicateJob.Status);

            // Give confirmation to override 
            duplicateJob.Status = UploadJobStatus.ConfirmationGiven;
            JobRepository.UpdateJob(duplicateJob);

            fileReader = new FileReaderFactory().Get(job.Filename);
            worker.ProcessJob(duplicateJob, validator, processor, fileReader, uploader);
            Assert.AreEqual(UploadJobStatus.SuccessfulUpload, duplicateJob.Status);

            // Cleanup
            CoreDataRepository.DeleteCoreData(duplicateJob.Guid);
        }

        private void ProcessSuccessfulUploadJob(string filename)
        {
            var job = ProcessUploadJob(filename);

            // Assert: successful upload
            Assert.AreEqual(UploadJobStatus.SuccessfulUpload, job.Status);
        }

        private UploadJob ProcessUploadJob(string filename)
        {
            // Create job
            var job = GetUploadJob(_jobGuid);
            job.Filename = GetTestFilePath(filename);
            job = JobRepository.SaveJob(job);

            // Process job
            var worker = new UploadJobWorker();
            var validator = new WorksheetNameValidator();
            var uploader = new DataUploader(CoreDataRepository, _logger);
            var processor = new DataValidator(CoreDataRepository, _logger);
            using (var fileReader = new FileReaderFactory().Get(job.Filename))
            {
                worker.ProcessJob(job, validator, processor, fileReader, uploader);
            }
            return job;
        }

        private void AssertAreCIsAreNull()
        {
            var data = GetUploadedData();
            Assert.IsNull(data.LowerCI95);
            Assert.IsNull(data.UpperCI95);
            Assert.IsNull(data.LowerCI99_8);
            Assert.IsNull(data.UpperCI99_8);
        }

        private void AssertValidCIs()
        {
            var data = GetUploadedData();
            Assert.AreEqual(4, data.LowerCI95);
            Assert.AreEqual(5, data.UpperCI95);
            Assert.AreEqual(2, data.LowerCI99_8);
            Assert.AreEqual(3, data.UpperCI99_8);
        }

        protected UploadJob GetUploadJob(Guid jobGuid)
        {
            var job = new UploadJob
            {
                Guid = jobGuid,
                Status = UploadJobStatus.NotStarted,
                DateCreated = DateTime.Now,
                Filename = @"fake.xls",
                UserId = UserIds.Doris,
                Username = UserNames.Doris
            };
            return job;
        }

        private string GetDataRow(CoreDataSet data)
        {
            return string.Join(",", new List<object> {
                IndicatorIds.GeneralHealthExcellent, 2030, 1, -1, -1,
                AgeIds.Aged15,
                SexIds.Persons,
                AreaCodes.England,
                -1,//Count
                35.3,
                data.LowerCI95.HasValue ? data.LowerCI95.ToString() : "",
                data.UpperCI95.HasValue ? data.UpperCI95.ToString() : "",
                data.LowerCI99_8.HasValue ? data.LowerCI99_8.ToString() : "",
                data.UpperCI99_8.HasValue ? data.UpperCI99_8.ToString() : "",
                56704, // Denominator
                -1, // Denominator 2
                0, // Value note
                -1,
                -1
            }.Select(x => x.ToString()));
        }

        private CoreDataSet GetUploadedData()
        {
            var data = CoreDataRepository.GetCoreDataSetByUploadJobId(_jobGuid).First();
            return data;
        }
    }
}
