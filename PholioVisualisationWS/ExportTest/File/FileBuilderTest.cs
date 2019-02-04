using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Export.File;

namespace PholioVisualisation.ExportTest.File
{
    [TestClass]
    public class FileBuilderTest
    {
        [TestMethod]
        public void TestGetFileContent()
        {
            var builder = new CsvFileBuilder();
            builder.AddContent(new byte[] {1});
            builder.AddContent(new byte[] {2});

            var content = builder.GetFileContent();

            Assert.AreEqual(2, content.Length);
        }

        [TestMethod]
        public void TestGetFileContent_When_No_Content()
        {
            var builder = new CsvFileBuilder();

            var content = builder.GetFileContent();

            Assert.AreEqual(0, content.Length);
        }
    }
}
