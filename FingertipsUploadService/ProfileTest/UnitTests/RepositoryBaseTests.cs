using System;
using System.Data;
using FingertipsUploadService.ProfileData.Entities.Core;
using FingertipsUploadService.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;

namespace FingertipsUploadService.ProfileDataTest.UnitTests
{
    [TestClass]
    public class RepositoryBaseTests
    {
        private RepositoryBaseTestHelper _repositoryBaseTestHelper;
        private Mock<ISessionFactory> _sessionFactoryMock;
        private Mock<ITransaction> _transactionMock;
        private Mock<ISession> _sessionMock;
        private Mock<Func<ISession>> _functionISessionMock;
        private Mock<Func<CoreDataSet, int>> _functionISessionCoreDataSetMock;
        private Mock<Action> _actionMock;

        [TestInitialize]
        public void Init()
        {
            _sessionFactoryMock = new Mock<ISessionFactory>(MockBehavior.Strict);
            _transactionMock = new Mock<ITransaction>(MockBehavior.Strict);
            _sessionMock = new Mock<ISession>(MockBehavior.Strict);
            _functionISessionMock = new Mock<Func<ISession>>(MockBehavior.Strict);
            _functionISessionCoreDataSetMock = new Mock<Func<CoreDataSet, int>>(MockBehavior.Strict);
            _actionMock = new Mock<Action>(MockBehavior.Strict);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _sessionFactoryMock.VerifyAll();
            _transactionMock.VerifyAll();
            _sessionMock.VerifyAll();
            _functionISessionMock.VerifyAll();
            _functionISessionCoreDataSetMock.VerifyAll();
            _actionMock.VerifyAll();
        }

        [TestMethod]
        public void ShouldOpenSessionTest()
        {
            // Arrange
            _sessionMock.Setup(x => x.IsOpen).Returns(true);
            _sessionMock.Setup(x => x.Close()).Returns((IDbConnection)null);
            _sessionMock.Setup(x => x.Dispose());
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _repositoryBaseTestHelper = new RepositoryBaseTestHelper(_sessionFactoryMock.Object);

            _repositoryBaseTestHelper.Dispose();

            // Act
            try
            {
                _repositoryBaseTestHelper.OpenSession();
            }
            catch (Exception e)
            {
               Assert.Fail("The session shouldn't throw any exception: " + e.Message);
            }
        }

        [TestMethod]
        public void ShouldNotOpenSessionTest()
        {
            // Arrange
            _sessionMock.Setup(x => x.IsOpen).Returns(true);
            _sessionMock.Setup(x => x.Close()).Returns((IDbConnection)null);
            _sessionMock.Setup(x => x.Dispose());
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _repositoryBaseTestHelper = new RepositoryBaseTestHelper(_sessionFactoryMock.Object);

            // Act
            try
            {
                _repositoryBaseTestHelper.OpenSession();
                _repositoryBaseTestHelper.Dispose();
            }
            catch (Exception e)
            {
                Assert.Fail("The session shouldn't throw any exception: " + e.Message);
            }
        }

        [TestMethod]
        public void ShouldCloseSessionTest()
        {
            // Arrange
            _sessionMock.Setup(x => x.IsOpen).Returns(true);
            _sessionMock.Setup(x => x.Close()).Returns((IDbConnection)null);
            _sessionMock.Setup(x => x.Dispose());
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _repositoryBaseTestHelper = new RepositoryBaseTestHelper(_sessionFactoryMock.Object);

            // Act
            _repositoryBaseTestHelper.Dispose();   
        }

        [TestMethod]
        public void ShouldNotCloseSessionTest()
        {
            // Arrange
            _sessionMock.Setup(x => x.IsOpen).Returns(false);
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _repositoryBaseTestHelper = new RepositoryBaseTestHelper(_sessionFactoryMock.Object);

            // Act
            _repositoryBaseTestHelper.Dispose();
        }

        [TestMethod]
        public void ShouldSecureExecuteQueryTest()
        {
            // Arrange
            _sessionMock.Setup(x => x.IsOpen).Returns(true);
            _sessionMock.Setup(x => x.Close()).Returns((IDbConnection)null);
            _sessionMock.Setup(x => x.Dispose());
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _repositoryBaseTestHelper = new RepositoryBaseTestHelper(_sessionFactoryMock.Object);
            _functionISessionMock.Setup(x => x()).Returns(_sessionMock.Object);

            _repositoryBaseTestHelper.Dispose();

            // Act
            try
            {
                var result = _repositoryBaseTestHelper.SecureExecuteQuery(_functionISessionMock.Object);

                Assert.IsNotNull(result);
                Assert.IsTrue(result is ISession);
            }
            catch (Exception e)
            {
                Assert.Fail("The session shouldn't throw any exception: " + e.Message);
            }
        }

