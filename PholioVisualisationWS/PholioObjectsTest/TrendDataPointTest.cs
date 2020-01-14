using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PholioObjectsTest
{
    [TestClass]
    public class TrendDataPointTest
    {
        [TestMethod]
        public void Test_Constructor_CoreDataSet_Is_Assigned()
        {
            var data = GetCoreDataSet();
            var point = new TrendDataPoint(data);

            Assert.IsNotNull(point.CoreDataSet);
        }

        [TestMethod]
        public void TestCopyValueProperties()
        {
            var data = GetCoreDataSet();

            var point = new TrendDataPoint(data);
            point.CopyValueProperties(data);

            Assert.AreEqual(2, point.Value);
            Assert.AreEqual("2", point.ValueF);
            Assert.AreEqual(1, point.LowerCI);
            Assert.AreEqual("1", point.LowerCIF);
            Assert.AreEqual(3, point.UpperCI);
            Assert.AreEqual("3", point.UpperCIF);
            Assert.AreEqual(2, point.LowerCI99_8);
            Assert.AreEqual("2", point.LowerCIF99_8);
            Assert.AreEqual(5, point.UpperCI99_8);
            Assert.AreEqual("5", point.UpperCIF99_8);
            Assert.AreEqual(4, point.ValueNoteId);
            Assert.AreEqual(6, point.Denominator);
        }

        [TestMethod]
        public void TestShouldValueNoteIdSerialiseFalseWhenNoValueNote()
        {
            var data = new CoreDataSet { ValueNoteId = ValueNoteIds.NoNote };
            var point = new TrendDataPoint(data);

            point.CopyValueProperties(data);

            Assert.IsFalse(point.ShouldSerializeValueNoteId());
        }

        [TestMethod]
        public void TestShouldValueNoteIdSerialiseTrueWhenValueNoteDefined()
        {
            var data = new CoreDataSet {ValueNoteId = ValueNoteIds.ValueAggregatedFromAllKnownGeographyValues};
            var point = new TrendDataPoint(data);

            point.CopyValueProperties(data);

            Assert.IsTrue(point.ShouldSerializeValueNoteId());
        }

        private static CoreDataSet GetCoreDataSet()
        {
            var data = new CoreDataSet
            {
                Value = 2,
                ValueFormatted = "2",
                LowerCI95 = 1,
                LowerCI95F = "1",
                UpperCI95 = 3,
                UpperCI95F = "3",
                LowerCI99_8 = 2,
                LowerCI99_8F = "2",
                UpperCI99_8 = 5,
                UpperCI99_8F = "5",
                ValueNoteId = 4,
                Denominator = 6
            };
            return data;
        }
    }
}
