using System;
using IndicatorsUI.MainUISeleniumTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;

namespace IndicatorsUI.MainUITest.Helpers
{
    [TestClass]
    public class RepositoryContainerTests
    {
        private Mock<ISessionFactory> _sessionFactoryMock;
        private Mock<ISession> _sessionMock;
        private Mock<ITransaction> _transactionMock;
        private Mock<ISQLQuery> _sqlQueryMock;

        [TestInitialize]
        public void Initialize()
        {
            _transactionMock = new Mock<ITransaction>(MockBehavior.Loose);
            _sessionMock = new Mock<ISession>(MockBehavior.Loose);
            _sessionFactoryMock = new Mock<ISessionFactory>(MockBehavior.Loose);
            _sqlQueryMock = new Mock<ISQLQuery>(MockBehavior.Loose);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _transactionMock.VerifyAll();
            _sessionMock.VerifyAll();
            _sessionFactoryMock.VerifyAll();
            _sqlQueryMock.VerifyAll();
        }

        [TestMethod]
        public void Test_Should_Execute_Insert_Without_result_id()
        {
            const int expectedNumber = 0;
            const string queryTest = "SQL Test query without result it";

            var repositoryContainerTest = new RepositoryContainer(_sessionFactoryMock.Object);

            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _sessionMock.Setup(x => x.BeginTransaction()).Returns(_transactionMock.Object);

            _sqlQueryMock.Setup(x => x.UniqueResult());
            _sessionMock.Setup(x => x.CreateSQLQuery(It.IsAny<string>())).Returns(_sqlQueryMock.Object);

            var result = repositoryContainerTest.ExecuteInsert(queryTest);

            Assert.AreEqual(expectedNumber, result);
        }

        [TestMethod]
        public void Test_Should_Execute_Insert_With_result_id()
        {
            const int expectedNumber = 1;
            const string queryTest = "SQL Test query with OUTPUT Inserted.ID";

            var repositoryContainerTest = new RepositoryContainer(_sessionFactoryMock.Object);

            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _sessionMock.Setup(x => x.BeginTransaction()).Returns(_transactionMock.Object);

            _sqlQueryMock.Setup(x => x.UniqueResult()).Returns(expectedNumber);
            _sessionMock.Setup(x => x.CreateSQLQuery(It.IsAny<string>())).Returns(_sqlQueryMock.Object);

            var result =  repositoryContainerTest.ExecuteInsert(queryTest);

            Assert.AreEqual(expectedNumber, result);
        }

        [TestMethod]
        public void Test_Should_Execute_Update()
        {
            try
            {
                const string queryTest = "SQL Test query";

                var repositoryContainerTest = new RepositoryContainer(_sessionFactoryMock.Object);

                _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
                _sessionMock.Setup(x => x.BeginTransaction()).Returns(_transactionMock.Object);

                _sqlQueryMock.Setup(x => x.UniqueResult());
                _sessionMock.Setup(x => x.CreateSQLQuery(It.IsAny<string>())).Returns(_sqlQueryMock.Object);

                repositoryContainerTest.ExecuteUpdate(queryTest);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void Test_Should_Fail_Execute_Update()
        {
            try
            {
                const string queryTest = "SQL Test query";

                var repositoryContainerTest = new RepositoryContainer(_sessionFactoryMock.Object);

                _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
                _sessionMock.Setup(x => x.BeginTransaction()).Returns(_transactionMock.Object);

                _sqlQueryMock.Setup(x => x.UniqueResult()).Throws<Exception>();
                _sessionMock.Setup(x => x.CreateSQLQuery(It.IsAny<string>())).Returns(_sqlQueryMock.Object);

                repositoryContainerTest.ExecuteUpdate(queryTest);

                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("The executed sql query failed", e.Message);
            }
        }

        [TestMethod]
        public void Test_Should_Execute_Query()
        {
            const string queryTest = "SQL Test query";
            var expectedValue = new object[] { 0 };

            var repositoryContainerTest = new RepositoryContainer(_sessionFactoryMock.Object);

            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _sessionMock.Setup(x => x.BeginTransaction()).Returns(_transactionMock.Object);

            _sqlQueryMock.Setup(x => x.List<object>()).Returns(expectedValue);
            _sessionMock.Setup(x => x.CreateSQLQuery(It.IsAny<string>())).Returns(_sqlQueryMock.Object);

            var result = repositoryContainerTest.ExecuteQuery(queryTest);

            Assert.AreEqual(expectedValue, result);
        }
    }
}
