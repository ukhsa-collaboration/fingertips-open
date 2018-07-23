using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.SearchIndexing;

namespace PholioVisualisation.SearchIndexingTest
{
    [TestClass]
    public class SynonymReaderTest
    {
        [TestMethod]
        public void TestGetSynonymLists()
        {
            var list = SynonymReader.GetSynonymLists();
            Assert.IsTrue(list.Any());
        }
    }
}
