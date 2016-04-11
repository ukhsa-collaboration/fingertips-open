
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.AnalysisTest
{
    [TestClass]
    public class SingleOverlappingCIsComparatorTest
    {
        private static SingleOverlappingCIsComparer New(int polarity)
        {
            return new SingleOverlappingCIsComparer { PolarityId = polarity};
        }

        [TestMethod]
        public void TestSameExactly()
        {
            CoreDataSet parent = new CoreDataSet { Value = 11 };
            CoreDataSet data = new CoreDataSet { Value = 11, LowerCI = 10, UpperCI = 12 };

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
            CoreDataSet data = new CoreDataSet { Value = 3.05, LowerCI = 2.7, UpperCI = 3.1 };

            Assert.AreEqual(Significance.Same, Compare(data, parent, 0));
        }

        [TestMethod]
        public void TestHigher()
        {
            CoreDataSet parent = new CoreDataSet() { Value = 3.0 };
            CoreDataSet data = new CoreDataSet() { Value = 3.3, LowerCI = 3.2, UpperCI = 3.5 };

            Assert.AreEqual(Significance.Worse, Compare(data, parent, 0));
            Assert.AreEqual(Significance.Better, Compare(data, parent, 1));
        }

        [TestMethod]
        public void TestLower()
        {
            CoreDataSet parent = new CoreDataSet { Value = 3.0 };
            CoreDataSet data = new CoreDataSet { Value = 2.5, LowerCI = 2.2, UpperCI = 2.8 };

            Assert.AreEqual(Significance.Better, Compare(data, parent, 0));
            Assert.AreEqual(Significance.Worse, Compare(data, parent, 1));
        }

        [TestMethod]
        public void TestMinusOneData()
        {
            CoreDataSet parent = new CoreDataSet { Value = 3.0 };
            CoreDataSet data = new CoreDataSet
            {
                Value = CoreDataSet.NullValue,
                LowerCI = CoreDataSet.NullValue,
                UpperCI = CoreDataSet.NullValue
            };

            Assert.AreEqual(Significance.None, Compare(data, parent, 0));
            Assert.AreEqual(Significance.None, Compare(data, parent, 1));
        }

        [TestMethod]
        public void TestMinusOneParent()
        {
            CoreDataSet parent = new CoreDataSet { Value = CoreDataSet.NullValue };
            CoreDataSet data = new CoreDataSet { Value = 2.5, LowerCI = 2.2, UpperCI = 2.8 };

            Assert.AreEqual(Significance.None, Compare(data, parent, 0));
            Assert.AreEqual(Significance.None, Compare(data, parent, 1));
        }

        [TestMethod]
        public void TestMinusOneBoth()
        {
            CoreDataSet parent = new CoreDataSet { Value = CoreDataSet.NullValue };
            CoreDataSet data = new CoreDataSet
            {
                Value = CoreDataSet.NullValue,
                LowerCI = CoreDataSet.NullValue,
                UpperCI = CoreDataSet.NullValue
            };

            Assert.AreEqual(Significance.None, Compare(data, parent, 0));
            Assert.AreEqual(Significance.None, Compare(data, parent, 1));
        }

        [TestMethod]
        public void TestNoCIsOnLocalDataReturnNoneSignificance()
        {
            // No Lower CI
            CoreDataSet parent = new CoreDataSet { Value = 3.0, LowerCI = 3.0, UpperCI = 3.0 };
            CoreDataSet data = new CoreDataSet { Value = 4.0, LowerCI = CoreDataSet.NullValue, UpperCI = 4.1 };
            Assert.AreEqual(Significance.None, Compare(data, parent, 0));

            // No Upper CI
            data = new CoreDataSet { Value = 4.0, LowerCI = 4.0, UpperCI = CoreDataSet.NullValue };
            Assert.AreEqual(Significance.None, Compare(data, parent, 0));

            // Both CIs missing
            data = new CoreDataSet { Value = 4.0, LowerCI = CoreDataSet.NullValue, UpperCI = CoreDataSet.NullValue };
            Assert.AreEqual(Significance.None, Compare(data, parent, 0));
        }

    }
}
