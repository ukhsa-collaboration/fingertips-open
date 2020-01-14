
using Fpm.ProfileData.Entities.Report;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;

namespace Fpm.ProfileData.Repositories
{
    public class ReportRepository : RepositoryBase, IReportRepository
    {
        public ReportRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public int AddReport(Report report)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();
                CurrentSession.Save(report);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            return report.Id;
        }

        public void UpdateReport(Report report)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();
                CurrentSession.Update(report);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public IList<Report> GetAllReports()
        {
            return CurrentSession.CreateCriteria<Report>().List<Report>();
        }

        public Report GetReportById(int id)
        {
            return CurrentSession
                .CreateCriteria<Report>()
                .Add(Restrictions.Eq("Id", id))
                .UniqueResult<Report>();
        }

        public void DeleteReportById(int id)
        {
            CurrentSession.CreateQuery("delete Report r where r.Id = :id ")
                .SetParameter("id", id)
                .ExecuteUpdate();
        }



        /// <summary>
        ///  Add a record for report mapping
        /// </summary>
        /// <param name="mapping"></param>
        public void AddProfileMapping(ReportsProfileMapping mapping)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();
                CurrentSession.Save(mapping);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public IEnumerable<ReportsProfileMapping> GetAllMapping()
        {
            return CurrentSession
                .CreateCriteria<ReportsProfileMapping>()
                .List<ReportsProfileMapping>();

        }

        public IList<ReportsProfileMapping> GetMappingByReportId(int reportId)
        {
            return CurrentSession
                .CreateCriteria<ReportsProfileMapping>()
                .Add(Restrictions.Eq("ReportId", reportId))
                .List<ReportsProfileMapping>();

        }

        public void DeleteMappingForReport(int reportId)
        {
            var query = "DELETE FROM SSRS_ReportsProfileMapping WHERE ReportId = :ReportId";
            CurrentSession.CreateSQLQuery(query)
                .SetParameter("ReportId", reportId)
                .UniqueResult();
        }

    }
}
