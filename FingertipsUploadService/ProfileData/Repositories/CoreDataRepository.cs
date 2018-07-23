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
        const int NumberOfRowsBatch = 100;
        // poor man injection, should be removed when we use DI containers
        public CoreDataRepository()
            : this(NHibernateSessionFactory.GetSession())
        {
        }

        public CoreDataRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public IEnumerable<CoreDataSet> GetCoreDataSets(IEnumerable<DuplicateRowInDatabaseError> duplicateRows)
        {
            return CurrentSession
                    .CreateQuery("from CoreDataSet cds where cds.Uid in (:Ids)")
                    .SetParameterList("Ids", duplicateRows.Select(x => x.Uid).ToList())
                    .List<CoreDataSet>();
        }

        public void InsertCoreDataArchive(IEnumerable<DuplicateRowInDatabaseError> duplicateRows, Guid replacementUploadloadBatchId)
        {

            foreach (var duplicateBatch in duplicateRows.Batch(NumberOfRowsBatch))
            {
                if (duplicateBatch != null)
                {
                    try
                    {
                        transaction = CurrentSession.BeginTransaction();

                        foreach (var coreDataSet in GetCoreDataSets(duplicateBatch))
                        {
                            var coreDataSetArchive = coreDataSet.ToCoreDataSetArchive();
                            coreDataSetArchive.ReplacedByUploadBatchId = replacementUploadloadBatchId;
                            CurrentSession.Save(coreDataSetArchive);
                        }


                        CurrentSession.CreateQuery("delete CoreDataSet c where c.Uid in (:idList)")
                       .SetParameterList("idList", duplicateBatch.Select(x => x.Uid).ToList())
                       .ExecuteUpdate();

                        transaction.Commit();
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
            return (int)CurrentSession.Save(coreDataSet);
        }

        public void DeleteCoreDataArchive(Guid uploadBatchId)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();


                CurrentSession.CreateQuery("delete CoreDataSetArchive ca  where ca.UploadBatchId = :uploadBatchId")
                    .SetParameter("uploadBatchId", uploadBatchId)
                    .ExecuteUpdate();

                transaction.Commit();

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
                transaction = CurrentSession.BeginTransaction();

                CurrentSession.CreateQuery("delete CoreDataSet c where c.UploadBatchId = :uploadBatchId")
                    .SetParameter("uploadBatchId", uploadBatchId)
                    .ExecuteUpdate();

                transaction.Commit();

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
                transaction = CurrentSession.BeginTransaction();

                CurrentSession.CreateQuery(
                    "delete CoreDataSet c where c.IndicatorId = :indicatorId and c.ValueNoteId = :valueNoteId")
                    .SetParameter("indicatorId", indicatorId)
                    .SetParameter("valueNoteId", ValueNoteIds.AggregatedFromAllKnownLowerGeographyValuesByFingertips)
                    .ExecuteUpdate();

                transaction.Commit();
            }
            catch (Exception exception)
            {

                HandleException(exception);
                throw;
            }
        }

        public IList<CoreDataSet> GetCoreDataSetByUploadJobId(Guid uploadBatchId)
        {
            return CurrentSession.CreateCriteria<CoreDataSet>()
                .Add(Restrictions.Eq("UploadBatchId", uploadBatchId))
                .List<CoreDataSet>();
        }

        public IList<CoreDataSetDuplicateResponse> GetDuplicateCoreDataSetForAnIndicator(CoreDataSet data)
        {
            return CurrentSession.GetNamedQuery("Find_Duplicate_CoreDataSet_Rows_SP")
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
                .List<CoreDataSetDuplicateResponse>();
        }
    }
}
