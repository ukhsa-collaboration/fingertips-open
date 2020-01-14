using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Core;
using FingertipsUploadService.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace FingertipsUploadService.ProfileDataTest.UnitTests
{
    [TestClass]
    public class CoreDataRepositoryTests
    {
        private CoreDataRepository _coreDataRepository;
        private Mock<ISessionFactory> _sessionFactoryMock;
        private Mock<IQuery> _queryMock;
        private Mock<ISession> _sessionMock;
        private Mock<Func<ISession>> _functionISessionMock;
        private Mock<Func<CoreDataSet, int>> _functionISessionCoreDataSetMock;
        private Mock<Action> _actionMock;
        private Mock<ITransaction> _transactionMock;
        private Mock<ICriteria> _criteriaMock;

        private const string QueryCoreDataSet = "from CoreDataSet cds where cds.Uid in (:Ids)";
        private const string Ids = "Ids";

        [TestInitialize]
        public void Init()
        {
            _sessionFactoryMock = new Mock<ISessionFactory>(MockBehavior.Strict);
            _queryMock = new Mock<IQuery>(MockBehavior.Strict);
            _sessionMock = new Mock<ISession>(MockBehavior.Strict);
            _functionISessionMock = new Mock<Func<ISession>>(MockBehavior.Strict);
            _functionISessionCoreDataSetMock = new Mock<Func<CoreDataSet, int>>(MockBehavior.Strict);
            _actionMock = new Mock<Action>(MockBehavior.Strict);
            _transactionMock = new Mock<ITransaction>(MockBehavior.Strict);
            _criteriaMock = new Mock<ICriteria>(MockBehavior.Strict);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _sessionFactoryMock.VerifyAll();
            _sessionMock.VerifyAll();
            _queryMock.VerifyAll();
            _functionISessionMock.VerifyAll();
            _functionISessionCoreDataSetMock.VerifyAll();
            _actionMock.VerifyAll();
        }

        [TestMethod]
        public void ShouldGetCoreDataSetsSecureTest()
        {
            // Arrange
            
            _sessionMock.Setup(x => x.IsOpen).Returns(true);
            _sessionMock.Setup(x => x.Close()).Returns((IDbConnection)null);
            _sessionMock.Setup(x => x.Dispose());
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _sessionMock.Setup(x => x.CreateQuery(QueryCoreDataSet)).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetParameterList(Ids, It.IsAny<IEnumerable>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.List<CoreDataSet>()).Returns(new List<CoreDataSet>());

            _coreDataRepository = new CoreDataRepository(_sessionFactoryMock.Object);

            // Act
            try
            {
                var result = _coreDataRepository.GetCoreDataSets(new List<DuplicateRowInDatabaseError>());

                Assert.IsNotNull(result);
                Assert.IsTrue(result is IEnumerable<CoreDataSet>);
            }
            catch (Exception e)
            {
                Assert.Fail("The session shouldn't throw any exception: " + e.Message);
            }
        }

        [TestMethod]
        public void ShouldGetCoreDataSetsNotSecureTest()
        {
            // Arrange
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _sessionMock.Setup(x => x.CreateQuery(QueryCoreDataSet)).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetParameterList(Ids, It.IsAny<IEnumerable>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.List<CoreDataSet>()).Returns(new List<CoreDataSet>());

            _coreDataRepository = new CoreDataRepository(_sessionFactoryMock.Object);

            // Act
            try
            {
                var result = _coreDataRepository.GetCoreDataSets(new List<DuplicateRowInDatabaseError>(), false);

                Assert.IsNotNull(result);
                Assert.IsTrue(result is IEnumerable<CoreDataSet>);
            }
            catch (Exception e)
            {
                Assert.Fail("The session shouldn't throw any exception: " + e.Message);
            }
        }

        [TestMethod]
        public void InsertCoreDataArchiveTest()
        {
            // Arrange
            const string deleteCoreDataSet = "delete CoreDataSet c where c.Uid in (:idList)";
            const string idList = "idList";
            var listOfDuplicateList = new List<DuplicateRowInDatabaseError> { new DuplicateRowInDatabaseError() };

            _sessionMock.Setup(x => x.IsOpen).Returns(true);
            _sessionMock.Setup(x => x.Close()).Returns((IDbConnection)null);
            _sessionMock.Setup(x => x.Dispose());
            _sessionMock.Setup(x => x.BeginTransaction()).Returns(_transactionMock.Object);
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _sessionMock.Setup(x => x.CreateQuery(QueryCoreDataSet)).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetParameterList(Ids, It.IsAny<IEnumerable>())).Returns(_queryMock.Object);
            _sessionMock.Setup(x => x.CreateQuery(deleteCoreDataSet)).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetParameterList(idList, It.IsAny<IEnumerable>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.List<CoreDataSet>()).Returns(new List<CoreDataSet>());
            _queryMock.Setup(x => x.ExecuteUpdate()).Returns(int.MaxValue);
            _transactionMock.Setup(x => x.WasRolledBack).Returns(false);
            _transactionMock.Setup(x => x.Commit());

            _coreDataRepository = new CoreDataRepository(_sessionFactoryMock.Object);

            // Act
            try
            {
                _coreDataRepository.InsertCoreDataArchive(listOfDuplicateList, It.IsAny<Guid>());
            }
            catch (Exception e)
            {
                Assert.Fail("The session shouldn't throw any exception: " + e.Message);
            }
        }

        [TestMethod]
        public void InsertCoreDataArchiveRollbackTest()
        {
            // Arrange
            var queryCoreDataSet2 = "delete CoreDataSet c where c.Uid in (:idList)";
            var parameterList2 = "idList";
            var listOfDuplicateList = new List<DuplicateRowInDatabaseError> { new DuplicateRowInDatabaseError() };

            _sessionMock.Setup(x => x.BeginTransaction()).Returns(_transactionMock.Object);
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _sessionMock.Setup(x => x.CreateQuery(QueryCoreDataSet)).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetParameterList(Ids, It.IsAny<IEnumerable>())).Returns(_queryMock.Object);
            _sessionMock.Setup(x => x.CreateQuery(queryCoreDataSet2)).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetParameterList(parameterList2, It.IsAny<IEnumerable>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.List<CoreDataSet>()).Returns(new List<CoreDataSet>());
            _queryMock.Setup(x => x.ExecuteUpdate()).Throws<Exception>();
            _transactionMock.Setup(x => x.WasRolledBack).Returns(false);
            _transactionMock.Setup(x => x.Rollback());

            _coreDataRepository = new CoreDataRepository(_sessionFactoryMock.Object);

            // Act
            try
            {
                _coreDataRepository.InsertCoreDataArchive(listOfDuplicateList, It.IsAny<Guid>());
            }
            catch (Exception e)
            {
                Assert.AreEqual("Exception of type 'System.Exception' was thrown.", e.Message);
            }
        }

        [TestMethod]
        public void InsertCoreDataTest()
        {
            // Arrange
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _sessionMock.Setup(x => x.Save(It.IsAny<CoreDataSet>())).Returns(int.MaxValue);
            _sessionMock.Setup(x => x.IsOpen).Returns(false);

            _coreDataRepository = new CoreDataRepository(_sessionFactoryMock.Object);

            // Act
            try
            {
                var result = _coreDataRepository.InsertCoreData(new CoreDataSet(), It.IsAny<Guid>());

                Assert.AreEqual(int.MaxValue, result);
            }
            catch (Exception e)
            {
                Assert.Fail("The session shouldn't throw any exception: " + e.Message);
            }
        }

        [TestMethod]
        public void DeleteCoreDataArchiveTest()
        {
            // Arrange
            const string deleteCoreDataSetArchive = "delete CoreDataSetArchive ca  where ca.UploadBatchId = :uploadBatchId";
            const string uploadBatchId = "uploadBatchId";

            var guid = new Guid();

            _sessionMock.Setup(x => x.IsOpen).Returns(true);
            _sessionMock.Setup(x => x.Close()).Returns((IDbConnection)null);
            _sessionMock.Setup(x => x.Dispose());
            _sessionMock.Setup(x => x.BeginTransaction()).Returns(_transactionMock.Object);
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _sessionMock.Setup(x => x.CreateQuery(deleteCoreDataSetArchive)).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetParameter(uploadBatchId, It.IsAny<Guid>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.ExecuteUpdate()).Returns(int.MaxValue);
            _transactionMock.Setup(x => x.Commit());

            _coreDataRepository = new CoreDataRepository(_sessionFactoryMock.Object);

            // Act
            try
            {
                _coreDataRepository.DeleteCoreDataArchive(guid);
            }
            catch (Exception e)
            {
                Assert.Fail("The session shouldn't throw any exception: " + e.Message);
            }
        }

        [TestMethod]
        public void DeleteCoreDataArchiveRollbackTest()
        {
            // Arrange
            const string deleteCoreDataSetArchive = "delete CoreDataSetArchive ca  where ca.UploadBatchId = :uploadBatchId";
            const string uploadBatchId = "uploadBatchId";

            var guid = new Guid();

            _sessionMock.Setup(x => x.BeginTransaction()).Returns(_transactionMock.Object);
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _sessionMock.Setup(x => x.CreateQuery(deleteCoreDataSetArchive)).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetParameter(uploadBatchId, It.IsAny<Guid>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.ExecuteUpdate()).Throws<Exception>();
            _transactionMock.Setup(x => x.WasRolledBack).Returns(false);
            _transactionMock.Setup(x => x.Rollback());

            _coreDataRepository = new CoreDataRepository(_sessionFactoryMock.Object);

            // Act
            try
            {
                _coreDataRepository.DeleteCoreDataArchive(guid);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Exception of type 'System.Exception' was thrown.", e.Message);
            }
        }

        [TestMethod]
        public void DeleteCoreDataTest()
        {
            // Arrange
            const string deleteCoreDataSet = "delete CoreDataSet c where c.UploadBatchId = :uploadBatchId";
            const string uploadBatchId = "uploadBatchId";

            var guid = new Guid();

            _sessionMock.Setup(x => x.IsOpen).Returns(true);
            _sessionMock.Setup(x => x.Close()).Returns((IDbConnection)null);
            _sessionMock.Setup(x => x.Dispose());
            _sessionMock.Setup(x => x.BeginTransaction()).Returns(_transactionMock.Object);
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _sessionMock.Setup(x => x.CreateQuery(deleteCoreDataSet)).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetParameter(uploadBatchId, It.IsAny<Guid>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.ExecuteUpdate()).Returns(int.MaxValue);
            _transactionMock.Setup(x => x.Commit());

            _coreDataRepository = new CoreDataRepository(_sessionFactoryMock.Object);

            // Act
            try
            {
                _coreDataRepository.DeleteCoreData(guid);
            }
            catch (Exception e)
            {
                Assert.Fail("The session shouldn't throw any exception: " + e.Message);
            }
        }

        [TestMethod]
        public void DeleteCoreDataRollbackTest()
        {
            // Arrange
            const string deleteCoreDataSet = "delete CoreDataSet c where c.UploadBatchId = :uploadBatchId";
            const string uploadBatchId = "uploadBatchId";

            var guid = new Guid();

            _sessionMock.Setup(x => x.BeginTransaction()).Returns(_transactionMock.Object);
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _sessionMock.Setup(x => x.CreateQuery(deleteCoreDataSet)).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetParameter(uploadBatchId, It.IsAny<Guid>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.ExecuteUpdate()).Throws<Exception>();
            _transactionMock.Setup(x => x.WasRolledBack).Returns(false);
            _transactionMock.Setup(x => x.Rollback());

            _coreDataRepository = new CoreDataRepository(_sessionFactoryMock.Object);

            // Act
            try
            {
                _coreDataRepository.DeleteCoreData(guid);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Exception of type 'System.Exception' was thrown.", e.Message);
            }
        }

        [TestMethod]
        public void DeletePrecalculatedCoreDataTest()
        {
            // Arrange
            const string deleteCoreDataSet = "delete CoreDataSet c where c.IndicatorId = :indicatorId and c.ValueNoteId = :valueNoteId";
            const string indicatorId = "indicatorId";
            const string valueNoteId = "valueNoteId";

            _sessionMock.Setup(x => x.IsOpen).Returns(true);
            _sessionMock.Setup(x => x.Close()).Returns((IDbConnection)null);
            _sessionMock.Setup(x => x.Dispose());
            _sessionMock.Setup(x => x.BeginTransaction()).Returns(_transactionMock.Object);
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _sessionMock.Setup(x => x.CreateQuery(deleteCoreDataSet)).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetParameter(indicatorId, It.IsAny<int>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetParameter(valueNoteId, It.IsAny<int>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetTimeout(It.IsAny<int>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.ExecuteUpdate()).Returns(int.MaxValue);
            _transactionMock.Setup(x => x.Commit());

            _coreDataRepository = new CoreDataRepository(_sessionFactoryMock.Object);

            // Act
            try
            {
                _coreDataRepository.DeletePrecalculatedCoreData(It.IsAny<int>());
            }
            catch (Exception e)
            {
                Assert.Fail("The session shouldn't throw any exception: " + e.Message);
            }
        }

        [TestMethod]
        public void DeletePrecalculatedCoreDataRollbackTest()
        {
            // Arrange
            const string deleteCoreDataSet = "delete CoreDataSet c where c.IndicatorId = :indicatorId and c.ValueNoteId = :valueNoteId";
            const string indicatorId = "indicatorId";
            const string valueNoteId = "valueNoteId";

            _sessionMock.Setup(x => x.BeginTransaction()).Returns(_transactionMock.Object);
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _sessionMock.Setup(x => x.CreateQuery(deleteCoreDataSet)).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetParameter(indicatorId, It.IsAny<int>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetParameter(valueNoteId, It.IsAny<int>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetTimeout(It.IsAny<int>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.ExecuteUpdate()).Throws<Exception>();
            _transactionMock.Setup(x => x.WasRolledBack).Returns(false);
            _transactionMock.Setup(x => x.Rollback());

            _coreDataRepository = new CoreDataRepository(_sessionFactoryMock.Object);

            // Act
            try
            {
                _coreDataRepository.DeletePrecalculatedCoreData(It.IsAny<int>());
            }
            catch (Exception e)
            {
                Assert.AreEqual("Exception of type 'System.Exception' was thrown.", e.Message);
            }
        }

        [TestMethod]
        public void ShouldGetCoreDataSetByUploadJobIdSecureTest()
        {
            // Arrange
            var guid = new Guid();
            _sessionMock.Setup(x => x.IsOpen).Returns(true);
            _sessionMock.Setup(x => x.Close()).Returns((IDbConnection)null);
            _sessionMock.Setup(x => x.Dispose());
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _sessionMock.Setup(x => x.CreateCriteria<CoreDataSet>()).Returns(_criteriaMock.Object);
            _criteriaMock.Setup(x => x.Add(It.IsAny<SimpleExpression>())).Returns(_criteriaMock.Object);
            _criteriaMock.Setup(x => x.List<CoreDataSet>()).Returns(new List<CoreDataSet>());

            _coreDataRepository = new CoreDataRepository(_sessionFactoryMock.Object);

            // Act
            try
            {
                var result = _coreDataRepository.GetCoreDataSetByUploadJobId(guid);

                Assert.IsNotNull(result);
                Assert.IsTrue(result is IEnumerable<CoreDataSet>);
            }
            catch (Exception e)
            {
                Assert.Fail("The session shouldn't throw any exception: " + e.Message);
            }
        }

        [TestMethod]
        public void ShouldGetCoreDataSetByUploadJobIdNotSecureTest()
        {
            // Arrange
            var guid = new Guid();
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _sessionMock.Setup(x => x.CreateCriteria<CoreDataSet>()).Returns(_criteriaMock.Object);
            _criteriaMock.Setup(x => x.Add(It.IsAny<SimpleExpression>())).Returns(_criteriaMock.Object);
            _criteriaMock.Setup(x => x.List<CoreDataSet>()).Returns(new List<CoreDataSet>());

            _coreDataRepository = new CoreDataRepository(_sessionFactoryMock.Object);

            // Act
            try
            {
                var result = _coreDataRepository.GetCoreDataSetByUploadJobId(guid, false);

                Assert.IsNotNull(result);
                Assert.IsTrue(result is IEnumerable<CoreDataSet>);
            }
            catch (Exception e)
            {
                Assert.Fail("The session shouldn't throw any exception: " + e.Message);
            }
        }

        [TestMethod]
        public void ShouldDuplicateCoreDataSetForAnIndicatorSecureTest()
        {
            // Arrange
            _sessionMock.Setup(x => x.IsOpen).Returns(true);
            _sessionMock.Setup(x => x.Close()).Returns((IDbConnection)null);
            _sessionMock.Setup(x => x.Dispose());
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _sessionMock.Setup(x => x.GetNamedQuery("Find_Duplicate_CoreDataSet_Rows_SP")).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetInt32("indicator_id", It.IsAny<int>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetInt32("year", It.IsAny<int>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetInt32("year_range", It.IsAny<int>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetInt32("quarter", It.IsAny<int>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetInt32("month", It.IsAny<int>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetInt32("age_id", It.IsAny<int>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetInt32("sex_id", It.IsAny<int>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetString("area_code", It.IsAny<string>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetInt32("category_type_id", It.IsAny<int>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetInt32("category_id", It.IsAny<int>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetResultTransformer(It.IsAny<IResultTransformer>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.List<CoreDataSetDuplicateResponse>()).Returns(new List<CoreDataSetDuplicateResponse>());

            _coreDataRepository = new CoreDataRepository(_sessionFactoryMock.Object);

            // Act
            try
            {
                var result = _coreDataRepository.GetDuplicateCoreDataSetForAnIndicator(new CoreDataSet());

                Assert.IsNotNull(result);
                Assert.IsTrue(result is IEnumerable<CoreDataSetDuplicateResponse>);
            }
            catch (Exception e)
            {
                Assert.Fail("The session shouldn't throw any exception: " + e.Message);
            }
        }

        [TestMethod]
        public void ShouldDuplicateCoreDataSetForAnIndicatorNotSecureTest()
        {
            // Arrange
            _sessionFactoryMock.Setup(x => x.OpenSession()).Returns(_sessionMock.Object);
            _sessionMock.Setup(x => x.GetNamedQuery("Find_Duplicate_CoreDataSet_Rows_SP")).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetInt32("indicator_id", It.IsAny<int>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetInt32("year", It.IsAny<int>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetInt32("year_range", It.IsAny<int>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetInt32("quarter", It.IsAny<int>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetInt32("month", It.IsAny<int>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetInt32("age_id", It.IsAny<int>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetInt32("sex_id", It.IsAny<int>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetString("area_code", It.IsAny<string>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetInt32("category_type_id", It.IsAny<int>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetInt32("category_id", It.IsAny<int>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.SetResultTransformer(It.IsAny<IResultTransformer>())).Returns(_queryMock.Object);
            _queryMock.Setup(x => x.List<CoreDataSetDuplicateResponse>()).Returns(new List<CoreDataSetDuplicateResponse>());

            _coreDataRepository = new CoreDataRepository(_sessionFactoryMock.Object);

            // Act
            try
            {
                var result = _coreDataRepository.GetDuplicateCoreDataSetForAnIndicator(new CoreDataSet(), false);

                Assert.IsNotNull(result);
                Assert.IsTrue(result is IEnumerable<CoreDataSetDuplicateResponse>);
            }
            catch (Exception e)
            {
                Assert.Fail("The session shouldn't throw any exception: " + e.Message);
            }
        }
    }
}
