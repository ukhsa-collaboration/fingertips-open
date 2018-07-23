using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServicesWeb.Helpers;

namespace PholioVisualisation.ServicesWebTest.Helpers
{
    [TestClass]
    public class RequestContentParserHelperTest
    {
        private const int IndicatorId = IndicatorIds.DeprivationScoreIMD2010;
        private const int ProfileId = ProfileIds.Phof;
        private const string ContentKey = "contact-us";

        private readonly string _liveUpdateKey = ApplicationConfiguration.Instance.GetLiveUpdateKey();
        private readonly IGroupDataReader _groupDataReader = ReaderFactory.GetGroupDataReader();
        private readonly IProfileReader _profileReader = ReaderFactory.GetProfileReader();
        private readonly IContentItemRepository _contentItemRepository = ReaderFactory.GetContentItemRepository();
        private readonly RequestContentParserHelper _parserHelper = new RequestContentParserHelper();

        [TestMethod]
        public async Task TestParseMetadata()
        {
            // Arrange
            var httpRequestMessage = GetHttpRequestMessage(ParseTypes.MetaDataTextValues);
            
            // Act
            var indicatorMetadataTextValues = await _parserHelper.ParseMetadata(httpRequestMessage);

            // Assert
            Assert.IsTrue(indicatorMetadataTextValues.Any());
        }

        [TestMethod]
        public async Task TestParseGroupings()
        {
            // Arrange
            var httpRequestMessage = GetHttpRequestMessage(ParseTypes.Groupings);
            
            // Act
            var groupings = await _parserHelper.ParseGroupings(httpRequestMessage);

            // Assert
            Assert.IsTrue(groupings.Any());
        }

        [TestMethod]
        public void TestParseCoreDataSets()
        {
            // Arrange
            var httpRequestMessage = GetHttpRequestMessage(ParseTypes.CoreDataSets);

            // Act
            var coreDataSets = _parserHelper.ParseCoreDataSets(httpRequestMessage);

            // Assert
            Assert.IsTrue(coreDataSets.Any());
        }

        [TestMethod]
        public async Task TestParseProfileId()
        {
            // Arrange
            var httpRequestMessage = GetHttpRequestMessage(ParseTypes.ProfileId);

            // Act
            var profileId = await _parserHelper.ParseProfileId(httpRequestMessage);

            // Assert
            Assert.AreEqual(ProfileId, profileId);
        }

        [TestMethod]
        public async Task TestParseContentItems()
        {
            // Arrange
            var httpRequestMessage = GetHttpRequestMessage(ParseTypes.ContentItems);

            // Act
            IList<ContentItem> contentItems = await _parserHelper.ParseContentItems(httpRequestMessage);

            // Assert
            Assert.IsTrue(contentItems.Any());
        }

        private HttpRequestMessage GetHttpRequestMessage(ParseTypes liveUpdateTypes)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage();
            MultipartFormDataContent formData;

            switch (liveUpdateTypes)
            {
                case ParseTypes.MetaDataTextValues:
                    var indicatorMetadataTextValues = _groupDataReader.GetIndicatorMetadataTextValues(IndicatorId);

                    formData = new MultipartFormDataContent
                    {
                        {new StringContent(JsonConvert.SerializeObject(_liveUpdateKey)), "LiveUpdateKey"},
                        {new StringContent(JsonConvert.SerializeObject(indicatorMetadataTextValues)), "indicator-metadata-textvalues"},
                    };

                    requestMessage.Content = formData;
                    break;

                case ParseTypes.Groupings:
                    var groupIds = _profileReader.GetGroupIdsForProfile(ProfileId);
                    var groupings = _groupDataReader.GetGroupingsByGroupIdsAndIndicatorId(groupIds.ToList(), IndicatorId);

                    formData = new MultipartFormDataContent
                    {
                        {new StringContent(JsonConvert.SerializeObject(_liveUpdateKey)), "LiveUpdateKey"},
                        {new StringContent(JsonConvert.SerializeObject(groupings)), "groupings"},
                    };

                    requestMessage.Content = formData;
                    break;

                case ParseTypes.CoreDataSets:
                    var coreDataSets = _groupDataReader.GetCoreDataForIndicatorId(IndicatorId);

                    formData = new MultipartFormDataContent
                    {
                        {new StringContent(JsonConvert.SerializeObject(_liveUpdateKey)), "LiveUpdateKey"},
                        {new StringContent(JsonConvert.SerializeObject(coreDataSets)), "core-dataset"},
                    };

                    requestMessage.Content = formData;
                    break;

                case ParseTypes.ProfileId:
                    formData = new MultipartFormDataContent
                    {
                        {new StringContent(JsonConvert.SerializeObject(_liveUpdateKey)), "LiveUpdateKey"},
                        {new StringContent(JsonConvert.SerializeObject(ProfileId)), "delete-all-groupings-for-profile"},
                    };

                    requestMessage.Content = formData;
                    break;

                case ParseTypes.ContentItems:
                    var contentItems = new List<ContentItem>();
                    var contentItem = _contentItemRepository.GetContentForProfile(ProfileId, ContentKey);
                    contentItems.Add(contentItem);

                    formData = new MultipartFormDataContent
                    {
                        {new StringContent(JsonConvert.SerializeObject(_liveUpdateKey)), "LiveUpdateKey"},
                        {new StringContent(JsonConvert.SerializeObject(contentItems)), "content-items"},
                    };

                    requestMessage.Content = formData;
                    break;
            }
            
            return requestMessage;
        }

        private enum ParseTypes
        {
            MetaDataTextValues = 0,
            Groupings = 1,
            CoreDataSets = 2,
            ProfileId = 3,
            ContentItems = 4
        }
    }
}
