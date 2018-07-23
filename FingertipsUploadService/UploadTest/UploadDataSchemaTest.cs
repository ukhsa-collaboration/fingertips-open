using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FingertipsUploadService.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FingertipsUploadService.UploadTest
{
    [TestClass]
    public class UploadDataSchemaTest
    {
        [TestMethod]
        public void TestCreateEmptyTable()
        {
            var table = UploadDataSchema.CreateEmptyTable();
            Assert.IsTrue(table.Columns.Count > 10);
        }

        [TestMethod]
        public void TestSetColumnDataTypesAndIgnoreMissingColumns()
        {
            var table = new DataTable();
            table.Columns.Add(UploadColumnNames.AreaCode);
            table.Columns.Add(UploadColumnNames.IndicatorId);
            UploadDataSchema.SetColumnDataTypes(table);

            Assert.AreEqual(typeof(string), table.Columns[UploadColumnNames.AreaCode].DataType);
            Assert.AreEqual(typeof(double), table.Columns[UploadColumnNames.IndicatorId].DataType);
        }
    }
}
