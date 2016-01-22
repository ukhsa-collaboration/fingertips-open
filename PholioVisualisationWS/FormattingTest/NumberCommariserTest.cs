using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Formatting;

namespace FormattingTest
{
    [TestClass]
    public class NumberCommariserTest
    {
        [TestMethod]
        public void TestCommarise0DPOver100()
        {
            Assert.AreEqual("1,000", NumberCommariser.Commarise0DP(1000));
            Assert.AreEqual("10,000", NumberCommariser.Commarise0DP(10000));
            Assert.AreEqual("100,000", NumberCommariser.Commarise0DP(100000));
            Assert.AreEqual("1,000,000", NumberCommariser.Commarise0DP(1000000));
            Assert.AreEqual("10,000,000", NumberCommariser.Commarise0DP(10000000));
            Assert.AreEqual("100,000,000", NumberCommariser.Commarise0DP(100000000));
            Assert.AreEqual("1,000,000,000", NumberCommariser.Commarise0DP(1000000000));
            Assert.AreEqual("10,000,000,000", NumberCommariser.Commarise0DP(10000000000));
        }

        [TestMethod]
        public void TestCommarise0DPRounding()
        {
            Assert.AreEqual("1", NumberCommariser.Commarise0DP(1.1));
            Assert.AreEqual("2", NumberCommariser.Commarise0DP(1.5));
            Assert.AreEqual("2", NumberCommariser.Commarise0DP(2.1));
            Assert.AreEqual("3", NumberCommariser.Commarise0DP(2.5));
            Assert.AreEqual("3", NumberCommariser.Commarise0DP(3.1));
            Assert.AreEqual("4", NumberCommariser.Commarise0DP(3.5));
        }

        [TestMethod]
        public void FixedDecimalCommaTest()
        {
            Assert.AreEqual("1,200.0", NumberCommariser.Commarise1DP(1200.0123));
            Assert.AreEqual("1,200.01", NumberCommariser.Commarise2DP(1200.0123));
            Assert.AreEqual("1,200.012", NumberCommariser.Commarise3DP(1200.0123));
        }
    }
}
