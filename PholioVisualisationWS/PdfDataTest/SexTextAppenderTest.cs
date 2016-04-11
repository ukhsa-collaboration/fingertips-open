using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PdfData;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfDataTest
{
    [TestClass]
    public class SexTextAppenderTest
    {
        public string IndicatorName = "a";

        [TestMethod]
        public void TestGetIndicatorName_DoNotStateSex()
        {
            Assert.AreEqual(IndicatorName, SexTextAppender.GetIndicatorName(IndicatorName,SexIds.Persons,false));
        }

        [TestMethod]
        public void TestGetIndicatorName_PersonsAdded()
        {
            Assert.AreEqual(IndicatorName + " (Persons)", SexTextAppender.GetIndicatorName(IndicatorName, SexIds.Persons, true));
        }

        [TestMethod]
        public void TestGetIndicatorName_NotApplicableIgnored()
        {
            Assert.AreEqual(IndicatorName, SexTextAppender.GetIndicatorName(IndicatorName, SexIds.NotApplicable, true));
        }

        [TestMethod]
        public void TestGetIndicatorName_FemaleAdded()
        {
            Assert.AreEqual(IndicatorName + " (Female)", SexTextAppender.GetIndicatorName(IndicatorName, SexIds.Female, true));
        }

        [TestMethod]
        public void TestGetIndicatorName_MaleAdded()
        {
            Assert.AreEqual(IndicatorName + " (Male)", SexTextAppender.GetIndicatorName(IndicatorName, SexIds.Male, true));
        }
    }
}
