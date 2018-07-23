
using System.Collections.Generic;

namespace FingertipsUploadService.Upload
{
    public interface IWorksheetNameValidator
    {
        bool Validate(List<string> worksheets);
    }
}
