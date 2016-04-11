using System;
using System.Collections.Generic;
using System.Linq;
using DIResolver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;

namespace PholioVisualisation.DataAccessTest
{
    [TestClass]
    public class DataAccessDiRegistryTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            IoC.Register();
        }

        [TestMethod]
        public void TestIProfileReader()
        {
            Assert.IsNotNull(IoC.Container.GetInstance<IProfileReader>());
        }

        [TestMethod]
        public void TestIContentReader()
        {
            Assert.IsNotNull(IoC.Container.GetInstance<IContentReader>());
        }
    }
}
