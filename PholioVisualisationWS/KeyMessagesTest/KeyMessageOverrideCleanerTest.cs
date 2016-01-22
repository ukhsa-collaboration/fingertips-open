using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.KeyMessages;

namespace KeyMessagesTest
{
    [TestClass]
    public class KeyMessageOverrideCleanerTest
    {
        [TestMethod]
        public void TestHrefInAllLinksStartsWithHttp()
        {
            Assert.AreEqual(
                "<a href=\"http://b.com\">b</a> <a href=\"http://c.com\">c</a> <a href=\"http://d.com\">d</a> <a href=\"http://e.com\">e</a>",
                GetCleanMessage(
                "<a href=\"http://b.com\">b</a> <a href=\"c.com\">c</a> <a href=\"http://d.com\">d</a> <a href=\"e.com\">e</a>"));
        }

        [TestMethod]
        public void TestEnsureHrefInAllLinksStartsWithHttpIsCaseInsensitive()
        {
            Assert.AreEqual(
                "<a HREF=\"http://b.com\">b</a>",
                GetCleanMessage(
                "<a HREF=\"b.com\">b</a>"));
        }

        [TestMethod]
        public void TestEnsureHrefInAllLinksStartsWithHttpHandlesDoubleQuotes()
        {
            Assert.AreEqual(
                "<a href=\"http://b.com\">b</a>", 
                GetCleanMessage(
                "<a href=\"b.com\">b</a>"));
        }

        [TestMethod]
        public void TestEnsureHrefInAllLinksStartsWithHttpHandlesOtherQuotes()
        {
            Assert.AreEqual(
                "<a href=`http://b.com`>b</a>",
                GetCleanMessage(
                "<a href=`b.com`>b</a>"));
        }

        [TestMethod]
        public void TestEnsureSingleQuotesAreChangedToDoubleQuotes()
        {
            Assert.AreEqual(
                "<a href=\"http://b.com\">b</a>",
                GetCleanMessage(
                "<a href='http://b.com'>b</a>"));
        }

        private static string GetCleanMessage(string message)
        {
            return new KeyMessageOverrideCleaner(message).CleanMessage;
        }
    }
}
