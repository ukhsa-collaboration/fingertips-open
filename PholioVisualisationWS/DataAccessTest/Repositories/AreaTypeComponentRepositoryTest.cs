using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;
using System.Linq;

namespace PholioVisualisation.DataAccessTest.Repositories
{
    [TestClass]
    public class AreaTypeComponentRepositoryTest
    {
        [TestMethod]
        public void Test_GetAreaTypeComponents()
        {
            var components = new AreaTypeComponentRepository()
                .GetAreaTypeComponents(AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019);

            Assert.AreEqual(4, components.Count);
            Assert.IsTrue(components.Select(x => x.ComponentAreaTypeId)
                .Contains(AreaTypeIds.CountyUnchanged));
        }
    }
}
