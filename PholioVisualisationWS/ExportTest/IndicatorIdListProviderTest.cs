using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ExportTest
{
    [TestClass]
    public class IndicatorIdListProviderTest
    {
        private Mock<IGroupDataReader> _groupDataReader;
        private Mock<IGroupIdProvider> _groupIdProvider;

        public const int GroupId = 5;
        public const int ProfileId = 4;

        [TestInitialize]
        public void TestInitialize()
        {
            _groupDataReader = new Mock<IGroupDataReader>(MockBehavior.Strict);
            _groupDataReader.Setup(x => x.GetGroupingsByGroupId(GroupId))
                .Returns(GetGroupings());

            _groupIdProvider = new Mock<IGroupIdProvider>(MockBehavior.Strict);

        }

        [TestMethod]
        public void TestGetIdsForGroup()
        {
            var indicatorIdListProvider = new IndicatorIdListProvider(_groupDataReader.Object, _groupIdProvider.Object);

            // Act: get indicator ids for group
            var ids = indicatorIdListProvider.GetIdsForGroup(GroupId);

            AssertAll(ids);
        }

        [TestMethod]
        public void TestGetIdsForProfile()
        {
            _groupIdProvider.Setup(x => x.GetGroupIds(ProfileId))
                .Returns(new List<int> {GroupId});

            var indicatorIdListProvider = new IndicatorIdListProvider(_groupDataReader.Object, _groupIdProvider.Object);

            // Act: get indicator ids for profile
            var ids = indicatorIdListProvider.GetIdsForProfile(ProfileId);

            AssertAll(ids);
        }

        private void AssertAll(IList<int> ids)
        {
            Assert.AreEqual(2, ids.Count);
            _groupDataReader.VerifyAll();
            _groupIdProvider.VerifyAll();
        }

        private IList<Grouping> GetGroupings()
        {
            return new List<Grouping>
            {
                new Grouping { IndicatorId = 1},
                new Grouping { IndicatorId = 1},
                new Grouping { IndicatorId = 2}
            };
        }
    }
}

