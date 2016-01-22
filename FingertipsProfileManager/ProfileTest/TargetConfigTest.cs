using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProfileDataTest
{
    [TestClass]
    public class TargetConfigTest
    {
        [TestMethod]
        public void TestIsBespokeTarget()
        {
            var target = new TargetConfig
            {
                BespokeTargetKey = "a"
            };
            Assert.IsTrue(target.IsBespokeTarget);

            Assert.IsFalse(new TargetConfig().IsBespokeTarget);
        }

        [TestMethod]
        public void TestGetDisplayValueMethods()
        {
            var target = new TargetConfig();
            Assert.AreEqual("n/a", target.GetLowerLimitDisplayValue());
            Assert.AreEqual("n/a", target.GetUpperLimitDisplayValue());

            target = new TargetConfig
            {
                LowerLimit = 1.1,
                UpperLimit = 2.2
            };
            Assert.AreEqual("1.1", target.GetLowerLimitDisplayValue());
            Assert.AreEqual("2.2", target.GetUpperLimitDisplayValue());
        }
    }
}
