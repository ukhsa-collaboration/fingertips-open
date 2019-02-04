using IndicatorsUI.MainUI.Routes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DataAccess.Repository;

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
                "wp",
                "wordpress",
                "{.*}/wp-admin",
                "site.webmanifest",
                "angular-app-dist/webpack.config.js", // Requested by angular code but does not exist
                "webpack.config.js", // Requested by angular code but does not exist
                "safari-pinned-tab.svg", // Apple fav icon - don't have SVG version yet
                "profile"
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
            // Redirection will affect tests. Because of that is off in test environment.
            // If you will need test with redirections change web.confing->appsetting->allowRedirection to true
            if (!AppConfig.Instance.isAllowingRedirection) return;

            var redirects = new RelativeUrlRedirectRepository().GetAllRelativeUrlRedirects();

            foreach (var redirect in redirects)
            {
                // Ensure to URL starts with "/"
                var toUrl = redirect.ToUrl.Trim();
                if (toUrl.StartsWith("/") == false)
                {
                    toUrl = "/" + toUrl;
                }

                // Change hash so can be added back later in browser
                if (toUrl.Contains("#"))
                {
                    var bits = toUrl.Split('#');

                    toUrl = bits[0] + "?redirectHash=" + bits[1];
                }

                // Prepend with hostname if absolute path required
                if (redirect.useFingertipsHostnameForAbsolutePath)
                {
                    toUrl = ConfigurationManager.AppSettings["DomainUrl"] + toUrl;
                }

                // Redirect action
                routes.MapRoute("Redirect" + Guid.NewGuid(),
                    redirect.FromUrl.Trim(),
                    new
                    {
                        controller = "Redirect",
                        action = "RedirectToUrl",
                        newUrl = toUrl
                    });
            }
        }
    }
}