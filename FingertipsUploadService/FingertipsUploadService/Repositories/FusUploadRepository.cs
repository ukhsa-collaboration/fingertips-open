using FingertipsUploadService.Entities.Job;
using FingertipsUploadService.ProfileData;
using NHibernate;
using System;
using System.Collections.Generic;

namespace FingertipsUploadService.Repositories
{
    public class FusUploadRepository : RepositoryBase, IFusUploadRepository
    {
        public FusUploadRepository()
            : this(NHibernateSessionFactory.GetSession())
        {
        }

        public FusUploadRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public List<UploadJob> GetNotStartedUploadJobs()
        {
            throw new NotImplementedException();
        }

        public UploadJobStatus UpdateJob(UploadJob uploadJob)
        {
            throw new NotImplementedException();
        }

        public void SaveValidationFailure(UploadJobStatus status)
        {
            throw new NotImplementedException();
        }
    }
}
