using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class IndicatorMetadataProviderTest
    {
        [TestMethod]
        public void CorrectLocalDocumentLinkTest()
        {
            var description = new Dictionary<string, string>();
            var testUrlString = @"<a href=""documents/test.doc";
            description.Add("Link", testUrlString);

            var metadata = new List<IndicatorMetadata>();
            metadata.Add(new IndicatorMetadata { Descriptive = description });

            IndicatorMetadataProvider indicatorMetadataProvider = IndicatorMetadataProvider.Instance;
            indicatorMetadataProvider.CorrectLocalDocumentLink(metadata);

            var updatedUrl = metadata[0].Descriptive["Link"];
            var expectUrl = @"<a href=""" + ApplicationConfiguration.Instance.UrlUI + "/documents/test.doc";

            Assert.AreEqual(expectUrl, updatedUrl);
        }
    }
}
