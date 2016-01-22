using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Fpm.MainUI.Helpers;
using Fpm.MainUI.Mapper;
using Fpm.MainUI.Models;
using Fpm.MainUI.ViewModels;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Entities.User;
using Fpm.ProfileData.Repositories;
using Newtonsoft.Json;

namespace Fpm.MainUI.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ProfilesReader _reader = ReaderFactory.GetProfilesReader();
        private readonly ProfilesWriter _writer = ReaderFactory.GetProfilesWriter();

        private  ProfileRepository _profileRepository;
        private  UserRepository _userRepository;
        private LookUpsRepository _lookUpsRepository;

        private string _userName;

        [AuthorizedUsers]
        public ActionResult ManageProfiles()
        {
            var model = new ProfileGridModel
            {
                SortBy = "Sequence",
                SortAscending = true,
                CurrentPageIndex = 1,
                PageSize = 200
            };

            GetAllProfiles(model);

            return View(model);
        }

        [AuthorizedUsers]
        public ActionResult ManageProfileCollections()
        {
            var model = new ProfileCollectionGridModel
            {
                SortBy = "Sequence",
                SortAscending = true,
                CurrentPageIndex = 1,
                PageSize = 200
            };

            GetAllProfileCollections(model);

            return View(model);
        }

        private void GetAllProfiles(ProfileGridModel model)
        {
            model.ProfileGrid = _reader.GetProfiles().OrderBy(x => x.Name).ToList();
        }

        private void GetAllProfileCollections(ProfileCollectionGridModel model)
        {
            model.ProfileCollectionGrid = _reader.GetProfileCollections().OrderBy(x => x.CollectionName).ToList();
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _userName = UserDetails.CurrentUser().Name;
        }

        public ActionResult GenericIndicator(int indicatorId)
        {
            var model = new IndicatorEdit { SelectedIndicatorId = indicatorId };
            IList<IndicatorMetadataTextProperty> properties = _reader.GetIndicatorMetadataTextProperties();
            model.TextValues = _reader.GetIndicatorTextValues(indicatorId, properties, null).ToList();
            return View("IndicatorEdit", model);
        }

        public ActionResult IndicatorEdit(string urlKey, int areaType, int selectedDomainNumber, int indicatorId, int ageId, int sexId)
        {
            Profile profile = GetProfile(urlKey, selectedDomainNumber, areaType);

            // Get text properties of selected indicator
            IList<IndicatorMetadataTextProperty> properties = _reader.GetIndicatorMetadataTextProperties();
            int groupId = profile.GetSelectedGroupingMetadata(selectedDomainNumber).GroupId;

            // Assemble model
            var model = new IndicatorEdit
            {
                SelectedIndicatorId = indicatorId,
                UrlKey = urlKey,
                Profile = profile,
                TextValues = _reader.GetIndicatorTextValues(indicatorId, properties, profile.Id).ToList()
            };

            // Prev/Next
            if (profile.IndicatorNames.Count > 0)
            {
                int index = -1;
                IList<GroupingPlusName> names = profile.IndicatorNames;
                for (int i = 0; i < names.Count(); i++)
                {
                    if (names[i].IndicatorId == indicatorId)
                    {
                        index = i;
                        break;
                    }
                }

                int prevIndex = index > 0 ? index - 1 : names.Count - 1;
                int nextIndex = index == names.Count - 1 ? 0 : index + 1;
                model.IndicatorIdNext = names[nextIndex].IndicatorId;
                model.IndicatorIdPrevious = names[prevIndex].IndicatorId;
            }

            //Get the indicatore meta data
            IndicatorMetadata indicatorMetaData = _reader.GetIndicatorMetadata(indicatorId);
            model.IndicatorMetadata = indicatorMetaData;

            IList<Grouping> groupList = _reader.GetGroupings(groupId);
            IEnumerable<Grouping> indicatorGroupData =
                groupList.Where(
                    g =>
                        g.IndicatorId == indicatorId &&
                        g.GroupId == groupId &&
                        g.AreaTypeId == areaType &&
                        g.AgeId == ageId &&
                        g.SexId == sexId);
            Grouping[] groupData = indicatorGroupData as Grouping[] ?? indicatorGroupData.ToArray();
            model.Grouping = groupData.FirstOrDefault();

            //Set the comparator Id if this is a multiple comparator grouping record
            if (groupData.Count() > 1)
            {
                //There are multiple comparator indicators
                if (model.Grouping != null)
                {
                    model.Grouping.ComparatorId = ComparatorIds.NationalAndSubnational;
                }
            }

            model.urlKey = urlKey;

            var listOfProfiles = CommonUtilities.GetOrderedListOfProfilesForCurrentUser(urlKey);

            ViewBag.listOfProfiles = listOfProfiles;
            
            var domains = new ProfileMembers();
            
            var defaultProfile = listOfProfiles.FirstOrDefault(x => x.Selected) ?? listOfProfiles.FirstOrDefault();
            defaultProfile.Selected = true;

            ViewBag.listOfDomains = CommonUtilities.GetOrderedListOfDomainsWithGroupId(domains, defaultProfile, _profileRepository);

            IEnumerable<UserGroupPermissions> userPermissions =
                CommonUtilities.GetUserGroupPermissionsByUserId(_reader.GetUserByUserName(_userName).Id);
            model.UserGroupPermissions =
                userPermissions.FirstOrDefault(x => x.ProfileId == _reader.GetProfileDetails(model.UrlKey).Id);

            if (HttpContext.Request.UrlReferrer != null) model.ReturnUrl = HttpContext.Request.UrlReferrer.ToString();

            ViewBag.SexId = new SelectList(_lookUpsRepository.GetSexes(), "SexID", "Description");

            ViewBag.AgeId = new SelectList(_lookUpsRepository.GetAges(), "AgeID", "Description");

            ViewBag.YearTypeId = new SelectList(_lookUpsRepository.GetYearTypes(), "Id", "Label");

            ViewBag.ValueTypeId = new SelectList(_lookUpsRepository.GetIndicatorValueTypes(), "Id", "Label");

            ViewBag.CIMethodId = new SelectList(_lookUpsRepository.GetConfidenceIntervalMethods(), "Id", "Name");

            var unitList = new SelectList(_lookUpsRepository.GetUnits(), "Id", "Label");
            ViewBag.UnitId = unitList;

            ViewBag.DenominatorTypeId = new SelectList(_lookUpsRepository.GetDenominatorTypes(), "Id", "Name");
            
            return View("IndicatorEdit", model);
        }

        [HttpPost]
        public ActionResult IndicatorEditSave(int? decimalPlaces, int? targetId, string urlKey, int areaType, int selectedDomainNumber,
            int indicatorId, int valueTypeId, int ciMethodId, string ciComparatorConfidence, int polarityId,
            int unitId, int denominatorTypeId, int yearTypeId, int areaTypeId, int sexId, int ageId, int comparatorId,
            int comparatorMethodId, double comparatorConfidence, int yearRange, int selectedFrequency, int baselineYear,
            int datapointYear, string returnUrl, int indicatorSequence, int currentAgeId, int currentSexId,
            int currentAreaTypeId, string userMTVChanges, int startQuarterRange = -1, int endQuarterRange = -1,
            int startMonthRange = -1, int endMonthRange = -1)
        {
            Profile profile = GetProfile(urlKey, selectedDomainNumber, areaType);

            int groupId = profile.GetSelectedGroupingMetadata(selectedDomainNumber).GroupId;

            SaveMetadataChanges(indicatorId, groupId, valueTypeId, ciMethodId, ciComparatorConfidence, polarityId,
                unitId,
                denominatorTypeId, yearTypeId, areaTypeId, sexId, ageId, comparatorId, comparatorMethodId,
                comparatorConfidence,
                yearRange, selectedFrequency, baselineYear, datapointYear, startQuarterRange, endQuarterRange,
                startMonthRange,
                endMonthRange, indicatorSequence, currentAgeId, currentSexId, currentAreaTypeId, decimalPlaces, targetId, profile.Id);

            return Redirect(returnUrl);
        }

        private void SaveMetadataChanges(int indicatorId, int? groupId, int valueTypeId, int ciMethodId,
            string ciComparatorConfidence, int polarityId, int unitId, int denominatorTypeId, int yearTypeId,
            int areaTypeId, int sexId, int ageId, int comparatorId, int comparatorMethodId, double comparatorConfidence,
            int yearRange, int selectedFrequency, int baselineYear, int datapointYear, int baselineQuarter,
            int dataPointQuarter, int baselineMonth, int dataPointMonth, int indicatorSequence, int currentAgeId,
            int currentSexId, int currentAreaTypeId, int? decimalPlaces, int? targetId, int profileId)
        {
            DateTime timeOfChange = DateTime.Now;

            switch (selectedFrequency)
            {
                case Frequencies.Annual:
                    baselineMonth = -1;
                    dataPointMonth = -1;
                    baselineQuarter = -1;
                    dataPointQuarter = -1;
                    break;
                case Frequencies.Quarterly:
                    baselineMonth = -1;
                    dataPointMonth = -1;
                    break;
                case Frequencies.Monthly:
                    baselineQuarter = -1;
                    dataPointQuarter = -1;
                    break;
            }

            var indicatorMetadataTextChanges = SaveIndicatorMetadataTextChanges(indicatorId, groupId, timeOfChange, profileId);

            _profileRepository.UpdateGroupingAndMetaData(Convert.ToInt32(groupId), indicatorId, areaTypeId, sexId, ageId,
                comparatorId, comparatorMethodId, ciComparatorConfidence, comparatorConfidence, yearTypeId, yearRange, valueTypeId,
                ciMethodId, polarityId, unitId, denominatorTypeId, baselineYear, datapointYear, baselineQuarter,
                dataPointQuarter, baselineMonth, dataPointMonth, indicatorSequence, currentAgeId, currentSexId,
                currentAreaTypeId, indicatorMetadataTextChanges, _userName, CommonUtilities.AuditType.Change.ToString(), decimalPlaces, targetId);
        }

        private string[] SaveIndicatorMetadataTextChanges(int indicatorId, int? groupId, DateTime timeOfChange, int profileId)
        {
            IList<IndicatorMetadataTextProperty> properties = _reader.GetIndicatorMetadataTextProperties();

            string metadataTextChanges = Request.Params["userMTVChanges"];

            string[] allChanges = Array.ConvertAll(Request.Params["userOtherChanges"].Split(Convert.ToChar("¬")), s => s);

            //Update the Indicator Meta Data Text Values
            if (string.IsNullOrWhiteSpace(metadataTextChanges) == false)
            {
                CommonUtilities.UpdateIndicatorTextValues(indicatorId, groupId, metadataTextChanges, properties,
                    timeOfChange, _userName, profileId, _profileRepository);
            }
            return allChanges;
        }

        private Profile GetProfile(string urlKey, int selectedDomainNumber, int areaType)
        {
            return new ProfileBuilder(_profileRepository).Build(urlKey, selectedDomainNumber, areaType);
        }

        [HttpGet]
        [Route("CreateProfile")]
        public ActionResult CreateProfile()
        {
            var profileViewModel = new ProfileViewModel()
            {
                ProfileUsers = new List<ProfileUser>(),
            };

            if (HttpContext.Request.UrlReferrer != null)
            {
                profileViewModel.ReturnUrl = HttpContext.Request.UrlReferrer.ToString();
            }

            ViewBag.AllUsers = _userRepository.GetAllFpmUsers().ToProfileUserList();

            ViewBag.ContactUserId = GetFpmUserList();

            return View("CreateProfile", profileViewModel);
        }

        [HttpPost]
        [Route("CreateProfile")]
        public ActionResult CreateProfile(ProfileViewModel profileViewModel)
        {
            profileViewModel.ProfileUsers = JsonConvert.DeserializeObject<IEnumerable<ProfileUser>>(Request["ProfileUsers"]);

            // Create new profile
            var profile = profileViewModel.ToProfileDetails();
            profile.SetDefaultValues();
            var userName = UserDetails.CurrentUser().Name;
            int newProfileId = _profileRepository.CreateProfile(profile);

            // New domain
            _writer.NewGroupingMetadata(profile.DefaultDomainName, 1, newProfileId);

            // Create default content items
            new DefaultProfileContentWriter(_writer, newProfileId, userName)
                .CreateContentItems();

            return RedirectToAction("ManageProfiles");
        }

        [Route("EditProfile")]
        [HttpGet]
        public ActionResult EditProfile(int profileId, string profileKey)
        {
            var profileViewModel = _reader.GetProfileDetails(profileKey).ToProfileViewModel();

            if (HttpContext.Request.UrlReferrer != null)
            {
                profileViewModel.ReturnUrl = HttpContext.Request.UrlReferrer.ToString();
            }

            profileViewModel.SelectedPdfAreaTypes = _reader.GetAreaTypesWhichContainsPdf(profileId).ToProfileAreaTypeList();

            profileViewModel.ProfileUsers = _reader.GetProfileUsers(profileId).ToProfileUserList();

            // Get all dropdowns in ViewBag

            ViewBag.AllUsers = _userRepository.GetAllFpmUsers().ToProfileUserList();

            ViewBag.DefaultAreaTypeId = new SelectList(_reader.GetSupportedAreaTypes(), "Id", "ShortName");

            ViewBag.SpineChartMinMaxLabelId = new SelectList(_reader.GetSpineChartMinMaxLabelOptions(), "Id", "Description");

            ViewBag.ContactUserId = GetFpmUserList();

            ViewBag.KeyColourId = new SelectList(_lookUpsRepository.GetKeyColours(), "Id", "Description");

            ViewBag.SkinId = new SelectList(_lookUpsRepository.GetSkins(), "Id", "Name");

            ViewBag.DefaultFingertipsTabId = new SelectList(GetListOfFingertipsTabs(), "Value", "Text");

            ViewBag.AvailableAreaTypes = _reader.GetAreaTypes(profileId).ToProfileAreaTypeList();

            return View("EditProfile", profileViewModel);
        }

        [Route("EditProfile")]
        [HttpPost]
        public ActionResult EditProfile(ProfileViewModel profileViewModel)
        {
            profileViewModel.SelectedPdfAreaTypes = JsonConvert.DeserializeObject<IEnumerable<ProfileAreaType>>(Request["SelectedPdfAreaTypes"]);
            
            profileViewModel.ProfileUsers = JsonConvert.DeserializeObject<IEnumerable<ProfileUser>>(Request["ProfileUsers"]);
            
            var profile = profileViewModel.ToProfileDetails();
            
            ProfileDetailsCleaner.CleanUserInput(profile);

            _profileRepository.UpdateProfile(profile);

            return RedirectToAction("ManageProfiles");
        }

        public ActionResult EditProfileCollection(int profileCollectionId)
        {
            //Get all profiles
            IOrderedEnumerable<ProfileDetails> allProfiles = _reader.GetProfiles().OrderBy(x => x.Name);

            ProfileCollection profileCollection = _reader.GetProfileCollection(profileCollectionId);
            profileCollection.ProfileCollectionItems = new List<ProfileCollectionItem>();

            profileCollection.Id = profileCollectionId;
            IList<ProfileCollectionItem> profileCollectionItems = _reader.GetProfileCollectionItems(profileCollectionId);

            foreach (ProfileDetails profile in allProfiles)
            {
                profileCollection.ProfileCollectionItems.Add(new ProfileCollectionItem
                {
                    ProfileId = profile.Id,
                    profileDetails = profile,
                    Selected = profileCollectionItems.Any(x => x.ProfileId == profile.Id),
                    DisplayDomains =
                        profileCollectionItems.Where(x => x.ProfileId == profile.Id)
                            .Select(x => x.DisplayDomains)
                            .FirstOrDefault()
                });
            }

            if (HttpContext.Request.UrlReferrer != null)
                profileCollection.ReturnUrl = HttpContext.Request.UrlReferrer.ToString();

            return View("EditProfileCollection", profileCollection);
        }

        public ActionResult CreateProfileCollection()
        {
            //Get all profiles
            IOrderedEnumerable<ProfileDetails> allProfiles = _reader.GetProfiles().OrderBy(x => x.Name);
            var profileCollection = new ProfileCollection { ProfileCollectionItems = new List<ProfileCollectionItem>() };

            foreach (ProfileDetails profileDetails in allProfiles)
            {
                profileCollection.ProfileCollectionItems.Add(new ProfileCollectionItem { profileDetails = profileDetails });
            }

            if (HttpContext.Request.UrlReferrer != null)
                profileCollection.ReturnUrl = HttpContext.Request.UrlReferrer.ToString();

            return View("CreateProfileCollection", profileCollection);
        }

        public ActionResult SortProfilesAndFilter(string sortBy = "Id", 
            bool ascending = true, int page = 1, int pageSize = 200)
        {
            var model = new ProfileGridModel
            {
                SortBy = sortBy,
                SortAscending = @ascending,
                CurrentPageIndex = page,
                PageSize = pageSize
            };

            //Get all the profiles
            GetAllProfiles(model);
            IList<ProfileDetails> sortedProfiles = null;

            if (ascending)
            {
                switch (sortBy)
                {
                    case "Id":
                        sortedProfiles = model.ProfileGrid.OrderBy(x => x.Id).ToList();
                        break;
                    case "Url_Key":
                        sortedProfiles = model.ProfileGrid.OrderBy(x => x.UrlKey).ToList();
                        break;
                }
            }
            else
            {
                switch (sortBy)
                {
                    case "Id":
                        sortedProfiles = model.ProfileGrid.OrderByDescending(x => x.Id).ToList();
                        break;
                    case "Url_Key":
                        sortedProfiles = model.ProfileGrid.OrderByDescending(x => x.UrlKey).ToList();
                        break;
                }
            }

            model.ProfileGrid = sortedProfiles;

            return View("ManageProfiles", model);
        }

        public ActionResult UpdateProfileCollection(int id, string assignedProfiles, 
            string collectionName, string collectionSkinTitle)
        {
            _profileRepository.UpdateProfileCollection(id, assignedProfiles,
               collectionName.Trim(), collectionSkinTitle.Trim());

            return RedirectToAction("ManageProfileCollections");
        }

        public ActionResult InsertProfileCollection(ProfileCollection profileCollection, string assignedProfiles)
        {
            var newProfileCollection = new ProfileCollection
            {
                CollectionName = profileCollection.CollectionName,
                CollectionSkinTitle = profileCollection.CollectionSkinTitle
            };

            _profileRepository.CreateProfileCollection(newProfileCollection, assignedProfiles);

            return RedirectToAction("ManageProfileCollections");
        }

        [HttpPost]
        public ActionResult SetDataPoint(int profileId, int areaTypeId, int indicatorId, 
            int sexId, int ageId, int year, int quarter, int month, int yearRange)
        {
            if (UserDetails.CurrentUser().HasWritePermissionsToProfile(profileId) == false)
            {
                throw new FpmException("User does not have rights to change time period");
            }
   
            var groupIds = _reader.GetGroupingIds(profileId);
            var groupings = _writer.GetGroupings(groupIds, areaTypeId, indicatorId,sexId,
                ageId,yearRange);

            // Change data points
            foreach (var grouping in groupings)
            {
                grouping.DataPointYear = year;
                grouping.DataPointQuarter = quarter;
                grouping.DataPointMonth = month;
            }

            _writer.UpdateGroupingList(groupings);

            return Json("Saved OK");
        }

        [HttpPost]
        public ActionResult CreateNewFromOld(int? selectedDecimalPlaces, int? selectedTargetId, string selectedProfileId, string selectedDomain,
            int selectedAreaType, int selectedSex, int selectedAge, int selectedComparator, int selectedComparatorMethod,
            string selectedComparatorConfidence, int selectedYearType, int selectedYearRange, int selectedValueType,
            int selectedCiMethodType, string selectedCiConfidenceLevel, int selectedPolarityType, int selectedUnitType,
            int selectedDenominatorType, string userMTVChanges, int startYear, int endYear, int startQuarterRange,
            int endQuarterRange, int startMonthRange, int endMonthRange)
        {
            int newIndicatorId = SaveIndicatorAs(selectedProfileId, selectedDomain, selectedAreaType, selectedSex,
                selectedAge, selectedComparator, selectedComparatorMethod, selectedComparatorConfidence,
                selectedYearType, selectedYearRange, selectedValueType, selectedCiMethodType, selectedCiConfidenceLevel,
                selectedPolarityType, selectedUnitType, selectedDenominatorType, userMTVChanges, startYear, endYear,
                startQuarterRange, endQuarterRange, startMonthRange, endMonthRange, selectedDecimalPlaces, selectedTargetId);
            var newList = new List<string>();
            newList.Add(selectedProfileId);
            newList.Add(selectedAreaType.ToString());
            newList.Add(newIndicatorId.ToString());
            newList.Add(selectedDomain);

            return Json(newList);
        }


        private int SaveIndicatorAs(string selectedProfileId, string selectedDomain, int selectedAreaType,
         int selectedSex, int selectedAge, int selectedComparator, int selectedComparatorMethod,
         string selectedComparatorConfidence, int selectedYearType, int selectedYearRange, int selectedValueType,
         int selectedCiMethodType, string selectedCiConfidenceLevel, int selectedPolarityType, int selectedUnitType,
         int selectedDenominatorType, string mtvText, int startYear, int endYear, int startQuarterRange,
         int endQuarterRange, int startMonthRange, int endMonthRange, int? selectedDecimalPlaces, int? selectedTargetId)
        {
            IList<IndicatorMetadataTextProperty> properties = _reader.GetIndicatorMetadataTextProperties();

            int nextIndicatorId;
           
             nextIndicatorId = _profileRepository.GetNextIndicatorId();
            
            if (string.IsNullOrWhiteSpace(mtvText) == false)
                {
                   
                    CommonUtilities.CreateNewIndicatorTextValues(selectedDomain, mtvText, properties, nextIndicatorId,
                        _userName, _profileRepository);

                    _profileRepository.CreateGroupingAndMetaData(_reader.GetProfileDetails(selectedProfileId).Id,
                        Convert.ToInt32(selectedDomain),
                        nextIndicatorId, selectedAreaType, selectedSex, selectedAge, selectedComparator,
                        selectedComparatorMethod,
                        Convert.ToDouble(selectedComparatorConfidence), selectedYearType, selectedYearRange,
                        selectedValueType, selectedCiMethodType,
                        Convert.ToSingle(selectedCiConfidenceLevel), selectedPolarityType, selectedUnitType,
                        selectedDenominatorType, startYear, endYear,
                        startQuarterRange, endQuarterRange, startMonthRange, endMonthRange, selectedDecimalPlaces, selectedTargetId);
                }
            
            return nextIndicatorId;
        }
       
        public ActionResult RedirectToNew(string redirectUrl, string areaType)
        {
            return RedirectToAction("SortPageAndFilter", "ProfilesAndIndicators",
                new { ProfileKey = redirectUrl, SelectedAreaTypeId = areaType });
        }


        private static IEnumerable<SelectListItem> GetListOfFingertipsTabs()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem { Text = "Not Applicable", Value = "-1" }, 
                    new SelectListItem { Text = "Overview", Value = "0" }, 
                    new SelectListItem { Text = "Map", Value = "8" }, 
                    new SelectListItem { Text = "Trends", Value = "4" },
                    new SelectListItem { Text = "Compare areas", Value = "3" },
                    new SelectListItem { Text = "Area profiles", Value = "1" },
                    new SelectListItem { Text = "Compare indicators", Value = "10" },
                };
        }

   
        private IEnumerable<SelectListItem> GetFpmUserList()
        {
            return new SelectList(_userRepository.GetAllFpmUsers()
                .OrderBy(x => x.DisplayName)
                .Select(x => new SelectListItem { Text = x.DisplayName, Value = x.Id.ToString() })
                .ToList(), "Value", "Text");
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
           _profileRepository = new ProfileRepository();
           _userRepository = new UserRepository();
           _lookUpsRepository = new LookUpsRepository();

            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _profileRepository.Dispose();
            _userRepository.Dispose();
            _lookUpsRepository.Dispose();
            
            base.OnActionExecuted(filterContext);
        }




         
    }
}