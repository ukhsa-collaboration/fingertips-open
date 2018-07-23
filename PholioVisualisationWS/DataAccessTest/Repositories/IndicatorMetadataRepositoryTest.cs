using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccessTest.Repositories
{
    [TestClass]
    public class IndicatorMetadataRepositoryTest
    {
        private readonly DateTime _date = DateTime.Now.AddDays(-100);

        [TestMethod]
        public void Test_GetIndicatorsDeletedSinceDate()
        {
            var repo = new IndicatorMetadataRepository();
            var ids = repo.GetIndicatorsDeletedSinceDate(_date);
            Assert.IsTrue(ids.Any());
        }

        [TestMethod]
        public void Test_GetIndicatorsUploadedSinceDate()
        {
            var repo = new IndicatorMetadataRepository();
            var ids = repo.GetIndicatorsUploadedSinceDate(_date);
            Assert.IsTrue(ids.Any());
        }

        [TestMethod]
        public void Test_GetIndicatorNames()
        {
            var repo = new IndicatorMetadataRepository();
            var names = repo.GetIndicatorNames(
                new List<int> { IndicatorIds.AdultSmokingRelatedDeaths});
            Assert.IsTrue(names.First().Name.ToLower().Contains("smoking"));
        }

        [TestMethod]
        public void Test_GetIndicatorNames_Return_Empty_List_For_No_Ids()
        {
            var repo = new IndicatorMetadataRepository();
            var names = repo.GetIndicatorNames(
                new List<int> ());
            Assert.IsFalse(names.Any());
        }

        [TestMethod]
        public void GetIndicatorMetadataSearchExpectations()
        {
            var repo = new IndicatorMetadataRepository();
            var expectations = repo.GetIndicatorMetadataSearchExpectations("smoking");
            Assert.IsTrue(expectations.Any());
        }
    }
}
