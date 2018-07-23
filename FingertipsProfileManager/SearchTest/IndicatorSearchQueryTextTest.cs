using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.Search;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.SearchTest
{
    [TestClass]
    public class IndicatorSearchQueryTextTest
    {
        [TestMethod]
        public void Test_Hyphens_Replaced_With_Wildcards()
        {
            var text = new IndicatorSearchQueryText().GetSqlSearchText("self-harm");
            Assert.AreEqual("%self_harm%", text);
        }
    }
}
