using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Export;
using SpreadsheetGear;

namespace PholioVisualisation.ExportTest
{
    [TestClass]
    public class SearchResultsFileInfoTest
    {
        [TestMethod]
        public void TestFileExtension1()
        {
            var fileInfo = new SearchResultsFileInfo();
            Assert.AreEqual("xlsx", fileInfo.FileExtension);
        }

        [TestMethod]
        public void TestFileFormat()
        {
            var fileInfo = new SearchResultsFileInfo();
            Assert.AreEqual(FileFormat.OpenXMLWorkbook, fileInfo.FileFormat);
        }
    }
}
