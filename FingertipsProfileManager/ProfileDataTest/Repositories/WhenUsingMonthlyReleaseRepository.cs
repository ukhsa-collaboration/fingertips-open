using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.ProfileDataTest.Repositories
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