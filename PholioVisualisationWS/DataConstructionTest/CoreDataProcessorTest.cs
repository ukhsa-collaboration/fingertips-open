using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class CoreDataProcessorTest
    {
        private const double Number = 1.111111111;
        private const double TruncatedNumber = 1.1111;

        [TestMethod]
        public void TestFormatCIs()
        {
            var data = new CoreDataSet
            {
                UpperCI = 2.222222222,
                LowerCI = Number
            };
            CoreDataProcessor().FormatAndTruncate(data);
            Assert.AreEqual("1.11", data.LowerCIF);
            Assert.AreEqual("2.22", data.UpperCIF);
        }

        [TestMethod]
        public void TestHasBeenTruncatedIsSetAfterTruncate()
        {
            var data = new CoreDataSet { Value = Number };
            Assert.IsFalse(data.HasBeenTruncated);
            TruncateData(data);
            Assert.IsTrue(data.HasBeenTruncated);
        }

        [TestMethod]
        public void TestTruncateCount()
        {
            var data = new CoreDataSet { Count = Number };
            TruncateData(data);
            Assert.AreEqual(TruncatedNumber, data.Count);
        }

        [TestMethod]
        public void TestTruncateLowerCI()
        {
            var data = new CoreDataSet { LowerCI = Number };
            TruncateData(data);
            Assert.AreEqual(TruncatedNumber, data.LowerCI);
        }

        [TestMethod]
        public void TestTruncateUpperCI()
        {
            var data = new CoreDataSet { UpperCI = Number };
            TruncateData(data);
            Assert.AreEqual(TruncatedNumber, data.UpperCI);
        }

        [TestMethod]
        public void TestTruncateDenominator2()
        {
            var data = new CoreDataSet { Denominator2 = Number };
            TruncateData(data);
            Assert.AreEqual(TruncatedNumber, data.Denominator2);
        }

        [TestMethod]
        public void TestFormatAndTruncateCoreDataList()
        {
            var data = new CoreDataSet { Value = Number };
            CoreDataProcessor().FormatAndTruncateList(new List<CoreDataSet> { data });
            AssertValueTrucatedAsExpected(data);
            Assert.AreEqual("1.11", data.ValueFormatted);
        }

        [TestMethod]
        public void TestFormatAndTruncateCoreData()
        {
            var data = new CoreDataSet { Value = Number };
            CoreDataProcessor().FormatAndTruncate(data);
            Assert.AreEqual("1.11", data.ValueFormatted);
            AssertValueTrucatedAsExpected(data);
        }

        [TestMethod]
        public void TestFormatAndTruncateValueDataIngoresNull()
        {
            CoreDataProcessor().FormatAndTruncate((CoreDataSet)null);
        }

        [TestMethod]
        public void TestTruncateCoreData()
        {
            var data = new CoreDataSet { Value = Number };
            CoreDataProcessor().Truncate(data);
            AssertValueTrucatedAsExpected(data);
        }

        [TestMethod]
        public void TestTruncateCoreDataList()
        {
            var data = new CoreDataSet { Value = Number };
            CoreDataProcessor().TruncateList(new List<CoreDataSet> { data });
            AssertValueTrucatedAsExpected(data);
        }

        private static void AssertValueTrucatedAsExpected(CoreDataSet data)
        {
            Assert.AreEqual(TruncatedNumber, data.Value);
        }

        private static void TruncateData(CoreDataSet data)
        {
            var processor = new CoreDataProcessor(new Moq.Mock<DefaultFormatter>().Object);
            processor.FormatAndTruncate(data);
        }

        private static CoreDataProcessor CoreDataProcessor()
        {
            return new CoreDataProcessor(Formatter());
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
