using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.DomainObjectsTest
{
    [TestClass]
    public class SkinTest
    {
        [TestMethod]
        public void TestIsNotTitle()
        {
            AssertIsNotTitle(null);
            AssertIsNotTitle("");
            AssertIsNotTitle(" ");
        }

        [TestMethod]
        public void TestIsTitle()
        {
            Assert.IsTrue(new Skin { Title = "a" }.IsTitle);
        }

        [TestMethod]
        public void TestIsLongerLives()
        {
            Assert.IsTrue(new Skin { Id = SkinIds.LongerLives }.IsLongerLives);
        }

        private static void AssertIsNotTitle(string title)
        {
            Assert.IsFalse(new Skin {Title = title}.IsTitle);
        }
    }
}
