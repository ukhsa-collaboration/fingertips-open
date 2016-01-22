
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioObjectsTest
{
    [TestClass]
    public class SparklineStatsTest
    {
        [TestMethod]
        public void TestConstruction()
        {
            SparklineStats point = new SparklineStats(new List<string>());
            Assert.IsNotNull(point.ComparatorValues);
        }
    }
}
