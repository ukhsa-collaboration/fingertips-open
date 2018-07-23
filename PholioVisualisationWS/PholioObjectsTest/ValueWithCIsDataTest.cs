using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PholioObjectsTest
{
    [TestClass]
    public class ValueWithCIsDataTest
    {
        [TestMethod]
        public void TestParse()
        {
            var data = ValueWithCIsData.Parse("1,2,3");

            Assert.AreEqual(1, data.Value);
            Assert.AreEqual(2, data.LowerCI95);
            Assert.AreEqual(3, data.UpperCI95);
        }

        [TestMethod]
        public void TestParse_NoValueRepresentedAsHyphen()
        {
            var data = ValueWithCIsData.Parse("-,-,-");

            Assert.AreEqual(ValueData.NullValue, data.Value);
            Assert.IsNull(data.LowerCI95);
            Assert.IsNull(data.UpperCI95);
        }

        [TestMethod]
        public void TestParse_NoValueRepresentedAsEmptySpace()
        {
            var data = ValueWithCIsData.Parse(",,");

            Assert.AreEqual(ValueData.NullValue, data.Value);
            Assert.IsNull(data.LowerCI95);
            Assert.IsNull(data.UpperCI95);
        }

        [TestMethod]
        public void TestParse_NoCIsRepresentedAsHyphens()
        {
            var data = ValueWithCIsData.Parse("1,-,-");

            Assert.AreEqual(1, data.Value);
            Assert.IsNull(data.LowerCI95);
            Assert.IsNull(data.UpperCI95);
        }

        [TestMethod]
        public void TestParse_NoCIsRepresentedAsNull()
        {
            var data = ValueWithCIsData.Parse("1,,");

            Assert.AreEqual(1, data.Value);
            Assert.IsNull(data.LowerCI95);
            Assert.IsNull(data.UpperCI95);
        }

        [TestMethod]
        public void TestShouldSerializeLowerCIF_FalseIfLowerCIFNull()
        {
            var data = new CoreDataSet
            {
                LowerCI95 = 1
            };

            Assert.IsFalse(data.ShouldSerializeLowerCI95F());
        }


        [TestMethod]
        public void TestShouldSerializeLowerCIF_True()
        {
            var data = new CoreDataSet
            {
                LowerCI95 = 1,
                LowerCI95F = "1",
                UpperCI95 = 1,
                UpperCI95F = "1"
            };

            Assert.IsTrue(data.ShouldSerializeLowerCI95F());
        }

        [TestMethod]
        public void TestShouldLowerCISerialiseTrue()
        {
            var data = new CoreDataSet { LowerCI95 = 1, UpperCI95 = 2};

            Assert.IsTrue(data.ShouldSerializeLowerCI95());
        }

        [TestMethod]
        public void TestShouldSerializeUpperCIF_FalseIfUpperCIFNull()
        {
            var data = new CoreDataSet
            {
                UpperCI95 = 1
            };

            Assert.IsFalse(data.ShouldSerializeUpperCI95F());
        }

        [TestMethod]
        public void TestShouldSerializeUpperCIF_FalseIfUpperCINullValue()
        {
            var data = new CoreDataSet
            {
                LowerCI95 = 1,
                UpperCI95 = null,
                UpperCI95F = "1"
            };

            Assert.IsFalse(data.ShouldSerializeUpperCI95F());
        }

        [TestMethod]
        public void TestShouldSerializeUpperCIF_True()
        {
            var data = new CoreDataSet
            {
                LowerCI95 = 1,
                UpperCI95 = 1,
                UpperCI95F = "1"
            };

            Assert.IsTrue(data.ShouldSerializeUpperCI95F());
        }

        [TestMethod]
        public void TestShouldUpperCISerialiseTrue()
        {
            var data = new CoreDataSet { UpperCI95 = 1, LowerCI95 = 2};

            Assert.IsTrue(data.ShouldSerializeUpperCI95());
        }

        [TestMethod]
        public void TestMinusOneToleratedForOneCI()
        {
            var data = new CoreDataSet
            {
                LowerCI95 = -1,
                UpperCI95 = 1
            };

            Assert.IsTrue(data.Are95CIsValid);
        }

        [TestMethod]
        public void TestAreCIsValidTrue()
        {
            var data = new CoreDataSet
            {
                LowerCI95 = 1,
                UpperCI95 = 1
            };

            Assert.IsTrue(data.Are95CIsValid);
        }

        [TestMethod]
        public void TestAreCIsValidTrueIfLowerCIMinusOne()
        {
            var data = new CoreDataSet
            {
                LowerCI95 = -1,
                UpperCI95 = 1
            };

            Assert.IsTrue(data.Are95CIsValid);
        }

        [TestMethod]
        public void TestAreCIsValidFalseIfUpperCIMinusOne()
        {
            var data = new CoreDataSet
            {
                LowerCI95 = 1,
                UpperCI95 = -1
            };

            Assert.IsTrue(data.Are95CIsValid);
        }
    }
}
