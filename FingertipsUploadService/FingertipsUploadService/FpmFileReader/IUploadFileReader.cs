using System.Collections.Generic;
using System.Data;

namespace FingertipsUploadService.FpmFileReader
{
    public interface IUploadFileReader
    {
        List<string> GetWorksheets();
        DataTable GetIndicatorDetails();
        DataTable GetPholioData();
        DataTable GetBatchData();
    }
}
