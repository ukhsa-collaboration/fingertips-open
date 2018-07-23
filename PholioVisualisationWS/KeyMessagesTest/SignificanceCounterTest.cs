using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.KeyMessages;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.KeyMessagesTest
{
    [TestClass]
    public class SignificanceCounterTest
    {
        [TestMethod]
        public void TestGreen()
        {
            Assert.AreEqual(3, new SignificanceCounter(Significances()).Green);
        }

        [TestMethod]
        public void TestAmber()
        {
            Assert.AreEqual(2, new SignificanceCounter(Significances()).Amber);
        }

        [TestMethod]
        public void TestRed()
        {
            Assert.AreEqual(1, new SignificanceCounter(Significances()).Red);
        }

        [TestMethod]
        public void TestProportionRed()
        {
            Assert.AreEqual(1.0 / 6.0, new SignificanceCounter(Significances()).GetProportionRed());
        }

        [TestMethod]
        public void TestProportionGreen()
        {
            Assert.AreEqual(3.0 / 6.0, new SignificanceCounter(Significances()).GetProportionGreen());
        }

        [TestMethod]
        public void TestProportionAmber()
        {
            Assert.AreEqual(2.0 / 6.0, new SignificanceCounter(Significances()).GetProportionAmber());
        }

        private static List<Significance> Significances()
        {
            var significances = new List<Significance>
            {
                Significance.Better,
                Significance.Better,
                Significance.Better,
                Significance.Same,
                Significance.Same,
                Significance.Worse
            };
            return significances;
        }
    }
}
