using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Export.File;

namespace PholioVisualisation.ExportTest.File
{
    [TestClass]
    public class CacheFileNamerTest
    {
        [TestMethod]
        public void TestGetAddressFileName()
        {
            Assert.AreEqual("a-1.addresses.csv", CacheFileNamer.GetAddressFileName("a", 1));
        }

        [TestMethod]
        public void TestGetIndicatorFileName()
        {
            Assert.AreEqual("4-a-2-3-1.data.csv", CacheFileNamer.GetIndicatorFileName(1,"a", 2,3,4));
        }
    }
}
