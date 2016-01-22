using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Profiles.DataAccess;
using Profiles.DomainObjects;

namespace DataAccessTest
{
    [TestClass]
    public class AppConfigTest
    {
        private const string NotDefined = " is not defined";

        [TestMethod]
        public void TestJavaScriptVersionFolder()
        {
            Assert.AreEqual("a", GetConfig("JavaScriptVersionFolder", "a").JavaScriptVersionFolder);
        }

        [TestMethod]
        public void TestJavaScriptVersionFolderAllowedToBeEmpty()
        {
            Assert.AreEqual("", GetConfig("JavaScriptVersionFolder", "").JavaScriptVersionFolder);
        }

        [TestMethod]
        public void TestBridgeWsUrl()
        {
            Assert.AreEqual("a", GetConfig("BridgeWsUrl", "a").BridgeWsUrl);
        }

        [TestMethod]
        public void TestIsIndicatorSearchAvailable()
        {
            const string key = "IsIndicatorSearchAvailable";

            Assert.IsTrue(GetConfig(key, null).IsIndicatorSearchAvailable, "Search should be available by default");

            Assert.IsTrue(GetConfig(key, "true").IsIndicatorSearchAvailable);
            Assert.IsFalse(GetConfig(key, "false").IsIndicatorSearchAvailable);
        }

        [TestMethod]
        public void TestIsEnvironmentLive()
        {
            Assert.IsTrue(GetConfig("Environment", "live").IsEnvironmentLive);
            Assert.IsFalse(GetConfig("Environment", "test").IsEnvironmentLive);
        }

        [TestMethod]
        public void TestIsEnvironmentTest()
        {
            Assert.IsFalse(GetConfig("Environment", "live").IsEnvironmentTest);
            Assert.IsTrue(GetConfig("Environment", "test").IsEnvironmentTest);
        }

        [TestMethod]
        public void TestEnvironment()
        {
            var config = GetConfig("Environment", "live");
            Assert.AreEqual("Live", config.Environment);
        }

        [TestMethod]
        public void TestExceptionIfJavaScriptVersionFolderNull()
        {
            var parameterName = "JavaScriptVersionFolder";

            try
            {
                var s = GetConfig("JavaScriptVersionFolder", null).JavaScriptVersionFolder;
            }
            catch (FingertipsException ex)
            {
                Assert.AreEqual(ex.Message, parameterName + NotDefined);
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public void TestCoreWsAlwaysEndsWithBackslash()
        {
            Assert.AreEqual("a/", GetConfig("CoreWsUrl", "a").CoreWsUrl);
        }

        [TestMethod]
        public void TestExceptionIfCoreWsEmpty()
        {
            var parameterName = "CoreWsUrl";

            try
            {
                var s = GetConfig(parameterName, "").CoreWsUrl;
            }
            catch (FingertipsException ex)
            {
                Assert.AreEqual(ex.Message, parameterName + NotDefined);
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public void TestStaticContentUrlAlwaysEndsWithBackslash()
        {
            Assert.AreEqual("a/", GetConfig("StaticContentUrl", "a").StaticContentUrl);
        }

        [TestMethod]
        public void TestSkinOverride()
        {
            var config = GetConfig("SkinOverride", "a");
            Assert.AreEqual("a", config.SkinOverride);
            Assert.IsTrue(config.IsSkinOverride);
        }

        [TestMethod]
        public void TestSkinOverrideNotDefined()
        {
            var config = GetConfig("", "");
            Assert.IsFalse(config.IsSkinOverride);

            config = GetConfig("SkinOverride", null);
            Assert.IsFalse(config.IsSkinOverride);
        }

        [TestMethod]
        public void TestCoreWsUrlForAjaxBridge_UsesCoreWsUrlIfNotSet()
        {
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("CoreWsUrl", "1");
            parameters.Add("CoreWsUrlForAjaxBridge", "2");
            var config = new AppConfig(parameters);
            Assert.IsTrue(config.CoreWsUrlForAjaxBridge.Contains("2"));

            parameters = new NameValueCollection();
            parameters.Add("CoreWsUrl", "1");
            config = new AppConfig(parameters);
            Assert.IsTrue(config.CoreWsUrlForAjaxBridge.Contains("1"));
        }

        [TestMethod]
        public void TestCoreWsUrlForLogging_UsesCoreWsUrlIfNotSet()
        {
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("CoreWsUrl", "1");
            parameters.Add("CoreWsUrlForLogging", "2");
            var config = new AppConfig(parameters);
            Assert.IsTrue(config.CoreWsUrlForLogging.Contains("2"));

            parameters = new NameValueCollection();
            parameters.Add("CoreWsUrl", "1");
            config = new AppConfig(parameters);
            Assert.IsTrue(config.CoreWsUrlForLogging.Contains("1"));
        }

        [TestMethod]
        public void EnsureUrlEndsWithForwardSlash_Returns_Correctly_For_UrlWithoutForwardSlash()
        {
            var url = "http://testurl.com";

            var result = AppConfig.Instance.EnsureUrlEndsWithForwardSlash(url);

            Assert.IsTrue(result.EndsWith("/"));
        }

        [TestMethod]
        public void EnsureUrlEndsWithForwardSlash_Returns_Correctly_For_UrlWithForwardSlash()
        {
            var url = "http://testurl.com/";

            var result = AppConfig.Instance.EnsureUrlEndsWithForwardSlash(url);

            Assert.IsTrue(result == url);
        }

        [TestMethod]
        public void EnsureUrlEndsWithForwardSlash_Returns_Correctly_For_UrlWithMultipleForwardSlashes()
        {
            var url = "http://testurl.com/test//";

            var result = AppConfig.Instance.EnsureUrlEndsWithForwardSlash(url);

            Assert.IsTrue(result == "http://testurl.com/test/");

        }
        private static AppConfig GetConfig(string key, string val)
        {
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add(key, val);
            return new AppConfig(parameters);
        }


    }
}
