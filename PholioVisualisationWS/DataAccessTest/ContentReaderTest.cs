using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccessTest
{
    [TestClass]
    public class ContentReaderTest
    {
        public const string ContentKey = ContentKeys.Test;
        public const string ContentValue = "Test Item";

        [TestMethod]
        public void TestGetContent()
        {
            var text = Reader().GetContent(ContentKey);
            Assert.AreEqual(ContentValue, text.Content);
        }

        [TestMethod]
        public void TestGetContentForProfile()
        {
            var text = Reader().GetContentForProfile(ProfileIds.PhysicalActivity, ContentKey);
            Assert.AreEqual(ContentValue, text.Content);
        }

        private static IContentReader Reader()
        {
            IContentReader reader = ReaderFactory.GetContentReader();
            return reader;
        }
    }
}
