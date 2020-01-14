using System;
using System.Collections.Generic;
using Fpm.ProfileData.Entities.User;

namespace Fpm.ProfileData.Repositories
{
    public interface IUserRepository
    {
        void DeleteUserGroupPermissions(int profileId, int userId);
        void SaveUserGroupPermissions(UserGroupPermissions userGroupPermissions);
        IEnumerable<FpmUser> GetAllFpmUsers();
        bool CreateUser(FpmUser user, string loggedInUser);
        bool UpdateUser(FpmUser user, string loggedInUser);
        IEnumerable<UserAudit> GetUserAudit(List<int> userIds);
        UserGroupPermissions GetUserGroupPermissions(int profileId,int userId);

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