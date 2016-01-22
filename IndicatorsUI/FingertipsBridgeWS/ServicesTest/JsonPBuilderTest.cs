using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FingertipsBridgeWS.Services;

namespace ServicesTest
{
    [TestClass]
    public class JsonPBuilderTest
    {
        [TestMethod]
        public void TestJsonp()
        {
            JsonpBuilder builder = new JsonpBuilder(new byte[] { 1 }, "a");

            Assert.AreEqual(4, builder.Jsonp.Length);
        }
    }
}
