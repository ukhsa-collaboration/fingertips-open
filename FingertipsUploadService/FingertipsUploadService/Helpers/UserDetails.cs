
using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Profile;
using System.Collections.Generic;
using System.Linq;

namespace FingertipsUploadService.Helpers
{
    public class UserDetails
    {
        private readonly ProfilesReader _reader = ReaderFactory.GetProfilesReader();
        private readonly int _userId;

        public UserDetails(int userId)
        {
            _userId = userId;
        }

        public IEnumerable<ProfileDetails> GetProfilesUserHasPermissionsTo()
        {
            var permissionIds = _reader.GetUserGroupPermissionsByUserId(_userId)
                .Select(x => x.ProfileId);

            return _reader.GetProfiles()
                .Where(x => permissionIds.Contains(x.Id))
                .OrderBy(x => x.Name);
        }
    }
}
