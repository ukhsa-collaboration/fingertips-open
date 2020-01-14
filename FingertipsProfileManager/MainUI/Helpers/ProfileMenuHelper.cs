using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Fpm.MainUI.Helpers
{
    public class ProfileMenuHelper : IProfileMenuHelper
    {
        public static SelectList GetProfileListForCurrentUser()
        {
            var user = UserDetails.CurrentUser();
            var menuHelper = new ProfileMenuHelper();
            if (user.IsAdministrator)
            {
                return menuHelper.GetProfilesUserHasPermissionToIncludingSpecialProfiles(user);
            }
            return menuHelper.GetProfilesUserHasPermissionToExcludingSpecialProfiles(user);
        }

        public SelectList GetProfilesUserHasPermissionToExcludingSpecialProfiles(UserDetails user)
        {
            var profiles = user.GetProfilesUserHasPermissionsTo()
                .Where(x => x.Id != ProfileIds.UnassignedIndicators && x.Id != ProfileIds.IndicatorsForReview)
                .OrderBy(x => x.Name)
                .ToList();

            AddDefaultOption(profiles);

            return GetSelectList(profiles);
        }

        public SelectList GetProfilesUserHasPermissionToIncludingSpecialProfiles(UserDetails user)
        {
            var profiles = user.GetProfilesUserHasPermissionsTo()
                .OrderBy(x => x.Name)
                .ToList();

            AddDefaultOption(profiles);

            return GetSelectList(profiles);
        }

        public SelectList GetAllProfiles(IProfilesReader profilesReader)
        {
            var profiles = profilesReader.GetProfiles()
                .OrderBy(x => x.Name)
                .ToList();

            AddDefaultOption(profiles);

            return GetSelectList(profiles);
        }

        private static SelectList GetSelectList(List<ProfileDetails> profiles)
        {
            var selectList = new SelectList(profiles, "Id", "Name");
            return selectList;
        }

        private static void AddDefaultOption(List<ProfileDetails> profiles)
        {
            profiles.Insert(0, new ProfileDetails
            {
                Id = ProfileIds.Undefined,
                Name = "-- Select profile --"
            });
        }
    }
}