using System.Web.Mvc;
using System.Web.Routing;

namespace Fpm.MainUI.Routes
{
    public class CoreRoutes
    {
        private const string ProfileController = "Profile";

        public static void RegisterRoutes(RouteCollection routes)
        {

            routes.MapRoute(
                "SearchAll", // Route name
                "SearchAll", // URL with parameters
                new
                {
                    controller = "Search",
                    action = "SearchAll",
                }
                );

            routes.MapRoute(
                "ProfileManager", // Route name
                "ProfileManager", // URL with parameters
                new
                {
                    controller = "Profile",
                    action = "ProfileIndex",
                }
                );

            routes.MapRoute(
                "IndicatorNew", // Route name
                "IndicatorNew", // URL with parameters
                new
                {
                    controller = "IndicatorNew",
                    action = "IndicatorNew",
                }
                );

            routes.MapRoute(
                "GetAudit", // Route name
                "GetAudit", // URL with parameters
                new
                {
                    controller = "ProfilesAndIndicators",
                    action = "GetAudit",
                }
                );

            routes.MapRoute(
                "UploadIndex", // Route name
                "Upload", // URL with parameters
                new
                {
                    controller = "Upload",
                    action = "Index",
                }
                );

            routes.MapRoute(
                "getWorksheets", // Route name
                "getWorksheets", // URL with parameters
                new
                {
                    controller = "Upload",
                    action = "getWorksheets",
                }
                );

            routes.MapRoute(
                "reloadDomains", // Route name
                "reloadDomains", // URL with parameters
                new
                {
                    controller = "IndicatorNew",
                    action = "reloadDomains",
                }
                );

            routes.MapRoute(
                "ShowDataTypeErrors", // Route name
                "ShowDataTypeErrors", // URL with parameters
                new
                {
                    controller = "Upload",
                    action = "ShowDataTypeErrors",
                },
                new { httpMethod = new HttpMethodConstraint("POST") }
                );

            routes.MapRoute(
                "CurrentUserJobProgress",
                "upload/progress/{userId}/{numberOfRecords}",
                new
                {
                    controller = "Upload",
                    action = "CurrentUserJobProgress"
                }
            );
            routes.MapRoute(
                "GetAllActiveJobProgress",
                "upload/progress",
                new
                {
                    controller = "Upload",
                    action = "GetAllActiveJobProgress"
                }
                );

            routes.MapRoute(
                "UploadJobSummary",
                "upload/summary/{guid}",
                new
                {
                    controller = "Upload",
                    action = "JobSummary"
                }
                );

            routes.MapRoute(
                "UploadJobChangeStatus",
                "upload/change-status/{guid}/{actioncode}",
                new
                {
                    controller = "Upload",
                    action = "ChangeStatus"
                }
                );

            routes.MapRoute(
                "UploadJobDownloadFile",
                "upload/download/{guid}",
                new
                {
                    controller = "Upload",
                    action = "Download"
                }
                );

            routes.MapRoute(
                "UploadJobSaveFile",
                "upload/file/save",
                new
                {
                    controller = "Upload",
                    action = "SaveFile"
                }
                );

            routes.MapRoute(
                "reloadGridDomains", // Route name
                "reloadGridDomains", // URL with parameters
                new
                {
                    controller = "ProfilesAndIndicators",
                    action = "reloadGridDomains",
                }
                );

            routes.MapRoute(
                "SaveNewIndicator", // Route name
                "SaveNewIndicator", // URL with parameters
                new
                {
                    controller = "indicatorNew",
                    action = "SaveNewIndicator",
                }
                );

            routes.MapRoute(
                "CreateNewFromOld", // Route name
                "CreateNewFromOld", // URL with parameters
                new
                {
                    controller = "Profile",
                    action = "CreateNewFromOld",
                }
                );

            routes.MapRoute(
                "SetDataPoint", // Route name
                "SetDataPoint", // URL with parameters
                new
                {
                    controller = "Profile",
                    action = "SetDataPoint",
                },
                new { httpMethod = new HttpMethodConstraint("POST") }
                );

            routes.MapRoute(
                "RedirectToNew", // Route name
                "RedirectToNew", // URL with parameters
                new
                {
                    controller = "Profile",
                    action = "RedirectToNew",
                }
                );

            routes.MapRoute(
                "saveDomains", // Route name
                "SaveDomains", // URL with parameters
                new
                {
                    controller = ProfileController,
                    action = "SaveDomains",
                }
                );

            routes.MapRoute(
                "saveGridDomains", // Route name
                "saveGridDomains", // URL with parameters
                new
                {
                    controller = "ProfilesAndIndicators",
                    action = "saveGridDomains",
                }
                );

            routes.MapRoute(
                "DeleteDomain", // Route name
                "DeleteDomain", // URL with parameters
                new
                {
                    controller = "ProfilesAndIndicators",
                    action = "DeleteDomain",
                } // Parameter defaults
                );

            routes.MapRoute(
                "ConfirmDeleteIndicators", // Route name
                "ConfirmDeleteIndicators", // URL with parameters
                new
                {
                    controller = "ProfilesAndIndicators",
                    action = "ConfirmDeleteIndicators",
                }
                );

            routes.MapRoute(
                "ConfirmMoveIndicators", // Route name
                "ConfirmMoveIndicators", // URL with parameters
                new
                {
                    controller = "ProfilesAndIndicators",
                    action = "ConfirmMoveIndicators",
                }
                );

            routes.MapRoute(
                "ConfirmCopyIndicators", // Route name
                "ConfirmCopyIndicators", // URL with parameters
                new
                {
                    controller = "ProfilesAndIndicators",
                    action = "ConfirmCopyIndicators",
                }
                );

            routes.MapRoute(
                "saveIndicators", // Route name
                "saveIndicators", // URL with parameters
                new
                {
                    controller = ProfileController,
                    action = "saveIndicators",
                }
                );

            routes.MapRoute(
                "saveReorderedIndicators", // Route name
                "saveReorderedIndicators", // URL with parameters
                new
                {
                    controller = "ProfilesAndIndicators",
                    action = "saveReorderedIndicators",
                }
                );

            routes.MapRoute(
                "Reorder", // Route name
                "Reorder", // URL with parameters
                new
                {
                    controller = ProfileController,
                    action = "Reorder",
                }
                );

            routes.MapRoute(
                "DeleteIndicators", // Route name
                "DeleteIndicators", // URL with parameters
                new
                {
                    controller = "ProfilesAndIndicators",
                    action = "DeleteIndicators",
                }
                );

            routes.MapRoute(
                "MoveIndicators", // Route name
                "MoveIndicators", // URL with parameters
                new
                {
                    controller = "ProfilesAndIndicators",
                    action = "MoveIndicators",
                }
                );

            routes.MapRoute(
                "CopyIndicators", // Route name
                "CopyIndicators", // URL with parameters
                new
                {
                    controller = "ProfilesAndIndicators",
                    action = "CopyIndicators",
                }
                );

            routes.MapRoute(
                "CopyMultipleIndicators", // Route name
                "CopyMultipleIndicators", // URL with parameters
                new
                {
                    controller = "Search",
                    action = "CopyMultipleIndicators",
                }
                );

            routes.MapRoute(
                "domainIndicators", // Route name
                "profile/{urlKey}/area-type/{areatype}/domain/{selectedDomainNumber}", // URL with parameters
                new
                {
                    controller = ProfileController,
                    action = "Profile",
                    urlKey = UrlParameter.Optional,
                    areaType = UrlParameter.Optional,
                    selectedDomainNumber = UrlParameter.Optional
                }
                );

            routes.MapRoute(
                "profile2", // Route name
                "profile/{urlKey}/area-type/{areatype}", // URL with parameters
                new
                {
                    controller = ProfileController,
                    action = "Profile",
                    urlKey = UrlParameter.Optional,
                    areaType = UrlParameter.Optional
                }
                );
        }

    }
}