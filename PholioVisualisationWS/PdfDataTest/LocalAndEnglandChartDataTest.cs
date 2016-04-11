using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PdfData;

namespace PholioVisualisation.PdfDataTest
{
    [TestClass]
    public class LocalAndEnglandChartDataTest
    {
        [TestMethod]
        public void TestCalculateLimitsGeneratesRoundedLimits()
        {
            var data = GetLocalAndEnglandChartData();
            data.CalculateLimitsAndStep(0, 10);
            var limits = data.YAxis;

            // Assert
            Assert.IsNotNull(limits);
            Assert.AreEqual(0, limits.Min);
            Assert.AreEqual(10, limits.Max);
            Assert.IsNotNull(limits.Step);
        }

        [TestMethod]
        public void TestCalculateLimitsIgnoresMinus1ForLocal()
        {
            var data = GetEmptyData();
            data.Local = new List<double> { 1, -1, 8 };
            data.CalculateLimitsAndStep(0, 10);
            Assert.IsTrue(data.YAxis.Min >= 0);
        }

        [TestMethod]
        public void TestCalculateLimitsIgnoresMinus1ForEngland()
        {
            var data = GetEmptyData();
            data.England = new List<double> { 1, -1, 8 };
            data.CalculateLimitsAndStep(0, 10);
            Assert.IsTrue(data.YAxis.Min >= 0);
        }

        public LocalAndEnglandChartData GetEmptyData()
        {
            var data = GetLocalAndEnglandChartData();
            data.Local = new List<double>();
            data.England = new List<double>();
            return data;
        }

        public static LocalAndEnglandChartData GetLocalAndEnglandChartData()
        {
            var data = new LocalAndEnglandChartData();
            Assert.IsNull(data.YAxis);
            data.England = new List<double> {1.1, 2.2};
            data.Local = new List<double> {0.1, 1.2};
            return data;
        }
    }
}
