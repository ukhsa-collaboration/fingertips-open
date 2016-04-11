using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PholioObjectsTest
{
    [TestClass]
    public class TimeRangeTest
    {
        [TestMethod]
        public void TestTimeRange()
        {
            var grouping = new Grouping
            {
                YearRange = 3,
                BaselineYear = 2001,
                DataPointYear = 2003
            };

            var timeRange = new TimeRange(grouping);

            // Check first time period
            var firstTimePeriod = timeRange.FirstTimePeriod;
            Assert.AreEqual(2001, firstTimePeriod.Year);
            Assert.AreEqual(3, firstTimePeriod.YearRange);

            // Check last time period
            var lastTimePeriod = timeRange.LastTimePeriod;
            Assert.AreEqual(2003, lastTimePeriod.Year);
            Assert.AreEqual(3, lastTimePeriod.YearRange);
        }
    }
}
