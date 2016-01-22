using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.KeyMessages;

namespace KeyMessagesTest
{
    [TestClass]
    public class SentenceJoinerTest
    {
        [TestMethod]
        public void TestAddIgnoresNull()
        {
            var joiner = new SentenceJoiner();
            joiner.Add(null);
            Assert.AreEqual(string.Empty, joiner.Join());
        }

        [TestMethod]
        public void TestAddIgnoresEmptyString()
        {
            var joiner = new SentenceJoiner();
            joiner.Add(string.Empty);
            Assert.AreEqual(string.Empty, joiner.Join());
        }

        [TestMethod]
        public void TestJoin()
        {
            var joiner = new SentenceJoiner();
            joiner.Add("a");
            joiner.Add("b");
            joiner.Add("c");
            Assert.AreEqual("a b c", joiner.Join());
        }
    }
}
