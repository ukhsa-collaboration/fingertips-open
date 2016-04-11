using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.KeyMessages;

namespace PholioVisualisation.FormattingTest
{
    [TestClass]
    public class PhraseJoinerFormatterTest
    {
        [TestMethod]
        public void TestFourItems()
        {
            var items = new List<string> {"one", "two", "three", "four"};

            Assert.AreEqual("one, two, three and four", PhraseJoiner.Join(items));
        }

        [TestMethod]
        public void TestThreeItems()
        {
            var items = new List<string> {"one", "two", "three"};

            Assert.AreEqual("one, two and three", PhraseJoiner.Join(items));
        }

        [TestMethod]
        public void TestTwoItems()
        {
            var items = new List<string> {"one", "two"};

            Assert.AreEqual("one and two", PhraseJoiner.Join(items));
        }

        [TestMethod]
        public void TestOneItem()
        {
            var items = new List<string> {"one"};

            Assert.AreEqual("one", PhraseJoiner.Join(items));
        }
    }
}