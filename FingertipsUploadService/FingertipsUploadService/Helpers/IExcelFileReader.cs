

using System.Collections.Generic;
using System.Data;

namespace FingertipsUploadService.Helpers
{
    public interface IExcelFileReader
    {
        List<string> GetWorksheets();
        DataTable GetIndicatorDetails();
        DataTable GetPholioData();
        DataTable GetBatchData();
    }
}
