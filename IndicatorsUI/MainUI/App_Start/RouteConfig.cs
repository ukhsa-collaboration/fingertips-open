using IndicatorsUI.MainUI.Routes;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace IndicatorsUI.MainUI
{
    public class RouteConfig
    {
        public const string ProfileController = "Profile";

        public static void RegisterRoutes(RouteCollection routes)
        {
            AddIgnoredRoutes(routes);
            Redirects(routes);
            routes.MapMvcAttributeRoutes();

            /*
             * IMPORTANT: Do not add any more routes here. Use route attributes instead
             */

            AddComplexRoutes(routes);

            /* General API routes added after MVC attribute routes
             so specific paths can be overridden */
            AddApiRoutes(routes);

            // Very general route: define last
            AddGeneralProfileRoute(routes);

            /* This route will be hit for any request that does not map to a target,
             * e.g. a JavaScript file that does not exist */
            routes.Add("frontPageRoute", new FrontPageRoute());

            Add404Routes(routes);
        }

        private static void AddGeneralProfileRoute(RouteCollection routes)
        {
            routes.MapRoute(
                "ProfileData", // Route name
                "{profileKey}", // URL with parameters
                new
                {
                    controller = ProfileController,
                    action = "Data"
                }
            );
        }

        private static void Add404Routes(RouteCollection routes)
        {
            routes.MapRoute("default", "{controller}/{action}",
                new {controller = "Error", action = "Http404"});

            routes.MapRoute("NotFound", "{*url}",
                new {controller = "Error", action = "Http404"});
        }

        private static void AddApiRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "AjaxBridgeApi", // Route name
                "api/{serviceAction1}", // URL with parameters
                new
                {
                    controller = "AjaxBridge",
                    action = "ApiPath"
                }
            );

            routes.MapRoute(
                "AjaxBridgeApiTwoLevel", // Route name
                "api/{serviceAction1}/{serviceAction2}", // URL with parameters
                new
                {
                    controller = "AjaxBridge",
                    action = "ApiPath"
                }
            );

            routes.MapRoute(
                "AjaxBridgeApiThreeLevel", // Route name
                "api/{serviceAction1}/{serviceAction2}/{serviceAction3}", // URL with parameters
                new
                {
                    controller = "AjaxBridge",
                    action = "ApiPath"
                }
            );
        }

        private static void AddIgnoredRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.ashx/{*pathInfo}");

            routes.IgnoreRoute("{*favicon}", new { favicon = @".*?favicon.ico" });

            var ignoredRoutes = new List<string>
            {
                "htaccess.txt",
                "administrator",
                "{page}.php",
                "wp-admin",
                "{.*}/wp-admin"
            };

            foreach (var ignoredRoute in ignoredRoutes)
            {
                routes.IgnoreRoute(ignoredRoute);
            }
        }

        /// <summary>
        /// Routes that contain url path parameters and query parameters
        /// </summary>
        /// <param name="routes"></param>
        private static void AddComplexRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "AreaSearchResultsFingertips", // Route name
                "profile/{profileKey}/area-search-results/{areaCodeList}", // URL with parameters
                new
                {
                    controller = "AreaSearch",
                    action = "AreaSearchResults",
                    place_name = UrlParameter.Optional,
                    search_type = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                "ProfileCollectionAreaSearchResultsPage", // Route name
                "profile-group/{leadProfileUrlKey}/profile/{profileKey}/area-search-results/{areaCodeList}", // URL with parameters
                new
                {
                    controller = "AreaSearch",
                    action = "AreaSearchResults",
                    place_name = UrlParameter.Optional,
                    search_type = UrlParameter.Optional
                }
                );

            routes.MapRoute(
                "ProfileCollectionProfileFrontPage", // Route name
                "profile-group/{leadProfileUrlKey}/profile/{profileUrlKey}", // URL with parameters
                new
                {
                    controller = "ProfileCollectionWithFrontPage",
                    action = "ProfileFrontPage",
                    leadProfileUrlKey = UrlParameter.Optional,
                    profileUrlKey = UrlParameter.Optional
                }
                );

        }

        /// <summary>
        /// Redirects for renamed and old profiles
        /// </summary>
        private static void Redirects(RouteCollection routes)
        {
            var redirects = new Dictionary<string, string>
            {
                // What about youth profiles renamed
                {"profile/what-aboutyouth", "/profile-group/child-health"},
                {"profile/what-about-youth", "/profile-group/child-health"},
                // Common route not handled in live logs
                {"profile/local-alcohol", "/profile/local-alcohol-profiles"},
                // CHMP profile retired
                {"profile-group/mental-health/profile/cmhp", "/profile-group/mental-health"},
                //FIN-1574 Redirect now separate profiles for Diabetes in Longer Lives and Fingertips
                {"profile/diabetes", "/profile/diabetes-ft"},
                {"diabetes", "/profile/diabetes-ft"},
                {"profile/diabetes/data", "/profile/diabetes-ft/data"}
            };

            foreach (var redirect in redirects)
            {
                routes.MapRoute("Redirect" + Guid.NewGuid(),
                    redirect.Key,
                    new
                    {
                        controller = "Redirect",
                        action = "RedirectToUrl",
                        newUrl = redirect.Value
                    });
            }
        }
    }
}