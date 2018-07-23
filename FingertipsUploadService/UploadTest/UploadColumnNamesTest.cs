using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FingertipsUploadService.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FingertipsUploadService.UploadTest
{
    [TestClass]
    public class UploadColumnNamesTest
    {
        [TestMethod]
        public void TestGetColumnNames()
        {
            var names = UploadColumnNames.GetColumnNames();
            Assert.IsTrue(names.Contains(UploadColumnNames.IndicatorId));
        }

        [TestMethod]
        public void TestChangeAllColumnNamesToLowerCase()
        {
            var table = DataTable();
            UploadColumnNames.ChangeAllColumnNamesToLowerCase(table);
            Assert.AreEqual("a", table.Columns[0].ColumnName);
        }

        [TestMethod]
        public void TestGetColumnNamesFromDataTable()
        {
            var table = DataTable();
            var names = UploadColumnNames.GetColumnNames(table);
            Assert.AreEqual("A", names.First());
        }

        [TestMethod]
        public void TestChangeDeprecatedColumnNames()
        {
            var table = new DataTable();
            table.Columns.Add("LowerCI");
            table.Columns.Add("UpperCI");

            UploadColumnNames.ChangeDeprecatedColumnNames(table);

            // Assert: check names have been changed
            Assert.AreEqual(UploadColumnNames.LowerCI95, table.Columns[0].ColumnName);
            Assert.AreEqual(UploadColumnNames.UpperCI95, table.Columns[1].ColumnName);
        }

        private static DataTable DataTable()
        {
            var table = new DataTable();
            table.Columns.Add("A");
            return table;
        }
    }
}
