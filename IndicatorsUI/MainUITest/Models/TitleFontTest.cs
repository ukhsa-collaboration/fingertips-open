using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUI.Models;

namespace IndicatorsUI.MainUITest.Models
{
    [TestClass]
    public class TitleFontTest
    {
        [TestMethod]
        public void TestPracticeProfilesReturnsEmptyString()
        {
            Assert.AreEqual(
                TitleFont.GetSize("national general practice profiles", ProfileIds.PracticeProfiles),
                "");
        }

        [TestMethod]
        public void TestShortTitleReturnsEmptyString()
        {
            Assert.AreEqual(
                TitleFont.GetSize("a short title", ProfileIds.SexualHealth),
                "");
        }

    }
}