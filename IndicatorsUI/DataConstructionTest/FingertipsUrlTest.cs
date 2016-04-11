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
        public void TestHostInDevEnvironment()
        {
            var parameters = new NameValueCollection();
            parameters.Add("Environment", "test");
            parameters.Add("BridgeWsUrl", "http://localhost:59822/");
            var config = new AppConfig(parameters);

            Assert.AreEqual("", new FingertipsUrl(config).Host);
        }

        [TestMethod]
        public void TestHostInTestEnvironment()
        {
            var parameters = new NameValueCollection();
            parameters.Add("Environment", "test");
            parameters.Add("BridgeWsUrl", "https://testprofiles.phe.org.uk/");
            var config = new AppConfig(parameters);

            var skin = ReaderFactory.GetProfileReader().GetSkinFromId(SkinIds.Core);
            var host = new FingertipsUrl(config).Host;
            Assert.AreEqual("http://" + skin.TestHost, host);
        }

        [TestMethod]
        public void TestHostInLiveEnvironment()
        {
            var parameters = new NameValueCollection();
            parameters.Add("Environment", "live");
            parameters.Add("BridgeWsUrl", "http://fingertips.phe.org.uk/");
            var config = new AppConfig(parameters);

            var skin = ReaderFactory.GetProfileReader().GetSkinFromId(SkinIds.Core);
            Assert.AreEqual("http://" + skin.LiveHost, new FingertipsUrl(config).Host);
        }
    }
}
