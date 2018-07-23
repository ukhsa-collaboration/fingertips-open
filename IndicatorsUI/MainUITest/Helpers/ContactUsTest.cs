using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndicatorsUI.MainUI.Helpers;

namespace IndicatorsUI.MainUITest.Helpers
{
    [TestClass]
    public class ContactUsTest
    {
        public const string UrlKey = "mental-health";

        public const string LinkEnd =
            @" title=""If you would like to ask a question or submit feedback then please send us an email"">Contact Us</a>";

         [TestMethod]
        public void TestLinkIsHtmlString()
        {
            Assert.IsInstanceOfType(new ContactUs("a", UrlKey).Link, typeof(HtmlString));
        }

        [TestMethod]
        public void TestLinkContainsEmailAddressAndUrlKey()
        {
            Assert.AreEqual(@"<a href=""mailto:1@2&subject=Profile%20Feedback%20[mental-health]""" + LinkEnd,
                new ContactUs("1@2", UrlKey).Link.ToString());
        }

        [TestMethod]
        public void TestLinkNoSquareBracketsIfNoProfile()
        {
            Assert.AreEqual(@"<a href=""mailto:1@2&subject=Profile%20Feedback""" + LinkEnd,
                new ContactUs("1@2", string.Empty).Link.ToString());
        }
    }
}
