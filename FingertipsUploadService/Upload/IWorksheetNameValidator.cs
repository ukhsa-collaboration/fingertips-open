
using System.Collections.Generic;

namespace FingertipsUploadService.Upload
{
    public interface IWorksheetNameValidator
    {
        bool ValidateBatch(List<string> worksheets);
    }
}
