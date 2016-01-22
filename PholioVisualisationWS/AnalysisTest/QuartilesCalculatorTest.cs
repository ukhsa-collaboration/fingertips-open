using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;

namespace AnalysisTest
{
    [TestClass]
    public class QuartilesCalculatorTest
    {
        [TestMethod]
        public void TestNullListReturnEmptyList()
        {
            Assert.AreEqual(0, new QuartilesCalculator(null).Bounds.Count);
        }

        [TestMethod]
        public void TestCalculateBounds()
        {
            var min = 1;
            var max = 50;
            var values = GetValues(min, max);

            var bounds = new QuartilesCalculator(values).Bounds;
            Assert.AreEqual(5, bounds.Count);

            Assert.AreEqual(min, bounds[0]);
            Assert.AreEqual(13.25, bounds[1]);
            Assert.AreEqual(25.5, bounds[2]);
            Assert.AreEqual(37.75, bounds[3]);
            Assert.AreEqual(max, bounds[4]);
        }

        private static List<double> GetValues(int min, int max)
        {
            var values = new List<double>();
            for (int i = min; i <= max; i++)
            {
                values.Add(i);
            }
            return values;
        }
    }
}
