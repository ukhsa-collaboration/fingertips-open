using IndicatorsUI.MainUISeleniumTest;
using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IndicatorsUI.MainUITest.Helpers
{
    [TestClass]
    public class RepositoryHelperTests
    {
        private Mock<IRepositoryContainer> _repositoryContainerMock;

        [TestInitialize]
        public void Initialize()
        {
            _repositoryContainerMock = new Mock<IRepositoryContainer>(MockBehavior.Strict);
        }
        
        [TestCleanup]
        public void CleanUp()
        {
            _repositoryContainerMock.VerifyAll();
        }

        [TestMethod]
        public void Test_Should_Execute_Insert()
        {
            const int expectedNumber = 0;
            const string queryTest = "SQL Test query";

            var repositoryContainerTest = new RepositoryHelper(_repositoryContainerMock.Object);

            _repositoryContainerMock.Setup(x => x.ExecuteInsert(It.IsAny<string>())).Returns(expectedNumber);

            var result = repositoryContainerTest.ExecuteInsert(queryTest);

            Assert.AreEqual(expectedNumber, result);
        }

        [TestMethod]
        public void Test_Should_Execute_Update()
        {
            const string queryTest = "SQL Test query";

            var repositoryContainerTest = new RepositoryHelper(_repositoryContainerMock.Object);

            _repositoryContainerMock.Setup(x => x.ExecuteUpdate(It.IsAny<string>()));

            repositoryContainerTest.ExecuteUpdate(queryTest);
        }

        [TestMethod]
        public void Test_Should_Execute_Query()
        {
            const string queryTest = "SQL Test query";
            var expectedEnumerableObject = new object[]{1};

            var repositoryContainerTest = new RepositoryHelper(_repositoryContainerMock.Object);

            _repositoryContainerMock.Setup(x => x.ExecuteQuery(It.IsAny<string>())).Returns(expectedEnumerableObject);

            var result = repositoryContainerTest.ExecuteQuery(queryTest);

            Assert.AreEqual(expectedEnumerableObject, result);
        }
    }
}
