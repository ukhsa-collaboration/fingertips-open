using System.Web.Mvc;
using System.Web.Routing;
using Profiles.DataAccess;
using Profiles.DomainObjects;

namespace Profiles.MainUI.Helpers
{
    public class AccessControlHelper
    {
        public static ActionResult GetAccessNotAllowedActionResult()
        {
            return new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "Error",
                                action = "AccessNotAllowed"
                            }));
        }

        /// <summary>
        /// Whether a user should be denied access to a profile
        /// </summary>
        public static bool ShouldDenyAccess(ProfileDetails details)
        {
            if (AppConfig.Instance.IsAccessControlToProfiles == false)
            {
                return false;
            }

            return UserDetails.CurrentUser()
                .IsUserMemberOfAccessControlGroup(details.AccessControlGroup) == false;
        }
    }
}