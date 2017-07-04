using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Export;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ExportTest
{
    [TestClass]
    public class AddressFileBuilderTest
    {
        private static string _fileText;

        [TestMethod]
        public void Test_Header()
        {
            Assert.IsTrue(GetFile().Contains("Code,Name,Address,Postcode"));
        }

        [TestMethod]
        public void Test_Data()
        {
            Assert.IsTrue(GetFile().Contains(AreaCodes.Gp_MonkfieldCambourne));
            Assert.IsTrue(GetFile().Contains("Cambourne"));
        }

        private static string GetFile()
        {
            if (_fileText == null)
            {
                var fileBuilder = new AddressFileBuilder(ReaderFactory.GetAreasReader());
                var file = fileBuilder.GetAddressFile(AreaTypeIds.GpPractice, AreaCodes.Ccg_CambridgeshirePeterborough);
                _fileText = Encoding.UTF8.GetString(file);
            }
            return _fileText;
        }
    }
}
