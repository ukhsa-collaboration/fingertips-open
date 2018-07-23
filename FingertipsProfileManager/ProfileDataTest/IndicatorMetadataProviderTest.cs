using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fpm.ProfileDataTest
{
    [TestClass]
    public class IndicatorMetadataProviderTest
    {
        private const int ProfileId = ProfileIds.Phof;
        private Mock<IProfilesReader> _profileReader;
        private IndicatorMetadataProvider _metadataProvider;

        [TestInitialize]
        public void Initialize()
        {
            _profileReader = new Mock<IProfilesReader>(MockBehavior.Strict);
            _metadataProvider = new IndicatorMetadataProvider(_profileReader.Object);
        }

        [TestMethod]
        public void TestGetAllIndicatorsForProfile()
        {
            // Arrange
            var groupIds = new List<int>()
            {
                new int()
            };
            _profileReader.Setup(x => x.GetGroupingIds(ProfileId)).Returns(groupIds);

            var indicatorIds = new List<int>()
            {
                new int()
            };
            _profileReader.Setup(x => x.GetGroupingIndicatorIds(groupIds)).Returns(indicatorIds);

            IList<IndicatorMetadataTextValue> list = new List<IndicatorMetadataTextValue>()
            {
                new IndicatorMetadataTextValue()
            };
            _profileReader
                .Setup(x => x.GetIndicatorMetadataTextValuesByIndicatorIdsAndProfileId(indicatorIds, ProfileId))
                .Returns(list);

            // Act
            var indicatorMetadataTextValues = _metadataProvider.GetAllIndicatorsForProfile(ProfileId);

            // Assert
            Assert.IsTrue(indicatorMetadataTextValues.Any());

            VerifyAll();
        }

        private void VerifyAll()
        {
            _profileReader.VerifyAll();
        }
    }
}
