using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.MainUI.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.MainUITest.Helpers
{
    [TestClass]
    public class FusStatusTest
    {
        [TestMethod]
        public void TestMessage()
        {
            Assert.IsTrue(FusStatus.Message().Contains("Upload"));
        }
    }
}
