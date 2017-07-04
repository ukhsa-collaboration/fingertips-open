using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServicesWeb.Helpers;

namespace PholioVisualisation.ServicesWebTest.Helpers
{
    [TestClass]
    public class FileResponseBuilderTest
    {
        [TestMethod]
        public void Test_Message_Can_Be_Created()
        {
            var builder = new FileResponseBuilder();
            builder.SetFileContent(new byte[] {});
            builder.SetFilename("a");
            var message = builder.Message;

            Assert.IsNotNull(message);
        }
    }
}
