using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Job;
using FingertipsUploadService.ProfileData.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace FingertipsUploadService.Upload
{
    public class CoreDataSetDuplicateChecker
    {
        public List<DuplicateRowInDatabaseError> GetDuplicates(List<UploadDataModel> data, CoreDataRepository repository, UploadJobType jobType)
        {
            var lastIndicatorId = -1;
            var rowIndex = 1;
            var duplicateRowInDatabaseErrors = new List<DuplicateRowInDatabaseError>();

            foreach (UploadDataModel row in data)
            {
                rowIndex++;

                if (row.IndicatorId != lastIndicatorId)
                {
                    lastIndicatorId = row.IndicatorId;
                }

                var duplicates = repository.GetDuplicateCoreDataSetForAnIndicator(row);

                if (duplicates.Count > 0)
                {
                    var duplicateDataError = new DuplicateRowInDatabaseError
                    {
                        ErrorMessage = "Data already exists for Indicator Id: " + row.IndicatorId,
                        RowNumber = rowIndex,
                        DbValue = duplicates.First().Value,
                        ExcelValue = row.Value,
                        IndicatorId = row.IndicatorId,
                        AgeId = row.AgeId,
                        SexId = row.SexId,
                        AreaCode = row.AreaCode,
                        Uid = duplicates.First().Uid
                    };

                    duplicateRowInDatabaseErrors.Add(duplicateDataError);
                    row.DuplicateExists = true;
                }
            }
            return duplicateRowInDatabaseErrors;
        }
    }
}
