using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using System;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class MonthlyReleaseHelperTest
    {
        private MonthlyReleaseHelper _helper;

        [TestInitialize]
        public void Initialize()
        {
            _helper = new MonthlyReleaseHelper();
        }

        [TestMethod]
        public void TestGetLatestReleaseDate()
        {
            var latestRelaseDate = _helper.GetFollowingReleaseDate(new DateTime(2018, 3, 1));
            Assert.AreEqual(latestRelaseDate, new DateTime(2018, 3, 6));
        }
    }
}
