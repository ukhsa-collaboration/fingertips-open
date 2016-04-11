using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PholioObjectsTest
{
    [TestClass]
    public class AreaTypeTest
    {
        [TestMethod]
        public void AreaTypeImplementsIAreaType()
        {
            Assert.IsNotNull(new AreaType() as IAreaType);
        }
    }
}
