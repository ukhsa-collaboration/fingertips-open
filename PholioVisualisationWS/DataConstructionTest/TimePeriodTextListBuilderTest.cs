using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class TimePeriodTextListBuilderTest
    {
        [TestMethod]
        public void TestEmptyListReturnedIfNoPeriodsAdded()
        {
            Assert.AreEqual(0, ListBuilder().GetTimePeriodStrings().Count);
        }

        [TestMethod]
        public void TestAdd()
        {
            var builder = ListBuilder();
            builder.Add(new TimePeriod { Year = 2000, YearRange = 1});
            builder.Add(new TimePeriod { Year = 2001, YearRange = 1 });
            Assert.AreEqual(2, builder.GetTimePeriodStrings().Count);
        }

        [TestMethod]
        public void TestAddRange()
        {
            var builder = ListBuilder();
            var periods = new List<TimePeriod>
            {
                new TimePeriod {Year = 2000, YearRange = 1},
                new TimePeriod {Year = 2001, YearRange = 1}
            };
            builder.AddRange(periods);
            Assert.AreEqual(2, builder.GetTimePeriodStrings().Count);
        }

        private static TimePeriodTextListBuilder ListBuilder()
        {
            TimePeriodTextListBuilder builder = new TimePeriodTextListBuilder(
                new IndicatorMetadata { 
                    YearTypeId = (int)YearTypeIds.Calendar
            });
            return builder;
        }
    }
}
