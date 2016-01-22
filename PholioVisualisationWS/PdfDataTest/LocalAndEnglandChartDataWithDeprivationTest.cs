using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PdfData;

namespace PdfDataTest
{
    [TestClass]
    public class LocalAndEnglandChartDataWithDeprivationTest
    {
        [TestMethod]
        public void TestCalculateLimitsGeneratesRoundedLimits()
        {
            var baseData = LocalAndEnglandChartDataTest.GetLocalAndEnglandChartData();
            var data = new LocalAndEnglandChartDataWithDeprivation(baseData);
            Assert.IsNull(data.YAxis);
            data.LocalLeastDeprived = new List<double> { 1.1, 7.1 };
            data.LocalMostDeprived = new List<double> { 0.1, 1.2 };
            data.CalculateLimitsAndStep(0,10);
            var limits = data.YAxis;

            // Assert
            Assert.IsNotNull(limits);
            Assert.AreEqual(0, limits.Min);
            Assert.AreEqual(10, limits.Max);
            Assert.IsNotNull(limits.Step);
        }

        [TestMethod]
        public void TestCalculateLimitsIgnoresMinus1ForLocaLeastDeprived()
        {
            var data = GetEmptyData();
            data.LocalLeastDeprived = new List<double> { 1, -1, 8 };
            data.CalculateLimitsAndStep(0, 10);
            Assert.IsTrue(data.YAxis.Min >= 0);
        }

        [TestMethod]
        public void TestCalculateLimitsIgnoresMinus1ForLocalMostDeprived()
        {
            var data = GetEmptyData();
            data.LocalMostDeprived = new List<double> { 1, -1, 8 };
            data.CalculateLimitsAndStep(0, 10);
            Assert.IsTrue(data.YAxis.Min >= 0);
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

        public LocalAndEnglandChartDataWithDeprivation GetEmptyData() 
        {
            var baseData = LocalAndEnglandChartDataTest.GetLocalAndEnglandChartData();
            var data = new LocalAndEnglandChartDataWithDeprivation(baseData);
            data.LocalLeastDeprived = new List<double>();
            data.LocalMostDeprived = new List<double>();
            data.Local = new List<double>();
            data.England = new List<double>();
            return data;
        }
    }
}
