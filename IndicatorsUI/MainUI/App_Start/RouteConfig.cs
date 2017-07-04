using Profiles.MainUI.Routes;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Profiles.MainUI
{
    public class RouteConfig
    {
        public const string ProfileController = "Profile";
        public const string LongerLivesController = "LongerLives";

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.ashx/{*pathInfo}");

            // General ignores
            routes.IgnoreRoute("{*favicon}", new { favicon = @".*?favicon.ico" });
            routes.IgnoreRoute("htaccess.txt");
            routes.IgnoreRoute("administrator");

            // Wordpress ignores
            routes.IgnoreRoute("{page}.php");
            routes.IgnoreRoute("wp-admin");
            routes.IgnoreRoute("{.*}/wp-admin");

            Redirects(routes);

            AddDocumentDownloadRoute(routes);
            AddProfileCollectionRoutes(routes);
            AddSingleProfileWithFrontPageRoutes(routes);
            AddAjaxBridgeRoutes(routes);
            AddTestRoutes(routes);
            AddLongerLivesRoutes(routes);
            AddPhofRoutes(routes);
            AddTwitterRoutes(routes);

            routes.MapRoute(
                "SsrsReports",
                "reports/ssrs",
                new
                {
                    controller = "Ssrs",
                    action = "Index"
                }
                );


            routes.MapRoute(
                "SssrsReportsImage",
                "reports/ssrs/image",
                new
                {
                    controller = "Ssrs",
                    action = "Image"
                }
                );

            routes.MapRoute(
                "VerticalText", // Route name
                "img/vertical-text",
                new
                {
                    controller = "Image",
                    action = "VerticalText"
                }
                );

            routes.MapRoute(
                "PracticeScatterChart", // Route name
                "img/gp-scatter-chart",
                new
                {
                    controller = "Image",
                    action = "PracticeScatterChart"
                }
                );

            routes.MapRoute(
                "CaptureShot",
                "capture-shot",
                new
                {
                    controller = "Image",
                    action = "CaptureShot"
                }
                );

            routes.MapRoute(
                "AccessNotAllowed", // Route name
                "access-not-allowed", // Dummy URL so route can be registered
                new
                {
                    controller = "Error",
                    action = "AccessNotAllowed"
                }
                );


            routes.MapRoute(
                "BrowserNotSupported",
                "browser-not-supported",
                new
                {
                    controller = "Error",
                    action = "BrowserNotSupported"
                }
                );

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
                "SupportingInformation", // Route name
                "profile/{profileKey}/supporting-information/{contentKey}", // URL with parameters
                new
                {
                    controller = "Profile",
                    action = "SupportingInformation"
                }
                );

            routes.MapRoute(
                "SelectedIndicators", // Route name
                "indicators/{indicatorList}", // URL with parameters
                new
                {
                    controller = "Search",
                    action = "SelectedIndicators",
                    indicatorList = UrlParameter.Optional
                }
                );

            routes.MapRoute(
                "Search", // Route name
                "search/{searchText}", // URL with parameters
                new
                {
                    controller = "Search",
                    action = "Data",
                    searchText = UrlParameter.Optional
                }
                );

            routes.MapRoute(
                "ProfileData", // Route name
                "{profileKey}", // URL with parameters
                new
                {
                    controller = ProfileController,
                    action = "Data"
                }
                );

            /* This route will be hit for any request that does not map to a target,
             * e.g. a JavaScript file that does not exist */
            routes.Add("frontPageRoute", new FrontPageRoute());

            routes.MapRoute("NotFound", "{*url}",
                new { controller = "Error", action = "Http404" });

        }

        private static void AddDocumentDownloadRoute(RouteCollection routes)
        {
            //TODO: Uncomment it once staging server moved to porton.
            routes.MapRoute(
                "DownloadDocuments",
                "documents/{filename}.{ext}",
                new
                {
                    controller = "Download",
                    action = "Index"
                }
            );
        }

        private static void AddTwitterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "Tweets", // Route name
                "tweets/{twitterHandle}", // URL with parameters
                new
                {
                    controller = "Twitter",
                    action = "Tweets",
                    account = UrlParameter.Optional
                }
                );
        }

        private static void AddPhofRoutes(RouteCollection routes)
        {
            // PHOF specific
            routes.MapRoute(
                "PhofContactUs", // Route name
                "contact-us", // URL with parameters
                new
                {
                    controller = "Profile",
                    action = "SimpleHtmlPage",
                    viewName = "ContactUs",
                }
                );

            // PHOF specific
            routes.MapRoute(
                "PhofFaqs", // Route name
                "faqs", // URL with parameters
                new
                {
                    controller = "Profile",
                    action = "SimpleHtmlPage",
                    viewName = "Faqs",
                }
                );

            routes.MapRoute(
                "PhofFurtherInfo", // Route name
                "further-information", // URL with parameters
                new
                {
                    controller = "Profile",
                    action = "SimpleHtmlPage",
                    viewName = "FurtherInfo",
                }
                );

            routes.MapRoute(
                "PhofDomains", // Route name
                "public-health-outcomes-framework/domain/{domain}", // URL with parameters
                new
                {
                    controller = ProfileController,
                    action = "Domain",
                    domain = UrlParameter.Optional
                }
                );
        }

        private static void AddSingleProfileWithFrontPageRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "PracticeProfiles", // Route name
                "profile/general-practice", // URL with parameters
                new
                {
                    controller = "SingleProfileWithFrontPage",
                    action = "PracticeProfilesFrontPage",
                    urlKey = "general-practice"
                }
                );

            routes.MapRoute(
                "PracticeProfilesData", // Route name
                "profile/general-practice/data", // URL with parameters
                new
                {
                    controller = "SingleProfileWithFrontPage",
                    action = "PracticeProfilesData",
                    urlKey = "general-practice"
                }
                );

            routes.MapRoute(
                "SingleProfileFrontPage", // Route name
                "profile/{urlKey}", // URL with parameters
                new
                {
                    controller = "SingleProfileWithFrontPage",
                    action = "FrontPage",
                    urlKey = UrlParameter.Optional
                }
                );

            routes.MapRoute(
                "SingleProfileDataPage", // Route name
                "profile/{urlKey}/data", // URL with parameters
                new
                {
                    controller = "SingleProfileWithFrontPage",
                    action = "Data",
                    urlKey = UrlParameter.Optional
                }
                );

            routes.MapRoute(
                "SingleProfileSupportingPage", // Route name
                "profile/{urlKey}/supporting-information/{contentKey}", // URL with parameters
                new
                {
                    controller = "SingleProfileWithFrontPage",
                    action = "SupportingPage",
                    urlKey = UrlParameter.Optional,
                    contentKey = UrlParameter.Optional
                }
                );
        }

        private static void AddProfileCollectionRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "ProfileCollectionFrontPage", // Route name
                "profile-group/{leadProfileUrlKey}", // URL with parameters
                new
                {
                    controller = "ProfileCollectionWithFrontPage",
                    action = "FrontPage",
                    leadProfileUrlKey = UrlParameter.Optional
                }
                );

            routes.MapRoute(
                "ProfileCollectionDataPage", // Route name
                "profile-group/{leadProfileUrlKey}/profile/{profileUrlKey}/data", // URL with parameters
                new
                {
                    controller = "ProfileCollectionWithFrontPage",
                    action = "Data",
                    leadProfileUrlKey = UrlParameter.Optional,
                    profileUrlKey = UrlParameter.Optional
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

        private static void AddLongerLivesRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "Policies", // Route name
                "policy/{policyType}", // URL with parameters
                new
                {
                    controller = LongerLivesController,
                    action = "Policy"
                }
                );

            routes.MapRoute(
                "MortalityRankings", // Route name
                "topic/mortality/comparisons", // URL with parameters
                new
                {
                    controller = LongerLivesController,
                    action = "MortalityRankings"
                }
                );

            routes.MapRoute(
                "MortalityAreaDetails", // Route name
                "topic/mortality/area-details", // URL with parameters
                new
                {
                    controller = LongerLivesController,
                    action = "MortalityAreaDetails"
                }
                );

            routes.MapRoute(
                "AboutProject", // Route name
                "about-project", // URL with parameters
                new
                {
                    controller = LongerLivesController,
                    action = "AboutProject"
                }
                );

            routes.MapRoute(
                "HomeProfileSpecific",
                "topic/{profileKey}",
                new
                {
                    controller = LongerLivesController,
                    action = "Home",
                    profileKey = UrlParameter.Optional
                }
                );

            routes.MapRoute(
                "AboutProjectProfileSpecific", // Route name
                "topic/{profileKey}/about-project", // URL with parameters
                new
                {
                    controller = LongerLivesController,
                    action = "AboutProject",
                    profileKey = UrlParameter.Optional
                }
                );

            routes.MapRoute(
                "AboutData", // Route name
                "about-data", // URL with parameters
                new
                {
                    controller = LongerLivesController,
                    action = "AboutData"
                }
                );

            routes.MapRoute(
                "AboutDataProfileSpecific", // Route name
                "topic/{profileKey}/about-data", // URL with parameters
                new
                {
                    controller = LongerLivesController,
                    action = "AboutData",
                    profileKey = UrlParameter.Optional
                }
                );

            routes.MapRoute(
                "Connect", // Route name
                "connect", // URL with parameters
                new
                {
                    controller = LongerLivesController,
                    action = "Connect"
                }
                );

            routes.MapRoute(
                "ConnectProfileSpecific", // Route name
                "topic/{profileKey}/connect", // URL with parameters
                new
                {
                    controller = LongerLivesController,
                    action = "Connect",
                    profileKey = UrlParameter.Optional
                }
                );

            routes.MapRoute(
                "Intervention", // Route name
                "topic/{profileKey}/health-intervention/{intervention}", // URL with parameters
                new
                {
                    controller = LongerLivesController,
                    action = "HealthIntervention"
                }
                );

            routes.MapRoute(
                "PracticeRankings", // Route name
                "topic/{profileKey}/comparisons", // URL with parameters
                new
                {
                    controller = LongerLivesController,
                    action = "PracticeRankings",
                    profileKey = UrlParameter.Optional
                }
                );

            routes.MapRoute(
                "PracticeDetails", // Route name
                "topic/{profileKey}/practice-details", // URL with parameters
                new
                {
                    controller = LongerLivesController,
                    action = "PracticeDetails",
                    profileKey = UrlParameter.Optional
                }
                );

            routes.MapRoute(
                "AreaDetails",
                "topic/{profileKey}/area-details",
                new
                {
                    controller = LongerLivesController,
                    action = "AreaDetails",
                    profileKey = UrlParameter.Optional
                }
                );

            routes.MapRoute(
                "KeyMessages", // Route name
                "topic/{profileKey}/key-messages", // URL with parameters
                new
                {
                    controller = LongerLivesController,
                    action = "KeyMessages",
                    profileKey = UrlParameter.Optional
                }
                );

            routes.MapRoute(
                "AreaSearchResultsLongerLives", // Route name
                "topic/{profileKey}/area-search-results", // URL with parameters
                new
                {
                    controller = LongerLivesController,
                    action = "AreaSearchResults",
                    profileKey = UrlParameter.Optional
                }
                );
        }

        private static void AddTestRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "TestIndex", // Route name
                "test", // URL with parameters
                new
                {
                    controller = "Test",
                    action = "Index"
                }
                );

            routes.MapRoute(
                "TestError", // Route name
                "test/error", // URL with parameters
                new
                {
                    controller = "Test",
                    action = "TestError"
                }
                );

            routes.MapRoute(
                "TestPage", // Route name
                "test/{page}", // URL with parameters
                new
                {
                    controller = "Test",
                    action = "TestPage",
                    page = UrlParameter.Optional
                }
                );
        }

        private static void AddAjaxBridgeRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "AjaxBridgeData", // Route name
                "data/{serviceAction1}", // URL with parameters
                new
                {
                    controller = "AjaxBridge",
                    action = "Data"
                }
                );

            routes.MapRoute(
                "AjaxBridgeDataForPdfs", // Route name
                "data/{serviceAction1}/{serviceAction2}", // URL with parameters
                new
                {
                    controller = "AjaxBridge",
                    action = "Data"
                }
                );


            routes.MapRoute(
                "ApiDocs", // Route name
                "api", // URL with parameters
                new
                {
                    controller = "SwaggerBridge",
                    action = "ApiDocsPage"
                }
                );

            routes.MapRoute(
                "ApiServiceDetails", // Route name
                "swagger/docs/v1", // URL with parameters
                new
                {
                    controller = "SwaggerBridge",
                    action = "ApiServiceDetails"
                }
                );

            routes.MapRoute(
                "ApiAsset", // Route name
                "api/asset/{part1}/{part2}", // URL with parameters
                new
                {
                    controller = "SwaggerBridge",
                    action = "ApiAsset",
                    part2 = UrlParameter.Optional
                }
                );

            routes.MapRoute(
                "AreaDetailInfo", // Route name
                "api/areadetail/{areaCode}", // URL with parameters
                new
                {
                    controller = "AreaSearch",
                    action = "AreaDetails",
                    place_name = UrlParameter.Optional,
                    search_type = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                "AjaxBridgeApi", // Route name
                "api/{serviceAction1}", // URL with parameters
                new
                {
                    controller = "AjaxBridge",
                    action = "Data"
                }
                );

            routes.MapRoute(
                "AjaxBridgeApiTwoLevel", // Route name
                "api/{serviceAction1}/{serviceAction2}", // URL with parameters
                new
                {
                    controller = "AjaxBridge",
                    action = "Data"
                }
                );

            routes.MapRoute(
                "AjaxBridgeApiThreeLevel", // Route name
                "api/{serviceAction1}/{serviceAction2}/{serviceAction3}", // URL with parameters
                new
                {
                    controller = "AjaxBridge",
                    action = "Data"
                }
                );

            routes.MapRoute(
                "AjaxBridgeLog", // Route name
                "log/{serviceAction}", // URL with parameters
                new
                {
                    controller = "AjaxBridge",
                    action = "Log"
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
                // Common route not handled in live logs
                {"profile/local-alcohol", "/profile/local-alcohol-profiles"},
                // CHMP profile retired
                {"profile-group/mental-health/profile/cmhp", "/profile-group/mental-health"},
                //FIN-1574 Redirect now separate profiles for Diabetes in Longer Lives and Fingertips
                {"profile/diabetes", "/profile/diabetes-ft"},
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