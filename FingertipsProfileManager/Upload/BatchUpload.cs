using System.Collections.Generic;
using Fpm.ProfileData;

namespace Fpm.Upload
{
    public class BatchUpload : BaseUpload
    {
        public BatchUpload()
        {
            DataToUpload = new List<UploadDataModel>();
            UploadValidationFailures = new List<UploadValidationFailure>();
            DuplicateRowInDatabaseErrors = new List<DuplicateRowInDatabaseError>();
            DuplicateRowInSpreadsheetErrors = new List<DuplicateRowInSpreadsheetError>();
        }

        public override string RequiredWorksheetText()
        {
            return "a \"PHOLIO\" worksheet";
        }
    }
}