using Fpm.MainUI.Controllers;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;
using NHibernate.Engine;
using System.Collections.Generic;

namespace Fpm.MainUITest.Controllers
{
    [TestClass]
    public class ProfilesAndIndicatorsControllerTests
    {
        public ProfilesAndIndicatorsController ProfAndIndiController;
        public Mock<ISessionFactory> SessionFactoryMock;
        public Mock<ISession> SessionMock;
        public Mock<ISessionImplementor> SessionImplementorMock;
        public Mock<IQuery> Query1Mock;
        public Mock<IQuery> Query2Mock;
        public Mock<IQuery> Query3Mock;
        public Mock<ITransaction> TransactionMock;

        public int AreaTypeId = AreaTypeIds.District;
        public int GroupId = GroupIds.UnassignedIndicators;
        public IList<GroupingSubheading> GroupSubheadingInDb;

        [TestInitialize]
        public void StartUp()
        {
            SessionMock = new Mock<ISession>(MockBehavior.Strict);
            SessionFactoryMock = new Mock<ISessionFactory>(MockBehavior.Strict);
            SessionImplementorMock = new Mock<ISessionImplementor>(MockBehavior.Strict);
            Query1Mock = new Mock<IQuery>(MockBehavior.Loose);
            TransactionMock = new Mock<ITransaction>(MockBehavior.Loose);
            Query2Mock = new Mock<IQuery>(MockBehavior.Loose);
            Query3Mock = new Mock<IQuery>(MockBehavior.Loose);

            SessionFactoryMock.Setup(x => x.OpenSession()).Returns(SessionMock.Object);

            ProfAndIndiController = new ProfilesAndIndicatorsController(
                new ProfilesReader(SessionFactoryMock.Object), new ProfilesWriter(SessionFactoryMock.Object), 
                new ProfileRepository(SessionFactoryMock.Object), new LookUpsRepository(SessionFactoryMock.Object),
                new CoreDataRepository(SessionFactoryMock.Object), new EmailRepository(SessionFactoryMock.Object)
            );

            GroupSubheadingInDb = new List<GroupingSubheading>
            {
                new GroupingSubheading {AreaTypeId = AreaTypeId, GroupId = GroupId, Sequence = 1, SubheadingId = 1, Subheading = "Test"},
                new GroupingSubheading {AreaTypeId = AreaTypeId, GroupId = GroupId, Sequence = 2, SubheadingId = 2, Subheading = "Test"},
                new GroupingSubheading {AreaTypeId = AreaTypeId, GroupId = GroupId, Sequence = 3, SubheadingId = 3, Subheading = "Test"},
                new GroupingSubheading {AreaTypeId = AreaTypeId, GroupId = GroupId, Sequence = 4, SubheadingId = 4, Subheading = "Test"}
            };
        }

        [TestMethod]
        public void ShouldDeleteMissingSubheadings()
        {
            var newGroupSubheading = new List<GroupingSubheading>
            {
                new GroupingSubheading {AreaTypeId = AreaTypeId, GroupId = GroupId, Sequence = 1, SubheadingId = 1, Subheading = "Test"},
                new GroupingSubheading {AreaTypeId = AreaTypeId, GroupId = GroupId, Sequence = 2, SubheadingId = 2, Subheading = "Test"},
                new GroupingSubheading {AreaTypeId = AreaTypeId, GroupId = GroupId, Sequence = 4, SubheadingId = 4, Subheading = "Test"},
                new GroupingSubheading {AreaTypeId = AreaTypeId, GroupId = GroupId, Sequence = 5, SubheadingId = 5, Subheading = "Test"}
            };

            SessionMock.Setup(x => x.CreateQuery(It.IsAny<string>())).Returns(Query1Mock.Object);
            Query1Mock.Setup(x => x.SetParameter("areaTypeId", It.IsAny<int>()));
            Query1Mock.Setup(x => x.SetParameter("groupId", It.IsAny<int>()));
            Query1Mock.SetReturnsDefault(GroupSubheadingInDb);

            SessionMock.Setup(x => x.BeginTransaction()).Returns(TransactionMock.Object);
            SessionMock.Setup(x => x.GetNamedQuery("Delete_GroupingSubheading")).Returns(Query2Mock.Object);
            Query2Mock.Setup(x => x.SetParameter("SubheadingId", GroupSubheadingInDb[2].SubheadingId)).Returns(Query2Mock.Object);
            Query2Mock.Setup(x => x.ExecuteUpdate()).Returns(1);

            ProfAndIndiController.DeleteMissingSubheadings(AreaTypeId, GroupId, newGroupSubheading);
        }

        [TestMethod]
        public void ShouldUpdateNewSubheadingData()
        {
            var newGroupSubheading = new List<GroupingSubheading>
            {
                new GroupingSubheading {AreaTypeId = AreaTypeId, GroupId = GroupId, Sequence = 1, SubheadingId = 1, Subheading = "Test"},
                new GroupingSubheading {AreaTypeId = AreaTypeId, GroupId = GroupId, Sequence = 5, SubheadingId = -5, Subheading = "Test"}
            };

            SessionMock.Setup(x => x.CreateQuery(It.IsAny<string>())).Returns(Query1Mock.Object);
            Query1Mock.Setup(x => x.SetParameter("areaTypeId", It.IsAny<int>()));
            Query1Mock.Setup(x => x.SetParameter("groupId", It.IsAny<int>()));
            Query1Mock.SetReturnsDefault(GroupSubheadingInDb);

            SessionMock.Setup(x => x.BeginTransaction()).Returns(TransactionMock.Object);
            SessionMock.Setup(x => x.GetNamedQuery("Update_GroupingSubheading")).Returns(Query2Mock.Object);
            Query2Mock.Setup(x => x.SetParameter("GroupId", GroupId)).Returns(Query2Mock.Object);
            Query2Mock.Setup(x => x.SetParameter("AreaTypeId", AreaTypeId)).Returns(Query2Mock.Object);
            Query2Mock.Setup(x => x.SetParameter("Subheading", newGroupSubheading[0].Subheading)).Returns(Query2Mock.Object);
            Query2Mock.Setup(x => x.SetParameter("Sequence", newGroupSubheading[0].Sequence)).Returns(Query2Mock.Object);
            Query2Mock.Setup(x => x.SetParameter("SubheadingId", newGroupSubheading[0].SubheadingId)).Returns(Query2Mock.Object);
            Query2Mock.Setup(x => x.ExecuteUpdate()).Returns(1);

            SessionMock.Setup(x => x.GetNamedQuery("Insert_GroupingSubheading")).Returns(Query3Mock.Object);
            Query3Mock.Setup(x => x.SetParameter("GroupId", GroupId)).Returns(Query3Mock.Object);
            Query3Mock.Setup(x => x.SetParameter("AreaTypeId", AreaTypeId)).Returns(Query3Mock.Object);
            Query3Mock.Setup(x => x.SetParameter("Subheading", newGroupSubheading[1].Subheading)).Returns(Query3Mock.Object);
            Query3Mock.Setup(x => x.SetParameter("Sequence", newGroupSubheading[1].Sequence)).Returns(Query3Mock.Object);
            Query3Mock.Setup(x => x.ExecuteUpdate()).Returns(1);

            ProfAndIndiController.UpdateNewSubheadingData(newGroupSubheading);
        }
    }
}
