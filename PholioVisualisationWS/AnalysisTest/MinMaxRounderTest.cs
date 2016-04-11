
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;

namespace PholioVisualisation.AnalysisTest
{
    [TestClass]
    public class MinMaxRounderTest
    {
        [TestMethod]
        public void TestHandleEqualValues()
        {
            MinMaxRounder rounder = new MinMaxRounder(1,1);
            Assert.AreEqual(1, rounder.Limits.Min);
            Assert.AreEqual(1, rounder.Limits.Max);
        }

        [TestMethod]
        public void TestGetStats1000()
        {
            MinMaxRounder rounder = new MinMaxRounder(450, 950);
            Assert.AreEqual(400, rounder.Limits.Min);
            Assert.AreEqual(1000, rounder.Limits.Max);
        }

        [TestMethod]
        public void TestGetStats500()
        {
            MinMaxRounder rounder = new MinMaxRounder(60, 340);
            Assert.AreEqual(0, rounder.Limits.Min);
            Assert.AreEqual(400, rounder.Limits.Max);
        }

        [TestMethod]
        public void TestGetStats200()
        {
            MinMaxRounder rounder = new MinMaxRounder(35, 190);
            Assert.AreEqual(0, rounder.Limits.Min);
            Assert.AreEqual(200, rounder.Limits.Max);
        }

        [TestMethod]
        public void TestGetStats100()
        {
            MinMaxRounder rounder = new MinMaxRounder(78.2, 87.2);
            Assert.AreEqual(78, rounder.Limits.Min);
            Assert.AreEqual(88, rounder.Limits.Max);
        }

        [TestMethod]
        public void TestCase50sFor350()
        {
            MinMaxRounder rounder = new MinMaxRounder(131.8, 406.1);
            Assert.AreEqual(100, rounder.Limits.Min);
            Assert.AreEqual(500, rounder.Limits.Max);
        }

        [TestMethod]
        public void TestGetStats20()
        {
            MinMaxRounder rounder = new MinMaxRounder(54, 69);
            Assert.AreEqual(50, rounder.Limits.Min);
            Assert.AreEqual(70, rounder.Limits.Max);
        }

        [TestMethod]
        public void TestGetStats0_05()
        {
            MinMaxRounder rounder = new MinMaxRounder(0.022, 0.044);
            Assert.AreEqual(0.02, rounder.Limits.Min);
            Assert.AreEqual(0.05, rounder.Limits.Max);
        }

        [TestMethod]
        public void TestGetStats0_1()
        {
            MinMaxRounder rounder = new MinMaxRounder(0.11, 0.89);
            Assert.AreEqual(0, rounder.Limits.Min);
            Assert.AreEqual(1.0, rounder.Limits.Max);
        }

        [TestMethod]
        public void TestGetStatsNegativeMin10()
        {
            MinMaxRounder rounder = new MinMaxRounder(-7, 7);
            Assert.AreEqual(-10, rounder.Limits.Min);
            Assert.AreEqual(10, rounder.Limits.Max);
        }

        [TestMethod]
        public void TestGetStatsNegativeMax0()
        {
            MinMaxRounder rounder = new MinMaxRounder(-9, -1);
            Assert.AreEqual(-10, rounder.Limits.Min);
            Assert.AreEqual(0, rounder.Limits.Max);
        }

        [TestMethod]
        public void TestGetStatsNegativeMin20()
        {
            MinMaxRounder rounder = new MinMaxRounder(-19, -11);
            Assert.AreEqual(-20, rounder.Limits.Min);
            Assert.AreEqual(-10, rounder.Limits.Max);
        }

        [TestMethod]
        public void TestGetStatsNegativeMin1000()
        {
            MinMaxRounder rounder = new MinMaxRounder(-896, 759);
            Assert.AreEqual(-1000, rounder.Limits.Min);
            Assert.AreEqual(1000, rounder.Limits.Max);
        }
    }
}
