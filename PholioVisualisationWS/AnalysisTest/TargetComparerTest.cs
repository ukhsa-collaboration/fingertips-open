using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.AnalysisTest
{
    /// <summary>
    /// Test helper methods
    /// </summary>
    [TestClass]
    public class TargetComparerTest
    {
        public static void TestNoComparisonIsMade(TargetComparer comparer)
        {
            TestNoComparisonIfPolarityNotApplicable(comparer);
            TestNoComparisonIfValueUndefined(comparer);
            TestNoComparisonIfValueNull(comparer);
        }

        public static void TestNoComparisonIfValueNull(TargetComparer comparer)
        {
            comparer.PolarityId = PolarityIds.RagLowIsGood;

            Assert.AreEqual(Significance.None, comparer.CompareAgainstTarget(null));
        }

        public static void TestNoComparisonIfValueUndefined(TargetComparer comparer)
        {
            comparer.PolarityId = PolarityIds.RagLowIsGood;

            Assert.AreEqual(Significance.None, comparer.CompareAgainstTarget(
                new CoreDataSet { Value = CoreDataSet.NullValue }));
        }

        public static void TestNoComparisonIfPolarityNotApplicable(TargetComparer comparer)
        {
            comparer.PolarityId = PolarityIds.NotApplicable;

            Assert.AreEqual(Significance.None, comparer.CompareAgainstTarget(
                new CoreDataSet { Value = 1 }));
        }

        [TestMethod]
        public void TestAddTargetSignificanceDoesNothingIfTargetSignificanceIsAlreadySet()
        {
            var comparer = new SingleValueTargetComparer(SingleValueTargetComparerTest.Config(PolarityIds.RagLowIsGood));
            var data = new CoreDataSet{Value = 1};
            data.Significance.Add(ComparatorIds.Target, (int)Significance.Better);
            TargetComparer.AddTargetSignificance(data, comparer);
        }

    }
}
