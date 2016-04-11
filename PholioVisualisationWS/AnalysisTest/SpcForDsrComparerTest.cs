
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
    public class SpcForDsrComparerTest
    {
        private static SpcForDsrComparer New95_0()
        {
            return new SpcForDsrComparer { 
                ConfidenceVariable = 0.025,
                PolarityId = PolarityIds.RagLowIsGood
            };
        }

        private static SpcForDsrComparer New99_8()
        {
            return new SpcForDsrComparer {
                ConfidenceVariable = 0.001,
                PolarityId = PolarityIds.RagLowIsGood
            };
        }

        [TestMethod]
        public void TestCompare()
        {
            // Values taken from IndicatorId 501, MRSA per 100,000 population, 2011
            CoreDataSet comparator = new CoreDataSet { Value = 2.132708657734625 };
            IndicatorMetadata metadata =
            new IndicatorMetadata { Unit = new Unit { Value = 100000 } };

            // 5PP - Cams PCT
            CoreDataSet data = new CoreDataSet
            {
                Value = 1.1358436559886544,
                LowerCI = 0.4566680649368694,
                UpperCI = 2.34027204390037,
                Denominator2 = 616282,
                YearRange = 1
            };
            Assert.AreEqual(Significance.Better, New95_0().Compare(data, comparator, metadata));
            Assert.AreEqual(Significance.Same, New99_8().Compare(data, comparator, metadata));

            // 5PT - Suffolk PCT
            data = new CoreDataSet
            {
                Value = 2.657913272289925,
                LowerCI = 1.5192270945178816,
                UpperCI = 4.3162846329446163,
                Denominator2 = 601976,
                YearRange = 1
            };
            Assert.AreEqual(Significance.Same, New95_0().Compare(data, comparator, metadata));
            Assert.AreEqual(Significance.Same, New99_8().Compare(data, comparator, metadata));

            // 5PG - Birmingham and East PCT
            data = new CoreDataSet
            {
                Value = 4.1515156695295845,
                LowerCI = 2.4184108857503652,
                UpperCI = 6.6469787446568454,
                Denominator2 = 409489,
                YearRange = 1
            };
            Assert.AreEqual(Significance.Worse, New95_0().Compare(data, comparator, metadata));
            Assert.AreEqual(Significance.Same, New99_8().Compare(data, comparator, metadata));
        }

    }
}
