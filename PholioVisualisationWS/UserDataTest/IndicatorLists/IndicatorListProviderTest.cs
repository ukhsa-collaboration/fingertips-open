using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.UserData.IndicatorLists;

namespace PholioVisualisation.UserDataTest.IndicatorLists
{
    [TestClass]
    public class IndicatorListProviderTest
    {
        private Mock<IIndicatorListRepository> _indicatorListRepo;
        private Mock<IIndicatorMetadataRepository> _indicatorMetadataRepo;

        private const int AgeId = AgeIds.From20To24;
        private const int SexId = SexIds.Persons;
        private const int IndicatorId = 2;
        private const string IndicatorListPublicId = "1";

        [TestInitialize]
        public void TestInitialize()
        {
            _indicatorListRepo = new Mock<IIndicatorListRepository>(MockBehavior.Strict);
            _indicatorMetadataRepo = new Mock<IIndicatorMetadataRepository>(MockBehavior.Strict);
        }

        [TestMethod]
        public void Test_Items_With_Non_Existant_Indicator_Ids_Are_Excluded()
        {
            SetUpRepoToReturnValidIndicatorList();

            // Set up indicator repo to return no indicator name
            _indicatorMetadataRepo
                .Setup(x => x.GetIndicatorNames(new List<int> { IndicatorId }))
                .Returns(new List<INamedEntity>());

            var provider = GetIndicatorListProvider();
            var list = provider.GetIndicatorList(IndicatorListPublicId);

            Assert.IsNotNull(list);
            Assert.IsFalse(list.IndicatorListItems.Any());
            VerifyAll();
        }

        [TestMethod]
        public void Test_Non_Existant_List()
        {
            _indicatorListRepo.Setup(x => x.GetIndicatorList(IndicatorListPublicId))
                .Returns((IndicatorList)null);

            var provider = GetIndicatorListProvider();
            var list = provider.GetIndicatorList(IndicatorListPublicId);

            Assert.IsNull(list);
            VerifyAll();
        }

        [TestMethod]
        public void Test_Non_Persisted_Properties_Are_Populated()
        {
            SetUpRepoToReturnValidIndicatorList();

            _indicatorMetadataRepo
                .Setup(x => x.GetIndicatorNames(new List<int> { IndicatorId }))
                .Returns(new List<INamedEntity> { new NamedEntity { Id = IndicatorId, Name = "name" } });

            // Act: Get indicator list
            var provider = GetIndicatorListProvider();
            var list = provider.GetIndicatorList(IndicatorListPublicId);

            // Assert: properties assigned
            var item = list.IndicatorListItems.First();
            Assert.AreEqual(item.Age.Id, AgeId);
            Assert.AreEqual(item.Sex.Id, SexId);
            Assert.AreEqual("name", item.IndicatorName);

            VerifyAll();
        }

        private void SetUpRepoToReturnValidIndicatorList()
        {
            var items = new List<IndicatorListItem>
            {
                new IndicatorListItem
                {
                    AgeId = AgeId,
                    SexId = SexId,
                    IndicatorId = IndicatorId
                }
            };

            _indicatorListRepo.Setup(x => x.GetIndicatorList(IndicatorListPublicId)).Returns(new IndicatorList
            {
                IndicatorListItems = items
            });
        }

        private void VerifyAll()
        {
            _indicatorMetadataRepo.VerifyAll();
            _indicatorListRepo.VerifyAll();
        }


        private IndicatorListProvider GetIndicatorListProvider()
        {
            var provider = new IndicatorListProvider(
                _indicatorListRepo.Object, ReaderFactory.GetPholioReader(),
                _indicatorMetadataRepo.Object);
            return provider;
        }
    }
}
