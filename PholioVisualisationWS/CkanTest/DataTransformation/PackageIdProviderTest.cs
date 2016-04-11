using System;
using System.Collections.Generic;
using System.Linq;
using Ckan.DataTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PholioVisualisation.CkanTest.DataTransformation
{
    [TestClass]
    public class PackageIdProviderTest
    {
        [TestMethod]
        public void TestNextID_Includes_Version_Number_After_First_ID()
        {
            var idProvider = new PackageIdProvider(33);
            Assert.AreEqual("phe-indicator-33", idProvider.NextID);
            Assert.AreEqual("phe-indicator-33-v2", idProvider.NextID);
            Assert.AreEqual("phe-indicator-33-v3", idProvider.NextID);
        }
    }
}
