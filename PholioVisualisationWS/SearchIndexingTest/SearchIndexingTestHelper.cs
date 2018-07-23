using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.SearchIndexing;

namespace PholioVisualisation.SearchIndexingTest
{
    [TestClass]
    public class SearchIndexingTestHelper
    {
        public static void GivenNoExistingIndexFiles(string indexDirectoryName)
        {
            var directoryPath = Path.Combine(
                ApplicationConfiguration.Instance.SearchIndexDirectory, indexDirectoryName);
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }
        }

        public static void BuildIndexesThenCheckIndexFolderExists(IndexBuilder indexBuilder, string folder)
        {
            indexBuilder.BuildIndexes();

            var path = Path.Combine(ApplicationConfiguration.Instance.SearchIndexDirectory, folder);
            Assert.IsTrue(Directory.Exists(path));
        }
    }
}
