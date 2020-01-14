using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.MainUI.Helpers;
using Fpm.ProfileData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.MainUITest.Helpers
{
    [TestClass]
    public class TimePeriodHelperTest
    {
        [TestMethod]
        public void TestTimePeriodStrings()
        {
            var grouping = new GroupingPlusName
            {
                YearRange = 1,
                YearTypeId = YearTypeIds.Calendar,
                BaselineMonth = TimePeriod.Undefined,
                BaselineQuarter = TimePeriod.Undefined,
                BaselineYear = 2002,
                DatapointMonth = TimePeriod.Undefined,
                DatapointQuarter = TimePeriod.Undefined,
                DatapointYear = 2005
            };

            var timePeriodHelper = new TimePeriodHelper(grouping);
            Assert.AreEqual("2002", timePeriodHelper.GetBaselineString());
            Assert.AreEqual("2005", timePeriodHelper.GetDatapointString());
        }

        [TestMethod]
        public void TestGetLatestAvailableDatapoint()
        {
            var grouping = new GroupingPlusName
            {
                IndicatorId = IndicatorIds.ObesityYear6,
                SexId= SexIds.Persons,
                AgeId = AgeIds.Years10To11,
                DatapointYear = 2001,
                DatapointQuarter = TimePeriod.Undefined,
                DatapointMonth = TimePeriod.Undefined,
                YearRange = 1,
                AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPre2019
            };

            var timePeriodHelper = new TimePeriodHelper(grouping);
            Assert.IsTrue(timePeriodHelper.GetPeriodIfLaterThanDatapoint().Year > 2001);
        }
    }
}
