using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class ValueDataProcessorTest
    {
        [TestMethod]
        public void TestTruncateDoesNotDoBankersRounding()
        {
            var val = 2.5555555;
            var expectedTruncatedVal = 2.5556;
            var bankersRounding = MidpointRounding.ToEven;

            var data = new ValueData
            {
                Value = val
            };
            Processor().Truncate(data);
            Assert.AreEqual(expectedTruncatedVal, data.Value);

            // Banker's rounding doesn't round to even when >1 digit after decimal point
            Assert.AreEqual(expectedTruncatedVal, 
                Math.Round(val, DataProcessor<ValueDataProcessor>.DecimalPlaceCount, bankersRounding));

            // Different outcomes depending on midpoint rounding method
            Assert.AreEqual(2, Math.Round(2.5, 0, bankersRounding));
            Assert.AreEqual(3, Math.Round(2.5, 0, MidpointRounding.AwayFromZero));
        }

        [TestMethod]
        public void TestTruncateData()
        {
            var data = Data();
            Processor().Truncate(data);
            AssertValueTruncatedAsExpected(data);
        }

        [TestMethod]
        public void TestTruncateDataList()
        {
            var data = Data();
            Processor().TruncateList(new List<ValueData> { data });
            AssertValueTruncatedAsExpected(data);
        }

        [TestMethod]
        public void TestFormatAndTruncateData()
        {
            var data = Data();
            Processor().FormatAndTruncate(data);
            AssertValueFormattedAsExpected(data);
            AssertValueTruncatedAsExpected(data);
        }

        [TestMethod]
        public void TestFormatAndTruncateDataList()
        {
            var data = Data();
            Processor().FormatAndTruncateList(new List<ValueData> { data });
            AssertValueFormattedAsExpected(data);
            AssertValueTruncatedAsExpected(data);
        }

        private static void AssertValueFormattedAsExpected(ValueData data)
        {
            Assert.AreEqual("1.11", data.ValueFormatted);
        }

        private static void AssertValueTruncatedAsExpected(ValueData data)
        {
            Assert.AreEqual(1.1111, data.Value);
        }

        private static ValueDataProcessor Processor()
        {
            ValueDataProcessor processor = new ValueDataProcessor(Formatter());
            return processor;
        }

        private static ValueData Data()
        {
            var data = new ValueData { Value = 1.111111111 };
            return data;
        }

        private static DefaultFormatter Formatter()
        {
            var formatter = new DefaultFormatter(
                new IndicatorMetadata { ValueTypeId = (int)ValueTypeId.CrudeRate },
                new Limits
                    {
                        Min = 1,
                        Max = 2
                    }
                );
            return formatter;
        }
    }
}
