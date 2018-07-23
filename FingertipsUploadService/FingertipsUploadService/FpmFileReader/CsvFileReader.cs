using FingertipsUploadService.Helpers;
using FingertipsUploadService.Upload;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace FingertipsUploadService.FpmFileReader
{
    public class CsvFileReader : IUploadFileReader, IDisposable
    {
        private StreamReader _streamReader;

        public CsvFileReader(string filePath)
        {
            _streamReader = new StreamReader(filePath);
        }

        public List<string> GetWorksheets()
        {
            return new List<string> { WorksheetNames.Pholio };
        }


        public DataTable ReadData()
        {
            return new CsvStreamReader().Read(_streamReader);
        }


        public void Dispose()
        {
            _streamReader.Close();
            _streamReader = null;
        }
    }

}
