using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.Search;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SearchTest
{
    [TestClass]
    public class DistinctGroupComparerTest
    {
        [TestMethod]
        public void TestDistinct()
        {
            var grouping2 = GetGrouping();
            grouping2.IndicatorId = 10;

            var groupings = new List<Grouping>
            {
                GetGrouping(),
                grouping2,
                GetGrouping()
            };
            Assert.AreEqual(2, groupings.Distinct(new DistinctGroupComparer()).Count());
        }

        public Grouping GetGrouping()
        {
            return new Grouping
            {
                GroupId = 1,
                IndicatorId = 2,
                AgeId = 3,
                SexId = 4,
                AreaTypeId = 5,
                YearRange = 6
            };
        }
    }
}
