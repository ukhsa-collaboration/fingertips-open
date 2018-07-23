using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndicatorsUI.MainUI.Helpers;

namespace IndicatorsUI.MainUITest.Helpers
{
    [TestClass]
    public class VerticalTextTest
    {
        [TestMethod]
        public void TestGetImage()
        {
            Assert.IsTrue(ImageLength("a") > 0);
        }

        [TestMethod]
        public void TestLongerStringsProduceBiggerImages()
        {
            Assert.IsTrue(ImageLength("aa") > ImageLength("a"));
        }

        public static int ImageLength(string text)
        {
            var stream = new VerticalText().GetImage(text);
            return stream.ToArray().Length;
        }
    }
}
