using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;
using PholioVisualisation.PholioObjects;

namespace AnalysisTest
{
    [TestClass]
    public class BespokeTargetPreviousYearEnglandValueComparerTest
    {
        [TestMethod]
        public void TestSignificanceIsCorrect()
        {
            var comparer = new BespokeTargetPreviousYearEnglandValueComparer(new TargetConfig())
            {
                BenchmarkData = new CoreDataSet {Value = 2}
            };

            Assert.AreEqual(Significance.Better, comparer.CompareAgainstTarget(Data(3)));
            Assert.AreEqual(Significance.Worse, comparer.CompareAgainstTarget(Data(1)));
        }

        [TestMethod]
        public void TestSignificanceIsNoneIfBenchmarkValueNotDefined()
        {
            var comparer = new BespokeTargetPreviousYearEnglandValueComparer(new TargetConfig())
            {
                BenchmarkData = new CoreDataSet { Value = ValueData.NullValue }
            };

            Assert.AreEqual(Significance.None, comparer.CompareAgainstTarget(Data(1)));
        }

        [TestMethod]
        public void TestSignificanceIsNoneIfBenchmarkIsNull()
        {
            var comparer = new BespokeTargetPreviousYearEnglandValueComparer(new TargetConfig());
            Assert.AreEqual(Significance.None, comparer.CompareAgainstTarget(Data(1)));
        }

        private CoreDataSet Data(double val)
        {
            return new CoreDataSet
            {
                Value = val
            };
        }
    }
}
