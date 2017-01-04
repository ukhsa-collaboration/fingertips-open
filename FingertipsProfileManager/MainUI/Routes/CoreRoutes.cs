using System.Web.Mvc;
using System.Web.Routing;

namespace Fpm.MainUI.Routes
{
    public class CoreRoutes
    {
        private const string ProfileController = "Profile";

        public static void RegisterRoutes(RouteCollection routes)
        {
            RegisterContentRoutes(routes);
            RegisterLookupTableRoutes(routes);

            routes.MapRoute(
                "Admin", // Route name
                "admin", // URL with parameters
                new
                {
                    controller = "Admin",
                    action = "Admin",
                }
                );

            routes.MapRoute(
                "AdminDeleteIndicator", // Route name
                "admin/DeleteIndicator", // URL with parameters
                new
                {
                    controller = "Admin",
                    action = "DeleteIndicator",
                }
                );

            routes.MapRoute(
                "AdminChangeIndicatorOwner", // Route name
                "admin/ChangeIndicatorOwner", // URL with parameters
                new
                {
                    controller = "Admin",
                    action = "ChangeIndicatorOwner",
                }
                );

            routes.MapRoute(
                "DeleteIndicator", // Route name
                "admin/deleteindicator", // URL with parameters
                new
                {
                    controller = "Admin",
                    action = "DeleteIndicator",
                },
                new { httpMethod = new HttpMethodConstraint("POST") }
                );

            routes.MapRoute(
                "ManageAreas", // Route name
                "ManageAreas", // URL with parameters
                new
                {
                    controller = "Areas",
                    action = "ManageAreas",
                }
                );

            routes.MapRoute(
                "SearchAreas", // Route name
                "SearchAreas", // URL with parameters
                new
                {
                    controller = "Areas",
                    action = "SearchAreas",
                }
                );

            routes.MapRoute(
                "UpdateArea", // Route name
                "UpdateArea", // URL with parameters
                new
                {
                    controller = "Areas",
                    action = "UpdateArea",
                }
                );

            routes.MapRoute(
                "ShowAreaDetails", // Route name
                "ShowAreaDetails", // URL with parameters
                new
                {
                    controller = "Areas",
                    action = "ShowAreaDetails",
                }
                );

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
                "SortPageAndFilter", // Route name
                "SortPageAndFilter", // URL with parameters
                new
                {
                    controller = "ProfilesAndIndicators",
                    action = "SortPageAndFilter",
                }
                );

            routes.MapRoute(
                "SortProfilesAndFilter", // Route name
                "SortProfilesAndFilter", // URL with parameters
                new
                {
                    controller = "Profile",
                    action = "SortProfilesAndFilter",
                }
                );


            routes.MapRoute(
                "GetAllUsers",
                "User/All",
                new
                {
                    controller = "User",
                    action = "GetAllUsers"
                });

            routes.MapRoute(
                "EditUser", // Route name
                "EditUser", // URL with parameters
                new
                {
                    controller = "User",
                    action = "EditUser",
                }
                );

            routes.MapRoute(
                "UpdateUser", // Route name
                "UpdateUser", // URL with parameters
                new
                {
                    controller = "User",
                    action = "UpdateUser",
                }
                );

            routes.MapRoute(
                "CreateUser", // Route name
                "CreateUser", // URL with parameters
                new
                {
                    controller = "User",
                    action = "CreateUser",
                }
                );

            routes.MapRoute(
                "InsertUser", // Route name
                "InsertUser", // URL with parameters
                new
                {
                    controller = "User",
                    action = "InsertUser",
                }
                );

            routes.MapRoute(
                "GetUserAudit", // Route name
                "GetUserAudit", // URL with parameters
                new
                {
                    controller = "User",
                    action = "GetUserAudit",
                }
                );
            routes.MapRoute(
              "AddProfile", // Route name
              "AddProfile", // URL with parameters
              new
              {
                  controller = "User",
                  action = "AddProfile",
              }
              );



            routes.MapRoute(
            "RemoveProfile", // Route name
            "RemoveProfile", // URL with parameters
            new
            {
                controller = "User",
                action = "RemoveProfile",
            }
            );


            routes.MapRoute(
                "ProfileManager", // Route name
                "ProfileManager", // URL with parameters
                new
                {
                    controller = "Profile",
                    action = "ManageProfiles",
                }
                );

