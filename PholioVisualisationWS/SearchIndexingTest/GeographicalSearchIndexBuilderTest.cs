using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.SearchIndexing;

namespace PholioVisualisation.SearchIndexingTest
{
    [TestClass]
    public class GeographicalSearchIndexBuilderTest
    {
        [ClassInitialize]
        public static void ExecuteOnceBeforeAnyTestMethods(TestContext testContext)
        {
            SearchIndexingTestHelper.GivenNoExistingIndexFiles(GeographicalSearchIndexBuilder.DirectoryPlacePlacecodes);
        }

        [TestMethod]
        public void TestBuildPlacePostcodes()
        {
            var builder = new GeographicalSearchIndexBuilder(ApplicationConfiguration.SearchIndexDirectory);
            SearchIndexingTestHelper.BuildIndexesThenCheckIndexFolderExists(builder,
                GeographicalSearchIndexBuilder.DirectoryPlacePlacecodes);
        }
    }
}
