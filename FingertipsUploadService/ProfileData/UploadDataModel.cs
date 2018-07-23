using FingertipsUploadService.ProfileData.Entities.Core;

namespace FingertipsUploadService.ProfileData
{
    public class UploadDataModel : CoreDataSet
    {
        public int RowNumber { get; set; }

        public bool DuplicateExists { get; set; }

    }
}