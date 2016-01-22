using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fpm.ProfileData.Entities.User;
using NHibernate;

namespace Fpm.ProfileData.Repositories
{
    public class UserRepository : RepositoryBase
    {
    
     // poor man injection, should be removed when we use DI containers
        public UserRepository() :this(NHibernateSessionFactory.GetSession())
        {
        }

        public UserRepository(ISessionFactory sessionFactory) : base(sessionFactory) 
        {
        }

        public IEnumerable<FpmUser> GetAllFpmUsers()
        {
            IQuery q = CurrentSession.CreateQuery("from FpmUser fu");
            return q.List<FpmUser>();
        }
        
        public bool CreateUserItem(FpmUser user, string loggedInUser)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();

                var newUserId = (int)CurrentSession.Save(user);

                LogUserChange(newUserId, user.UserName, DateTime.Now, "NewTarget User Created", loggedInUser, user.DisplayName, user.IsAdministrator);

                transaction.Commit();

                return true;
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
            return false;
        }

        public bool UpdateUserItem(FpmUser user, string loggedInUser)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();

                CurrentSession.Update(user);

                LogUserChange(user.Id, user.UserName, DateTime.Now, "User Updated", loggedInUser, user.DisplayName, user.IsAdministrator);

                transaction.Commit();

                return true;
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
            return false;
        }

        public IEnumerable<UserAudit> GetUserAudit(List<int> userIds)
        {
            IQuery q = CurrentSession.CreateQuery(
                "from UserAudit ua where ua.UserId in (:userIds) order by ua.Timestamp desc");
            q.SetParameterList("userIds", userIds);
            return q.List<UserAudit>();
        }

        /// <summary>
        /// Logs a change made to a metadata property.
        /// </summary>
        private void LogUserChange(int userId, string userName, DateTime timestamp,
            string auditType, string user, string displayName, bool isAdministrator)
        {
            CurrentSession.Save(new UserAudit()
            {
                UserId = userId,
                UserName = userName,
                DisplayName = displayName,
                IsAdministrator = isAdministrator,
                Timestamp = timestamp,
                User = user,
                AuditType = auditType
            });
        }

    }
 }
