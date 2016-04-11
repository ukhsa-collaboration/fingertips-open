
using System.Collections.Generic;

namespace FingertipsUploadService.Upload
{
    public interface IWorksheetNameValidator
    {
        bool ValidateSimple(List<string> worksheets);
        bool ValidateBatch(List<string> worksheets);
    }
}
