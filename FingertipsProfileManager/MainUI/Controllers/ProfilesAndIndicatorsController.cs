using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using System.Web.Routing;
using Fpm.MainUI.Helpers;
using Fpm.MainUI.Models;
using Fpm.MainUI.ViewModels.ProfilesAndIndicators;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Entities.User;
using Fpm.ProfileData.Repositories;

namespace Fpm.MainUI.Controllers
{
    public class ProfilesAndIndicatorsController : Controller
    {
        public const string ProfilesAndIndicators = "ProfilesAndIndicators";

        private readonly ProfilesReader _reader = ReaderFactory.GetProfilesReader();
        private readonly ProfilesWriter _writer = ReaderFactory.GetProfilesWriter();
        
        private ProfileRepository _profileRepository;
        private LookUpsRepository _lookUpsRepository;
        private CoreDataRepository _coreDataRepository;

        private string _userName;


        public ProfilesAndIndicatorsController()
        {
                
        }
        
        public ProfilesAndIndicatorsController(ProfileRepository profileRepository, LookUpsRepository lookUpsRepository, CoreDataRepository coreDataRepository)
        {
            _profileRepository = profileRepository;
            _lookUpsRepository = lookUpsRepository;
            _coreDataRepository = coreDataRepository;
        }


        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _userName = UserName;
        }

        [Route("TestPage")]    
        public ActionResult Test()
        {
            return Content("Test");
        }

        public ActionResult Index(int profileId = -1/*this is currently ignored but one for the future*/)
        {
            var model = new IndicatorGridModel
                                           {
                                               SortBy = "Sequence",
                                               SortAscending = true,
                                               CurrentPageIndex = 1,
                                               PageSize = 200
                                           };

            GetProfiles(model);
            GetDomains(model, null);

            Profile profile = null;
            var firstOrDefault = model.DomainList.FirstOrDefault();
            if (firstOrDefault != null)
            {
                profile = GetProfile(model.ProfileKey, Convert.ToInt32(firstOrDefault.Value), -1);
            }

            if (profile != null)
            {
                IQueryable<GroupingPlusName> indicators = profile.IndicatorNames.AsQueryable();

                model.Profile = profile;
                model.UrlKey = model.ProfileKey;

                var profileContact = _reader.GetUserByUserId(profile.ContactUserId);
                model.ContactUserName = profileContact.DisplayName;
                model.EmailAddress = profileContact.EmailAddress;

                AssignAreaTypeData(model, profile);

                var userPermissions = GetUserGroupPermissions();

                model.UserHasAssignedPermissions = userPermissions.Any();

                model.UserGroupPermissions = userPermissions.FirstOrDefault(x => x.ProfileId == _reader.GetProfileDetails(model.UrlKey).Id);

                indicators = indicators.Where(i => i.AreaTypeId == profile.SelectedAreaType);

                model.TotalRecordCount = indicators.Count();

                model.IndicatorNamesGrid = indicators
                    .Distinct()
                    .OrderBy(model.SortExpression)
                    .Skip((model.CurrentPageIndex - 1) * model.PageSize)
                    .Take(model.PageSize);
            }

            return View(ProfilesAndIndicators, model);
        }

        private List<UserGroupPermissions> GetUserGroupPermissions()
        {
            var fpmUser = _reader.GetUserByUserName(_userName);
            List<UserGroupPermissions> userPermissions = fpmUser != null
                ? CommonUtilities.GetUserGroupPermissionsByUserId(fpmUser.Id).ToList()
                : new List<UserGroupPermissions>();
            return userPermissions;
        }

        private void GetDomains(IndicatorGridModel model, int? domainSequence)
        {
            var reloadedDomains = ReloadGridDomains(model.ProfileKey, _profileRepository);

            if (domainSequence.HasValue)
            {
                model.SelectedGroupId =
                    reloadedDomains.Where(x => x.Sequence == domainSequence).Select(x => x.GroupId).FirstOrDefault();
            }
            else
            {
                model.SelectedGroupId = reloadedDomains.Select(x => x.GroupId).FirstOrDefault();
            }

            model.DomainList = new SelectList(reloadedDomains.OrderBy(g => g.Sequence), "Sequence", "GroupName");
        }

        private void GetProfiles(IndicatorGridModel model)
        {
            var profiles = _reader.GetProfiles().OrderBy(x => x.Name);
            IList<ProfileDetails> profilesWithAssignedIndicators = profiles.ToList();
            var listOfProfiles = CommonUtilities.GetOrderedListOfProfiles(profilesWithAssignedIndicators);
            model.ProfileList = listOfProfiles;
            var firstOrDefault = model.ProfileList.FirstOrDefault();
            if (firstOrDefault != null) model.ProfileKey = firstOrDefault.Value;
        }

