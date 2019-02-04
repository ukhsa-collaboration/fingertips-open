using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using System;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class MonthlyReleaseHelperTest
    {
        private IMonthlyReleaseHelper _helper;

        [TestInitialize]
        public void Initialize()
        {
            _helper = new MonthlyReleaseHelper();
        }

        [TestMethod]
        public void Test_Get_Release_Date()
        {
            var releaseDate = _helper.GetReleaseDate(3);
            Assert.IsNotNull(releaseDate);

        }

        [TestMethod]
        public void Test_Are_More_Release_Dates()
        {
            var latestRelaseDate = _helper.GetFollowingReleaseDate(DateTime.Now);
        }
    }
}
