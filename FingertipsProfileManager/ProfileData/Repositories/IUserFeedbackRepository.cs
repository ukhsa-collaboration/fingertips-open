using System;
using System.Collections.Generic;
using Fpm.ProfileData.Entities.UserFeedback;

namespace Fpm.ProfileData.Repositories
{
    public interface IUserFeedbackRepository
    {
        int NewUserFeedback(UserFeedback feedback);
        IEnumerable<UserFeedback> GetLatestUserFeedback(int feedbackCount);
        UserFeedback GetFeedbackById(int id);
        void UpdateFeedback(UserFeedback feedback);
        void DeleteUserFeedback(int id);

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