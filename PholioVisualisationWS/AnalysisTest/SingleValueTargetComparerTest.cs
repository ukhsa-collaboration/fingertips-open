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

        [TestMethod]
        public void TestCompareRagLowIsGoodForCIsComparison()
        {
            var config = Config(PolarityIds.RagLowIsGood);
            config.UseCIsForLimitComparison = true;
            var comparer = new SingleValueTargetComparer(config);

            // Worse
            var significance = comparer.CompareAgainstTarget(new CoreDataSet {LowerCI95 = 8, UpperCI95 = 9});
            Assert.AreEqual(Significance.Better, significance);

            // Within range
            significance = comparer.CompareAgainstTarget(new CoreDataSet { LowerCI95 = 9, UpperCI95 = 11 });
            Assert.AreEqual(Significance.Same, significance);

            // Better
            significance = comparer.CompareAgainstTarget(new CoreDataSet { LowerCI95 = 11, UpperCI95 = 12 });
            Assert.AreEqual(Significance.Worse, significance);
        }

        [TestMethod]
        public void TestCompareRagHighIsGoodForCIsComparison()
        {
            var config = Config(PolarityIds.RagHighIsGood);
            config.UseCIsForLimitComparison = true;
            var comparer = new SingleValueTargetComparer(config);

            // Worse
            var significance = comparer.CompareAgainstTarget(new CoreDataSet { LowerCI95 = 8, UpperCI95 = 9 });
            Assert.AreEqual(Significance.Worse, significance);

            // Within range
            significance = comparer.CompareAgainstTarget(new CoreDataSet { LowerCI95 = 9, UpperCI95 = 11 });
            Assert.AreEqual(Significance.Same, significance);

            // Better
            significance = comparer.CompareAgainstTarget(new CoreDataSet { LowerCI95 = 11, UpperCI95 = 12 });
            Assert.AreEqual(Significance.Better, significance);
        }

        [TestMethod]
        public void TestUseCIsComparisonWhenNoCIs()
        {
            var config = Config(PolarityIds.RagLowIsGood);
            config.UseCIsForLimitComparison = true;
            var comparer = new SingleValueTargetComparer(config);

            // No CIs
            var significance = comparer.CompareAgainstTarget(new CoreDataSet());
            Assert.AreEqual(Significance.None, significance);
        }

        [TestMethod]
        public void TestUseCIsComparisonWhenCIsEqualLimit()
        {
            var config = Config(PolarityIds.RagLowIsGood);
            config.UseCIsForLimitComparison = true;
            var comparer = new SingleValueTargetComparer(config);

            // No CIs
            var significance = comparer.CompareAgainstTarget(new CoreDataSet
            {
                LowerCI95 = 10.0,
                UpperCI95 = 10.0
            });
            Assert.AreEqual(Significance.Worse, significance);
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
