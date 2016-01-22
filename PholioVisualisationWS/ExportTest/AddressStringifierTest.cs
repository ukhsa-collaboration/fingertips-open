using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Export;
using PholioVisualisation.PholioObjects;

namespace ExportTest
{
    [TestClass]
    public class AddressStringifierTest
    {
        private string[] ignoreAddressParts = new[] { null, string.Empty, " " };

        [TestMethod]
        public void TestGetAddressWithoutPostcode()
        {
            ExpectString(AreaAddress(), "a, b, c, d");
        }

        [TestMethod]
        public void TestNullAddress1IsOmitted()
        {
            foreach (var ignoreAddressPart in ignoreAddressParts)
            {
                var address = AreaAddress();
                address.Address1 = ignoreAddressPart;
                ExpectString(address, "b, c, d");
            }
        }

        [TestMethod]
        public void TestNullAddress2IsOmitted()
        {
            foreach (var ignoreAddressPart in ignoreAddressParts)
            {
                var address = AreaAddress();
                address.Address2 = ignoreAddressPart;
                ExpectString(address, "a, c, d");
            }
        }

        [TestMethod]
        public void TestNullAddress3IsOmitted()
        {
            foreach (var ignoreAddressPart in ignoreAddressParts)
            {
                var address = AreaAddress();
                address.Address3 = ignoreAddressPart;
                ExpectString(address, "a, b, d");
            }
        }

        [TestMethod]
        public void TestNullAddress4IsOmitted()
        {
            foreach (var ignoreAddressPart in ignoreAddressParts)
            {
                var address = AreaAddress();
                address.Address4 = ignoreAddressPart;
                ExpectString(address, "a, b, c");
            }
        }

        private static void ExpectString(AreaAddress address, string expectedAddressString)
        {
            var addressString = new AddressStringifier(address).AddressWithoutPostcode;
            Assert.AreEqual(expectedAddressString, addressString);
        }

        private static AreaAddress AreaAddress()
        {
            var address = new AreaAddress
            {
                Address1 = "a",
                Address2 = "b",
                Address3 = "c",
                Address4 = "d"
            };
            return address;
        }
    }
}
