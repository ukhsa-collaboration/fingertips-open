using System.Collections.Generic;
using Fpm.MainUI.Helpers;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Entities.User;

namespace Fpm.MainUI.Models
{
    public class BaseDataModel
    {
        public string UrlKey { get; set; }
        public Profile Profile { get; set; }

        public IEnumerable<ProfileDetails> Profiles { get; set; }

        public bool UserHasAssignedPermissions { get; set; }

        public UserGroupPermissions UserGroupPermissions { get; set; }

        public IndicatorMetadata IndicatorMetadata { get; set; }

        public string GetIndicatorUrl(int indicatorId, int ageId, int sexId)
        {
            return "/profile/" + UrlKey +
                   "/area-type/" + Profile.SelectedAreaType +
                   "/domain/" + Profile.SelectedDomain +
                   "/indicator/" + indicatorId +
                   "/ageId/" + ageId +
                   "/sexId/" + sexId;
        }

        public bool DoesProfileOwnIndicator()
        {
            return UserGroupPermissions != null &&
                   IndicatorMetadata.OwnerProfileId == UserGroupPermissions.ProfileId;
        }

        public bool DoesUserHaveWritePermission()
        {
            return UserGroupPermissions != null &&
                   UserGroupPermissions.ProfileId == Profile.Id;
        }

        public bool IsArchiveProfile()
        {
            return Profile.Id == ProfileIds.ArchivedIndicators;
        }
    }
}