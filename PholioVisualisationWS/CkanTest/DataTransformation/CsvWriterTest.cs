using System;
using System.Collections.Generic;
using System.Linq;
using Ckan.DataTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PholioVisualisation.CkanTest.DataTransformation
{
    [TestClass]
    public class CsvWriterTest
    {
        [TestMethod]
        public void TestWriteEmptyFile()
        {
            var writer = new CsvWriter();
            Assert.IsNotNull(writer.WriteAsBytes());
        }

        [TestMethod]
        public void TestAddingBothHeaderAndLineIncreasesSizeOfFile()
        {
            var writer = new CsvWriter();
            writer.AddHeader("a");
            var bytesWithOnlyHeader = writer.WriteAsBytes();

            writer = new CsvWriter();
            writer.AddHeader("a");
            writer.AddLine("b");
            var bytesWithHeaderAndLine = writer.WriteAsBytes();

            Assert.AreNotEqual(0, bytesWithOnlyHeader);
            Assert.IsTrue(bytesWithOnlyHeader.Length < bytesWithHeaderAndLine.Length);
        }
    }
}
