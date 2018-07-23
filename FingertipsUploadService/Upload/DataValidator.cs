using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Repositories;
using NLog;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FingertipsUploadService.Upload
{
    public interface IDataValidator
    {
        void ValidateRows(DataTable table, UploadJobAnalysis analysis, bool hasCellsBeingValidatedOnce);
        void CheckSmallNumbers(DataTable table, UploadJobAnalysis analysis);
        bool CheckDuplicatesInFile(UploadJobAnalysis analysis);
        void ValidateAndParseData(DataTable dataTable, UploadJobAnalysis uploadJobAnalysis);
        void CheckGetDuplicatesInDb(DataTable table, UploadJobAnalysis analysis);
    }

    public class DataValidator : IDataValidator
    {
        private readonly ProfilesReader _profilesReader = ReaderFactory.GetProfilesReader();
        private readonly CoreDataRepository _coreDataRepository;
        private AllowedData allowedData;
        private Logger _logger;

        public DataValidator(CoreDataRepository coreDataRepository, Logger logger)
        {
            _coreDataRepository = coreDataRepository;
            _logger = logger;
        }


        public void CheckSmallNumbers(DataTable table, UploadJobAnalysis analysis)
        {
            var smallNumberChecker = new SmallNumberChecker(_profilesReader, new DisclosureControlRepository(), new AreaTypeRepository());
            for (var i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];
                smallNumberChecker.Check(row, i, analysis);
            }
        }

        public void ValidateRows(DataTable table, UploadJobAnalysis analysis, bool hasCellsBeingValidatedOnce)
        {
            allowedData = new AllowedData(_profilesReader);
            var validationManager = new DataRowValidationManager(allowedData);
            for (var i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];

                var rowNumber = i + 2;
                var validationFailures = new List<UploadValidationFailure>();
                var rowValidator = new DataRowValidator(row, rowNumber);

                if (!hasCellsBeingValidatedOnce)
                {
                    _logger.Debug("Validating current row . # {0} ", rowNumber);
                    validationFailures = validationManager.ValidateRow(rowValidator, i);
                }

                if (validationFailures.Count == 0)
                {
                    var upload = new UploadRowParser(row).ParseRow();
                    upload.RowNumber = i + 2; /*ignore header row and start from 1 not zero*/

                    analysis.DataToUpload.Add(upload);
                }
                else
                {

                    _logger.Debug("Assigning the validation failures");
                    // Add errors
                    foreach (UploadValidationFailure error in validationFailures)
                        analysis.UploadValidationFailures.Add(error);
                }
            }
        }


        public bool CheckDuplicatesInFile(UploadJobAnalysis analysis)
        {
            var areThereAnyDuplicates = false;
            if (analysis.DataToUpload.Any())
            {
                areThereAnyDuplicates = CheckForDuplicateData(analysis);
            }
            return areThereAnyDuplicates;
        }

        public void CheckGetDuplicatesInDb(DataTable table, UploadJobAnalysis analysis)
        {
            var errors = new CoreDataSetDuplicateChecker().GetDuplicates(analysis.DataToUpload, _coreDataRepository);
            analysis.DuplicateRowInDatabaseErrors.Clear();
            analysis.DuplicateRowInDatabaseErrors.AddRange(errors);
        }

        public void ValidateAndParseData(DataTable dataTable, UploadJobAnalysis uploadJobAnalysis)
        {
            allowedData = new AllowedData(_profilesReader);

            CheckSmallNumbers(dataTable, uploadJobAnalysis);


            // Validate spreadsheet data
            ValidateDataRows(dataTable, uploadJobAnalysis);

            if (uploadJobAnalysis.DataToUpload.Any())
            {
                _logger.Debug("Validate the spreadsheet data to see if there's duplication within it");
                //Validate the spreadsheet data to see if there's duplication within it.
                uploadJobAnalysis.DuplicateUploadErrorsExist = CheckForDuplicateData(uploadJobAnalysis);

                var errors = new CoreDataSetDuplicateChecker().GetDuplicates(uploadJobAnalysis.DataToUpload, _coreDataRepository);
                uploadJobAnalysis.DuplicateRowInDatabaseErrors.Clear();
                uploadJobAnalysis.DuplicateRowInDatabaseErrors.AddRange(errors);
            }
        }

        private void ValidateDataRows(DataTable dataTable, UploadJobAnalysis uploadJobAnalysis)
        {
            var smallNumberChecker = new SmallNumberChecker(_profilesReader,
                new DisclosureControlRepository(), new AreaTypeRepository());
            var validationManager = new DataRowValidationManager(allowedData);

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                var row = dataTable.Rows[i];

                var rowNumber = i + 2;
                var rowValidator = new DataRowValidator(row, rowNumber);
                _logger.Debug("Validating current row # {0} ", rowNumber);
                var validationFailures = validationManager.ValidateRow(rowValidator, i);

                if (validationFailures.Count == 0)
                {
                    _logger.Debug("Checking for small numbers for row # {0}", rowNumber);
                    smallNumberChecker.Check(row, i, uploadJobAnalysis);
                    // Parse row
                    var upload = new UploadRowParser(row).ParseRow();
                    upload.RowNumber = i + 2/*ignore header row and start from 1 not zero*/;
                    uploadJobAnalysis.DataToUpload.Add(upload);
                }
                else
                {
                    _logger.Debug("Assigning the validation failures");
                    // Add errors
                    foreach (UploadValidationFailure error in validationFailures)
                        uploadJobAnalysis.UploadValidationFailures.Add(error);
                }
            }
        }

        /// <summary>
        ///     Checks the data for duplicate rows within itself.
        /// </summary>
        private static bool CheckForDuplicateData(UploadJobAnalysis jobAnalysis)
        {
            var duplicatedData = new DuplicateDataFilter()
                .GetDuplicatedData(jobAnalysis.DataToUpload);

            if (duplicatedData.Any())
            {
                foreach (var data in duplicatedData)
                {
                    var duplicate = new DuplicateRowInSpreadsheetError
                    {
                        RowNumber = data.RowNumber,
                        DuplicateRowMessage = "Indicator " + data.IndicatorId + " is duplicated. "
                    };
                    jobAnalysis.DuplicateRowInSpreadsheetErrors.Add(duplicate);
                }
                return true;
            }
            return false;
        }

    }
}