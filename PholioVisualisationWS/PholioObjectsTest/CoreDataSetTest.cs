
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioObjectsTest
{
    [TestClass]
    public class CoreDataSetTest
    {
        [TestMethod]
        public void TestIsCountValid()
        {
            Assert.IsFalse(new CoreDataSet { Count = null }.IsCountValid);
            Assert.IsFalse(new CoreDataSet { Count = ValueData.NullValue }.IsCountValid);
            Assert.IsTrue(new CoreDataSet { Count = 0 }.IsCountValid);
            Assert.IsTrue(new CoreDataSet { Count = -2 }.IsCountValid);
            Assert.IsTrue(new CoreDataSet { Count = 1 }.IsCountValid);
        }

        [TestMethod]
        public void TestIsDenominatorValid()
        {
            Assert.IsFalse(new CoreDataSet { Denominator = ValueData.NullValue }.IsDenominatorValid);
            Assert.IsFalse(new CoreDataSet { Denominator = 0 }.IsDenominatorValid);
            Assert.IsTrue(new CoreDataSet { Denominator = 1 }.IsDenominatorValid);
        }

        [TestMethod]
        public void TestIsDenominator2Valid()
        {
            Assert.IsFalse(new CoreDataSet { Denominator2 = ValueData.NullValue }.IsDenominator2Valid);
            Assert.IsFalse(new CoreDataSet { Denominator2 = 0 }.IsDenominator2Valid);
            Assert.IsTrue(new CoreDataSet { Denominator2 = 1 }.IsDenominator2Valid);
        }

        [TestMethod]
        public void TestShouldValueNoteIdSerialise()
        {
            Assert.IsFalse(new CoreDataSet { ValueNoteId = CoreDataSet.NoValueNote }.ShouldSerializeValueNoteId());
            Assert.IsTrue(new CoreDataSet { ValueNoteId = 100 }.ShouldSerializeValueNoteId());
        }

        [TestMethod]
        public void TestShouldDenominator2Serialise()
        {
            Assert.IsFalse(new CoreDataSet { Denominator2 = ValueData.NullValue }.ShouldSerializeDenominator2());
            Assert.IsTrue(new CoreDataSet { Denominator2 = 1 }.ShouldSerializeDenominator2());
        }

        [TestMethod]
        public void TestCountDividedByYearRange()
        {
            Assert.AreEqual(0.5, new CoreDataSet { Count = 1, YearRange = 2}.CountPerYear);
        }

        [TestMethod]
        public void TestAddSignificanceIgnoresDuplicateAdditions()
        {
            var data = new CoreDataSet();
            data.AddSignificance(1, Significance.Better);
            data.AddSignificance(1, Significance.None);
            Assert.AreEqual((int)Significance.Better, data.Significance.Values.First());
        }

        [TestMethod]
        public void TestCountDividedByYearRange_IsUndefinedIfCountIsInvalid()
        {
            Assert.AreEqual(ValueData.NullValue,
                new CoreDataSet { Count = ValueData.NullValue, YearRange = 2 }.CountPerYear);
            Assert.AreEqual(ValueData.NullValue,
                new CoreDataSet { Count = null, YearRange = 2 }.CountPerYear);
        }
    }
}
