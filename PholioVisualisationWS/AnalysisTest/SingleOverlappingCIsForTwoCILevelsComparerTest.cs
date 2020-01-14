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
        public void Test_Benchmark_Value_Higher_Than_Upper_99_8()
        {
            CoreDataSet parent = new CoreDataSet { Value = 3.0 };
            var data = GetData();

            Assert.AreEqual(Significance.Best, Compare(data, parent, PolarityIds.RagLowIsGood));
            Assert.AreEqual(Significance.Worst, Compare(data, parent, PolarityIds.RagHighIsGood));
        }

        [TestMethod]
        public void Test_Benchmark_Value_Between_Upper_CIs()
        {
            CoreDataSet parent = new CoreDataSet { Value = 2.8 };
            var data = GetData();

            Assert.AreEqual(Significance.Better, Compare(data, parent, PolarityIds.RagLowIsGood));
            Assert.AreEqual(Significance.Worse, Compare(data, parent, PolarityIds.RagHighIsGood));
        }

        [TestMethod]
        public void TestSame()
        {
            CoreDataSet parent = new CoreDataSet { Value = 2.6 };
            var data = GetData();

            Assert.AreEqual(Significance.Same, Compare(data, parent, PolarityIds.RagLowIsGood));
            Assert.AreEqual(Significance.Same, Compare(data, parent, PolarityIds.RagHighIsGood));
        }

        [TestMethod]
        public void Test_Benchmark_Value_Between_Lower_CIs()
        {
            CoreDataSet parent = new CoreDataSet { Value = 2.4};
            var data = GetData();

            Assert.AreEqual(Significance.Worse, Compare(data, parent, PolarityIds.RagLowIsGood));
            Assert.AreEqual(Significance.Better, Compare(data, parent, PolarityIds.RagHighIsGood));
        }

        [TestMethod]
        public void Test_Benchmark_Value_Lower_Than_Upper_99_8()
        {
            CoreDataSet parent = new CoreDataSet { Value = 2.2 };
            var data = GetData();

            Assert.AreEqual(Significance.Worst, Compare(data, parent, PolarityIds.RagLowIsGood));
            Assert.AreEqual(Significance.Best, Compare(data, parent, PolarityIds.RagHighIsGood));
        }

        private static CoreDataSet GetData()
        {
            CoreDataSet data = new CoreDataSet
            {
                LowerCI99_8 = 2.3, LowerCI95 = 2.5, Value = 2.6,
                UpperCI95 = 2.7, UpperCI99_8 = 2.9
            };
            return data;
        }

        private Significance Compare(CoreDataSet data, CoreDataSet parent, int polarity)
        {
            return New(polarity).Compare(data, parent, new IndicatorMetadata());
        }

    }
}
