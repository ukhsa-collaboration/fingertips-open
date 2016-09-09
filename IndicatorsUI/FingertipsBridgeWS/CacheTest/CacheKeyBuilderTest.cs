using System;
using System.Collections.Specialized;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using FingertipsBridgeWS.Cache;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IndicatorsUI.FingertipsBridgeWS.CacheTest
 
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class CacheKeyBuilderTest
    {
        [TestMethod]
        public void TestWithoutKey()
        {
            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add("e", "99");
            nameValues.Add("a", "16");
            nameValues.Add("c", "88");
            nameValues.Add("s", "bb");
            CacheKeyBuilder builder = new CacheKeyBuilder(nameValues, null);

            Assert.AreEqual(builder.ServiceId, "bb");
            Assert.AreEqual(builder.CacheKey, "a16c88e99");
        }

        [TestMethod]
        public void TestWithKey()
        {
            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add("e", "99");
            nameValues.Add("a", "16");
            nameValues.Add("c", "88");
            CacheKeyBuilder builder = new CacheKeyBuilder(nameValues, "bb");

            Assert.AreEqual(builder.ServiceId, "bb");
            Assert.AreEqual(builder.CacheKey, "a16c88e99");
        }

        [TestMethod]
        public void TestJqueryParametersNotIncludedInCacheKey()
        {
            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add("a", "9");
            nameValues.Add(CacheKeyBuilder.ParameterAjaxCallId, "7");
            CacheKeyBuilder builder = new CacheKeyBuilder(nameValues, "b");

            Assert.IsFalse(builder.CacheKey.Contains(CacheKeyBuilder.ParameterAjaxCallId));
            Assert.IsFalse(builder.CacheKey.Contains("7"));
            Assert.IsFalse(builder.CacheKey.Contains("8"));
        }

        [TestMethod]
        public void TestNoCache()
        {
            // No cache
            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add("nocache", "");
            nameValues.Add(CacheKeyBuilder.ParameterAjaxCallId, "7");
            CacheKeyBuilder builder = new CacheKeyBuilder(nameValues, "b");
            Assert.IsFalse(builder.CanJsonBeCached);

            // Can cache
            nameValues = new NameValueCollection();
            nameValues.Add(CacheKeyBuilder.ParameterAjaxCallId, "7");
             builder = new CacheKeyBuilder(nameValues, "b");
            Assert.IsTrue(builder.CanJsonBeCached);
        }
    }
}
