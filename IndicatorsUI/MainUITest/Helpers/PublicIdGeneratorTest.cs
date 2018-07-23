using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using IndicatorsUI.MainUI.Helpers;
using IndicatorsUI.UserAccess;
using IndicatorsUI.UserAccess.UserList.IRepository;

namespace IndicatorsUI.MainUITest.Helpers
{
    [TestClass]
    public class PublicIdGeneratorTest
    {
        private Mock<IIndicatorListRepository> _repo;

        [TestInitialize]
        public void TestInitialize()
        {
            _repo = new Mock<IIndicatorListRepository>(MockBehavior.Strict);
        }

        [TestMethod]
        public void Test_UID_Generated_Ok()
        {
            _repo.Setup(x => x.GetListByPublicId(It.IsAny<string>()))
                .Returns((IndicatorList)null);

            AssertIdGeneratedOk();
        }

        [TestMethod]
        public void Test_Repeated_UID_Check_Until_Unique()
        {
            _repo.Setup(x => x.GetListByPublicId(It.IsAny<string>()))
                .Returns(new IndicatorList());
            _repo.Setup(x => x.GetListByPublicId(It.IsAny<string>()))
                .Returns(new IndicatorList());
            _repo.Setup(x => x.GetListByPublicId(It.IsAny<string>()))
                .Returns((IndicatorList)null);

            AssertIdGeneratedOk();
        }

        private void AssertIdGeneratedOk()
        {
            // Check ID has expected length
            Assert.AreEqual(PublicIdGenerator.UidLength, 
                new PublicIdGenerator(_repo.Object).GetIndicatorListPublicId().Length);
            _repo.VerifyAll();
        }
    }
}
