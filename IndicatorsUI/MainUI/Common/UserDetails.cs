using System;
using System.DirectoryServices.AccountManagement;
using System.Web;
using Profiles.DomainObjects;

namespace Profiles.MainUI.Common
{
    public class UserDetails
    {
        public string Name { get; private set; }

        /// <summary>
        /// New user from current HTTP context.
        /// </summary>
        private UserDetails()
        {
            InitName();
        }

        /// <summary>
        /// New user from user ID.
        /// </summary>
        private UserDetails(string userName)
        {
            InitName(userName);
        }

        /// <summary>
        /// Initialises the Name of the user.
        /// </summary>
        private void InitName(string userName = null)
        {
            if (string.IsNullOrEmpty(userName) == false)
            {
                // Name specified in a test
                Name = userName;
            }
            else if (string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name) == false)
            {
                // On test server with authentication
                Name = HttpContext.Current.User.Identity.Name;
            }
            else
            {
                // On developer's machine
                Name = Environment.UserName;
            }
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
        public static UserDetails NewUserFromName(string userName)
        {
            return new UserDetails(userName);
        }

        public bool IsUserMemberOfAccessControlGroup(string accessControlGroup)
        {
            if (string.IsNullOrWhiteSpace(accessControlGroup))
            {
                return true;
            }

            PrincipalContext principalContext = new PrincipalContext(ContextType.Domain);

            GroupPrincipal groupPrincipal = GroupPrincipal
                .FindByIdentity(principalContext, accessControlGroup);

            if (groupPrincipal == null)
            {
                throw new FingertipsException("Could not find group: " + accessControlGroup);
            }

            UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(principalContext, Name);
            if (userPrincipal == null)
            {
                throw new FingertipsException("Could not find user: " + Name);
            }

            return userPrincipal.IsMemberOf(groupPrincipal);
        }
    }
}