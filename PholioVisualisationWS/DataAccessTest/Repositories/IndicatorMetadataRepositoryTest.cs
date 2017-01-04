using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess.Repositories;

namespace PholioVisualisation.DataAccessTest.Repositories
{
    [TestClass]
    public class IndicatorMetadataRepositoryTest
    {
        [TestMethod]
        public void TestGetIndicatorsForWhichDataHasChangedInPreviousDays()
        {
            var repo = new IndicatorMetadataRepository();
            var ids = repo.GetIndicatorsForWhichDataHasChangedInPreviousDays(30);
            Assert.IsTrue(ids.Any());
        }
    }
}
