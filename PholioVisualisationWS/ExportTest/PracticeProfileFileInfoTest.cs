using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Export;
using SpreadsheetGear;

namespace PholioVisualisation.ExportTest
{
    [TestClass]
    public class PracticeProfileFileInfoTest
    {
        [TestMethod]
        public void TestSingleGroupId()
        {
            PracticeProfileFileInfo maker = new PracticeProfileFileInfo(
                "pp", new List<int>{1}, "eng");
            Assert.AreEqual("pp-eng-1.xlsx", maker.FileName);
        }

        [TestMethod]
        public void TestMultipleGroupIds()
        {
            PracticeProfileFileInfo maker = new PracticeProfileFileInfo(
                "pp", new List<int> { 1,2 }, "eng");
            Assert.AreEqual("pp-eng-1-2.xlsx", maker.FileName);
        }

        [TestMethod]
        public void TestFileFormat()
        {
            var fileInfo = new SearchResultsFileInfo();
            Assert.AreEqual(FileFormat.OpenXMLWorkbook, fileInfo.FileFormat);
        }
    }
}
