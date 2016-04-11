using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PholioObjectsTest
{
    [TestClass]
    public class GroupingTest
    {
        [TestMethod]
        public void TestMonthAndQuarterAreInitialisedToUndefined()
        {
            var g = new Grouping();
            const int undefined = TimePoint.Undefined;
            Assert.AreEqual(undefined, g.BaselineMonth);
            Assert.AreEqual(undefined, g.BaselineQuarter);
            Assert.AreEqual(undefined, g.DataPointMonth);
            Assert.AreEqual(undefined, g.DataPointQuarter);
        }
    }
}
