using FingertipsUploadService.ProfileData.Entities.Logging;
using NHibernate;
using System;

namespace FingertipsUploadService.ProfileData.Repositories
{
    public class LoggingRepository : RepositoryBase
    {

        // poor man injection, should be removed when we use DI containers
        public LoggingRepository()
            : this(NHibernateSessionFactory.GetSession())
        {
        }

        public LoggingRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public UploadAudit GetUploadAudit(int uploadId)
        {
            return CurrentSession.CreateQuery(string.Format("from UploadAudit  where Id = {0}", uploadId))
                .UniqueResult<UploadAudit>();
        }

        public int InsertUploadAudit(Guid uploadBatchId, string userName, int rowCount, string excelFile, string selectedWorksheet)
        {
            return
                (int)CurrentSession.Save(new UploadAudit
                {
                    UploadId = uploadBatchId,
                    UploadedBy = userName,
                    UploadedOn = DateTime.Now,
                    RowsUploaded = rowCount,
                    UploadFilename = excelFile,
                    WorksheetName = selectedWorksheet
                });
        }

        public void DeleteUploadAudit(int uploadId)
        {
            var queryString = string.Format(@"delete from UploadAudit  where Id = '{0}'", uploadId);

            CurrentSession.CreateQuery(queryString).ExecuteUpdate();
        }
    }
}
