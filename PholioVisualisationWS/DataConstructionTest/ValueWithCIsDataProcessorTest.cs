using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class ValueWithCIsDataProcessorTest
    {
        private const double Number = 1.111111111;
        private const double TruncatedNumber = 1.1111;

        [TestMethod]
        public void TestFormatAndTruncateDataList()
        {
            var data = new ValueWithCIsData() { Value = Number };
            DataProcessor().FormatAndTruncateList(new List<ValueWithCIsData>{data});
            AssertValueTruncatedAsExpected(data);
            Assert.AreEqual("1.11", data.ValueFormatted);
        }

        [TestMethod]
        public void TestFormatCIs()
        {
            var data = new ValueWithCIsData
            {
                UpperCI95 = 2.222222222,
                LowerCI95 = Number
            };
            DataProcessor().FormatAndTruncate(data);
            Assert.AreEqual("1.11", data.LowerCI95F);
            Assert.AreEqual("2.22", data.UpperCI95F);
        }

        [TestMethod]
        public void TestTruncateLowerCI()
        {
            var data = new ValueWithCIsData { LowerCI95 = Number };
            TruncateData(data);
            Assert.AreEqual(TruncatedNumber, data.LowerCI95);
        }

        [TestMethod]
        public void TestTruncateUpperCI()
        {
            var data = new ValueWithCIsData { UpperCI95 = Number };
            TruncateData(data);
            Assert.AreEqual(TruncatedNumber, data.UpperCI95);
        }

        [TestMethod]
        public void TestFormatAndTruncateData()
        {
            var data = new CoreDataSet { Value = Number };
            DataProcessor().FormatAndTruncate(data);
            Assert.AreEqual("1.11", data.ValueFormatted);
            AssertValueTruncatedAsExpected(data);
        }

        [TestMethod]
        public void TestFormatAndTruncateValueDataIngoresNull()
        {
            DataProcessor().FormatAndTruncate((CoreDataSet)null);
        }

        [TestMethod]
        public void TestTruncateData()
        {
            var data = new ValueWithCIsData { Value = Number };
            DataProcessor().Truncate(data);
            AssertValueTruncatedAsExpected(data);
        }

        [TestMethod]
        public void TestTruncateDataList()
        {
            var data = new ValueWithCIsData { Value = Number };
            DataProcessor().TruncateList(new List<ValueWithCIsData> { data });
            AssertValueTruncatedAsExpected(data);
        }

        private static void AssertValueTruncatedAsExpected(ValueWithCIsData data)
        {
            Assert.AreEqual(1.1111, data.Value);
        }

        private static void TruncateData(ValueWithCIsData data)
        {
            var processor = new ValueWithCIsDataProcessor(new Moq.Mock<DefaultFormatter>().Object);
            processor.FormatAndTruncate(data);
        }

        private static ValueWithCIsDataProcessor DataProcessor()
        {
            return new ValueWithCIsDataProcessor(Formatter());
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
