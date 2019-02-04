using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServicesWeb.Controllers;

namespace PholioVisualisation.ServicesWebTest.Controllers
{
    [TestClass]
    public class ProfilesControllerTest
    {
        [TestMethod]
        public void TestGetProfilesPerIndicatorWithAreaTypeId()
        {
            var response = new ProfilesController().GetProfilesPerIndicator(
                IndicatorIds.Aged0To4Years.ToString(), AreaTypeIds.GpPractice);
            Assert.IsTrue(response.Count > 0);
        }

        [TestMethod]
        public void TestGetProfilesPerIndicator()
        {
            var response = new ProfilesController().GetProfilesPerIndicator(
                IndicatorIds.Aged0To4Years.ToString());
            Assert.IsTrue(response.Count > 0);
        }

        [TestMethod]
        public void TestGetAreaTypesWithPdfsForProfile()
        {
            var areaTypes = new ProfilesController().GetAreaTypesWithPdfsForProfile(ProfileIds.ChildAndMaternalHealth);

            Assert.AreEqual(AreaTypeIds.CountyAndUnitaryAuthority, areaTypes.First().Id);
        }
    }
}
