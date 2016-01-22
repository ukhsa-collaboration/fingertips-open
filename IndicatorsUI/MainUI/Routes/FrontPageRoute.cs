using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Profiles.MainUI.Skins;

namespace Profiles.MainUI.Routes
{
    /// <summary>
    /// Route that selects a front page based on the site Skin.
    /// </summary>
    public class FrontPageRoute : RouteBase
    {
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            if (httpContext.Request.FilePath == "/")
            {
                if (httpContext.Request.FilePath == "/")
                {
                    if (SkinFactory.GetSkin().Name == SkinNames.Mortality)
                    {
                        var routeData = new RouteData(this, new MvcRouteHandler());
                        routeData.Values.Add("controller", "LongerLives");
                        routeData.Values.Add("action", "Home");
                        return routeData;
                    }
                    else
                    {
                        var routeData = new RouteData(this, new MvcRouteHandler());
                        routeData.Values.Add("controller", RouteConfig.ProfileController);
                        routeData.Values.Add("action", "FrontPage");
                        return routeData;
                    }
                }
            }
            return null;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            //Implement your formating Url formating here
            return null;
        }
    }
}