        [TestMethod]
        public void ShouldHandleExceptionSecureExecuteQueryTest()
        {
            // Arrange
            _sessionMock.Setup(x => x.IsOpen).Returns(true);
            _sessionMock.Setup(x => x.Close()).Returns((IDbConnection)null);
            _sessionMock.Setup(x => x.Dispose());
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _repositoryBaseTestHelper = new RepositoryBaseTestHelper(_sessionFactoryMock.Object);
            _functionISessionMock.Setup(x => x()).Throws<Exception>();
            _transactionMock.Setup(x => x.WasRolledBack).Returns(false);
            _transactionMock.Setup(x => x.Rollback());

            _repositoryBaseTestHelper.AddTransaction(_transactionMock.Object);
            _repositoryBaseTestHelper.Dispose();

            // Act
            try
            {
                _repositoryBaseTestHelper.SecureExecuteQuery(_functionISessionMock.Object);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Exception of type 'System.Exception' was thrown.", e.Message);
            }
        }

        [TestMethod]
        public void ShouldSecureExecuteSqlActionTest()
        {
            // Arrange
            var coreDataSet = new CoreDataSet();
            _sessionMock.Setup(x => x.IsOpen).Returns(true);
            _sessionMock.Setup(x => x.Close()).Returns((IDbConnection)null);
            _sessionMock.Setup(x => x.Dispose());
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _repositoryBaseTestHelper = new RepositoryBaseTestHelper(_sessionFactoryMock.Object);
            _functionISessionCoreDataSetMock.Setup(x => x(coreDataSet)).Returns(int.MaxValue);

            _repositoryBaseTestHelper.Dispose();

            // Act
            try
            {
                var result = _repositoryBaseTestHelper.SecureExecuteSqlAction(_functionISessionCoreDataSetMock.Object, coreDataSet);

                Assert.IsNotNull(result);
                Assert.IsTrue(result == int.MaxValue);
            }
            catch (Exception e)
            {
                Assert.Fail("The session shouldn't throw any exception: " + e.Message);
            }
        }

        [TestMethod]
        public void ShouldHandleExceptionSecureExecuteSqlActionTest()
        {
            // Arrange
            var coreDataSet = new CoreDataSet();
            _sessionMock.Setup(x => x.IsOpen).Returns(true);
            _sessionMock.Setup(x => x.Close()).Returns((IDbConnection)null);
            _sessionMock.Setup(x => x.Dispose());
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _repositoryBaseTestHelper = new RepositoryBaseTestHelper(_sessionFactoryMock.Object);
            _functionISessionCoreDataSetMock.Setup(x => x(coreDataSet)).Throws<Exception>();
            _transactionMock.Setup(x => x.WasRolledBack).Returns(false);
            _transactionMock.Setup(x => x.Rollback());

            _repositoryBaseTestHelper.AddTransaction(_transactionMock.Object);
            _repositoryBaseTestHelper.Dispose();

            // Act
            try
            {
                _repositoryBaseTestHelper.SecureExecuteSqlAction(_functionISessionCoreDataSetMock.Object, coreDataSet);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Exception of type 'System.Exception' was thrown.", e.Message);
            }
        }

        [TestMethod]
        public void ShouldExecuteTransactionTest()
        {
            // Arrange
            _sessionMock.Setup(x => x.IsOpen).Returns(true);
            _sessionMock.Setup(x => x.Close()).Returns((IDbConnection)null);
            _sessionMock.Setup(x => x.Dispose());
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _repositoryBaseTestHelper = new RepositoryBaseTestHelper(_sessionFactoryMock.Object);
            _actionMock.Setup(x => x());

            _repositoryBaseTestHelper.Dispose();

            // Act
            try
            {
                _repositoryBaseTestHelper.SecureExecuteTransaction(_actionMock.Object);
            }
            catch (Exception e)
            {
                Assert.Fail("The session shouldn't throw any exception: " + e.Message);
            }
        }

        [TestMethod]
        public void ShouldHandleExecuteTransactionTest()
        {
            // Arrange
            _sessionMock.Setup(x => x.IsOpen).Returns(true);
            _sessionMock.Setup(x => x.Close()).Returns((IDbConnection)null);
            _sessionMock.Setup(x => x.Dispose());
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _repositoryBaseTestHelper = new RepositoryBaseTestHelper(_sessionFactoryMock.Object);
            _actionMock.Setup(x => x()).Throws<Exception>();
            _transactionMock.Setup(x => x.WasRolledBack).Returns(false);
            _transactionMock.Setup(x => x.Rollback());

            _repositoryBaseTestHelper.AddTransaction(_transactionMock.Object);
            _repositoryBaseTestHelper.Dispose();

            // Act
            try
            {
                _repositoryBaseTestHelper.SecureExecuteTransaction(_actionMock.Object);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Exception of type 'System.Exception' was thrown.", e.Message);
            }
        }
    }
}
