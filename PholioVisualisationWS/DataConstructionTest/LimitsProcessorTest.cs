using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class LimitsProcessorTest
    {
        [TestMethod]
        public void TestTruncateList()
        {
            var limits = new Limits {Min = 1.111111111, Max = 8.8888888888};
            Truncate(limits);
            Assert.AreEqual(1.1111, limits.Min);
            Assert.AreEqual(8.8889, limits.Max);
        }

        [TestMethod]
        public void TestTruncateList_NullsIgnored()
        {
            var limitsList = new List<Limits>
            {
                null
            };
            new LimitsProcessor().TruncateList(limitsList);
            Assert.AreEqual(1, limitsList.Count);
            Assert.IsNull(limitsList[0]);
        }

        private static void Truncate(Limits limits)
        {
            new LimitsProcessor().TruncateList(new List<Limits> { limits });
        }

    }
}
