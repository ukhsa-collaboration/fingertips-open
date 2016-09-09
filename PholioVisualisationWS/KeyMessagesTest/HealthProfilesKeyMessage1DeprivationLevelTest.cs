using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.KeyMessages;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.KeyMessagesTest
{
    [TestClass]
    public class HealthProfilesKeyMessage1DeprivationLevelTest
    {
        [TestMethod]
        public void When_County_And_Deprivation_High()
        {
            Assert.AreEqual(DeprivationLevel.High, GetDeprivationLevel(30, AreaTypeIds.County));
        }

        [TestMethod]
        public void When_County_And_Deprivation_Low()
        {
            Assert.AreEqual(DeprivationLevel.Low, GetDeprivationLevel(1, AreaTypeIds.County));
        }

        [TestMethod]
        public void When_District_And_Deprivation_High()
        {
            Assert.AreEqual(DeprivationLevel.High, GetDeprivationLevel(27, AreaTypeIds.District));
        }

        [TestMethod]
        public void When_UA_And_Deprivation_Low()
        {
            Assert.AreEqual(DeprivationLevel.Low, GetDeprivationLevel(1, AreaTypeIds.UnitaryAuthority));
        }

        private DeprivationLevel GetDeprivationLevel(double val, int areaTypeId)
        {
            return HealthProfilesKeyMessage1DeprivationLevel.GetDeprivationLevel(val,areaTypeId);
        }
    }
}
