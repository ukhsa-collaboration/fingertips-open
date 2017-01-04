using FingertipsUploadService.Helpers;
using FingertipsUploadService.Upload;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace FingertipsUploadService.FpmFileReader
{
    public class CsvFileReader : IUploadFileReader
    {
        private readonly StreamReader _streamReader;

        public CsvFileReader(string csvfile)
        {
            _streamReader = new StreamReader(csvfile);
        }

        public List<string> GetWorksheets()
        {
            return new List<string> { WorksheetNames.BatchPholio };
        }

        public DataTable GetIndicatorDetails()
        {
            // We don't need to implement as CSV files are not supported for 
            // single uploadsw
            throw new System.NotImplementedException();
        }

        public DataTable GetPholioData()
        {
            // We don't need to implement as CSV files are not supported for 
            // single uploadsw
            throw new System.NotImplementedException();
        }

        public DataTable GetBatchData()
        {
            return new CsvRowParser().Parse(_streamReader);
        }
    }

}
