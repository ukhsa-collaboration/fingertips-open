using System;
using System.Collections.Generic;
using Fpm.ProfileData.Entities.Email;
using NHibernate;

namespace Fpm.ProfileData.Repositories
{
    public class EmailRepository : RepositoryBase, IEmailRepository
    {
        public EmailRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public IList<Email> GetEmailsAwaitingProcess()
        {
            var q = CurrentSession.CreateQuery("from Email e where e.SentTimestamp is not null and e.RetryCount <= 5");
            return q.List<Email>();
        }

        public int CreateEmail(Email email)
        {
            var newEmailId = -1;

            try
            {
                transaction = CurrentSession.BeginTransaction();

                newEmailId = (int) CurrentSession.Save(email);

                transaction.Commit();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }

            return newEmailId;
        }
    }
}
