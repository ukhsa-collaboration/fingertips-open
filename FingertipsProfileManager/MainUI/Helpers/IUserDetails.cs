using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Entities.User;
using System.Collections.Generic;

namespace Fpm.MainUI.Helpers
{
    public interface IUserDetails
    {
        string Name { get; }
        FpmUser FpmUser { get; }
        int Id { get; }
        bool IsAdministrator { get; }
        bool IsReviewer { get; }
        bool IsPholioDataManager { get; }
        bool IsMemberOfFpmSecurityGroup { get; }
        bool HasWritePermissionsToProfile(int profileId);
        IEnumerable<ProfileDetails> GetProfilesUserHasPermissionsTo();
    }
}