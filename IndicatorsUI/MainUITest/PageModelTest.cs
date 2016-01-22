using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Profiles.DataAccess;
using Profiles.MainUI.Models;

namespace IndicatorsUITest
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
