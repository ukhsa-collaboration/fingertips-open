using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Export;
using PholioVisualisation.Export.File;
using SpreadsheetGear;

namespace PholioVisualisation.ExportTest
{
    [TestClass]
    public class ExcelFileWriterTest
    {
        private string filepath = Path.Combine(ApplicationConfiguration.ExportFileDirectory, "1.xlsx");

        [TestCleanup]
        public void CalledAfterEachTest()
        {
            DeleteFile();
        }

        [TestInitialize]
        public void CalledBeforeEachTest()
        {
            DeleteFile();
            EnsureExportDirectoryExists();
        }

        [TestMethod]
        public void TestWrite()
        {
            Assert.IsFalse(System.IO.File.Exists(filepath));

            var fileInfo = new Mock<BaseExcelFileInfo>();
            fileInfo.Setup(x => x.FilePath).Returns(filepath);
            fileInfo.Setup(x => x.FileFormat).Returns(FileFormat.OpenXMLWorkbook);

            new ExcelFileWriter { UseFileCache = true }
                .Write(fileInfo.Object, Factory.GetWorkbook());

            Assert.IsTrue(System.IO.File.Exists(filepath), "Export file path does not exist: " + filepath);
        }

        private static void EnsureExportDirectoryExists()
        {
            string exportFileDirectory = ApplicationConfiguration.ExportFileDirectory;
            if (Directory.Exists(exportFileDirectory) == false)
            {
                Directory.CreateDirectory(exportFileDirectory);
            }

            if (Directory.Exists(exportFileDirectory) == false)
            {
                Assert.Fail("Could not create export directory");
            }
        }

        private void DeleteFile()
        {
            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }
        }
    }
}
