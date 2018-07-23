using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndicatorsUI.DataAccess;

namespace IndicatorsUI.DataAccessTest
{
    [TestClass]
    public class TwitterSettingReaderTest
    {
        [TestMethod]
        public void SingleSettingTest()
        {
            var reader = ReaderFactory.GetTwitterSettingReadr();
            var settings = reader.GetSettings("PHE_uk");
            Assert.IsNotNull(settings);
            Assert.AreEqual(settings.Handle, "PHE_uk");
        }
    }
}
