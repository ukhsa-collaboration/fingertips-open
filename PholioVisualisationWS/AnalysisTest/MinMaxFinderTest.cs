using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;

namespace AnalysisTest
{
    [TestClass]
    public class MinMaxFinderTest
    {
        private static readonly IEnumerable<double> Values1 =
            new[] { 5.5, 1.1, 2.2, 4.4, 0.3, 3.3 };

        [TestMethod]
        public void TestGetLimitsForOneList()
        {
            var finder = new MinMaxFinder();
            finder.AddRange(Values1);
            var limits = finder.GetLimits();
            Assert.AreEqual(0.3, limits.Min);
            Assert.AreEqual(5.5, limits.Max);
        }

        [TestMethod]
        public void TestGetLimitsForMultipleLists()
        {
            var finder = new MinMaxFinder();
            finder.AddRange(Values1);
            finder.AddRange(new[] { 3, 2, 6.6, 1 });
            var limits = finder.GetLimits();
            Assert.AreEqual(0.3, limits.Min);
            Assert.AreEqual(6.6, limits.Max);
        }

        [TestMethod]
        public void TestNullLimitsIfNoValues()
        {
            var finder = new MinMaxFinder();
            Assert.IsNull(finder.GetLimits());
        }

        [TestMethod]
        public void TestNullLimitsIfOnlyOneValue()
        {
            var finder = new MinMaxFinder();
            finder.AddRange(new[] { 1.1 });
            Assert.IsNull(finder.GetLimits());
        }

        [TestMethod]
        public void TestLimitsIfOnlyTwoValues()
        {
            var finder = new MinMaxFinder();
            finder.AddRange(new[] { 1.1, 3.3 });
            var limits = finder.GetLimits();
            Assert.AreEqual(1.1, limits.Min);
            Assert.AreEqual(3.3, limits.Max);
        }
    }
}
