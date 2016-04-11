

using System.Collections.Generic;

namespace FingertipsUploadService.Upload
{
    public interface ISimpleUploadWorkSheetsValidator
    {
        bool Validate(List<string> worksheets);
    }
}
