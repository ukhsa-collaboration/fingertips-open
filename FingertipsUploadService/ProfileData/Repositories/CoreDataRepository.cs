using FingertipsUploadService.ProfileData.Entities.Core;
using FingertipsUploadService.ProfileData.Helpers;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FingertipsUploadService.ProfileData.Repositories
{
    public class CoreDataRepository : RepositoryBase
    {
        const int NumberOfRowsBatch = 2000;
        const int ConnectionDBTimeOut = 120;

        // poor man injection, should be removed when we use DI containers
        public CoreDataRepository()
            : this(NHibernateSessionFactory.GetSession())
        {
        }

        public CoreDataRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        // If the method is called inside a secure execute, you must set secure to false
        public IEnumerable<CoreDataSet> GetCoreDataSets(IEnumerable<DuplicateRowInDatabaseError> duplicateRows, bool secure = true)
        {
            try
            {
                var query = new Func<IEnumerable<CoreDataSet>>(() =>
                    CurrentSession
                        .CreateQuery("from CoreDataSet cds where cds.Uid in (:Ids)")
                        .SetParameterList("Ids", duplicateRows.Select(x => x.Uid).ToList())
                        .List<CoreDataSet>());

                return secure ? SecureExecuteQuery(query) : query.Invoke();
            }
            catch (Exception exception)
            {
                HandleException(exception);
                throw;
            }
        }

        public void InsertCoreDataArchive(IEnumerable<DuplicateRowInDatabaseError> duplicateRows, Guid replacementUploadloadBatchId)
        {

            foreach (var duplicateBatch in duplicateRows.Batch(NumberOfRowsBatch))
            {
                if (duplicateBatch != null)
                {
                    try
                    {
                        // Wrap into an Action for secure transaction
                        var transactionAction = new Action(() => {
                            transaction = CurrentSession.BeginTransaction();

                            foreach (var coreDataSet in GetCoreDataSets(duplicateBatch, false))
                            {
                                var coreDataSetArchive = coreDataSet.ToCoreDataSetArchive();
                                coreDataSetArchive.ReplacedByUploadBatchId = replacementUploadloadBatchId;
                                CurrentSession.Save(coreDataSetArchive);
                            }
                            CurrentSession.CreateQuery("delete CoreDataSet c where c.Uid in (:idList)")
                                .SetParameterList("idList", duplicateBatch.Select(x => x.Uid).ToList())
                                .ExecuteUpdate();

                            transaction.Commit();
                        });

                        SecureExecuteTransaction(transactionAction);
                    }
                    catch (Exception exception)
                    {
                        HandleException(exception);
                    }
                }
            }
        }

        public int InsertCoreData(CoreDataSet coreDataSet, Guid uploadBatchId)
        {
            coreDataSet.UploadBatchId = uploadBatchId;

            // Wrap the sql action for a secure execution
            Func<CoreDataSet, int> sqlAction = (data) => (int) CurrentSession.Save(data);

            return SecureExecuteSqlAction(sqlAction, coreDataSet);
        }

        public void DeleteCoreDataArchive(Guid uploadBatchId)
        {
            try
            {
                // Wrap into an Action for secure transaction
                var transactionAction = new Action(() => {

                    transaction = CurrentSession.BeginTransaction();
                    
                    CurrentSession.CreateQuery("delete CoreDataSetArchive ca  where ca.UploadBatchId = :uploadBatchId")
                        .SetParameter("uploadBatchId", uploadBatchId)
                        .ExecuteUpdate();

                    transaction.Commit();
                });

                SecureExecuteTransaction(transactionAction);

            }
            catch (Exception exception)
            {

                HandleException(exception);
                throw;
            }
        }

        public void DeleteCoreData(Guid uploadBatchId)
        {
            try
            {
                // Wrap into an Action for secure transaction
                var transactionAction = new Action(() => {

                    transaction = CurrentSession.BeginTransaction();

                    CurrentSession.CreateQuery("delete CoreDataSet c where c.UploadBatchId = :uploadBatchId")
                        .SetParameter("uploadBatchId", uploadBatchId)
                        .ExecuteUpdate();

                    transaction.Commit();
                });

                SecureExecuteTransaction(transactionAction);

            }
            catch (Exception exception)
            {

                HandleException(exception);
                throw;
            }
        }

        /// <summary>
        /// Deletes core data that has been precalculated by Fingertips and stored in PHOLIO
        /// </summary>
        public void DeletePrecalculatedCoreData(int indicatorId)
        {
            try
            {
                // Wrap into an Action for secure transaction
                var transactionAction = new Action(() => {

                    transaction = CurrentSession.BeginTransaction();

                    CurrentSession.CreateQuery(
                            "delete CoreDataSet c where c.IndicatorId = :indicatorId and c.ValueNoteId = :valueNoteId")
                        .SetParameter("indicatorId", indicatorId)
                        .SetParameter("valueNoteId",
                            ValueNoteIds.AggregatedFromAllKnownLowerGeographyValuesByFingertips)
                        .SetTimeout(ConnectionDBTimeOut)
                        .ExecuteUpdate();

                    transaction.Commit();
                });

                SecureExecuteTransaction(transactionAction);
            }
            catch (Exception exception)
            {
                HandleException(exception);
                throw;
            }
        }

        // If the method is called inside a secure execute, you must set secure to false
        public IList<CoreDataSet> GetCoreDataSetByUploadJobId(Guid uploadBatchId, bool secure = true)
        {
            // Wrap the query for a secure execution
            var query = new Func<IList<CoreDataSet>>(() =>
                CurrentSession.CreateCriteria<CoreDataSet>()
                    .Add(Restrictions.Eq("UploadBatchId", uploadBatchId))
                    .List<CoreDataSet>());

            return secure ? SecureExecuteQuery(query) : query.Invoke();
        }

        // If the method is called inside a secure execute, you must set secure to false
        public IList<CoreDataSetDuplicateResponse> GetDuplicateCoreDataSetForAnIndicator(CoreDataSet data, bool secure = true)
        {
            // Wrap the query for a secure execution
            var query = new Func<IList<CoreDataSetDuplicateResponse>>(() =>
             CurrentSession.GetNamedQuery("Find_Duplicate_CoreDataSet_Rows_SP")
                .SetInt32("indicator_id", data.IndicatorId)
                .SetInt32("year", data.Year)
                .SetInt32("year_range", data.YearRange)
                .SetInt32("quarter", data.Quarter)
                .SetInt32("month", data.Month)
                .SetInt32("age_id", data.AgeId)
                .SetInt32("sex_id", data.SexId)
                .SetString("area_code", data.AreaCode)
                .SetInt32("category_type_id", data.CategoryTypeId)
                .SetInt32("category_id", data.CategoryId)
                .SetResultTransformer(Transformers.AliasToBean<CoreDataSetDuplicateResponse>())
                .List<CoreDataSetDuplicateResponse>());

            return secure ? SecureExecuteQuery(query) : query.Invoke();
        }
    }
}
