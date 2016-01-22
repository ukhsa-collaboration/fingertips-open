using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.SearchQuerying;

namespace SearchQueryingTest
{
    [TestClass]
    public class IndicatorSearchTest
    {
        [TestMethod]
        public void TestSearchIndicators()
        {
            // Standard search
            List<int> ids = new IndicatorSearch().SearchIndicators("drug");
            Assert.IsTrue(ids.Count > 10);

            // Nonsense search string
            ids = new IndicatorSearch().SearchIndicators("xhoawecapqfhasfdg");
            Assert.AreEqual(0, ids.Count);
        }

        [TestMethod]
        public void TestIndicatorSearchIsCaseInsensitive()
        {
            var search = new IndicatorSearch();
            Assert.AreEqual(search.SearchIndicators("TUBERCULOSIS").Count, search.SearchIndicators("tuberculosis").Count);
            Assert.IsTrue(search.SearchIndicators("tuberculosis").Count > 0);
        }
    }
}