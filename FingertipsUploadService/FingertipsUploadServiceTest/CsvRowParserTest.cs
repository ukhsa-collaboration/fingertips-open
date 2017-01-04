using System;
using FingertipsUploadService.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.IO;
using FingertipsUploadService.ProfileData;

namespace FingertipsUploadServiceTest
{
    [TestClass]
    public class CsvRowParserTest
    {
        private const double MinusOne = -1;

        public string Header = "INDICATORID,YEAR,YEARRANGE,QUARTER,MONTH,AGEID,SEXID,AREACODE,COUNT,VALUE,LOWERCI,UPPERCI,DENOMINATOR,DENOMINATOR_2,VALUENOTEID,CATEGORYTYPEID,CATEGORYID" + 
            Environment.NewLine;

        [TestMethod]
        public void EmptyAreaCodeTest()
        {
            var csvData = Header + "92600,2020,1,-1,-1,8,1,,2,48.54881266,45.00714866,52.10511141,758,758,0,-1,-1";

            var data = GetData(csvData);
            var rows = data.Select("AreaCode = ''");
            Assert.AreEqual(string.Empty, rows[0]["AreaCode"]);
        }

        [TestMethod]
        public void EmptyCountCodeTest()
        {
            var csvData = Header + "92600,2020,1,-1,-1,8,1,A81001,,48.54881266,45.00714866,52.10511141,758,758,0,-1,-1";
            var data = GetData(csvData);
            var rows = data.Select("Count = -1");
            Assert.AreEqual(MinusOne, rows[0]["Count"]);
        }

        [TestMethod]
        public void EmptyValueNoteTest()
        {
            var csvData = Header + "92600,2020,1,-1,-1,8,1,A81001,10,48.54881266,45.00714866,52.10511141,758,758,,-1,-1";
            var data = GetData(csvData);
            var rows = data.Select("ValueNoteId = 0");
            Assert.AreEqual((double)ValueNoteIds.NoNote, rows[0]["ValueNoteId"]);
        }

        private DataTable GetData(string csvData)
        {
            var parser = new CsvRowParser();
            var data = parser.Parse(GetStreamReader(csvData));
            return data;
        }

        private StreamReader GetStreamReader(string data)
        {
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            streamWriter.Write(data);
            streamWriter.Flush();
            stream.Position = 0;
            return new StreamReader(stream);
        }

    }
}
