using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PholioObjectsTest
{
    [TestClass]
    public class AreaAddressTest
    {
        [TestMethod]
        public void TestCleanAddressOfTrailingCommas()
        {
            AreaAddress address = new AreaAddress
            {
                Address1 = "a1,",
                Address2 = "a2,",
                Address3 = "a3,",
                Address4 = "a4,",
                Postcode = "cb1,"
            };

            address.CleanAddress();

            AssertAddressCleaned(address);
        }

        [TestMethod]
        public void TestCleanAddressOfLeadingAndTrailingSpaces()
        {
            AreaAddress address = new AreaAddress
            {
                Address1 = " a1 ",
                Address2 = " a2 ",
                Address3 = " a3 ",
                Address4 = " a4 ",
                Postcode = " cb1 "
            };

            address.CleanAddress();

            AssertAddressCleaned(address);
        }

        private static void AssertAddressCleaned(AreaAddress address)
        {
            Assert.AreEqual("a1", address.Address1);
            Assert.AreEqual("a2", address.Address2);
            Assert.AreEqual("a3", address.Address3);
            Assert.AreEqual("a4", address.Address4);
            Assert.AreEqual("cb1", address.Postcode);
        }

    }
}
