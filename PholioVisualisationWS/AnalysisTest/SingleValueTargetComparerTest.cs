using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.AnalysisTest
{
    [TestClass]
    public class SingleValueTargetComparerTest
    {
        [TestMethod]
        public void TestNoComparisonIsMade()
        {
            var comparer = new SingleValueTargetComparer(Config(PolarityIds.RagHighIsGood));
            TargetComparerTest.TestNoComparisonIsMade(comparer);
        }

        [TestMethod]
        public void TestCompareRagLowIsGood()
        {
            var comparer = new SingleValueTargetComparer(Config(PolarityIds.RagLowIsGood));

            // Worse
            var significance = comparer.CompareAgainstTarget(Value(9));
            Assert.AreEqual(Significance.Better, significance);

            // Same
            significance = comparer.CompareAgainstTarget(Value(10.0));
            Assert.AreEqual(Significance.Better, significance);

            // Better
            significance = comparer.CompareAgainstTarget(Value(11));
            Assert.AreEqual(Significance.Worse, significance);
        }

        [TestMethod]
        public void TestCompareRagHighIsGood()
        {
            var comparer = new SingleValueTargetComparer(Config(PolarityIds.RagHighIsGood));

            // Worse
            var significance = comparer.CompareAgainstTarget(Value(9));
            Assert.AreEqual(Significance.Worse, significance);

            // Same
            significance = comparer.CompareAgainstTarget(Value(10.0));
            Assert.AreEqual(Significance.Better, significance);

            // Better
            significance = comparer.CompareAgainstTarget(Value(11));
            Assert.AreEqual(Significance.Better, significance);
        }

        private CoreDataSet Value(double val)
        {
            return new CoreDataSet { Value = val };
        }

        public static TargetConfig Config(int polarityId)
        {
            return new TargetConfig
            {
                LowerLimit = 10.0,
                PolarityId = polarityId
            };

        }
    }
}
