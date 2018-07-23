using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.SearchQuerying;

namespace PholioVisualisation.SearchQueryingTest
{
    [TestClass]
    public class IndicatorSearchTest
    {
        private IGroupDataReader _groupDataReader = ReaderFactory.GetGroupDataReader();

        [TestMethod]
        public void TestSearchIndicators()
        {
            // Standard search
            IList<int> ids = Search("drug");
            Assert.IsTrue(ids.Count > 10);

            // Nonsense search string
            ids = Search("xhoawecapqfhasfdg");
            Assert.AreEqual(0, ids.Count);
        }

        [TestMethod]
        public void TestIndicatorSearch_Is_Case_Insensitive()
        {

            Assert.AreEqual(Search("TUBERCULOSIS").Count, Search("tuberculosis").Count);
            Assert.IsTrue(Search("tuberculosis").Count > 0);
        }

        [TestMethod]
        public void TestIndicatorSearch_Parses_Indicator_Id_List()
        {
            var ids = Search("97, 98");

            Assert.AreEqual(2, ids.Count);
            Assert.AreEqual(97, ids[0]);
            Assert.AreEqual(98, ids[1]);
        }

        private IList<int> Search(string text)
        {
            var properties = _groupDataReader
               .GetIndicatorMetadataTextProperties()
               .Where(p => p.SearchBoost > 0);
            var search = new IndicatorSearch();
            return search.SearchIndicators(text, properties);
        }
    }
}