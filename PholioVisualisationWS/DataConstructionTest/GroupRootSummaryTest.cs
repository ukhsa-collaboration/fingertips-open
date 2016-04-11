using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class GroupRootSummaryTest
    {
        [TestMethod]
        public void TestCompare()
        {
            var summary1 = Summary("a");
            var summary2 = Summary("b");

            var summaries = new List<GroupRootSummary>
            {
                summary2,summary1
            };

            summaries.Sort();
            Assert.AreEqual("a",summaries.First().IndicatorName);
        }

        public static GroupRootSummary Summary(string name)
        {
            return new GroupRootSummary
            {
                IndicatorName = name
            };
        }
    }
}
