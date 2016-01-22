using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioObjectsTest
{
    [TestClass]
    public class IndicatorMetadataTest
    {
        [TestMethod]
        public void TestHasTarget()
        {
            Assert.IsTrue(new IndicatorMetadata{TargetConfig = new TargetConfig()}.HasTarget);
            Assert.IsFalse(new IndicatorMetadata().HasTarget);
        }
    }
}
