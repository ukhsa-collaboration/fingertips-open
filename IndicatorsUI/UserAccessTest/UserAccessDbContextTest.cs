using System;
using System.Collections.Generic;
using System.Linq;
using IndicatorsUI.UserAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IndicatorsUI.UserAccessTest
{
    [TestClass]
    public class UserAccessDbContextTest
    {
        [TestMethod]
        public void TestHasEmailAddressAlreadyBeenRegistered_False_For_Non_Existant_Email()
        {
            Assert.IsFalse(new UserAccessDbContext().HasEmailAddressAlreadyBeenRegistered("a"));
        }
    }
}
