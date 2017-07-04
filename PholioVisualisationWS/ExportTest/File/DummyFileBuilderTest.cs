using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Export.File;

namespace PholioVisualisation.ExportTest
{
    [TestClass]
    public class DummyFileBuilderTest
    {
        [TestMethod]
        public void Test_Empty_Content_Always_Returned()
        {
            var builder = new DummyFileBuilder();
            builder.AddContent(new byte[] { 1 });

            var content = builder.GetFileContent();

            Assert.AreEqual(0, content.Length);
        }
    }
}
