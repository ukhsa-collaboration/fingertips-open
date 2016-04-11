using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PholioObjectsTest
{
    [TestClass]
    public class TrendDataPointTest
    {
        [TestMethod]
        public void TestCopyConstructor()
        {
            var data = new CoreDataSet
            {
                Value = 2,
                ValueFormatted = "2",
                LowerCIF = "1",
                UpperCIF = "3",
                ValueNoteId = 4
            };

            var point = new TrendDataPoint(data);

            Assert.AreEqual(2, point.Value);
            Assert.AreEqual("2", point.ValueF);
            Assert.AreEqual("1", point.LowerCIF);
            Assert.AreEqual("3", point.UpperCIF);
            Assert.AreEqual(4, point.ValueNoteId);
        }

        [TestMethod]
        public void TestShouldValueNoteIdSerialiseFalseWhenNoValueNote()
        {
            var point = new TrendDataPoint(new CoreDataSet { ValueNoteId = CoreDataSet.NoValueNote });
            Assert.IsFalse(point.ShouldSerializeValueNoteId());
        }

        [TestMethod]
        public void TestShouldValueNoteIdSerialiseTrueWhenValueNoteDefined()
        {
            var point = new TrendDataPoint(new CoreDataSet { ValueNoteId = 100 });
            Assert.IsTrue(point.ShouldSerializeValueNoteId());
        }

    }
}
