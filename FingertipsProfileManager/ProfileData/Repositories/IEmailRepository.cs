using System.Collections.Generic;
using Fpm.ProfileData.Entities.Email;

namespace Fpm.ProfileData.Repositories
{
    public interface IEmailRepository
    {
        IList<Email> GetEmailsAwaitingProcess();
        int CreateEmail(Email email);
    }
}