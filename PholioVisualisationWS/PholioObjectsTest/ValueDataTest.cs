
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PholioObjectsTest
{
    [TestClass]
    public class BaseDataTest
    {
        [TestMethod]
        public void TestGetBaseData()
        {
            ValueWithCIsData data = new ValueWithCIsData
            {
                Value = 1,
                LowerCI = 2,
                UpperCI = 3,
                Count = 4
            };
            ValueWithCIsData copy = data.GetValueWithCIsData();
            Assert.AreEqual(data.Value, copy.Value);
            Assert.AreEqual(data.Count, copy.Count);
            Assert.AreEqual(data.LowerCI, copy.LowerCI);
            Assert.AreEqual(data.UpperCI, copy.UpperCI);
        }

        [TestMethod]
        public void TestAreCIsValid()
        {
            Assert.IsFalse(new ValueWithCIsData
            {
                Value = 1,
                LowerCI = ValueData.NullValue,
                UpperCI = ValueData.NullValue
            }.AreCIsValid);

            Assert.IsTrue(new ValueWithCIsData
            {
                Value = 1,
                LowerCI = ValueData.NullValue,
                UpperCI = 3
            }.AreCIsValid);

            Assert.IsTrue(new ValueWithCIsData
            {
                Value = 1,
                LowerCI = 2,
                UpperCI = ValueData.NullValue
            }.AreCIsValid);

            Assert.IsTrue(new ValueWithCIsData
            {
                Value = 1,
                LowerCI = 2,
                UpperCI = 3
            }.AreCIsValid);
        }
    }
}
