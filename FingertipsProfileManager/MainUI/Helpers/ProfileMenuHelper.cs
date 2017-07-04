using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.MainUI.Helpers
{
    public class ProfileMenuHelper
    {
        public SelectList GetProfilesUserHasPermissionToExcludingSpecialProfiles(UserDetails user)
        {
            var profiles = user.GetProfilesUserHasPermissionsTo()
                .Where(x => x.Id != ProfileIds.ArchivedIndicators & x.Id != ProfileIds.UnassignedIndicators)
                .OrderBy(x => x.Name)
                .ToList();

            AddDefaultOption(profiles);

            return GetSelectList(profiles);
        }

        public SelectList GetAllProfiles(ProfilesReader profilesReader)
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