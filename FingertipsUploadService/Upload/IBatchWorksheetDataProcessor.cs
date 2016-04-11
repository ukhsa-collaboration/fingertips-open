
using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Job;
using System.Collections.Generic;
using System.Data;

namespace FingertipsUploadService.Upload
{
    public interface IBatchWorksheetDataProcessor
    {
        List<int> GetIndicatorIdsInBatch();
        BatchUpload Validate(DataTable indicators, BatchUpload batchUpload);
        BatchUpload UploadData(DataTable indicators, UploadJob job);
        void ArchiveDuplicates(IEnumerable<DuplicateRowInDatabaseError> duplicateRows, UploadJob job);
    }
}
