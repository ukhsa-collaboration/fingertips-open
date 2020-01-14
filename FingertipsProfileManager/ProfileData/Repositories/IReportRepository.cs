using System;
using System.Collections.Generic;
using Fpm.ProfileData.Entities.Report;

namespace Fpm.ProfileData.Repositories
{
    public interface IReportRepository
    {
        int AddReport(Report report);
        void UpdateReport(Report report);
        IList<Report> GetAllReports();
        Report GetReportById(int id);
        void DeleteReportById(int id);

        /// <summary>
        ///  Add a record for report mapping
        /// </summary>
        /// <param name="mapping"></param>
        void AddProfileMapping(ReportsProfileMapping mapping);

        IEnumerable<ReportsProfileMapping> GetAllMapping();
        IList<ReportsProfileMapping> GetMappingByReportId(int reportId);
        void DeleteMappingForReport(int reportId);

        /// <summary>
        /// Opens a data access session
        /// </summary>
        /// <exception cref="Exception">Thrown if an error occurs while opening the session</exception>
        void OpenSession();

        /// <summary>
        /// IDisposable.Dispose implementation (closes and disposes of the encapsulated session)
        /// </summary>
        void Dispose();
    }
}