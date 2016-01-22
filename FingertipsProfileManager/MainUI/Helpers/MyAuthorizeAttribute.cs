using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Fpm.MainUI.Helpers
{
    public class AuthorizedUsersAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return UserDetails.CurrentUser().IsAdministrator;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (UserDetails.CurrentUser().IsAdministrator)
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new
                RouteValueDictionary(
                new { controller = "ProfilesAndIndicators", action = "ProfilesAndIndicators", url = "" }));
            }
        }

    }
}