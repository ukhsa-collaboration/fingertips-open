using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServicesWeb.Common;

namespace PholioVisualisation.ServicesWebTest.Common
{
    [TestClass]
    public class ServiceHelperTest
    {
        [TestMethod]
        public void ParseYesOrNo_WhereCorrectString()
        {
            Assert.IsTrue(ServiceHelper.ParseYesOrNo("yes",false));
            Assert.IsFalse(ServiceHelper.ParseYesOrNo("no",true));
        }

        [TestMethod]
        public void ParseYesOrNo_ReturnsDefault()
        {
            Assert.IsTrue(ServiceHelper.ParseYesOrNo("", true));
            Assert.IsTrue(ServiceHelper.ParseYesOrNo(null, true));
            Assert.IsTrue(ServiceHelper.ParseYesOrNo("a", true));

            Assert.IsFalse(ServiceHelper.ParseYesOrNo("", false));
            Assert.IsFalse(ServiceHelper.ParseYesOrNo(null, false));
            Assert.IsFalse(ServiceHelper.ParseYesOrNo("a", false));
        }
    }
}
