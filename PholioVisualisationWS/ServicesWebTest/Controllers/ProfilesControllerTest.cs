using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using ServicesWeb.Controllers;

namespace PholioVisualisation.ServicesWebTest.Controllers
{
    [TestClass]
    public class ProfilesControllerTest
    {
        [TestMethod]
        public void TestGetProfilesPerIndicator()
        {
            var response = new ProfilesController().GetProfilesPerIndicator(
                IndicatorIds.DeprivationScoreIMD2010.ToString(), AreaTypeIds.CountyAndUnitaryAuthority);
            Assert.IsTrue(response.Count > 0);
        }
    }
}
