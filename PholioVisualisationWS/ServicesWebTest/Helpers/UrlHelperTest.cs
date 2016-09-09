using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServicesWeb.Helpers;

namespace PholioVisualisation.ServicesWebTest
{
    [TestClass]
    public class UrlHelperTest
    {
        [TestMethod]
        public void TestTrimProtocol()
        {
            Assert.AreEqual("localhost", UrlHelper.TrimProtocol("http://localhost"));
            Assert.AreEqual("localhost", UrlHelper.TrimProtocol("https://localhost"));
        }

    }
}
