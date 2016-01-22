using Fpm.ProfileData.Entities.Core;

namespace Fpm.ProfileData
{
    public class UploadDataModel : CoreDataSet
    {
        public int RowNumber { get; set; }

        public bool DuplicateExists { get; set; }
    }
}