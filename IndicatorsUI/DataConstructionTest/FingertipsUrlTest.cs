using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Profiles.DataAccess;
using Profiles.DataConstruction;
using Profiles.DomainObjects;

namespace IndicatorsUI.DataConstructionTest
{
    [TestClass]
    public class FingertipsUrlTest
    {
        [TestMethod]
        public void TestHostAndProtocolInDevEnvironment()
        {
            var url = "http://localhost:59822/";
            var parameters = new NameValueCollection();
            parameters.Add("Environment", "test");
            parameters.Add("BridgeWsUrl", "http://localhost:59822/");
            var config = new AppConfig(parameters);
            var host = new FingertipsUrl(config, new Uri(url)).ProtocolAndHost;

            // Assert 
            Assert.AreEqual("", host);
        }

        [TestMethod]
        public void TestHostAndProtocolInTestEnvironment()
        {
            var url = "http://testprofiles.phe.org.uk/";
            var parameters = new NameValueCollection();
            parameters.Add("Environment", "test");
            parameters.Add("BridgeWsUrl", url);
            var config = new AppConfig(parameters);
            var host = new FingertipsUrl(config, new Uri(url)).ProtocolAndHost;

            // Assert 
            var skin = ReaderFactory.GetProfileReader().GetSkinFromId(SkinIds.Core);
            Assert.AreEqual("https://" + skin.TestHost, host);
        }

        [TestMethod]
        public void TestHostAndProtocolInLiveEnvironment()
        {
            // Arrange: construct protocol and host
            var parameters = new NameValueCollection();
            parameters.Add("Environment", "live");
            var config = new AppConfig(parameters);
            var url = "https://fingertips.phe.org.uk";
            var host = new FingertipsUrl(config, new Uri(url)).ProtocolAndHost;

            // Assert 
            var skin = ReaderFactory.GetProfileReader().GetSkinFromId(SkinIds.Core);
            Assert.AreEqual("http://" + skin.LiveHost, host);
        }
    }
}