            routes.MapRoute(
                "ProfileCollectionManager", // Route name
                "ProfileCollectionManager", // URL with parameters
                new
                {
                    controller = "Profile",
                    action = "ManageProfileCollections",
                }
                );

            routes.MapRoute(
                "ExceptionLog", // Route name
                "ExceptionLog", // URL with parameters
                new
                {
                    controller = "ExceptionLog",
                    action = "Index",
                }
                );

            routes.MapRoute(
                "ShowExceptionDetails", // Route name
                "ShowExceptionDetails", // URL with parameters
                new
                {
                    controller = "ExceptionLog",
                    action = "ShowExceptionDetails",
                }
                );

            routes.MapRoute(
                "ReloadExceptions", // Route name
                "ReloadExceptions", // URL with parameters
                new
                {
                    controller = "ExceptionLog",
                    action = "ReloadExceptions",
                }
                );

            routes.MapRoute(
                "EditProfileCollection", // Route name
                "EditProfileCollection", // URL with parameters
                new
                {
                    controller = "Profile",
                    action = "EditProfileCollection",
                }
                );

            routes.MapRoute(
                "InsertProfileCollection", // Route name
                "InsertProfileCollection", // URL with parameters
                new
                {
                    controller = "Profile",
                    action = "InsertProfileCollection",
                }
                );

            routes.MapRoute(
                "UpdateProfileCollection", // Route name
                "UpdateProfileCollection", // URL with parameters
                new
                {
                    controller = "Profile",
                    action = "UpdateProfileCollection",
                }
                );

            routes.MapRoute(
                "CreateProfileCollection", // Route name
                "CreateProfileCollection", // URL with parameters
                new
                {
                    controller = "Profile",
                    action = "CreateProfileCollection",
                }
                );

            routes.MapRoute(
                "DocumentsIndex",
                "Documents",
                new
                {
                    controller = "Documents",
                    action = "Index",
                }
                );

            routes.MapRoute(
                "DocumentUpload",
                "DocumentUpload",
                new
                {
                    controller = "Documents",
                    action = "Upload",
                }
                );

            routes.MapRoute(
                "DocumentDelete",
                "Documents/Delete/{id}",
                new
                {
                    controller = "Documents",
                    action = "Delete",
                }
                );


            routes.MapRoute(
                "ValidateUploadDocumentName",
                "validateDocumentName",
                new
                {
                    controller = "Documents",
                    action = "IsFileNameUnique",
                }
                );


