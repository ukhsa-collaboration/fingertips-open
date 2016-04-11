using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.AnalysisTest
{
    [TestClass]
    public class RangeTargetComparerTest
    {
        [TestMethod]
        public void TestNoComparisonIsMade()
        {
            var comparer = new SingleValueTargetComparer(DefaultConfig());
            TargetComparerTest.TestNoComparisonIsMade(comparer);
        }

        [TestMethod]
        public void TestExceptionThrownIfLowerLimitGreaterThanUpperLimit()
        {
            try
            {
                new RangeTargetComparer(new TargetConfig
                    {
                        LowerLimit = 2,
                        UpperLimit = 1
                    });
            }
            catch (FingertipsException ex)
            {
                Assert.AreEqual(ex.Message, "Lower limit cannot be greater than upper limit");
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public void TestLowerLimitAllowedToEqualUpperLimit()
        {
            new RangeTargetComparer(new TargetConfig
            {
                LowerLimit = 1,
                UpperLimit = 1
            });
        }

        [TestMethod]
        public void TestHighIsGoodPolarity()
        {
            var comparer = new RangeTargetComparer(DefaultConfig())
            {
                PolarityId = PolarityIds.RagHighIsGood
            };

            AssertExpected(comparer, Significance.Worse, 0);
            AssertExpected(comparer, Significance.Same, 1);
            AssertExpected(comparer, Significance.Same, 1.5);
            AssertExpected(comparer, Significance.Better, 2);
            AssertExpected(comparer, Significance.Better, 3);
        }

        private TargetConfig DefaultConfig()
        {
            return new TargetConfig
                {
                    LowerLimit = 1,
                    UpperLimit = 2
                };
        }

        [TestMethod]
        public void TestLowIsGoodPolarity()
        {
            var comparer = new RangeTargetComparer(DefaultConfig())
            {
                PolarityId = PolarityIds.RagLowIsGood
            };

            // Better
            var significance = comparer.CompareAgainstTarget(
                new CoreDataSet { Value = 0 });
            Assert.AreEqual(Significance.Better, significance);

            // On lower limit
            significance = comparer.CompareAgainstTarget(
                new CoreDataSet { Value = 1 });
            Assert.AreEqual(Significance.Same, significance);

            // On upper limit
            significance = comparer.CompareAgainstTarget(
                new CoreDataSet { Value = 2 });
            Assert.AreEqual(Significance.Worse, significance);

            // Worse
            significance = comparer.CompareAgainstTarget(
                new CoreDataSet { Value = 3 });
            Assert.AreEqual(Significance.Worse, significance);
        }

        private static void AssertExpected(RangeTargetComparer comparer, Significance expected, double val)
        {
            var significance = comparer.CompareAgainstTarget(new CoreDataSet { Value = val });
            Assert.AreEqual(expected, significance);
        }
    }
}
