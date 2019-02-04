using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.ProfileDataTest.Respositories
{
    [TestClass]
    public class WhenUsingMonthlyReleaseRepository
    {
        [TestMethod]
        public void TestGetUpcomingMonthlyReleases()
        {
            var releases = new MonthlyReleaseRepository().GetUpcomingMonthlyReleases(3);
            Assert.AreEqual(3, releases.Count);
        }
    }
}
