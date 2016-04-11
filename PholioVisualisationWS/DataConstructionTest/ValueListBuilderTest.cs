using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class ValueListBuilderTest
    {
        [TestMethod]
        public void TestNullListIngored()
        {
            var builder = new ValueListBuilder(null);
            Assert.AreEqual(0, builder.ValidValues.Count);
        }

        [TestMethod]
        public void TestValidValuesReturned()
        {
            var list = new []
                {
                    new ValueData{Value = 3},
                    new ValueData{Value = 1}
                };

            var values = new ValueListBuilder(list).ValidValues;

            Assert.AreEqual(2, values.Count);
            Assert.AreEqual(3, values[0]);
            Assert.AreEqual(1, values[1]);
        }

        [TestMethod]
        public void TestInvalidValuesNotReturned()
        {
            var list = new[]
                {
                    new ValueData{Value = ValueData.NullValue},
                    new ValueData{Value = 1}
                };

            var values = new ValueListBuilder(list).ValidValues;

            Assert.AreEqual(1, values.Count);
            Assert.AreEqual(1, values[0]);
        }

    }
}
