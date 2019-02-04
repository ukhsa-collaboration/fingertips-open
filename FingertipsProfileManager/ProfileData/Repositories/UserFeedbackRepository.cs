using Fpm.ProfileData.Entities.UserFeedback;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fpm.ProfileData.Repositories
{
    public class UserFeedbackRepository : RepositoryBase
    {
        public UserFeedbackRepository()
            : this(NHibernateSessionFactory.GetSession())
        {
        }

        public UserFeedbackRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        public int NewUserFeedback(UserFeedback feedback)
        {
            CurrentSession.Save(feedback);
            return feedback.Id;
        }

        public IEnumerable<UserFeedback> GetLatestUserFeedback(int feedbackCount)
        {
            return CurrentSession
                .CreateCriteria<UserFeedback>()
                .AddOrder(Order.Desc("Timestamp"))
                .List<UserFeedback>().Take(feedbackCount);
        }

        public UserFeedback GetFeedbackById(int id)
        {
            return CurrentSession
                .CreateCriteria<UserFeedback>()
                .Add(Restrictions.Eq("Id", id))
                .UniqueResult<UserFeedback>();
        }

        public void UdateFeedback(UserFeedback feedback)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();
                CurrentSession.Update(feedback);
                transaction.Commit();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        public void DeleteUserFeedback(int id)
        {
            const string query = "delete from userfeedback where id= :id";
            CurrentSession.CreateSQLQuery(query)
                .SetParameter("id", id)
                .ExecuteUpdate();
        }
    }
}
