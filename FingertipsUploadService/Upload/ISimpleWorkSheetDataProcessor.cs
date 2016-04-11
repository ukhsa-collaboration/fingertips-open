
using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Job;
using System.Collections.Generic;
using System.Data;

namespace FingertipsUploadService.Upload
{
    public interface ISimpleWorksheetDataProcessor
    {
        void Validate(DataTable indicator, DataTable pholio, SimpleUpload simpleUpload);
        SimpleUpload UploadData(DataTable indicatorDetails, DataTable pholioData, UploadJob job);
        void ArchiveDuplicates(IEnumerable<DuplicateRowInDatabaseError> duplicateRows, UploadJob job);
    }
}