        public ActionResult SortPageAndFilter(int? domainSequence, int? indicatorId, string resetArea,
            int selectedAreaTypeId = -1, string search_text = "", string sortBy = "Sequence",
            bool ascending = true, int page = 1, int pageSize = 200, string profileKey = null)
        {
            var model = new IndicatorGridModel
                {
                    SortBy = sortBy,
                    SortAscending = @ascending,
                    CurrentPageIndex = page,
                    PageSize = pageSize
                };

            // Get all the profiles
            GetProfiles(model);
            var firstOrDefault = model.ProfileList.FirstOrDefault();
            if (firstOrDefault != null)
                model.ProfileKey = string.IsNullOrEmpty(profileKey) ? firstOrDefault.Value : profileKey;
            GetDomains(model, domainSequence);

            var areaTypeId = resetArea == "True"
                ? -1
                : selectedAreaTypeId;

            if (domainSequence == null)
            {
                var selectListItem = model.DomainList.FirstOrDefault();
                if (selectListItem != null)
                {
                    domainSequence = int.Parse(selectListItem.Value);
                }
            }

            Profile profile = null;
            if (domainSequence.HasValue)
            {
                profile = GetProfile(model.ProfileKey, domainSequence.Value, areaTypeId);
            }

            if (profile != null)
            {
                IQueryable<GroupingPlusName> indicators = profile.IndicatorNames.AsQueryable();

                AssignAreaTypeData(model, profile);

                model.Profile = profile;
                model.UrlKey = model.ProfileKey;

                var profileContact = _reader.GetUserByUserId(profile.ContactUserId);
                model.ContactUserName = profileContact.DisplayName;
                model.EmailAddress = profileContact.EmailAddress;

                // Filter indicators for any search terms
                if (indicatorId != null)
                {
                    indicators = indicators.Where(i => i.IndicatorId.ToString().Contains(indicatorId.ToString()));
                }

                if (string.IsNullOrEmpty(search_text) == false)
                {
                    indicators = indicators.Where(i => i.IndicatorName.ToLower().Contains(search_text.ToLower()));
                }

                // Filter indicators by area type
                indicators = indicators.Where(i => i.AreaTypeId == profile.SelectedAreaType);

                model.TotalRecordCount = indicators.Count();

                var userPermissions = GetUserGroupPermissions();

                model.UserHasAssignedPermissions = userPermissions.Any();

                model.UserGroupPermissions = userPermissions
                    .FirstOrDefault(x => x.ProfileId == _reader.GetProfileDetails(model.UrlKey).Id);

                model.IndicatorNamesGrid = indicators
                    .Skip((model.CurrentPageIndex - 1) * model.PageSize)
                    .Take(model.PageSize);
            }

            return View("ProfilesAndIndicators", model);
        }

        private static void AssignAreaTypeData(IndicatorGridModel model, Profile profile)
        {
            var builder = new AreaTypeSelectListBuilder(profile.AreaTypes, profile.SelectedAreaType);
            model.SelectedAreaTypeId = builder.SelectedAreaTypeId;
            model.AreaTypeList = builder.SelectListItems;
        }

        private  Profile GetProfile(string urlKey, int selectedDomainNumber, int areaType)
        {
            return new ProfileBuilder(_profileRepository).Build(urlKey, selectedDomainNumber, areaType);
        }

        private IList<GroupingMetadata> ReloadGridDomains(string selectedProfile, ProfileRepository profileRepository)
        {
            var model = new ProfileMembers
                {
                    UrlKey = selectedProfile,
                    Profile = CommonUtilities.GetProfile(selectedProfile, 1, -1, profileRepository)
                };

            return model.Profile.GroupingMetadatas;
        }

        public int GetSelectedGroupIdUsingProfileKeyDomainAndAreaTypeId(string selectedProfile, int domainNumber, int areaTypeId)
        {
            var model = new ProfileMembers
            {
                UrlKey = selectedProfile,
                Profile = CommonUtilities.GetProfile(selectedProfile, domainNumber, areaTypeId, _profileRepository)
            };

            IList<GroupingMetadata> groupingMetadatas = model.Profile.GroupingMetadatas;

            return groupingMetadatas.FirstOrDefault(x => x.Sequence == domainNumber).GroupId;
        }

        public ActionResult ReorderIndicators(int selectedDomain, string urlKey, int selectedGroupId, int selectedAreaType)
        {
            var model = new ProfileMembers
            {
                UrlKey = urlKey,
                Profile = GetProfile(urlKey, selectedDomain, selectedAreaType)
            };

            if (Request.IsAjaxRequest())
            {
                return PartialView("_ReorderIndicators", model);
            }

            return View("ProfilesAndIndicators");
        }

