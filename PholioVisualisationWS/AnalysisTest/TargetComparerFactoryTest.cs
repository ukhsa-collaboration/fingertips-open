using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.AnalysisTest
{
    [TestClass]
    public class TargetComparerFactoryTest
    {
        private const int PolarityId = PolarityIds.RagHighIsGood;

        [TestMethod]
        public void TestNullReturnedIfConfigIsNull()
        {
            Assert.IsNull(TargetComparerFactory.New(null));
        }

        [TestMethod]
        public void TestNewSingleValueTargetComparer()
        {
            TargetComparer comparer = TargetComparerFactory.New(new TargetConfig
            {
                LowerLimit = 0,
                PolarityId = PolarityId
            });
            Assert.IsTrue(comparer is SingleValueTargetComparer);
            Assert.AreEqual(PolarityId, comparer.PolarityId);
        }

        [TestMethod]
        public void TestBespokeTargetPercentileTargetComparer()
        {
            TargetComparer comparer = TargetComparerFactory.New(new TargetConfig
            {
                BespokeTargetKey = BespokeTargets.TargetPercentileRange,
                LowerLimit = 0.9
            });
            Assert.IsTrue(comparer is BespokeTargetPercentileRangeComparer);
        }

        [TestMethod]
        public void TestNewRangeTargetComparer()
        {
            TargetComparer comparer = TargetComparerFactory.New(new TargetConfig
            {
                LowerLimit = 0,
                UpperLimit = 1,
                PolarityId = PolarityId
            });

            Assert.IsTrue(comparer is RangeTargetComparer);
            Assert.AreEqual(PolarityId, comparer.PolarityId);
        }

        [TestMethod]
        public void TestBespokeTargetPreviousYearEnglandValueComparer()
        {
            TargetComparer comparer = TargetComparerFactory.New(new TargetConfig
            {
                BespokeTargetKey = BespokeTargets.ComparedWithPreviousYearEnglandValue
            });
            Assert.IsTrue(comparer is BespokeTargetPreviousYearEnglandValueComparer);
        }
    }
}