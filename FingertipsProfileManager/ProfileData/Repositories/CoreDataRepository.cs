using Fpm.ProfileData.Entities.Core;
using Fpm.ProfileData.Entities.LookUps;
using NHibernate;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Fpm.ProfileData.Repositories
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

        public IEnumerable<CoreDataSetArchive> GetCoreDataArchives(string areaCode)
        {
            var query = CurrentSession.CreateQuery("from CoreDataSetArchive cds where cds.AreaCode = :areaCode");
            query.SetParameterList("areaCode", areaCode);
            return query.List<CoreDataSetArchive>();
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

        public Area GetAreaDetail(string areaCode)
        {
            var q = CurrentSession.CreateQuery("from Area a where a.AreaCode = :areaCode");
            q.SetParameter("areaCode", areaCode);
            var area = q.UniqueResult<Area>();
            return area;
        }

        public void UpdateAreaDetail(Area newAreaDetail, string originalAreaCode, string loggedInUser)
        {
            try
            {
                var areaDetail = GetAreaDetail(originalAreaCode);

                areaDetail.AreaCode = newAreaDetail.AreaCode;
                areaDetail.AreaShortName = newAreaDetail.AreaShortName;
                areaDetail.AreaName = newAreaDetail.AreaName;
                areaDetail.AddressLine1 = newAreaDetail.AddressLine1;
                areaDetail.AddressLine2 = newAreaDetail.AddressLine2;
                areaDetail.AddressLine3 = newAreaDetail.AddressLine3;
                areaDetail.AddressLine4 = newAreaDetail.AddressLine4;
                areaDetail.IsCurrent = newAreaDetail.IsCurrent;
                areaDetail.Postcode = newAreaDetail.Postcode;


                transaction = CurrentSession.BeginTransaction();

                CurrentSession.Save(areaDetail);

                if (areaDetail.AreaCode != originalAreaCode)
                {
                    // Update other tables with new areacode

                    /* Columns that are updated automatically by foreign key cascade updates:
                     *  L_AreaLinks.areacode
                     *  GIS_Postcodes.areacode
                     *  L_AreaMapping.ChildLevelGeographyCode
                     *  CoreDataSet.AreaCode
                     */

                    foreach (var coreDataSetArchive in GetCoreDataArchives(originalAreaCode))
                    {
                        coreDataSetArchive.AreaCode = areaDetail.AreaCode;
                        CurrentSession.Save(coreDataSetArchive);
                    }

                    CurrentSession.GetNamedQuery("Update_AreaMapping")
                        .SetParameter("AreaCode", areaDetail.AreaCode)
                        .SetParameter("OriginalAreaCode", originalAreaCode)
                        .ExecuteUpdate();

                    // Insert Audit
                    CurrentSession.GetNamedQuery("Insert_FPMAreaAudit")
                          .SetParameter("AreaCode", areaDetail.AreaCode)
                          .SetParameter("AreaName", areaDetail.AreaName)
                          .SetParameter("AreaShortName", areaDetail.AreaShortName)
                          .SetParameter("AreaTypeId", areaDetail.AreaTypeId)
                          .SetParameter("AddressLine1", areaDetail.AddressLine1)
                          .SetParameter("AddressLine2", areaDetail.AddressLine2)
                          .SetParameter("AddressLine3", areaDetail.AddressLine3)
                          .SetParameter("AddressLine4", areaDetail.AddressLine4)
                          .SetParameter("IsCurrent", areaDetail.IsCurrent)
                          .SetParameter("Postcode", areaDetail.Postcode)

                          .SetParameter("ChangedAreaCode",
                              areaDetail.AreaCode == originalAreaCode ? (object)DBNull.Value : areaDetail.AreaCode)
                          .SetParameter("UserName", loggedInUser)
                          .SetParameter("Timestamp", DateTime.Now)
                          .ExecuteUpdate();
                }

                transaction.Commit();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        public virtual IEnumerable<CategoryType> GetCategoryTypes(int indicatorId)
        {
            var categoryTypes = CurrentSession.GetNamedQuery("GetDistinctCategoryTypesByIndicator")
               .SetParameter("indicatorId", indicatorId)
               .SetResultTransformer(Transformers.AliasToBean<CategoryType>())
               .List<CategoryType>()
               .OrderBy(x => x.Id);
            return categoryTypes;
        }

        public virtual IEnumerable<AreaType> GetAreaTypes(int indicatorId)
        {
            var areaTypes = CurrentSession.GetNamedQuery("GetDistinctAreaTypesByIndicator")
               .SetParameter("indicatorId", indicatorId)
               .SetResultTransformer(Transformers.AliasToBean<AreaType>())
               .List<AreaType>();
            return areaTypes;
        }

        public virtual IEnumerable<Sex> GetSexes(int indicatorId)
        {
            var sexes = CurrentSession.GetNamedQuery("GetDistinctSexesByIndicator")
               .SetParameter("indicatorId", indicatorId)
               .SetResultTransformer(Transformers.AliasToBean<Sex>())
               .List<Sex>();
            return sexes;
        }

        public virtual IEnumerable<Age> GetAges(int indicatorId)
        {
            var ages = CurrentSession.GetNamedQuery("GetDistinctAgesByIndicator")
               .SetParameter("indicatorId", indicatorId)
               .SetResultTransformer(Transformers.AliasToBean<Age>())
               .List<Age>();

            return ages;
        }

        public virtual IEnumerable<int> GetYearRanges(int indicatorId)
        {
            return CurrentSession.GetNamedQuery("GetDistinctYearRangeByIndicator")
               .SetParameter("indicatorId", indicatorId)
               .List<int>();
        }

        public virtual IEnumerable<int> GetYears(int indicatorId)
        {
            return CurrentSession.GetNamedQuery("GetDistinctYearsByIndicator")
               .SetParameter("indicatorId", indicatorId)
               .List<int>();
        }

        public virtual IEnumerable<int> GetMonths(int indicatorId)
        {
            return CurrentSession.GetNamedQuery("GetDistinctMonthsByIndicator")
               .SetParameter("indicatorId", indicatorId)
               .List<int>();
        }

        public virtual IEnumerable<int> GetQuarters(int indicatorId)
        {
            return CurrentSession.GetNamedQuery("GetDistinctQuartersByIndicator")
               .SetParameter("indicatorId", indicatorId)
               .List<int>();
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

        public int DeleteCoreDataSet(int indicatorId, Dictionary<string, int> filters, string userName)
        {

            const string sqlColumns = " [IndicatorID] ,[Year] ,[YearRange] ,[Quarter] ,[Month] ,[AgeID] ,[SexID] ,[AreaCode] " +
                                      ",[Count] ,[Value] ,[LowerCI] ,[UpperCI] ,[Denominator] ,[Denominator_2] ,[ValueNoteId] " +
                                      ",[uploadbatchid] ,[CategoryTypeID] ,[CategoryID]";

            var deleteBatchId = Guid.NewGuid();

            var sqlWhere = " WHERE indicatorId = " + indicatorId + GetSqlFromFilters(filters);

            var sqlInsertArchive = "INSERT INTO Coredataset_Archive(" + sqlColumns + ", DeleteBatchId) " +
                                   "SELECT " + sqlColumns + ", '" + deleteBatchId + "' FROM Coredataset " +
                                   sqlWhere;

            var sqlDeleteCoreData = "DELETE FROM Coredataset " + sqlWhere;

            try
            {
                transaction = CurrentSession.BeginTransaction();

                // Delete all values calculated by Fingertips as they may need to be recalculated
                CurrentSession.CreateSQLQuery(string.Format("DELETE FROM CoreDataSet WHERE ValueNoteId = {0} AND IndicatorId = {1}",
                    ValueNoteIds.AggregatedFromAllKnownLowerGeographyValuesByFingertips, indicatorId))
                    .ExecuteUpdate();

                CurrentSession.CreateSQLQuery(sqlInsertArchive)
                    .ExecuteUpdate();

                var rowsDeleted = CurrentSession.CreateSQLQuery(sqlDeleteCoreData)
                    .ExecuteUpdate();

                CurrentSession.GetNamedQuery("Insert_CoreData_DeleteAudit")
                    .SetParameter("indicatorId", indicatorId)
                    .SetParameter("whereClause", sqlWhere)
                    .SetParameter("rowsDeleted", rowsDeleted)
                    .SetParameter("userName", userName)
                    .SetParameter("deleteBatchId", deleteBatchId)
                    .ExecuteUpdate();

                transaction.Commit();
                return rowsDeleted;

            }
            catch (Exception exception)
            {
                HandleException(exception);

            }
            return -1;
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

        public bool CanDeleteDataSet(int indicatorId, string userName)
        {
            var result = CurrentSession.GetNamedQuery("CheckIfAuthorisedUser")
                .SetParameter("indicatorId", indicatorId)
                .SetParameter("userName", userName)
                .UniqueResult<int>();

            return result > 0;
        }
    }
}
