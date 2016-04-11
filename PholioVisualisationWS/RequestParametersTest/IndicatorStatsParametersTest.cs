using System;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.RequestParametersTest
{
    [TestClass]
    public class IndicatorStatsParametersTest
    {
        [TestMethod]
        public void TestIndicatorStatsType()
        {
            // Control limit
            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add(ParameterNames.Type, "cl");
            IndicatorStatsParameters parameters = new IndicatorStatsParameters(nameValues);
            Assert.AreEqual(IndicatorStatsType.ControlLimits, parameters.IndicatorStatsType);

            // Percentile
            nameValues = new NameValueCollection();
            nameValues.Add(ParameterNames.Type, "pc");
            parameters = new IndicatorStatsParameters(nameValues);
            Assert.AreEqual(IndicatorStatsType.Percentiles25And75, parameters.IndicatorStatsType);

            // Percentile default
            parameters = new IndicatorStatsParameters(new NameValueCollection());
            Assert.AreEqual(IndicatorStatsType.Percentiles25And75, parameters.IndicatorStatsType);

        }
    }
}
