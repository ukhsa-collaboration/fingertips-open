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
            var data = new CoreDataSet();
            Assert.AreEqual(Significance.None, _comparer.Compare(data, null,null));
        }

        [TestMethod]
        public void When_Value_Is_Valid_Returns_Data_Value()
        {
            var data = new CoreDataSet {Value = 3};
            Assert.AreEqual(Significance.Better, _comparer.Compare(data, null, null));
        }
    }
}
