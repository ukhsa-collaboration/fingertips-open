using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Profiles.DataAccess;
using Profiles.DataConstruction;
using Profiles.MainUI.Helpers;
using Profiles.MainUI.Models;
using Profiles.MainUI.Skins;
using Profiles.DomainObjects;

namespace Profiles.MainUI.Controllers
{
    public class BaseController : Controller
    {
        protected PageModel PageModel;
        protected string Host;
        protected AppConfig appConfig = AppConfig.Instance;
        protected ProfileCollectionBuilder ProfileCollectionBuilder;

        protected override void HandleUnknownAction(string actionName)
        {
            ErrorController.InvokeHttp404(HttpContext);
        }

        protected virtual void NewPageModel()
        {
            PageModel = new PageModel(appConfig);
        }

        protected void InitPageModel()
        {  
            ProfileCollectionBuilder = new ProfileCollectionBuilder(ReaderFactory.GetProfileReader(), appConfig);
            NewPageModel();

            var versionFolder = appConfig.JavaScriptVersionFolder;

            PageModel.CoreServicesUrl = appConfig.CoreWsUrl;
            PageModel.UseMinifiedJavaScript = appConfig.UseMinifiedJavaScript;

            PageModel.SetJavaScriptVersionFolder(versionFolder);
            PageModel.Skin = SkinFactory.GetSkin();

            PageModel.ProfileCollection = new List<ProfileCollection>();
            PageModel.ProfileCollectionIdList = new int[] { };

            PageModel.NationalProfileCollection = ProfileCollectionBuilder.GetCollection(ProfileCollectionIds.NationalProfiles);

            SetBridgeServicesUrl();
            ViewBag.ImagesUrl = appConfig.StaticContentUrl + versionFolder + "images/";
            ViewBag.PdfUrl = appConfig.PdfUrl;
            ViewBag.UseGoogleAnalytics = appConfig.UseGoogleAnalytics;
            ViewBag.UseJsonp = appConfig.UseJsonp;
            ViewBag.JavaScriptVersion = versionFolder;
            ViewBag.ShowCancer = appConfig.ShowCancer;
            ViewBag.ShowNearestNeighbours = appConfig.ShowNearestNeighbours;
            ViewBag.FeatureSwitch = appConfig.FeatureSwitch;
        }

        private void SetBridgeServicesUrl()
        {
            string bridge = appConfig.BridgeWsUrl;
            if (string.IsNullOrWhiteSpace(bridge))
            {
                IfRequestNotDefinedExplainWhy();
                var protocol = new FingertipsUrl(appConfig).Protocol;
                ViewBag.Protocol = protocol;
                bridge = protocol + Request.Url.Authority + "/";
            }
            PageModel.BridgeServicesUrl = bridge;
        }


        private void IfRequestNotDefinedExplainWhy()
        {
            if (Request == null)
            {
                throw new FingertipsException("Request is not available in the constructor, call from OnActionExecuting instead");
            }
        }

        protected ProfileDetails ConfigureFingertipsProfileAndPageModelWithProfileDetails(string profileKey)
        {
            ProfileDetails details = new ProfileDetailsBuilder(profileKey).Build();

            if (details != null)
            {
                ConfigureWithProfile(details);
                CheckSkinIsNotLongerLives();
            }
            return details;
        }

        private void CheckSkinIsNotLongerLives()
        {
            if (PageModel.Skin.IsLongerLives)
            {
                throw new FingertipsException("This view is not available for Longer Lives skin");
            }
        }

        protected void ConfigureWithProfile(ProfileDetails details)
        {
            ViewBag.ProfileDetails = details;
            ViewBag.ProfileUrlKey = details.ProfileUrlKey;

            if (PageModel != null)
            {
                PageModel.ProfileId = details.Id;
                PageModel.TemplateProfileId = details.Id;
                AssignDomainHeadings(details);
                ViewBag.AvailableDomains = details.Domains;
                ViewData["groupIds"] = details.Domains.Select(x => x.GroupId).ToArray();

                if (details.LongerLivesProfileDetails != null)
                {
                    ConfigureLongerLivesProfile(details);
                }
                else
                {
                    // Fingertips profile
                    ViewBag.Title = details.Title;
                }

                ViewBag.DefaultAreaType = details.DefaultAreaType;
                PageModel.RagColourId = details.RagColourId;
                PageModel.StartZeroYAxis = details.StartZeroYAxis;
                PageModel.DefaultFingertipsTabId = details.DefaultFingertipsTabId;
                PageModel.HasTrendMarkers = details.HasTrendMarkers;
                PageModel.UseTargetBenchmarkByDefault = details.UseTargetBenchmarkByDefault;
                PageModel.AreAnyPdfsForProfile = details.ArePdfs;

                ViewBag.EnumParentDisplay = details.EnumParentDisplay;
                ViewBag.ExtraJsFiles = details.ExtraJavaScriptFiles;
                ViewBag.ExtraCssFiles = details.ExtraCssFiles;

                PageModel.IgnoredSpineChartAreas = details.AreasToIgnoreForSpineCharts;
                PageModel.HasExclusiveSkin = details.HasExclusiveSkin;
                PageModel.YearRange = details.Id == ProfileIds.HealthInequalities
                                          ? new YearRange { StartYear = 2005, EndYear = 2008 }
                                          : null;
                ViewBag.ShowDataQuality = details.ShowDataQuality.ToString().ToLower();
                ViewBag.IsNational = details.IsNational.ToString().ToLower();

                ViewBag.FingertipsUrl = new FingertipsUrl(appConfig).Host;
                PageModel.IsOfficialStatistics = details.IsOfficialStatistics;
                PageModel.IsProfileWithOnlyStaticReports = details.IsProfileWithOnlyStaticReports;
                ViewBag.StaticReportsTimePeriods = details.StaticReportsTimePeriods ?? string.Empty;

                PageModel.SpineChartMinMaxLabel = new SpineChartMinMaxLabelBuilder(
                    details.SpineChartMinMaxLabel, 
                    PageModel.RagColourId
                    ).MinMaxLabels;
            }
        }

        private void ConfigureLongerLivesProfile(ProfileDetails details)
        {
            var longerLivesDetails = details.LongerLivesProfileDetails;

            ViewBag.LongerLivesSupportingProfileId =
                longerLivesDetails.SupportingProfileId;

            ViewBag.SupportingGroupId =
                ReaderFactory.GetProfileReader().GetDomainIds(longerLivesDetails.SupportingProfileDetails.Id).ToArray();

            ViewBag.RankingsMiddleColumnGroup = details.Domains[longerLivesDetails.DomainsToDisplay];
            ViewBag.DefaultGroupId = details.Domains[0].GroupId;

            ViewBag.HasPracticeData = JsHelper.GetJsBool(longerLivesDetails.HasPracticeData);

            ViewBag.LongerLivesExtraJsFiles =
                ProfileDetailsBuilder.ParseStringList(longerLivesDetails.ExtraJsFiles);

            ViewBag.Title = longerLivesDetails.Title;
            ViewBag.DomainsToDisplay = longerLivesDetails.DomainsToDisplay;

        }

        protected void AssignDomainHeadings(ProfileDetails details)
        {
            if (details.HasDomains)
            {
                ViewBag.DomainHeadings = details.Domains;
            }
        }
    }
}
