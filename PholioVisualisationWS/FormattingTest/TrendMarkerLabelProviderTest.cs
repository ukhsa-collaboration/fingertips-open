using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.FormattingTest
{
    [TestClass]
    public class TrendMarkerLabelProviderTest
    {
        [TestMethod]
        public void Test_NotApplicable()
        {
            Assert.AreEqual("Increasing", GetLabel(PolarityIds.NotApplicable,TrendMarker.Increasing));
            Assert.AreEqual("Decreasing", GetLabel(PolarityIds.NotApplicable,TrendMarker.Decreasing));
            Assert.AreEqual("No significant change", GetLabel(PolarityIds.NotApplicable,TrendMarker.NoChange));
            Assert.AreEqual("Cannot be calculated", GetLabel(PolarityIds.NotApplicable,TrendMarker.CannotBeCalculated));
        }

        [TestMethod]
        public void Test_Blue_Orange_Blue()
        {
            Assert.AreEqual("Increasing", GetLabel(PolarityIds.BlueOrangeBlue, TrendMarker.Increasing));
        }

        [TestMethod]
        public void Test_Rag_Low_Is_Good()
        {
            Assert.AreEqual("Increasing and getting worse", GetLabel(PolarityIds.RagLowIsGood, TrendMarker.Increasing));
        }

        [TestMethod]
        public void Test_Rag_High_Is_Good()
        {
            Assert.AreEqual("Increasing and getting better", GetLabel(PolarityIds.RagHighIsGood, TrendMarker.Increasing));
        }

        public string GetLabel(int polarityId, TrendMarker trendMarker)
        {
            return new TrendMarkerLabelProvider(polarityId).GetLabel(trendMarker).Text;
        }
    }
}
