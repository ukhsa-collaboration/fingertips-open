using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web.Mvc;
using System.Web.Routing;
using Fpm.MainUI.Helpers;
using Fpm.MainUI.Models;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Repositories;

namespace Fpm.MainUI.Controllers
{
    public class IndicatorNewController : Controller
    {
        private readonly ProfilesReader _reader = ReaderFactory.GetProfilesReader();
      
        private ProfileRepository _profileRepository;
        private LookUpsRepository _lookUpsRepository;

        private string _userName;

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _userName = UserDetails.CurrentUser().Name;
        }

        public ActionResult IndicatorNew()
        {
            // Get text properties of selected indicator
            var properties = _reader.GetIndicatorMetadataTextProperties();

            var userId = _reader.GetUserByUserName(UserDetails.CurrentUser().Name).Id;
            var permissionIds = _reader.GetUserGroupPermissionsByUserId(userId).Select(x => x.ProfileId);
            var model = new ProfileIndex { Profiles = 
                _reader.GetProfiles()
                .OrderBy(x => x.Name)
                .Where(x => permissionIds.Contains(x.Id)) };

            var listOfProfiles = CommonUtilities.GetOrderedListOfProfiles(model.Profiles);
            ViewBag.listOfProfiles = listOfProfiles;

            var domains = new ProfileMembers();
            var defaultProfile = listOfProfiles.FirstOrDefault();
            
            ViewBag.listOfDomains = CommonUtilities.GetOrderedListOfDomainsWithGroupId(domains, defaultProfile,_profileRepository);

            ViewBag.selectedSex = new SelectList(_lookUpsRepository.GetSexes(), "SexID", "Description");

            ViewBag.selectedAge = new SelectList(_lookUpsRepository.GetAges(), "AgeID", "Description");

            var comparatorOptions = CommonUtilities.GetListOfComparators(ComparatorIds.NationalAndSubnational);
            ViewBag.selectedComparator = new SelectList(comparatorOptions, "Value", "Text");

            ViewBag.selectedComparatorMethod = new SelectList(_reader.GetAllComparatorMethods(), "Id", "Name");

            ViewBag.selectedYearType = new SelectList(_lookUpsRepository.GetYearTypes(), "Id", "Label");

            ViewBag.selectedValueType = new SelectList(_lookUpsRepository.GetIndicatorValueTypes(), "Id", "Label");

            ViewBag.selectedCIMethodType = new SelectList(_lookUpsRepository.GetConfidenceIntervalMethods(), "Id", "Name");

            ViewBag.selectedUnitType = new SelectList(_lookUpsRepository.GetUnits(), "Id", "Label");

            ViewBag.selectedDenominatorType = new SelectList(_lookUpsRepository.GetDenominatorTypes(), "Id", "Name");
            
            return View(properties);
        }

        [HttpPost]
        public JsonResult ReloadDomains(string selectedProfile)
        {
            var model = new ProfileMembers
                {
                    UrlKey = selectedProfile,
                    Profile = CommonUtilities.GetProfile(selectedProfile, 1, AreaTypeIds.CountyAndUnitaryAuthority, _profileRepository)
                };

            IList<GroupingMetadata> groupingMetadatas = model.Profile.GroupingMetadatas;

            var domains = new SelectList(groupingMetadatas.OrderBy(g => g.Sequence), "GroupId", "GroupName");
            return Json(domains);
        }

        [HttpPost]
        public JsonResult SaveNewIndicator(int? selectedDecimalPlaces, int? selectedTargetId, string selectedProfileId, string selectedDomain,
            int selectedAreaType, int selectedSex, int selectedAge, int selectedComparator, int selectedComparatorMethod,
            double selectedComparatorConfidence, int selectedYearType, int selectedYearRange, int selectedValueType,
            int selectedCiMethodType, double selectedCiConfidenceLevel, int selectedPolarityType, int selectedUnitType,
            int selectedDenominatorType, string userMTVChanges, int startYear, int endYear,
            int startQuarterRange, int endQuarterRange, int startMonthRange, int endMonthRange)
        {
            var newIndicatorId = SaveUserMTVChanges(selectedProfileId, selectedDomain, selectedAreaType, selectedSex,
                selectedAge, selectedComparator, selectedComparatorMethod, selectedComparatorConfidence, selectedYearType,
                selectedYearRange, selectedValueType, selectedCiMethodType, selectedCiConfidenceLevel, selectedPolarityType,
                selectedUnitType, selectedDenominatorType, Uri.UnescapeDataString(userMTVChanges), startYear, endYear,
                startQuarterRange, endQuarterRange, startMonthRange, endMonthRange, selectedDecimalPlaces, selectedTargetId);
            return Json(newIndicatorId);
        }

        [Route("indicator/metadata/{indicatorId}")]
        public ActionResult GetIndicatorDefaultMetadata(int indicatorId)
        {
            var indicatorDefaultMetadata = _reader.GetIndicatorDefaultTextValues(indicatorId)[0];
            return Json(indicatorDefaultMetadata, JsonRequestBehavior.AllowGet);
        }

        private int SaveUserMTVChanges(string selectedProfileId, string selectedDomain, int selectedAreaType,
            int selectedSex, int selectedAge, int selectedComparator, int selectedComparatorMethod, 
            double selectedComparatorConfidence, int selectedYearType, int selectedYearRange, int selectedValueType, 
            int selectedCiMethodType, double selectedCiConfidenceLevel, int selectedPolarityType, int selectedUnitType, 
            int selectedDenominatorType, string userMTVChanges, int startYear, int endYear, 
            int startQuarterRange, int endQuarterRange, int startMonthRange, int endMonthRange, int? selectedDecimalPlaces, int? selectedTargetId)
        {   
            var properties = _reader.GetIndicatorMetadataTextProperties();

            var nextIndicatorId = _profileRepository.GetNextIndicatorId() + 1;
           
            if (string.IsNullOrWhiteSpace(userMTVChanges) == false)
            {
                CommonUtilities.CreateNewIndicatorTextValues(selectedDomain, userMTVChanges, properties, nextIndicatorId, _userName, _profileRepository);

                _profileRepository.CreateGroupingAndMetaData(_reader.GetProfileDetails(selectedProfileId).Id, Convert.ToInt32(selectedDomain),
                    nextIndicatorId, selectedAreaType, selectedSex, selectedAge, selectedComparator, selectedComparatorMethod,
                    selectedComparatorConfidence, selectedYearType, selectedYearRange, selectedValueType, selectedCiMethodType,
                    selectedCiConfidenceLevel, selectedPolarityType, selectedUnitType, selectedDenominatorType, startYear, endYear,
                    startQuarterRange, endQuarterRange, startMonthRange, endMonthRange, selectedDecimalPlaces,
                    selectedTargetId);
            }

            return nextIndicatorId;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _profileRepository = new ProfileRepository();
            _lookUpsRepository = new LookUpsRepository();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _profileRepository.Dispose();
            _lookUpsRepository.Dispose();

            base.OnActionExecuted(filterContext);
        }
    }
}