            routes.MapRoute(
                "UserIndex", // Route name
                "User", // URL with parameters
                new
                {
                    controller = "User",
                    action = "UserIndex",
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
                "ProfilesAndIndicators", // Route name
                "ProfilesAndIndicators", // URL with parameters
                new
                {
                    controller = "ProfilesAndIndicators",
                    action = "Index",
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
                "UploadSimpleFile", // Route name
                "UploadSimpleFile", // URL with parameters
                new
                {
                    controller = "Upload",
                    action = "UploadSimpleFile"
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
                "upload/progress/{userId}",
                new
                {
                    controller = "Upload",
                    action = "CurrentUserJobProgress"
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
                "ReorderIndicators", // Route name
                "ReorderIndicators", // URL with parameters
                new
                {
                    controller = "ProfilesAndIndicators",
                    action = "ReorderIndicators",
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
                "genericIndicatorSave", // Route name
                "indicator/{indicatorId}", // URL with parameters
                new
                {
                    controller = ProfileController,
                    action = "GenericIndicatorSave"
                },
                new { httpMethod = new HttpMethodConstraint("POST") }
                );

            routes.MapRoute(
                "genericIndicator", // Route name
                "indicator/{indicatorId}", // URL with parameters
                new
                {
                    controller = ProfileController,
                    action = "GenericIndicator"
                },
                new { httpMethod = new HttpMethodConstraint("GET") }
                );

            routes.MapRoute(
                "indicatorEditSave", // Route name
                "profile/{urlKey}/area-type/{areatype}/domain/{selectedDomainNumber}/indicator/{indicatorId}",
                // URL with parameters
                new
                {
                    controller = ProfileController,
                    action = "IndicatorEditSave",
                    valueTypeId = UrlParameter.Optional,
                    baselineQuarter = UrlParameter.Optional
                },
                new { httpMethod = new HttpMethodConstraint("POST") }
                );

            routes.MapRoute(
                "indicatorEdit", // Route name
                "profile/{urlKey}/area-type/{areatype}/domain/{selectedDomainNumber}/indicator/{indicatorId}/ageId/{ageId}/sexId/{sexId}",
                // URL with parameters
                new
                {
                    controller = ProfileController,
                    action = "IndicatorEdit",
                    urlKey = UrlParameter.Optional,
                    areaType = UrlParameter.Optional,
                    selectedDomainNumber = UrlParameter.Optional,
                    indicatorId = UrlParameter.Optional,
                    ageId = UrlParameter.Optional,
                    sexId = UrlParameter.Optional,
                },
                new { httpMethod = new HttpMethodConstraint("GET") }
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

            routes.MapRoute(
                "default", // Route name
                "", // URL with parameters
                new
                {
                    controller = "ProfilesAndIndicators",
                    action = "Index"
                }
                );
        }

        private static void RegisterLookupTableRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "LookupTableIndex", // Route name
                "lookup-tables", // URL with parameters
                new
                {
                    controller = "LookUpTables",
                    action = "Index",
                }
                );

            routes.MapRoute(
                "LookupTableSexes", // Route name
                "lookup-tables/sexes", // URL with parameters
                new
                {
                    controller = "LookUpTables",
                    action = "Sexes",
                }
                );

            routes.MapRoute(
                "LookupTableAges", // Route name
                "lookup-tables/ages", // URL with parameters
                new
                {
                    controller = "LookUpTables",
                    action = "Ages",
                }
                );

            routes.MapRoute(
                "LookupTableValueNotes", // Route name
                "lookup-tables/value-notes", // URL with parameters
                new
                {
                    controller = "LookUpTables",
                    action = "ValueNotes",
                }
                );

            routes.MapRoute(
                "LookupTableCategories", // Route name
                "lookup-tables/categories", // URL with parameters
                new
                {
                    controller = "LookUpTables",
                    action = "Categories",
                    categoryTypeId = UrlParameter.Optional
                }
                );

            routes.MapRoute(
                "LookupTableTargets", // Route name
                "lookup-tables/targets", // URL with parameters
                new
                {
                    controller = "LookUpTables",
                    action = "Targets",
                }
                );

            routes.MapRoute(
                "LookupTableTargetEdit", // Route name
                "lookup-tables/target/edit/{targetId}", // URL with parameters
                new
                {
                    controller = "LookUpTables",
                    action = "TargetEdit",
                    targetId = UrlParameter.Optional
                }
                );

            routes.MapRoute(
                "LookupTableTargetNew", // Route name
                "lookup-tables/target/new", // URL with parameters
                new
                {
                    controller = "LookUpTables",
                    action = "TargetNew"
                }
                );
        }

        private static void RegisterContentRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "DeleteContentItem", // Route name
                "content/delete-content-item", // URL with parameters
                new
                {
                    controller = "Content",
                    action = "DeleteContentItem",
                },
                new { httpMethod = new HttpMethodConstraint("POST") }
                );

            routes.MapRoute(
                "EditContent", // Route name
                "EditContent", // URL with parameters
                new
                {
                    controller = "Content",
                    action = "EditContent",
                }
                );

            routes.MapRoute(
                "InsertContent", // Route name
                "InsertContent", // URL with parameters
                new
                {
                    controller = "Content",
                    action = "InsertContent",
                }
                );

            routes.MapRoute(
                "UpdateContent", // Route name
                "UpdateContent", // URL with parameters
                new
                {
                    controller = "Content",
                    action = "UpdateContent",
                }
                );

            routes.MapRoute(
                "CreateContent", // Route name
                "CreateContent", // URL with parameters
                new
                {
                    controller = "Content",
                    action = "CreateContent",
                }
                );

            routes.MapRoute(
                "GetContentAuditList", // Route name
                "GetContentAuditList", // URL with parameters
                new
                {
                    controller = "Content",
                    action = "GetContentAuditList",
                }
                );

            routes.MapRoute(
                "Content", // Route name
                "Content", // URL with parameters
                new
                {
                    controller = "Content",
                    action = "ContentIndex",
                }
                );

            routes.MapRoute(
                "ReloadContentItems", // Route name
                "ReloadContentItems", // URL with parameters
                new
                {
                    controller = "Content",
                    action = "ReloadContentItems",
                }
                );
        }
    }
}