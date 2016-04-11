
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PholioObjectsTest
{
    [TestClass]
    public class TrendRootTest
    {
        [TestMethod]
        public void TestCopyConstructor()
        {
            GroupRoot root = new GroupRoot { 
                AreaTypeId = 2, 
                StateSex = true, 
                IndicatorId = 3, 
                SexId = 5,
                PolarityId = 6,
                AgeId = 7
            };

            TrendRoot rootTrend = new TrendRoot(root);

            Assert.AreEqual(root.IndicatorId, rootTrend.IndicatorId);
            Assert.AreEqual(root.StateSex, rootTrend.StateSex);
            Assert.AreEqual(root.AreaTypeId, rootTrend.AreaTypeId);
            Assert.AreEqual(5, rootTrend.SexId);
            Assert.AreEqual(6, rootTrend.PolarityId);
            Assert.AreEqual(7, rootTrend.AgeId);
        }
    }
}
