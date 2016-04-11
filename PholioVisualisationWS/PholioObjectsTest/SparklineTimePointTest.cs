
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PholioObjectsTest
{
    [TestClass]
    public class SparklineTimePointTest
    {
        [TestMethod]
        public void TestConstruction()
        {
            SparklineTimePoint point = new SparklineTimePoint();
            Assert.IsNotNull(point.Data);
            Assert.IsFalse(point.AreDifferent);
        }
    }
}
