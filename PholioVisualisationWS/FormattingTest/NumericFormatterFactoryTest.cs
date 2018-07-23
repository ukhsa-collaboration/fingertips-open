using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.FormattingTest
{
    [TestClass]
    public class NumericFormatterByNationalLimitsTest
    {
        private IndicatorMetadata metadata;
        private NumericFormatter formatter;

        [TestMethod]
        public void TestValuesAreWithinExpectedLimits()
        {
            GivenAnIndicatorWhereMaximumValueis100();
            WhenNumericFormatterIsCreated();
            ThenValuesAreFormattedTo1DecimalPlace();
        }

        private void WhenNumericFormatterIsCreated()
        {
            var groupDataReader = ReaderFactory.GetGroupDataReader();
            formatter = new NumericFormatterFactory(groupDataReader).New(metadata);
        }

        private void ThenValuesAreFormattedTo1DecimalPlace()
        {
            Assert.IsNotNull(formatter);

            var data = new ValueData { Value = 96.4444 };
            formatter.Format(data);
            Assert.AreEqual("96.4", data.ValueFormatted);

            data = new ValueData { Value = 96.5555 };
            formatter.Format(data);
            Assert.AreEqual("96.6", data.ValueFormatted);
        }

        private void GivenAnIndicatorWhereMaximumValueis100()
        {
            metadata = new IndicatorMetadata
                 {
                     IndicatorId = IndicatorIds.QofPoints
                 };
        }
    }
}
