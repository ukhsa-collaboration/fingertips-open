using System;
using System.Collections.Generic;
using System.Data;

namespace FingertipsUploadService.FpmFileReader
{
    public interface IUploadFileReader : IDisposable
    {
        List<string> GetWorksheets();
        DataTable ReadData();
    }
}