using System;
using System.Collections.Generic;
using System.Linq;
using Ckan.DataTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace CkanTest.DataTransformation
{
    [TestClass]
    public class CkanFrequencyTest
    {
        [TestMethod]
        public void TestAnnually()
        {
            var timePeriod = new TimePeriod
            {
                Year = 2001
            };

            Assert.AreEqual(CkanFrequency.Annually, new CkanFrequency(timePeriod).Frequency);
        }

        [TestMethod]
        public void TestQuarterly()
        {
            var timePeriod = new TimePeriod
            {
                Year = 2001,
                Quarter = 1
            };

            Assert.AreEqual(CkanFrequency.Quarterly, new CkanFrequency(timePeriod).Frequency);
        }

        [TestMethod]
        public void TestMonthly()
        {
            var timePeriod = new TimePeriod
            {
                Year = 2001,
                Month = 1
            };

            Assert.AreEqual(CkanFrequency.Monthly, new CkanFrequency(timePeriod).Frequency);
        }
    }
}
