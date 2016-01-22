using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Entities.User;

namespace Fpm.MainUI.Helpers
{
    public class UserDetails
    {
        private readonly ProfilesReader reader = ReaderFactory.GetProfilesReader();
        public string Name { get; private set; }
        private FpmUser _fpmUser;

        /// <summary>
        /// New user from current HTTP context.
        /// </summary>
        private UserDetails()
        {
            InitName(AppConfig.CurrentUserName);
        }

        /// <summary>
        /// New user from user ID.
        /// </summary>
        private UserDetails(int userId)
        {
            _fpmUser = reader.GetUserByUserId(userId);
            InitName(_fpmUser.UserName);
        }

        /// <summary>
        /// Initialises the Name of the user.
        /// </summary>
        private void InitName(string userName)
        {
            Name = string.IsNullOrEmpty(userName)
                    ? HttpContext.Current.User.Identity.Name
                    : userName;
        }

        /// <summary>
        /// Factory method to create new instance.
        /// </summary>
        public static UserDetails CurrentUser()
        {
            return new UserDetails();
        }

        /// <summary>
        /// Factory method to create new instance.
        /// </summary>
        public static UserDetails NewUserFromUserId(int userId)
        {
            return new UserDetails(userId);
        }

        public FpmUser FpmUser
        {
            get
            {
                if (_fpmUser == null)
                {
                    _fpmUser = reader.GetUserByUserName(Name);
                }
                return _fpmUser;
            }
        }

        public int Id
        {
            get { return FpmUser.Id; }
        }

        public bool IsAdministrator
        {
            get
            {
                FpmUser user = FpmUser;
                CheckUserExists(user);
                return user.IsAdministrator;
            }
        }

        private void CheckUserExists(FpmUser user)
        {
            if (user == null)
            {
                throw new FpmException("FPM user does not exist for " + Name);
            }
        }

        public bool IsPholioDataManager
        {
            get
            {
                FpmUser user = FpmUser;
                CheckUserExists(user);
                return user.Id == FpmUserIds.Doris;
            }
        }

        public bool HasWritePermissionsToProfile(int profileId)
        {
            FpmUser fpmUser = FpmUser;
            return CommonUtilities.GetUserGroupPermissionsByUserId(fpmUser.Id)
                .FirstOrDefault(x => x.ProfileId == profileId) != null;
        }

        public IEnumerable<ProfileDetails> GetProfilesUserHasPermissionsTo()
        {
            var permissionIds = reader.GetUserGroupPermissionsByUserId(FpmUser.Id)
                .Select(x => x.ProfileId);

            return reader.GetProfiles()
                .Where(x => permissionIds.Contains(x.Id))
                .OrderBy(x => x.Name);
        }
    }
}