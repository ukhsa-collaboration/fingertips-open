using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class GroupIdProviderTest
    {
        [TestMethod]
        public void TestNonExistantProfileIdReturnsEmptyList()
        {
            var reader = new Mock<ProfileReader>();
            var groupIds = GetGroupIds(reader, -99);
            Assert.AreEqual(0, groupIds.Count);
        }

        [TestMethod]
        public void TestGetIdsOfProfileThatExists()
        {
            var groupIds = new GroupIdProvider(ReaderFactory.GetProfileReader())
                .GetGroupIds(ProfileIds.Phof);
            Assert.IsTrue(groupIds.Contains(GroupIds.Phof_WiderDeterminantsOfHealth));
        }

        [TestMethod]
        public void TestSearchIdReturnsAllGroupIds()
        {
            var profileId = ProfileIds.Search;
            var groupId = 2;
            var expectedGroupIds = new List<int> { groupId };
            var reader = new Mock<ProfileReader>();
            reader.Setup(x => x.GetGroupIdsFromAllProfiles()).Returns(expectedGroupIds);
            var groupIds = GetGroupIds(reader, profileId);
            Assert.AreEqual(groupId, groupIds[0]);
        }

        [TestMethod]
        public void TestUndefinedProfileIdReturnsAllGroupIds()
        {
            var profileId = ProfileIds.Undefined;
            var groupId = 2;
            var expectedGroupIds = new List<int> { groupId };
            var reader = new Mock<ProfileReader>();
            reader.Setup(x => x.GetGroupIdsFromAllProfiles()).Returns(expectedGroupIds);
            var groupIds = GetGroupIds(reader, profileId);
            Assert.AreEqual(groupId, groupIds[0]);
        }

        private IList<int> GetGroupIds(Mock<ProfileReader> reader, int profileId)
        {
            var provider = new GroupIdProvider(reader.Object);
            return provider.GetGroupIds(profileId); ;
        }
    }
}
