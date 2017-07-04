using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.Linq;
using System.Web;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Entities.User;
using Fpm.ProfileData.Repositories;

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

                    if (_fpmUser == null)
                    {
                        CreateNewUser();
                        _fpmUser = reader.GetUserByUserName(Name);
                    }
                }

                return _fpmUser;
            }
        }

        public static string ConvertUserNameToDisplayName(string name)
        {
            name = name
                 .Replace('.', ' ')
                 .Substring(4); // Remove "phe\" prefix

            TextInfo textInfo = new CultureInfo("en-GB", false).TextInfo;
            return textInfo.ToTitleCase(name);
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
                return user.IsAdministrator;
            }
        }

        public bool IsPholioDataManager
        {
            get
            {
                FpmUser user = FpmUser;
                return user.Id == FpmUserIds.Doris;
            }
        }

        public bool IsMemberOfFpmSecurityGroup
        {
            get
            {
                try
                {
                    var accessControlGroup = "Global.Fingertips.FingertipsProfileManager";
                    PrincipalContext principalContext = new PrincipalContext(ContextType.Domain);

                    GroupPrincipal groupPrincipal = GroupPrincipal
                        .FindByIdentity(principalContext, accessControlGroup);

                    UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(principalContext, Name);
                    if (userPrincipal == null)
                    {
                        // User name not found
                        return false;
                    }

                    return userPrincipal.IsMemberOf(groupPrincipal);
                }
                catch (Exception AppDomainUnloadedException)
                {
                    // Sometimes security check cannot be made so give benefit of the doubt
                    return true;
                }
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

        private void CreateNewUser()
        {
            new UserRepository().CreateUserItem(new FpmUser
            {
                UserName = Name,
                DisplayName = ConvertUserNameToDisplayName(Name)
            }, Name);
        }
    }
}