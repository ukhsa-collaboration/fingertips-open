using NHibernate;
using NHibernate.Criterion;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;

namespace PholioVisualisation.DataAccess.Repositories
{
    public class UserFeedbackRepository : RepositoryBase
    {
        public UserFeedbackRepository() : this(NHibernateSessionFactory.GetSession())
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

        public IEnumerable<UserFeedback> GetAllUserFeedbacks()
        {
            return CurrentSession
                .CreateCriteria<UserFeedback>()
                .List<UserFeedback>();
        }

        public UserFeedback GetFeedbackById(int id)
        {
            return CurrentSession
                .CreateCriteria<UserFeedback>()
                .Add(Restrictions.Eq("Id", id))
                .UniqueResult<UserFeedback>();
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
