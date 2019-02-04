using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.AnalysisTest
{
    /// <summary>
    /// Value confidence intervals overlapping with benchmark value
    /// </summary>
    [TestClass]
    public class SingleOverlappingCIsForTwoCILevelsComparerTest
    {
        private static SingleOverlappingCIsForTwoCILevelsComparer New(int polarity)
        {
            return new SingleOverlappingCIsForTwoCILevelsComparer { PolarityId = polarity };
        }

        [TestMethod]
        public void TestLower99_8()
        {
            CoreDataSet parent = new CoreDataSet { Value = 4.0 };
            CoreDataSet data = new CoreDataSet { Value = 3.0, LowerCI99_8 = 3.2, LowerCI95 = 3.4,
                UpperCI95 = 3.6, UpperCI99_8 = 3.8 };

            Assert.AreEqual(Significance.Best, Compare(data, parent, PolarityIds.RagLowIsGood));
            Assert.AreEqual(Significance.Worst, Compare(data, parent, PolarityIds.RagHighIsGood));
        }

        [TestMethod]
        public void TestLower95()
        {
            CoreDataSet parent = new CoreDataSet { Value = 3.0 };
            CoreDataSet data = new CoreDataSet { Value = 3.2, LowerCI99_8 = 2.8, LowerCI95 = 3.2,
                UpperCI95 = 3.5, UpperCI99_8 = 3.7 };

            Assert.AreEqual(Significance.Better, Compare(data, parent, PolarityIds.RagLowIsGood));
            Assert.AreEqual(Significance.Worse, Compare(data, parent, PolarityIds.RagHighIsGood));
        }

        [TestMethod]
        public void TestSame()
        {
            CoreDataSet parent = new CoreDataSet { Value = 3.0 };
            CoreDataSet data = new CoreDataSet { Value = 2.3, LowerCI99_8 = 2.5, LowerCI95 = 2.7,
                UpperCI95 = 3.3, UpperCI99_8 = 3.7 };

            Assert.AreEqual(Significance.Same, Compare(data, parent, PolarityIds.RagHighIsGood));
        }

        [TestMethod]
        public void TestHigher95()
        {
            CoreDataSet parent = new CoreDataSet { Value = 3.0 };
            CoreDataSet data = new CoreDataSet { Value = 3.0, LowerCI99_8 = 2.5, LowerCI95 = 2.7,
                UpperCI95 = 2.9, UpperCI99_8 = 3.5 };

            Assert.AreEqual(Significance.Worse, Compare(data, parent, PolarityIds.RagLowIsGood));
            Assert.AreEqual(Significance.Better, Compare(data, parent, PolarityIds.RagHighIsGood));
        }

        [TestMethod]
        public void TestHigher99_8()
        {
            CoreDataSet parent = new CoreDataSet { Value = 3.0 };
            CoreDataSet data = new CoreDataSet { Value = 3.0, LowerCI99_8 = 2.3, LowerCI95 = 2.5,
                UpperCI95 = 2.7, UpperCI99_8 = 2.9 };

            Assert.AreEqual(Significance.Worst, Compare(data, parent, PolarityIds.RagLowIsGood));
            Assert.AreEqual(Significance.Best, Compare(data, parent, PolarityIds.RagHighIsGood));
        }

        private Significance Compare(CoreDataSet data, CoreDataSet parent, int polarity)
        {
            return New(polarity).Compare(data, parent, new IndicatorMetadata());
        }

    }
}
