using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.AnalysisTest
{
    [TestClass]
    public class SuicidePreventPlanComparerTest
    {
        private SuicidePreventPlanComparer _comparer = new SuicidePreventPlanComparer();

        [TestMethod]
        public void When_Value_Is_Invalid_Returns_No_Significance()
        {
            var data = new CoreDataSet { Value = ValueData.NullValue };
            Assert.AreEqual(Significance.None, _comparer.Compare(data, null, null));
        }

        [TestMethod]
        public void When_Value_Is_Valid_Returns_Data_Value()
        {
            var data = new CoreDataSet { Value = SuicidePlanStatus.None };
            Assert.AreEqual(Significance.Worse, _comparer.Compare(data, null, null));
        }
    }
}
