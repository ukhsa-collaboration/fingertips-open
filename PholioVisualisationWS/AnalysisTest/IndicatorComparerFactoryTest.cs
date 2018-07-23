using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.AnalysisTest
{
    [TestClass]
    public class IndicatorComparerFactoryTest
    {
        [TestMethod]
        public void TestPolarityIsAssigned()
        {
            var polarityId = PolarityIds.RagHighIsGood;
            var comparatorMethodId = ComparatorMethodIds.DoubleOverlappingCIs;

            var comparer = new IndicatorComparerFactory { PholioReader = MockPholioReader() }.New(
                new Grouping
                {
                    ComparatorMethodId = comparatorMethodId,
                    PolarityId = polarityId
                });

            Assert.AreEqual(polarityId, comparer.PolarityId);
        }

        private static PholioReader MockPholioReader()
        {
            return new Moq.Mock<PholioReader>().Object;
        }

        private static PholioReader MockPholioReader(int targetId, TargetConfig config)
        {
            var mock = new Moq.Mock<PholioReader>();
            mock.Setup(x => x.GetTargetConfig(targetId)).Returns(config);
            return mock.Object;
        }

        [TestMethod]
        public void TestNewNoComparisonComparer()
        {
            var comparer = New(ComparatorMethodIds.NoComparison, 0);
            Assert.IsTrue(comparer is NoComparisonComparer);
        }

        [TestMethod]
        public void TestExceptionIfInvalidComparatorMethodId()
        {
            try
            {
                new IndicatorComparerFactory { PholioReader = MockPholioReader() }.New(GetGrouping(-99, 0));
            }
            catch (FingertipsException ex)
            {
                Assert.AreEqual(ex.Message, "Invalid comparator method ID: -99");
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public void TestNewQuintilesComparer()
        {
            var comparer = New(ComparatorMethodIds.Quintiles, 0);
            Assert.IsTrue(comparer is QuintilesComparer);
        }

        [TestMethod]
        public void TestNewQuartilesComparer()
        {
            var comparer = New(ComparatorMethodIds.Quartiles, 0);
            Assert.IsTrue(comparer is QuartilesComparer);
        }

        [TestMethod]
        public void TestNewSingleOverlappingCIsComparer()
        {
            var comparer = New(ComparatorMethodIds.SingleOverlappingCIs, 0);
            Assert.IsTrue(comparer is SingleOverlappingCIsComparer);
        }

        [TestMethod]
        public void TestNewDoubleOverlappingCIsComparer()
        {
            var comparer = New(ComparatorMethodIds.DoubleOverlappingCIs, 0);
            Assert.IsTrue(comparer is DoubleOverlappingCIsComparer);
        }

        [TestMethod]
        public void TestNewSpcForDsrComparer()
        {
            var comparer = GetIndicatorComparerWithConfidenceVariable(
                ComparatorMethodIds.SpcForDsr, 95);
            Assert.IsTrue(comparer is SpcForDsrComparer);
        }

        private static IndicatorComparer GetIndicatorComparerWithConfidenceVariable(int comparatorMethodId,
                                                                                    int comparatorConfidence)
        {
            var mock = new Moq.Mock<PholioReader>();
            mock.Setup(x => x
                .GetComparatorConfidence(comparatorMethodId, comparatorConfidence))
                .Returns(new ComparatorConfidence());

            var grouping = GetGrouping(comparatorMethodId, comparatorConfidence);

            return new IndicatorComparerFactory { PholioReader = mock.Object }.New(grouping);
        }

        [TestMethod]
        public void TestNewSpcForProportionsComparer()
        {
            var comparer = GetIndicatorComparerWithConfidenceVariable(
                ComparatorMethodIds.SpcForProportions, 95);
            Assert.IsTrue(comparer is SpcForProportionsComparer);
        }

        [TestMethod]
        public void TestNewThrowsExceptionIfPholioReaderNotAvailable()
        {
            try
            {
                new IndicatorComparerFactory().New(new Grouping());
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "PholioReader not set");
                return;
            }
            Assert.Fail();
        }

        private IndicatorComparer New(int comparatorMethodId, double comparatorConfidence)
        {
            return new IndicatorComparerFactory { PholioReader = MockPholioReader(1, new TargetConfig()) }.New(
                new Grouping
                {
                    ComparatorMethodId = comparatorMethodId,
                    ComparatorConfidence = comparatorConfidence
                });
        }

        private static Grouping GetGrouping(int comparatorMethodId, int comparatorConfidence)
        {
            var grouping = new Grouping
            {
                ComparatorMethodId = comparatorMethodId,
                ComparatorConfidence = comparatorConfidence,
            };
            return grouping;
        }
    }
}
