using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PdfData;
using PholioVisualisation.PholioObjects;

namespace PdfDataTest
{
    [TestClass]
    public class LimitsWithStepTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            LimitsWithStep limitsWithStep = new LimitsWithStep(
                new Limits{Min =1, Max = 2}, 3);

            Assert.AreEqual(1, limitsWithStep.Min);
            Assert.AreEqual(2, limitsWithStep.Max);
            Assert.AreEqual(3, limitsWithStep.Step);
        }
    }
}
