using Fpm.ProfileData;
using System.Web.Mvc;

namespace Fpm.MainUI.Helpers
{
    public interface IProfileMenuHelper
    {
        SelectList GetProfilesUserHasPermissionToExcludingSpecialProfiles(UserDetails user);
        SelectList GetProfilesUserHasPermissionToIncludingSpecialProfiles(UserDetails user);
        SelectList GetAllProfiles(IProfilesReader profilesReader);
    }
}