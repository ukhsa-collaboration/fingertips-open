using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;

namespace PholioVisualisation.AnalysisTest
{
    [TestClass]
    public class QuintilesCalculatorTest
    {
        [TestMethod]
        public void TestNullListReturnEmptyList()
        {
            Assert.AreEqual(0, new QuintilesCalculator(null).Bounds.Count);
        }

        [TestMethod]
        public void TestCalculateBounds()
        {
            var min = 1;
            var max = 50;
            var values = GetValues(min, max);

            var bounds = new QuintilesCalculator(values).Bounds;
            Assert.AreEqual(6, bounds.Count);

            Assert.AreEqual(min, bounds[0]);
            Assert.AreEqual(10.8, bounds[1]);
            Assert.AreEqual(20.6, bounds[2]);
            Assert.IsTrue(30.4.Equals(bounds[3]));
            Assert.AreEqual(40.2, bounds[4]);
            Assert.AreEqual(max, bounds[5]);
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
