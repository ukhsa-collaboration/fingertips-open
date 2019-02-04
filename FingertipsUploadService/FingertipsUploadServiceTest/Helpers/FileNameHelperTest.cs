using System;
using System.Collections.Generic;
using System.Linq;
using FingertipsUploadService.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FingertipsUploadServiceTest
{
    [TestClass]
    public class FileNameHelperTest
    {
        private const string FilePath = @"C:\automated-uploads\1.csv";
        private const string GuidString = "00000000-0000-0000-0000-000000000123";
        private Guid _guid = Guid.Parse(GuidString);

        [TestMethod]
        public void TestFileName()
        {
            Assert.AreEqual("1.csv", FileNameHelper().GetFileName());
        }

        [TestMethod]
        public void TestGetFileNameForUploadFolder()
        {
            Assert.AreEqual(GuidString + ".csv", FileNameHelper().GetFileNameForUploadFolder());
        }

        [TestMethod]
        public void TestGetFileNameForArchiveFolder()
        {
            Assert.AreEqual("1-" + GuidString + ".csv", FileNameHelper().GetFileNameForArchiveFolder());
        }

        private FileNameHelper FileNameHelper()
        {
            var fileNameHelper = new FileNameHelper(FilePath, _guid);
            return fileNameHelper;
        }
    }
}
