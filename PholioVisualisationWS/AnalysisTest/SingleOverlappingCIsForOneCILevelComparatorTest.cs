
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.AnalysisTest
{
    /// <summary>
    /// Value confidence intervals overlapping with benchmark value
    /// </summary>
    [TestClass]
    public class SingleOverlappingCIsForOneCILevelComparatorTest
    {
        private static SingleOverlappingCIsForOneCILevelComparer New(int polarity)
        {
            return new SingleOverlappingCIsForOneCILevelComparer { PolarityId = polarity };
        }

        [TestMethod]
        public void TestSameExactly()
        {
            CoreDataSet parent = new CoreDataSet { Value = 11 };
            CoreDataSet data = new CoreDataSet { Value = 11, LowerCI95 = 10, UpperCI95 = 12 };

            Assert.AreEqual(Significance.Same, Compare(data, parent, 0));
            Assert.AreEqual(Significance.Same, Compare(data, parent, 1));
        }

        private Significance Compare(CoreDataSet data, CoreDataSet parent, int polarity)
        {
            return New(polarity).Compare(data, parent, new IndicatorMetadata());
        }

        [TestMethod]
        public void TestSame()
        {
            CoreDataSet parent = new CoreDataSet { Value = 3.0 };
            CoreDataSet data = new CoreDataSet { Value = 3.05, LowerCI95 = 2.7, UpperCI95 = 3.1 };

            Assert.AreEqual(Significance.Same, Compare(data, parent, 0));
        }

        [TestMethod]
        public void TestHigher()
        {
            CoreDataSet parent = new CoreDataSet { Value = 3.0 };
            CoreDataSet data = new CoreDataSet { Value = 3.3, LowerCI95 = 3.2, UpperCI95 = 3.5 };

            Assert.AreEqual(Significance.Worse, Compare(data, parent, 0));
            Assert.AreEqual(Significance.Better, Compare(data, parent, 1));
        }

        [TestMethod]
        public void TestLower()
        {
            CoreDataSet parent = new CoreDataSet { Value = 3.0 };
            CoreDataSet data = new CoreDataSet { Value = 2.5, LowerCI95 = 2.2, UpperCI95 = 2.8 };

            Assert.AreEqual(Significance.Better, Compare(data, parent, 0));
            Assert.AreEqual(Significance.Worse, Compare(data, parent, 1));
        }

        [TestMethod]
        public void TestMinusOneData()
        {
            CoreDataSet parent = new CoreDataSet { Value = 3.0 };
            CoreDataSet data = new CoreDataSet
            {
                Value = ValueData.NullValue
            };

            Assert.AreEqual(Significance.None, Compare(data, parent, 0));
            Assert.AreEqual(Significance.None, Compare(data, parent, 1));
        }

        [TestMethod]
        public void TestMinusOneParent()
        {
            CoreDataSet parent = new CoreDataSet { Value = ValueData.NullValue };
            CoreDataSet data = new CoreDataSet { Value = 2.5, LowerCI95 = 2.2, UpperCI95 = 2.8 };

            Assert.AreEqual(Significance.None, Compare(data, parent, 0));
            Assert.AreEqual(Significance.None, Compare(data, parent, 1));
        }

        [TestMethod]
        public void TestMinusOneBoth()
        {
            CoreDataSet parent = new CoreDataSet { Value = ValueData.NullValue };
            CoreDataSet data = new CoreDataSet
            {
                Value = ValueData.NullValue
            };

            Assert.AreEqual(Significance.None, Compare(data, parent, 0));
            Assert.AreEqual(Significance.None, Compare(data, parent, 1));
        }

        [TestMethod]
        public void TestNoCIsOnLocalDataReturnNoneSignificance()
        {
            // No Lower CI
            CoreDataSet parent = new CoreDataSet { Value = 3.0, UpperCI95 = 3.0 };
            CoreDataSet data = new CoreDataSet { Value = 4.0 };
            Assert.AreEqual(Significance.None, Compare(data, parent, 0));

            // No Upper CI
            data = new CoreDataSet { Value = 4.0, LowerCI95 = 3.0 };
            Assert.AreEqual(Significance.None, Compare(data, parent, 0));

            // Both CIs missing
            data = new CoreDataSet { Value = 4.0 };
            Assert.AreEqual(Significance.None, Compare(data, parent, 0));
        }

    }
}
