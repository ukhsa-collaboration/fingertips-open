using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioObjectsTest
{
    [TestClass]
    public class ValueWithCIsDataTest
    {
        [TestMethod]
        public void TestParse()
        {
            var data = ValueWithCIsData.Parse("1,2,3");

            Assert.AreEqual(1, data.Value);
            Assert.AreEqual(2, data.LowerCI);
            Assert.AreEqual(3, data.UpperCI);
        }

        [TestMethod]
        public void TestParse_NoValueRepresentedAsHyphen()
        {
            var data = ValueWithCIsData.Parse("-,-,-");

            Assert.AreEqual(ValueData.NullValue, data.Value);
            Assert.AreEqual(ValueData.NullValue, data.LowerCI);
            Assert.AreEqual(ValueData.NullValue, data.UpperCI);
        }

        [TestMethod]
        public void TestParse_NoValueRepresentedAsEmptySpace()
        {
            var data = ValueWithCIsData.Parse(",,");

            Assert.AreEqual(ValueData.NullValue, data.Value);
            Assert.AreEqual(ValueData.NullValue, data.LowerCI);
            Assert.AreEqual(ValueData.NullValue, data.UpperCI);
        }

        [TestMethod]
        public void TestParse_NoCIsRepresentedAsHyphens()
        {
            var data = ValueWithCIsData.Parse("1,-,-");

            Assert.AreEqual(1, data.Value);
            Assert.AreEqual(ValueData.NullValue, data.LowerCI);
            Assert.AreEqual(ValueData.NullValue, data.UpperCI);
        }

        [TestMethod]
        public void TestParse_NoCIsRepresentedAsEmptyString()
        {
            var data = ValueWithCIsData.Parse("1,,");

            Assert.AreEqual(1, data.Value);
            Assert.AreEqual(ValueData.NullValue, data.LowerCI);
            Assert.AreEqual(ValueData.NullValue, data.UpperCI);
        }

        [TestMethod]
        public void TestShouldSerializeLowerCIF_FalseIfLowerCIFNull()
        {
            var data = new CoreDataSet
            {
                LowerCI = 1
            };

            Assert.IsFalse(data.ShouldSerializeLowerCIF());
        }

        [TestMethod]
        public void TestShouldSerializeLowerCIF_FalseIfLowerCINullValue()
        {
            var data = new CoreDataSet
            {
                LowerCI = ValueData.NullValue,
                LowerCIF = "1"
            };

            Assert.IsFalse(data.ShouldSerializeLowerCIF());
        }

        [TestMethod]
        public void TestShouldSerializeLowerCIF_True()
        {
            var data = new CoreDataSet
            {
                LowerCI = 1,
                LowerCIF = "1"
            };

            Assert.IsTrue(data.ShouldSerializeLowerCIF());
        }


        [TestMethod]
        public void TestShouldLowerCISerialiseFalse()
        {
            var data = new CoreDataSet { LowerCI = ValueData.NullValue };

            Assert.IsFalse(data.ShouldSerializeLowerCI());
        }

        [TestMethod]
        public void TestShouldLowerCISerialiseTrue()
        {
            var data = new CoreDataSet { LowerCI = 1 };

            Assert.IsTrue(data.ShouldSerializeLowerCI());
        }

        [TestMethod]
        public void TestShouldSerializeUpperCIF_FalseIfUpperCIFNull()
        {
            var data = new CoreDataSet
            {
                UpperCI = 1
            };

            Assert.IsFalse(data.ShouldSerializeUpperCIF());
        }

        [TestMethod]
        public void TestShouldSerializeUpperCIF_FalseIfUpperCINullValue()
        {
            var data = new CoreDataSet
            {
                UpperCI = ValueData.NullValue,
                UpperCIF = "1"
            };

            Assert.IsFalse(data.ShouldSerializeUpperCIF());
        }

        [TestMethod]
        public void TestShouldSerializeUpperCIF_True()
        {
            var data = new CoreDataSet
            {
                UpperCI = 1,
                UpperCIF = "1"
            };

            Assert.IsTrue(data.ShouldSerializeUpperCIF());
        }

        [TestMethod]
        public void TestShouldUpperCISerialiseFalse()
        {
            var data = new CoreDataSet { UpperCI = ValueData.NullValue };

            Assert.IsFalse(data.ShouldSerializeUpperCI());
        }

        [TestMethod]
        public void TestShouldUpperCISerialiseTrue()
        {
            var data = new CoreDataSet { UpperCI = 1 };

            Assert.IsTrue(data.ShouldSerializeUpperCI());
        }

        [TestMethod]
        public void TestAreCIsValidTrue()
        {
            var data = new CoreDataSet
            {
                LowerCI = 1,
                UpperCI = 1
            };

            Assert.IsTrue(data.AreCIsValid);
        }

        [TestMethod]
        public void TestAreCIsValidFalseIfNoLowerCI()
        {
            var data = new CoreDataSet
            {
                LowerCI = ValueData.NullValue,
                UpperCI = 1
            };

            Assert.IsFalse(data.AreCIsValid);
        }

        [TestMethod]
        public void TestAreCIsValidFalseIfNoUpperCI()
        {
            var data = new CoreDataSet
            {
                LowerCI = 1,
                UpperCI = ValueData.NullValue
            };

            Assert.IsFalse(data.AreCIsValid);
        }
    }
}
