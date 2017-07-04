﻿using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FingertipsUploadService.ProfileDataTest.Respositories
{
    [TestClass]
    public class WhenUsingAreaTypeRepository
    {
        [TestMethod]
        public void TestShouldWarnAboutSmallNumbersForCountyUa()
        {
            var should = new AreaTypeRepository().ShouldWarnAboutSmallNumbersForArea(AreaCodes.CountyUa_Bedfordshire);
            Assert.IsTrue(should);
        }
    }
}
