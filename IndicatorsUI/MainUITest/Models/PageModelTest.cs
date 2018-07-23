using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndicatorsUI.DataAccess;
using IndicatorsUI.MainUI.Models;

namespace IndicatorsUI.MainUITest.Models
{
    [TestClass]
    public class PageModelTest
    {
        [TestMethod]
        public void TestPageTypeIsUndefinedByDefault()
        {
            var model = new PageModel(new AppConfig(new NameValueCollection()));
            Assert.AreEqual(PageType.Undefined,model.PageType);
        }
    }
}
