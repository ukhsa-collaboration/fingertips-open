
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.AnalysisTest
{
    [TestClass]
    public class DoubleOverlappingCIsComparerTest
    {
        [TestMethod]
        public void TestSame()
        {
            // Overlapping CIs & data > parent
            CoreDataSet d1 = new CoreDataSet { Value = 3.0, LowerCI = 2.7, UpperCI = 3.6 };
            CoreDataSet d2 = new CoreDataSet { Value = 4.0, LowerCI = 3.5, UpperCI = 4.5 };
            Assert.AreEqual(Significance.Same, Compare(d2, d1, 0));

            // Overlapping CIs & data < parent
            Assert.AreEqual(Significance.Same, Compare(d1, d2, 0));

            // Overlapping CI overlaps with value & data < parent
            d1 = new CoreDataSet { Value = 3.0, LowerCI = 2.7, UpperCI = 4.1 };
            d2 = new CoreDataSet { Value = 4.0, LowerCI = 3.5, UpperCI = 4.5 };
            Assert.AreEqual(Significance.Same, Compare(d1, d2, 0));

            // Overlapping CI overlaps with value & data > parent
            Assert.AreEqual(Significance.Same, Compare(d2, d1, 0));
        }

        [TestMethod]
        public void TestDifferent()
        {
            // Different data > parent
            CoreDataSet d1 = new CoreDataSet { Value = 3.0, LowerCI = 2.7, UpperCI = 3.1 };
            CoreDataSet d2 = new CoreDataSet { Value = 4.0, LowerCI = 3.5, UpperCI = 4.5 };
            Assert.AreEqual(Significance.Worse, Compare(d2, d1, 0));

            // Different data < parent
            Assert.AreEqual(Significance.Better, Compare(d1, d2, 0));
        }

        [TestMethod]
        public void TestNoCIsReturnNoneSignificance()
        {
            // No Lower CI
            CoreDataSet d1 = new CoreDataSet { Value = 3.0, LowerCI = ValueWithCIsData.NullValue, UpperCI = 3.1 };
            CoreDataSet d2 = new CoreDataSet { Value = 4.0, LowerCI = 3.5, UpperCI = 4.5 };
            Assert.AreEqual(Significance.None, Compare(d2, d1, 0));
            Assert.AreEqual(Significance.None, Compare(d1, d2, 0));

            // No Upper CI
            d1 = new CoreDataSet { Value = 3.0, LowerCI = 2.9, UpperCI = ValueWithCIsData.NullValue };
            d2 = new CoreDataSet { Value = 4.0, LowerCI = 3.5, UpperCI = 4.5 };
            Assert.AreEqual(Significance.None, Compare(d2, d1, 0));
            Assert.AreEqual(Significance.None, Compare(d1, d2, 0));
        }

        private Significance Compare(CoreDataSet data, CoreDataSet parent, int polarity)
        {
            var comparer = new DoubleOverlappingCIsComparer
            {
                PolarityId = polarity
            };
            return comparer.Compare(data, parent, null);
        }

    }
}
