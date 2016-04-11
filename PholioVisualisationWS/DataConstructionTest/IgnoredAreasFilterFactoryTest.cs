using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class IgnoredAreasFilterFactoryTest
    {
        [TestMethod]
        public void TestNew()
        {
            var filter = IgnoredAreasFilterFactory.New(ProfileIds.Phof);
            Assert.IsNotNull(filter);
        }
    }
}
