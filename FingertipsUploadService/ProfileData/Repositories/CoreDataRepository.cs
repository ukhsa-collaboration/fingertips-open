using FingertipsUploadService.ProfileData.Entities.Core;
using FingertipsUploadService.ProfileData.Helpers;
using NHibernate;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FingertipsUploadService.ProfileData.Repositories
{
    public class CoreDataRepository : RepositoryBase
    {
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
                    .CreateQuery("from CoreDataSet cds where cds.Uid in (:Id)")
                    .SetParameterList("Id", duplicateRows.Select(x => x.Uid).ToList())
                    .List<CoreDataSet>();
        }

        public void InsertCoreDataArchive(IEnumerable<DuplicateRowInDatabaseError> duplicateRows, Guid replacementUploadloadBatchId)
        {
            try
            {
                foreach (var duplicateBatch in duplicateRows.Batch(100))
                {
                    if (duplicateBatch != null)
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
                }

            }
            catch (Exception exception)
            {
                HandleException(exception);
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

        public IEnumerable<CoreDataSet> GetCoreDataSet(int indicatorId, out int totalRows)
        {
            return GetCoreDataSet(indicatorId, null, out totalRows);
        }

        public IEnumerable<CoreDataSet> GetCoreDataSet(int indicatorId, Dictionary<string, int> filters, out int totalRows)
        {
            // Dynamic Query
            const string sqlSelectColumns = "SELECT Top 500 Uid, IndicatorId,Year,YearRange,Quarter,Month," +
                                            "AgeId,SexId,AreaCode,Count,Value,LowerCi,UpperCi,Denominator,Denominator_2," +
                                            "ValueNoteId,UploadBatchId,CategoryTypeId, CategoryId " +
                                            "FROM Coredataset cds ";

            const string sqlSelectCount = "SELECT count(*) totalRows FROM Coredataset cds ";

            var sqlWhere = "WHERE indicatorId = " + indicatorId;

            var sqlFilters = filters != null ? GetSqlFromFilters(filters) : string.Empty;

            Debug.WriteLine(sqlSelectCount + sqlWhere + sqlFilters);

            totalRows = CurrentSession.CreateSQLQuery(sqlSelectCount + sqlWhere + sqlFilters)
                 .UniqueResult<int>();

            var result = CurrentSession.CreateSQLQuery(sqlSelectColumns + sqlWhere + sqlFilters)
                .SetResultTransformer(Transformers.AliasToBean<CoreDataSet>())
                .List<CoreDataSet>();

            return result;
        }

        public IList<CoreDataSetDuplicateResponse> GetDuplicateCoreDataSetForAnIndicator(CoreDataSet data)
        {
            return CurrentSession.GetNamedQuery("Find_Duplciate_Rows_In_CoreDataSet_SP")
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

        private string GetSqlFromFilters(Dictionary<string, int> filters)
        {
            if (filters == null) return string.Empty;

            var sqlBuilder = new StringBuilder();

            foreach (var key in filters.Keys)
            {
                if (key == CoreDataFilters.AreaTypeId)
                {
                    sqlBuilder.Append(" AND AreaCode IN (SELECT areacode FROM l_areas where areatypeId = " + filters[key] + ")");
                }
                else
                {
                    sqlBuilder.Append(" AND " + key + " = " + filters[key]);
                }
            }
            return sqlBuilder.ToString();
        }
    }
}
