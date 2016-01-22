
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace DataAccessTest
{
    [TestClass]
    public class PholioLabelReaderTest
    {
        [TestMethod]
        public void TestLookUpAgeLabel()
        {
            PholioLabelReader reader = new PholioLabelReader();
            Assert.AreEqual("All ages", reader.LookUpAgeLabel(1));
        }

        [TestMethod]
        public void TestLookUpYearTypeLabel()
        {
            PholioLabelReader reader = new PholioLabelReader();
            Assert.AreEqual("Academic", reader.LookUpYearTypeLabel(3));
        }

        [TestMethod]
        public void TestLookUpSexLabel()
        {
            PholioLabelReader reader = new PholioLabelReader();
            Assert.AreEqual("Persons", reader.LookUpSexLabel(4));
        }

        [TestMethod]
        public void TestLookUpComparatorMethodLabel()
        {
            PholioLabelReader reader = new PholioLabelReader();
            string s = "Overlapping confidence intervals";
            Assert.AreEqual(s, reader.LookUpComparatorMethodLabel(12));
            Assert.AreNotEqual(s, reader.LookUpComparatorMethodLabel(1));
        }
    }
}
