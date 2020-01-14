
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccessTest
{
    [TestClass]
    public class PholioLabelReaderTest
    {
        [TestMethod]
        public void TestLookUpAgeLabel()
        {
            PholioLabelReader reader = new PholioLabelReader();
            Assert.AreEqual("All ages", reader.LookUpAgeLabel(AgeIds.AllAges));
        }

        [TestMethod]
        public void TestLookUpYearTypeLabel()
        {
            PholioLabelReader reader = new PholioLabelReader();
            Assert.AreEqual("Academic", reader.LookUpYearTypeLabel(YearTypeIds.Academic));
        }

        [TestMethod]
        public void TestLookUpSexLabel()
        {
            PholioLabelReader reader = new PholioLabelReader();
            Assert.AreEqual("Persons", reader.LookUpSexLabel(SexIds.Persons));
        }

        [TestMethod]
        public void TestLookUpComparatorMethodLabel()
        {
            PholioLabelReader reader = new PholioLabelReader();
            string label = "Overlapping confidence intervals (95.0)";
            Assert.AreEqual(label, reader.LookUpComparatorMethodLabel(ComparatorMethodIds.DoubleOverlappingCIs));
            Assert.AreNotEqual(label, reader.LookUpComparatorMethodLabel(ComparatorMethodIds.Quintiles));
        }
    }
}
