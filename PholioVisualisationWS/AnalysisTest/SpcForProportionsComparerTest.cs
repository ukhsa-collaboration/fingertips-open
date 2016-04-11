
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.AnalysisTest
{
    [TestClass]
    public class SpcForProportionsComparerTest
    {
        private static SpcForProportionsComparer New95_0()
        {
            return new SpcForProportionsComparer
            {
                ConfidenceVariable = 1.96,
                PolarityId = PolarityIds.RagLowIsGood
            };
        }

        private static SpcForProportionsComparer New99_8()
        {
            return new SpcForProportionsComparer
            {
                ConfidenceVariable = 3.0902,
                PolarityId = PolarityIds.RagLowIsGood
            };
        }

        [TestMethod]
        public void TestDenominatorInvalid()
        {
            CoreDataSet comparator = new CoreDataSet { Value = 10 };
            IndicatorMetadata metadata = new IndicatorMetadata();

            Significance sig = New95_0().Compare(new CoreDataSet
            {
                Value = 10,
                LowerCI = 11,
                UpperCI = 12,
                Denominator = 0
            },
            comparator, metadata
            );

            Assert.AreEqual(Significance.None, sig);

            sig = New95_0().Compare(new CoreDataSet
            {
                Value = 10,
                LowerCI = 11,
                UpperCI = 12,
                Denominator = -1
            },
            comparator, metadata
            );

            Assert.AreEqual(Significance.None, sig);
        }

        [TestMethod]
        public void TestComparisionWithDifferentConfidencesWhereSignifancesSame()
        {
            // Values taken from IndicatorId=1, Year=2010 (search by value if need to retrieve)
            CoreDataSet comparator = new CoreDataSet { Value = 8.9611086876279327 };
            IndicatorMetadata metadata =
            new IndicatorMetadata { Unit = new Unit { Value = 100 } };

            //5PP
            CoreDataSet data = new CoreDataSet
            {
                Value = 7.7065767284991562,
                LowerCI = 7.0548320523366375,
                UpperCI = 8.4130813590727076,
                Denominator = 5930,
                YearRange = 1
            };
            Assert.AreEqual(Significance.Better, New95_0().Compare(data, comparator, metadata));
            Assert.AreEqual(Significance.Better, New99_8().Compare(data, comparator, metadata));

            //5PN
            data = new CoreDataSet
            {
                Value = 9.8988121425428961,
                LowerCI = 8.7379055891773572,
                UpperCI = 11.195035154784872,
                Denominator = 2273,
                YearRange = 1
            };
            Assert.AreEqual(Significance.Same, New95_0().Compare(data, comparator, metadata));
            Assert.AreEqual(Significance.Same, New99_8().Compare(data, comparator, metadata));

            //5PR
            data = new CoreDataSet
            {
                Value = 12.474645030425965,
                LowerCI = 11.088798553603118,
                UpperCI = 14.006406155616993,
                Denominator = 1972,
                YearRange = 1
            };
            Assert.AreEqual(Significance.Worse, New95_0().Compare(data, comparator, metadata));
            Assert.AreEqual(Significance.Worse, New99_8().Compare(data, comparator, metadata));
        }

        [TestMethod]
        public void TestComparisionWithDifferentConfidencesWhereSignifancesDifferent()
        {
            // Values taken from IndicatorId=514, Year=2010 (search by value if need to retrieve)
            CoreDataSet comparator = new CoreDataSet { Value = 12.8379546895717 };
            IndicatorMetadata metadata =
            new IndicatorMetadata { Unit = new Unit { Value = 100 } };

            //5PY
            CoreDataSet data = new CoreDataSet
            {
                Value = 10.9783420463032,
                LowerCI = 9.41418865607498,
                UpperCI = 12.7657535004819,
                Denominator = 1339,
                YearRange = 1
            };
            Assert.AreEqual(Significance.Better, New95_0().Compare(data, comparator, metadata));
            Assert.AreEqual(Significance.Same, New99_8().Compare(data, comparator, metadata));

            //5GC
            data = new CoreDataSet
            {
                Value = 15.9065628476085,
                LowerCI = 13.6615426206213,
                UpperCI = 18.4417082867185,
                Denominator = 899,
                YearRange = 1
            };
            Assert.AreEqual(Significance.Worse, New95_0().Compare(data, comparator, metadata));
            Assert.AreEqual(Significance.Same, New99_8().Compare(data, comparator, metadata));
        }
    }
}
