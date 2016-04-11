using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.AnalysisTest
{
    [TestClass]
    public class QuintilesComparerTest
    {
        [TestMethod]
        public void Test_Category0IfNoDataSet()
        {
            var comparer = new QuintilesComparer();
            Assert.AreEqual(0, comparer.GetCategory(Data()));
        }

        [TestMethod]
        public void Test_Category0IfValueCountLessThan5()
        {
            var comparer = new QuintilesComparer();
            comparer.SetDataForCategories(new List<double> { 1, 2, 3, 4 });
            Assert.AreEqual(0, comparer.GetCategory(Data()));
        }

        [TestMethod]
        public void Test_Category0IfNoValues()
        {
            var comparer = new QuintilesComparer();
            comparer.SetDataForCategories(new List<double>());
            Assert.AreEqual(0, comparer.GetCategory(Data()));
        }

        [TestMethod]
        public void Test_Category0IfNullValueList()
        {
            var comparer = new QuintilesComparer();
            comparer.SetDataForCategories(null);
            Assert.AreEqual(0, comparer.GetCategory(Data()));
        }

        [TestMethod]
        public void Test_GetCategories()
        {
            var comparer = new QuintilesComparer();
            comparer.SetDataForCategories(DataList1To10());
            Assert.AreEqual(1, comparer.GetCategory(new CoreDataSet { Value = 1 }));
            Assert.AreEqual(5, comparer.GetCategory(new CoreDataSet { Value = 10 }));
        }

        [TestMethod]
        public void Test_Significance0WhenDataIsNull()
        {
            var comparer = new QuintilesComparer();
            comparer.SetDataForCategories(DataList1To10());
            Assert.AreEqual(0, comparer.GetCategory(null));
        }

        private static List<double> DataList1To10()
        {
            return new List<double> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        }

        public CoreDataSet Data()
        {
            return new CoreDataSet { Value = 1 };
        }
    }
}