        /// <summary>
        /// Remove indicators from a profile
        /// </summary>
        public ActionResult DeleteIndicators(IEnumerable<string> jdata, string selectedProfileUrlkey,
            string selectedProfileName, int selectedDomainId, string selectedDomainName, int selectedAreaTypeId)
        {
            var model = new DeleteIndicatorsModel
            {
                UrlKey = selectedProfileUrlkey,
                Profile = GetProfile(selectedProfileUrlkey, selectedDomainId, selectedAreaTypeId),
                DomainName = selectedDomainName,
                DomainId = selectedDomainId,
                ProfileName = selectedProfileName,
                SelectedAreaTypeId = selectedAreaTypeId,
                IndicatorsThatCantBeRemoved = null,
            };

            var userPermissions = CommonUtilities.GetUserGroupPermissionsByUserId(_reader.GetUserByUserName(_userName).Id);

            model.UserGroupPermissions = userPermissions
                .FirstOrDefault(x => x.ProfileId == _reader.GetProfileDetails(model.UrlKey).Id);

            var indicatorSpecifiers = IndicatorSpecifierParser.Parse(jdata.ToArray());
            var indicatorList = GetSpecifiedIndicatorNames(indicatorSpecifiers, model.Profile.IndicatorNames);

            var cannotRemove = new List<GroupingPlusName>();
            var canRemove = new List<GroupingPlusName>();
            foreach (var indicator in indicatorList)
            {
                model.IndicatorMetadata = new IndicatorMetadata { OwnerProfileId = _reader.GetIndicatorMetadata(indicator.IndicatorId).OwnerProfileId };

                //check to see if the indicator is being used in more than one domain
                var indicatorGrouping = _reader.DoesIndicatorExistInMoreThanOneGroup(indicator.IndicatorId, indicator.AgeId, indicator.SexId);

                if (indicatorGrouping.GroupBy(x => x.GroupId).Count() > 1)
                {
                    if (model.DoesProfileOwnIndicator())
                    {
                        //If the profile owns the indicator, check to see if this is the last occurance within this profile that is being deleted
                        var groupIds = _reader.GetGroupingIds(model.Profile.Id);
                        var indicatorGroups = _reader.GetGroupingByIndicatorId(new List<int> { indicator.IndicatorId });

                        var uniqueIndicatorGroups = new List<Grouping>();
                        foreach (Grouping @group in indicatorGroups.Where(group => groupIds.Contains(@group.GroupId)).Where(@group => uniqueIndicatorGroups.All(x => x.AreaTypeId != @group.AreaTypeId)))
                        {
                            uniqueIndicatorGroups.Add(@group);
                        }

                        var lastIndicatorOccuranceInProfile = uniqueIndicatorGroups.Count == 1;

                        if (!lastIndicatorOccuranceInProfile)
                        {
                            canRemove.Add(indicator);
                        }
                        else
                        {
                            cannotRemove.Add(indicator);
                        }
                    }
                    else
                    {
                        //Indicator Can Be Deleted because it is being used elsewhere and we ARE NOT trying to delete the owner 
                        canRemove.Add(indicator);
                    }
                }
                else
                {
                    //Indicator Can Be Deleted because it is NOT being used elsewhere
                    canRemove.Add(indicator);
                }
            }

            model.IndicatorsThatCantBeRemoved = cannotRemove;
            model.IndicatorsToDelete = canRemove;

            if (Request.IsAjaxRequest())
            {
                return PartialView("_DeleteSelectedIndicators", model);
            }

            return View("ProfilesAndIndicators");
        }

        private static List<GroupingPlusName> GetSpecifiedIndicatorNames(List<IndicatorSpecifier> indicatorSpecifiers, IList<GroupingPlusName> allIndicators)
        {
            List<GroupingPlusName> indicatorList = new List<GroupingPlusName>();
            foreach (var indicatorSpecifier in indicatorSpecifiers)
            {
                var indicatorId = indicatorSpecifier.IndicatorId;
                var sexId = indicatorSpecifier.SexId;
                var ageId = indicatorSpecifier.AgeId;

                var selectedIndicator =
                    allIndicators
                        .Where(x => x.IndicatorId == indicatorId)
                        .Where(x => x.SexId == sexId)
                        .Where(x => x.AgeId == ageId)
                        .ToList()
                        .FirstOrDefault();

                indicatorList.Add(selectedIndicator);
            }
            return indicatorList;
        }

        public ActionResult GetAudit(IEnumerable<string> jdata)
        {
            string[] selectedIndicators = jdata.ToArray();
            List<int> indicatorIds = selectedIndicators.Select(x => Int32.Parse(x.Split('~')[0])).Distinct().ToList();

            var auditLog = CommonUtilities.GetIndicatorAudit(indicatorIds);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_IndicatorAudit", auditLog);
            }

