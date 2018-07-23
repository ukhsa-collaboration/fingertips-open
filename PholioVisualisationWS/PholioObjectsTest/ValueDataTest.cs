
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
                LowerCI95 = 2,
                UpperCI95 = 3,
                Count = 4
            };
            ValueWithCIsData copy = data.GetValueWithCIsData();
            Assert.AreEqual(data.Value, copy.Value);
            Assert.AreEqual(data.Count, copy.Count);
            Assert.AreEqual(data.LowerCI95, copy.LowerCI95);
            Assert.AreEqual(data.UpperCI95, copy.UpperCI95);
        }

        [TestMethod]
        public void TestAreCIsValid()
        {
            Assert.IsFalse(new ValueWithCIsData
            {
                Value = 1,
                LowerCI95 = null,
                UpperCI95 = null
            }.Are95CIsValid);

            Assert.IsFalse(new ValueWithCIsData
            {
                Value = 1,
                LowerCI95 = null,
                UpperCI95 = 3
            }.Are95CIsValid);

            Assert.IsFalse(new ValueWithCIsData
            {
                Value = 1,
                LowerCI95 = 2,
                UpperCI95 = null
            }.Are95CIsValid);

            Assert.IsTrue(new ValueWithCIsData
            {
                Value = 1,
                LowerCI95 = 2,
                UpperCI95 = 3
            }.Are95CIsValid);
        }
    }
}
