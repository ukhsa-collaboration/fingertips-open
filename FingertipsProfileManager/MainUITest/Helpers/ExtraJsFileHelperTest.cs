using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.MainUI.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.MainUITest.Helpers
{
    [TestClass]
    public class ExtraJsFileHelperTest
    {
        [TestMethod]
        public void TestDefaults()
        {
            var helper = new ExtraJsFileHelper();

            Assert.IsFalse(helper.IsEnglandTab);
            Assert.IsTrue(helper.IsMapTab);
            Assert.IsFalse(helper.IsPopulationTab);
            Assert.IsFalse(helper.IsReportsTab);
            Assert.IsFalse(helper.IsScatterPlotTab);
            Assert.IsFalse(helper.IsCompareAreasTab);
        }

        [TestMethod]
        public void TestDefaultJsFileString()
        {
            var helper = new ExtraJsFileHelper();

            Assert.AreEqual("PageTartanRug.js,PageMap.js,PageAreaTrends.js,PageBarChart.js,PageAreaProfile.js,PageMetadata.js,PageDownload.js", 
                helper.GetExtraJsFiles());
        }

        [TestMethod]
        public void TestMapIncludedOptionally()
        {
            var helper = new ExtraJsFileHelper();

            helper.IsMapTab = false;

            Assert.AreEqual("PageTartanRug.js,PageAreaTrends.js,PageBarChart.js,PageAreaProfile.js,PageMetadata.js,PageDownload.js",
                helper.GetExtraJsFiles());
        }
    }
}
