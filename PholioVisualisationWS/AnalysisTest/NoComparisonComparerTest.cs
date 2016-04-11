using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.AnalysisTest
{
    [TestClass]
    public class NoComparisonComparerTest
    {
        [TestMethod]
        public void TestCompareSignificanceIsNone()
        {
            var comparer = new NoComparisonComparer();
            var significance = comparer.Compare(new CoreDataSet(), new CoreDataSet(), new IndicatorMetadata());
            Assert.AreEqual(significance, Significance.None);
        }
    }
}
