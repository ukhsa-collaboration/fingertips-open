using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.SearchQuerying;

namespace PholioVisualisation.SearchQueryingTest
{
    [TestClass]
    public class RelatedTermsProviderTest
    {
        [TestMethod]
        public void Test_Where_Term_Has_No_Related_Terms()
        {
            var term = "sausages";
            var terms = RelatedTermsProvider.GetRelatedTerms(term);
            Assert.AreEqual(1, terms.Count);
            Assert.AreEqual(term, terms.First());
        }

        [TestMethod]
        public void Test_Where_Term_Has_Related_Terms()
        {
            var term = "asthma";
            var terms = RelatedTermsProvider.GetRelatedTerms(term);
            Assert.IsTrue(terms.Count > 1);
            Assert.IsTrue(terms.Contains(term), "Related terms list should include main term");
        }
    }
}