            return View("ProfilesAndIndicators");
        }

        public ActionResult MoveIndicators(IEnumerable<string> jdata, string selectedProfileUrlkey, string selectedProfileName,
            int selectedDomainId, string selectedDomainName, int selectedAreaTypeId)
        {
            var model = new MoveIndicatorsModel
                {
                    UrlKey = selectedProfileUrlkey,
                    Profile = GetProfile(selectedProfileUrlkey, selectedDomainId, selectedAreaTypeId),
                    DomainName = selectedDomainName,
                    DomainId = selectedDomainId,
                    ProfileName = selectedProfileName,
                    AreaTypeId = selectedAreaTypeId,
                    IndicatorsThatCantBeTransferred = null,
                };

            var listOfProfiles = GetOrderedListOfProfilesForCurrentUser(model);

            var domains = new ProfileMembers();
            var defaultProfile = listOfProfiles.FirstOrDefault(x => x.Selected) ?? listOfProfiles.FirstOrDefault();
            defaultProfile.Selected = true;

            model.ListOfDomains = CommonUtilities.GetOrderedListOfDomainsWithGroupId(domains, defaultProfile,  _profileRepository);

            var userPermissions = CommonUtilities.GetUserGroupPermissionsByUserId(_reader.GetUserByUserName(_userName).Id);
            model.UserGroupPermissions = userPermissions.FirstOrDefault(x => x.ProfileId == _reader.GetProfileDetails(model.UrlKey).Id);

            var indicatorSpecifiers = IndicatorSpecifierParser.Parse(jdata.ToArray());
            model.IndicatorsToTransfer = GetSpecifiedIndicatorNames(indicatorSpecifiers,
                model.Profile.IndicatorNames);

            ViewBag.SexList = _lookUpsRepository.GetSexes();

            ViewBag.AgeList = _lookUpsRepository.GetAges();
         

            if (Request.IsAjaxRequest())
            {
                return PartialView("_MoveSelectedIndicators", model);
            }

            return View("ProfilesAndIndicators");
        }

        private IEnumerable<SelectListItem> GetOrderedListOfProfilesForCurrentUser(BaseDataModel model)
        {
            var listOfProfiles = CommonUtilities.GetOrderedListOfProfilesForCurrentUser(model);
            ViewBag.listOfProfiles = listOfProfiles;
            return listOfProfiles;
        }

        public ActionResult CopyIndicators(IEnumerable<string> jdata, string selectedProfileUrlkey, string selectedProfileName, int selectedDomainId, string selectedDomainName, int selectedAreaTypeId)
        {
            var model = new CopyIndicatorsModel
            {
                UrlKey = selectedProfileUrlkey,
                Profile = GetProfile(selectedProfileUrlkey, selectedDomainId, selectedAreaTypeId),
                DomainName = selectedDomainName,
                DomainId = selectedDomainId,
                ProfileName = selectedProfileName,
                AreaTypeId = selectedAreaTypeId,
                IndicatorsThatCantBeTransferred = null,
            };

            var listOfProfiles = GetOrderedListOfProfilesForCurrentUser(model);

            var domains = new ProfileMembers();
            var defaultProfile = listOfProfiles.FirstOrDefault(x => x.Selected) ?? listOfProfiles.FirstOrDefault();
            defaultProfile.Selected = true;

            model.ListOfDomains = CommonUtilities.GetOrderedListOfDomainsWithGroupId(domains, defaultProfile, _profileRepository);
            model.GroupId = Convert.ToInt32(model.ListOfDomains.FirstOrDefault(x => x.Selected).Value);

            var userPermissions = CommonUtilities.GetUserGroupPermissionsByUserId(_reader.GetUserByUserName(_userName).Id);
            model.UserGroupPermissions = userPermissions.FirstOrDefault(x => x.ProfileId == _reader.GetProfileDetails(model.UrlKey).Id);

            var indicatorSpecifiers = IndicatorSpecifierParser.Parse(jdata.ToArray());
            model.IndicatorsToTransfer = GetSpecifiedIndicatorNames(indicatorSpecifiers,
                model.Profile.IndicatorNames);

            ViewBag.SexList = _lookUpsRepository.GetSexes();

            ViewBag.AgeList = _lookUpsRepository.GetAges();

            if (Request.IsAjaxRequest())
            {
                return PartialView("_CopySelectedIndicators", model);
            }

            return View("ProfilesAndIndicators");
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
        public ActionResult DeleteDomain(ProfileMembers pm, int domainDeleteId)
        {
            var profileId = -1;

            if (ModelState.IsValid)
            {
                List<int> groupIds = new List<int>();

                var profile = pm.Profile;
                profileId = profile.Id;
                foreach (var domain in profile.GroupingMetadatas)
                {
                    if (domain.GroupId != domainDeleteId)
                    {
                        groupIds.Add(domain.GroupId);
                    }
                }

                _writer.DeleteGroupingMetadata(domainDeleteId);
                _writer.ReorderDomainSequence(groupIds);
            }

            return Index(profileId);
        }

        [HttpPost]
        public ActionResult SaveGridDomains(ProfileMembers pm, string newDomain)
        {
            if (ModelState.IsValid)
            {
                string allGroupIds = null;
                // Update the posted-back data
                foreach (var domain in pm.Profile.GroupingMetadatas)
                {
                    allGroupIds += domain.GroupId + ",";
                    _writer.UpdateGroupingMetadata(domain);
                }

                // Insert any newly added domains
                if (!String.IsNullOrEmpty(newDomain))
                {
                    var nextSequenceNumber = pm.Profile.GroupingMetadatas.Last().Sequence + 1;
                    _writer.NewGroupingMetadata(newDomain, nextSequenceNumber, pm.Profile.Id);
                }
            }

            if (Request.UrlReferrer != null)
            {
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            }

            return View("ProfilesAndIndicators");
        }

        [HttpPost]
        public ActionResult SaveReorderedIndicators(FormCollection fc, int? currentDomainId, string indicatorOrder)
        {
            if (currentDomainId.HasValue)
            {
                var indicatorSpecifierStrings = indicatorOrder.Split('¬').ToList();
                indicatorSpecifierStrings.RemoveAt(indicatorSpecifierStrings.Count - 1);
                var sequenceNumber = 1;

                var areaTypeId = Convert.ToInt32(fc["AreaType"]);

                var indicatorSpecifiers = IndicatorSpecifierParser.Parse(indicatorSpecifierStrings);
                foreach (var indicatorSpecifier in indicatorSpecifiers)
                {
                    var groupings = _reader.GetGroupingsByGroupIdAreaTypeIdIndicatorIdAndSexId(currentDomainId.Value,
                        areaTypeId, indicatorSpecifier.IndicatorId, indicatorSpecifier.SexId);

                    foreach (var grouping in groupings)
                    {
                        grouping.Sequence = sequenceNumber;
                    }

                    _writer.UpdateGroupingList(groupings);

                    sequenceNumber++;
                }
            }

            if (Request.UrlReferrer != null)
            {
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            }

            return View("ProfilesAndIndicators");
        }

        public ActionResult ConfirmCopyIndicators(CopyIndicatorsModel cim, List<int> selectedDomainId,
            int selectedAreaTypeId, string indicatorTransferDetails, bool copyMetadataOption, string targetProfile)
        {
            var groupId = GetSelectedGroupIdUsingProfileKeyDomainAndAreaTypeId(cim.UrlKey, cim.DomainId, cim.AreaTypeId);
            var modelCount = 0;

            var indicatorSpecifierStrings = indicatorTransferDetails.Split(',').ToList();
            var indicatorSpecifiers = IndicatorSpecifierParser.Parse(indicatorSpecifierStrings);

            
            string targetProfileId = string.Empty;
            int targetDomain=0, targetAreaTypeId=0;
            if (copyMetadataOption)
            {
                var targetProfileData = targetProfile.Split('~');
                targetProfileId = targetProfileData[0];
                targetDomain = Convert.ToInt32(targetProfileData[1]);
                targetAreaTypeId = Convert.ToInt32(targetProfileData[2]);
            }

            foreach (var indicatorSpecifier in indicatorSpecifiers)
            {
                var indicatorId = indicatorSpecifier.IndicatorId;
                var sexId = indicatorSpecifier.SexId;
                var ageId = indicatorSpecifier.AgeId;

                // Don't copy if identical indicator (including age and sex Id) already exists in destination.
                if (!_profileRepository.IndicatorGroupingsExist(indicatorId, Convert.ToInt32(selectedDomainId[0]),
                    selectedAreaTypeId, Convert.ToInt32(ageId), Convert.ToInt32(sexId)))
                {
                    var currentIndicator = cim.IndicatorsToTransfer[modelCount];

                    _profileRepository.CopyIndicatorToDomain(indicatorId, groupId, cim.AreaTypeId,
                        currentIndicator.SexId, currentIndicator.AgeId,
                        Convert.ToInt32(selectedDomainId[0]), selectedAreaTypeId, sexId,
                        ageId);

                    // Copy indicator meta data
                    if (copyMetadataOption)
                    {

                        var targetProfileDetails = CommonUtilities.GetProfile(targetProfileId, targetDomain, targetAreaTypeId, _profileRepository);
                        var indicatorMetadataTextValue = _reader.GetMetadataTextValueForAnIndicatorById(indicatorId, cim.Profile.Id);
                        if (indicatorMetadataTextValue != null)
                        {
                            var indicatorMetadataTextValuesCopy = new IndicatorMetadataTextValue
                            {
                                IndicatorId = indicatorId,
                                ProfileId = targetProfileDetails.Id,
                                Name = indicatorMetadataTextValue.Name,
                                NameLong = indicatorMetadataTextValue.NameLong,
                                Definition = indicatorMetadataTextValue.Definition,
                                Rationale = indicatorMetadataTextValue.Rationale,
                                Policy = indicatorMetadataTextValue.Policy,
                                DataSource = indicatorMetadataTextValue.DataSource,
                                Producer = indicatorMetadataTextValue.Producer,
                                IndMethod = indicatorMetadataTextValue.IndMethod,
                                StandardPop = indicatorMetadataTextValue.StandardPop,
                                CIMethod = indicatorMetadataTextValue.CIMethod,
                                CountSource = indicatorMetadataTextValue.CountSource,
                                CountDefinition = indicatorMetadataTextValue.CountDefinition,
                                DenomSource = indicatorMetadataTextValue.DenomSource,
                                DenomDefinition = indicatorMetadataTextValue.DenomDefinition,
                                DiscControl = indicatorMetadataTextValue.DiscControl,
                                Caveats = indicatorMetadataTextValue.Caveats,
                                Copyright = indicatorMetadataTextValue.Copyright,
                                Reuse = indicatorMetadataTextValue.Reuse,
                                Links = indicatorMetadataTextValue.Links,
                                RefNum = indicatorMetadataTextValue.RefNum,
                                Notes = indicatorMetadataTextValue.Notes,
                                Frequency = indicatorMetadataTextValue.Frequency,
                                Rounding = indicatorMetadataTextValue.Rounding,
                                DataQuality = indicatorMetadataTextValue.DataQuality
                            };

                            _writer.NewIndicatorMetadataTextValue(indicatorMetadataTextValuesCopy);
                        }
                    }

                    _profileRepository.LogAuditChange("Indicator " + indicatorId + " copied from [" +
                        cim.DomainName + " (Area: " + cim.AreaTypeId + ", SexId:" + currentIndicator.SexId +
                        ", AgeId:" + currentIndicator.AgeId + " )] Into " +
                        "[" + _reader.GetGroupingMetadataList(selectedDomainId)[0].GroupName +
                        " (Area: " + selectedAreaTypeId + ", SexId:" + sexId + ", AgeId:" + ageId + " )]",
                        indicatorId, null, _userName, DateTime.Now, CommonUtilities.AuditType.Copy.ToString());
                }
                modelCount++;
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        public ActionResult ConfirmMoveIndicators(MoveIndicatorsModel mim, string selectedProfileId,
            List<int> selectedDomainId, int selectedAreaTypeId, string indicatorTransferDetails)
        {
            var groupId = GetSelectedGroupIdUsingProfileKeyDomainAndAreaTypeId(mim.UrlKey, mim.DomainId, mim.AreaTypeId);
            var modelCount = 0;

            //There are indicators to delete (move to the archive domain)
            var indicatorSpecifierStrings = indicatorTransferDetails.Split(',').ToList();
            var indicatorSpecifiers = IndicatorSpecifierParser.Parse(indicatorSpecifierStrings);

            foreach (var indicatorSpecifier in indicatorSpecifiers)
            {
                var indicatorId = indicatorSpecifier.IndicatorId;
                var sexId = indicatorSpecifier.SexId;
                var ageId = indicatorSpecifier.AgeId;

                //Don't move if identical indicator already exists in destination.
                if (!_profileRepository.IndicatorGroupingsExist(indicatorId,
                    Convert.ToInt32(selectedDomainId[0]), mim.AreaTypeId, ageId,
                    sexId))
                {
                    var currentIndicator = mim.IndicatorsToTransfer[modelCount];

                    _profileRepository.MoveIndicatorToDomain(indicatorId, groupId, mim.AreaTypeId,
                        currentIndicator.SexId, currentIndicator.AgeId,
                        selectedDomainId[0], selectedAreaTypeId, sexId, ageId);

                    _profileRepository.LogAuditChange("Indicator " + indicatorId + " moved from [" + mim.DomainName +
                        " (Area: " + mim.AreaTypeId + ", SexId:" + currentIndicator.SexId +
                        ", AgeId:" + currentIndicator.AgeId + " )] Into [" +
                        _reader.GetGroupingMetadataList(selectedDomainId)[0].GroupName +
                        " (Area: " + selectedAreaTypeId + ", SexId:" + sexId + ", AgeId:" + ageId + " )]",
                        indicatorId, null, _userName, DateTime.Now, CommonUtilities.AuditType.Move.ToString());

                    var indicatorMetadata = _reader.GetIndicatorMetadata(indicatorId);
                    if (indicatorMetadata.OwnerProfileId == mim.Profile.Id)
                    {
                        // Indicator is being moved out of owner profile so owner must change
                        var newOwnerProfileId = _reader.GetProfileDetails(selectedProfileId).Id;
                        new IndicatorOwnerChanger(_reader, _profileRepository)
                            .AssignIndicatorToProfile(indicatorId, newOwnerProfileId);

                    }
                }
                modelCount++;
            }

            _profileRepository.Dispose();

            _writer.ReorderIndicatorSequence(groupId, mim.AreaTypeId);

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        public ActionResult ConfirmDeleteIndicators(DeleteIndicatorsModel indicatorsModel, string indicatorDeleteDetails)
        {
            if (indicatorDeleteDetails != string.Empty)
            {
                var areaTypeId = indicatorsModel.SelectedAreaTypeId;
                var groupId = GetSelectedGroupIdUsingProfileKeyDomainAndAreaTypeId(
                    indicatorsModel.UrlKey, indicatorsModel.DomainId, areaTypeId);

                var indicatorSpecifierStrings = indicatorDeleteDetails.Split(',').ToList();
                var indicatorSpecifiers = IndicatorSpecifierParser.Parse(indicatorSpecifierStrings);

                foreach (var indicatorSpecifier in indicatorSpecifiers)
                {
                    var indicatorId = indicatorSpecifier.IndicatorId;
                    var sexId = indicatorSpecifier.SexId;
                    var ageId = indicatorSpecifier.AgeId;
                    var profileId = indicatorsModel.Profile.Id;

                    var indicatorMetadata = _reader.GetIndicatorMetadata(indicatorId);

                    if (indicatorMetadata.OwnerProfileId == profileId)
                    {
                        // Indicator owned by profile

                        var groupings = GetGroupingsForIndicatorInProfile(profileId, indicatorId);

                        var distinctGroupingsForProfile = 
                            GetDistinctGroupingsByGroupIdAndAreaTypeId(groupings);

                        if (distinctGroupingsForProfile.Count > 1)
                        {
                            //This isn't the last occurance of this indicator in this profile so it doesn't need to be archived and can simply be deleted from the grouping table
                            DeleteFromGrouping(groupId, indicatorId, areaTypeId, sexId, ageId);
                        }
                        else
                        {
                            //Indicator is owned by the profile so archive it
                            _profileRepository.ArchiveIndicatorFromGrouping(groupId, indicatorId,
                                areaTypeId, sexId, ageId);

                            //Set the indicator ownership to the archive profile
                            new IndicatorOwnerChanger(_reader, _profileRepository)
                            .AssignIndicatorToProfile(indicatorId, ProfileIds.ArchivedIndicators);

                            _profileRepository.LogAuditChange("Indicator " + indicatorId +
                                " (Area: " + areaTypeId + ", SexId:" + sexId +
                                ", AgeId:" + ageId + " )  has been archived.",
                                indicatorId, null, _userName, DateTime.Now,
                                CommonUtilities.AuditType.Delete.ToString());
                        }
                    }
                    else
                    {
                        // Indicator not owned by profile
                        DeleteFromGrouping(groupId, indicatorId, areaTypeId, sexId, ageId);
                    }
                }

                _profileRepository.Dispose();

                _writer.ReorderIndicatorSequence(groupId, areaTypeId);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [HttpGet]
        [Route("BrowseData/full-page/{indicatorId}")]
        public ActionResult BrowseIndicatorDataFullPage(int indicatorId)
        {
            IList<IndicatorMetadataTextProperty> properties = _reader.GetIndicatorMetadataTextProperties();
            var values = _reader.GetIndicatorTextValues(indicatorId, properties, null).ToList();
            return View(new BrowseIndicatorDataFullPageViewModel
            {
                IndicatorId = indicatorId,
                IndicatorName = values.First(x => x.IndicatorMetadataTextProperty.PropertyId == IndicatorTextMetadataPropertyIds.Name).ValueGeneric
            });
        }

        [HttpGet]
        [AjaxOnly]
        [Route("BrowseData/{indicatorId}")]
        public ActionResult BrowseIndicatorData(int indicatorId)
        {
            var rowsCount = 0;
            var model = new BrowseDataViewModel()
            {
                IndicatorId = indicatorId,
                Results = new CoreDataSetViewModel()
                {
                    DataSet = _coreDataRepository.GetCoreDataSet(indicatorId,out rowsCount),
                    RowsFound = rowsCount,
                    CanDeleteDataSet = _coreDataRepository.CanDeleteDataSet(indicatorId, UserName)
                }
            };
            
            PopulateFilterDataDropDownLists(model);

            return PartialView("BrowseIndicatorData", model);
        }

        /// <summary>
        /// Accessor that ensures username is defined.
        /// </summary>
        private string UserName
        {
            get
            {
                if (_userName == null)
                {
                    _userName = UserDetails.CurrentUser().Name;
                }
                return _userName;
            }
        }

        [HttpPost]
        [AjaxOnly]
        [Route("BrowseData")]
        public ActionResult BrowseIndicatorData(BrowseDataViewModel model)
        {
            var filters = GetFilters(model);
            int rowsCount = 0;
            var results = new CoreDataSetViewModel()
            {
                DataSet = _coreDataRepository.GetCoreDataSet(model.IndicatorId, filters, out rowsCount),
                RowsFound = rowsCount,
                CanDeleteDataSet = _coreDataRepository.CanDeleteDataSet(model.IndicatorId, _userName)
            };
            return PartialView("_CoreData", results);
        }

        [HttpPost]
        [AjaxOnly]
        [Route("DeleteData")]
        public ActionResult DeleteIndicatorData(BrowseDataViewModel model)
        {
           
            var filters = GetFilters(model);

            var coreDataSetViewModel = new CoreDataSetViewModel();

            try
            {
               var rowsDeleted = _coreDataRepository.DeleteCoreDataSet(model.IndicatorId, filters, _userName);
                coreDataSetViewModel.DeleteStatus = true;
                coreDataSetViewModel.Message = string.Format("{0} rows have been deleted",
                    rowsDeleted);
            }
            catch (Exception exception)
            {
                ExceptionLogger.LogException(exception, "Global.asax");
                coreDataSetViewModel.DeleteStatus = false;
                coreDataSetViewModel.Message = "Error occurred while deleting data - " + exception.Message;
            }
            
           return PartialView("_CoreData", coreDataSetViewModel);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (_profileRepository == null) _profileRepository = new ProfileRepository();
            if (_coreDataRepository == null) _coreDataRepository = new CoreDataRepository();
            if (_lookUpsRepository == null) _lookUpsRepository = new LookUpsRepository();

            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _profileRepository.Dispose();
            _lookUpsRepository.Dispose();
            _coreDataRepository.Dispose();

            base.OnActionExecuted(filterContext);
        }

        private Dictionary<string, int> GetFilters(BrowseDataViewModel model)
        {
            var filters = new Dictionary<string, int>();

            if (model.AgeId > 0) filters.Add(CoreDataFilters.AgeId, model.AgeId);
            if (model.SexId > 0) filters.Add(CoreDataFilters.SexId, model.SexId);
            if (model.AreaTypeId > 0) filters.Add(CoreDataFilters.AreaTypeId, model.AreaTypeId);
            if (model.CategoryTypeId > 0) filters.Add(CoreDataFilters.CategoryTypeId, model.CategoryTypeId);
            if (model.Month > 0) filters.Add(CoreDataFilters.Month, model.Month);
            if (model.Year > 0) filters.Add(CoreDataFilters.Year, model.Year);
            if (model.Quarter > 0) filters.Add(CoreDataFilters.Quarter, model.Quarter);
            if (model.YearRange > 0) filters.Add(CoreDataFilters.YearRange, model.YearRange);

            return filters;
        }
        
        private void PopulateFilterDataDropDownLists(BrowseDataViewModel model)
        {
            if (model.IndicatorId == 0) return;
            
            ViewBag.AreaTypeId = new SelectList(_coreDataRepository.GetAreaTypes(model.IndicatorId), "Id", "Name").AddAnyOption();

            ViewBag.SexId = new SelectList(_coreDataRepository.GetSexes(model.IndicatorId), "SexID", "Description").AddAnyOption();

            ViewBag.AgeId = new SelectList(_coreDataRepository.GetAges(model.IndicatorId), "AgeID", "Description").AddAnyOption();

            // Category types
            var categoryTypes = _coreDataRepository.GetCategoryTypes(model.IndicatorId);
            var undefinedCategoryType = categoryTypes.FirstOrDefault(x => x.Id == -1);
            if (undefinedCategoryType != null)
            {
                undefinedCategoryType.Name = "Any";
            }
            ViewBag.CategoryTypeId = new SelectList(categoryTypes, "Id", "Name");

            ViewBag.YearRange = _coreDataRepository.GetYearRanges(model.IndicatorId).ConvertToSelectListWithAnyOption(); ;

            ViewBag.Year = _coreDataRepository.GetYears(model.IndicatorId).ConvertToSelectListWithAnyOption(); 

            ViewBag.Month = _coreDataRepository.GetMonths(model.IndicatorId).ConvertToSelectListWithAnyOption(); ;

            ViewBag.Quarter = _coreDataRepository.GetQuarters(model.IndicatorId).ConvertToSelectListWithAnyOption(); ;
        }

        private static List<Grouping> GetDistinctGroupingsByGroupIdAndAreaTypeId(IEnumerable<Grouping> groupings)
        {
            var distinctGroupingsForProfile = new List<Grouping>();
            foreach (Grouping grouping in groupings)
            {
                var isAlreadyFound = false;
                foreach (var distinctGrouping in distinctGroupingsForProfile)
                {
                    if (grouping.GroupId == distinctGrouping.GroupId &&
                        grouping.AreaTypeId == distinctGrouping.AreaTypeId)
                    {
                        isAlreadyFound = true;
                        break;
                    }
                }

                if (isAlreadyFound == false)
                {
                    distinctGroupingsForProfile.Add(grouping);
                }
            }
            return distinctGroupingsForProfile;
        }

        private IEnumerable<Grouping> GetGroupingsForIndicatorInProfile(int profileId, int indicatorId)
        {
            var groupingsForAllProfiles = _reader
                .GetGroupingByIndicatorId(new List<int> {indicatorId});

            var profileGroupIds = _reader.GetGroupingIds(profileId);

            var groupingsForCurrentProfile = groupingsForAllProfiles
                .Where(x => profileGroupIds.Contains(x.GroupId));

            return groupingsForCurrentProfile;
        }

        private void DeleteFromGrouping(int groupId, int indicatorId, int areaTypeId, int sexId, int ageId)
        {
            // Profile doesn't own the indicator so actually deleted it from the grouping table 
            _profileRepository.DeleteIndicatorFromGrouping(groupId, indicatorId, areaTypeId, sexId, ageId);

            // Also delete from the IndicatorMetaDataTextValue table (where it has an overridden groupId)
            _profileRepository.DeleteOverriddenMetaDataTextValues(indicatorId, groupId);

            _profileRepository.LogAuditChange("Indicator " + indicatorId + " (Area: " + areaTypeId +
                                       ", SexId:" + sexId + ", AgeId:" + ageId + " )  has been deleted.",
                indicatorId, null, _userName, DateTime.Now, CommonUtilities.AuditType.Delete.ToString());
        }
    }
}
