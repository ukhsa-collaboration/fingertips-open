using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Profiles.MainUI.Models;

namespace IndicatorsUI.MainUITest.Models
{
    [TestClass]
    public class JsHelperTest
    {
        [TestMethod]
        public void TestGetJsBool()
        {
            Assert.AreEqual("true", JsHelper.GetJsBool(true));
            Assert.AreEqual("false", JsHelper.GetJsBool(false));
        }

        [TestMethod]
        public void GetIncludePath()
        {
            // Path used appended
            Assert.AreEqual("http://a.com/a", JsHelper.GetIncludePath("a", @"http://a.com/"));

            // Path ignored as file already has it
            Assert.AreEqual("http://a.com/a", JsHelper.GetIncludePath("//a.com/a", @"http://a.com/"));

            // Empty to empty
            Assert.AreEqual("", JsHelper.GetIncludePath("", @""));
        }

    }
}
