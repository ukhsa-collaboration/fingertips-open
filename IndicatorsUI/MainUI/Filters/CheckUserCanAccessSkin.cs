using System.Web.Mvc;
using Profiles.DataAccess;
using Profiles.MainUI.Helpers;
using Profiles.MainUI.Skins;

namespace Profiles.MainUI.Filters
{
    public class CheckUserCanAccessSkin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Access is only controlled on test environment
            if (AppConfig.Instance.IsEnvironmentTest)
            {
                var skin = SkinFactory.GetSkin();

                if (UserDetails.CurrentUser()
                    .IsUserMemberOfAccessControlGroup(skin.AccessControlGroup) == false)
                {
                    filterContext.Result = AccessControlHelper.GetAccessNotAllowedActionResult();
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}