using Fpm.MainUI.Helpers;
using Fpm.MainUI.Models;
using Fpm.MainUI.ViewModels.ProfilesAndIndicators;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.DataAudit;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Entities.User;
using Fpm.ProfileData.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;

namespace Fpm.MainUI.Controllers
{
    public class ProfilesAndIndicatorsController : Controller
    {
        public const string IndexView = "ListIndicatorsInProfile";

        private readonly ProfilesReader _reader = ReaderFactory.GetProfilesReader();
        private readonly ProfilesWriter _writer = ReaderFactory.GetProfilesWriter();

        private ProfileRepository _profileRepository;
        private LookUpsRepository _lookUpsRepository;
        private CoreDataRepository _coreDataRepository;

        private string _userName;

        public ProfilesAndIndicatorsController()
        { }

        public ProfilesAndIndicatorsController(ProfileRepository profileRepository,
            LookUpsRepository lookUpsRepository, CoreDataRepository coreDataRepository)
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

        [Route("")]
        [Route("profile/indicators")]
        public ActionResult ListIndicatorsInProfile(int profileId = ProfileIds.Undefined)
        {
            var model = new IndicatorGridModel
            {
                SortBy = "Sequence",
                SortAscending = true,
                CurrentPageIndex = 1,
                PageSize = 200
            };

            GetProfiles(model);

            // Select profile
            Profile profile = null;
            if (profileId != ProfileIds.Undefined)
            {
                var urlKey = _reader.GetProfileUrlKeyFromId(profileId);
                profile = GetProfile(urlKey);
                if (profile != null)
                {
                    model.ProfileKey = urlKey;
                }
            }
            GetDomains(model, null);
            if (profile == null)
            {
                var domain = model.DomainList.FirstOrDefault();
                if (domain != null)
                {
                    profile = GetProfile(model.ProfileKey, Convert.ToInt32(domain.Value));
                }
            }

            // Init profile
            if (profile != null)
            {
                ViewBag.ProfileId = profile.Id;
                model.Profile = profile;
                model.UrlKey = model.ProfileKey;


                // Select area type
                var areaType = (profile.AreaTypes != null) ? profile.AreaTypes.FirstOrDefault() : null;
                profile.SelectedAreaType = (areaType != null) ? areaType.Id : AreaTypeIds.Undefined;

                AssignContactDetails(profile, model);

                AssignAreaTypeData(model, profile);

                AssignUserPermissionsToIndicatorGridModel(model);

                // Indicators
                IQueryable<GroupingPlusName> indicators = profile.IndicatorNames.AsQueryable();
                indicators = indicators.Where(i => i.AreaTypeId == profile.SelectedAreaType);

                model.TotalRecordCount = indicators.Count();

                model.IndicatorNamesGrid = indicators
                    .Distinct()
                    .OrderBy(model.SortExpression)
                    .Skip((model.CurrentPageIndex - 1) * model.PageSize)
                    .Take(model.PageSize);
            }

            return View(IndexView, model);
        }

        [Route("profile/indicators/specific")]
        public ActionResult ListIndicatorsInProfileSpecific(int? domainSequence, int? indicatorId, string resetArea,
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
            {
                model.ProfileKey = string.IsNullOrEmpty(profileKey) ? firstOrDefault.Value : profileKey;
            }

            GetDomains(model, domainSequence);
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
                var areaTypeId = resetArea == "True"
                    ? AreaTypeIds.Undefined
                    : selectedAreaTypeId;
                profile = GetProfile(model.ProfileKey, domainSequence.Value, areaTypeId);
            }

            if (profile != null)
            {
                ViewBag.ProfileId = profile.Id;

                AssignAreaTypeData(model, profile);

                model.Profile = profile;
                model.UrlKey = model.ProfileKey;

                AssignContactDetails(profile, model);

                AssignUserPermissionsToIndicatorGridModel(model);

                // Filter indicators for any search terms
                if (profile.IndicatorNames != null)
                {
                    IQueryable<GroupingPlusName> indicators = profile.IndicatorNames.AsQueryable();
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

                    model.IndicatorNamesGrid = indicators
                        .Skip((model.CurrentPageIndex - 1) * model.PageSize)
                        .Take(model.PageSize);
                }
            }

            return View(IndexView, model);
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

            return View(IndexView);
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

            model.UserGroupPermissions = GetProfileUserPermissions(userPermissions, model.UrlKey);

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
                    //                    if (model.DoesProfileOwnIndicator())
                    //                    {
                    //                        //If the profile owns the indicator, check to see if this is the last occurance within this profile that is being deleted
                    //                        var groupIds = _reader.GetGroupingIds(model.Profile.Id);
                    //                        var indicatorGroups = _reader.GetGroupingByIndicatorId(new List<int> { indicator.IndicatorId });
                    //
                    //                        var uniqueIndicatorGroups = new List<Grouping>();
                    //                        foreach (Grouping @group in indicatorGroups.Where(group => groupIds.Contains(@group.GroupId)).Where(@group => uniqueIndicatorGroups.All(x => x.AreaTypeId != @group.AreaTypeId)))
                    //                        {
                    //                            uniqueIndicatorGroups.Add(@group);
                    //                        }
                    //
                    //                        var lastIndicatorOccuranceInProfile = uniqueIndicatorGroups.Count == 1;
                    //
                    //                        if (!lastIndicatorOccuranceInProfile)
                    //                        {
                    //                            canRemove.Add(indicator);
                    //                        }
                    //                        else
                    //                        {
                    //                            cannotRemove.Add(indicator);
                    //                        }
                    //                    }
                    //                    else
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

            return View(IndexView);
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

            return View(IndexView);
        }

        public ActionResult MoveIndicators(IEnumerable<string> jdata, string selectedProfileUrlkey, string selectedProfileName,
            int selectedDomainId, string selectedDomainName, int selectedAreaTypeId)
        {
            var model = new MoveIndicatorsModel
            {
                UrlKey = selectedProfileUrlkey,
                Profile = GetProfile(selectedProfileUrlkey, selectedDomainId, selectedAreaTypeId),
                DomainName = selectedDomainName,
                DomainIndex = selectedDomainId,
                ProfileName = selectedProfileName,
                AreaTypeId = selectedAreaTypeId,
                IndicatorsThatCantBeTransferred = null,
            };

            var listOfProfiles = GetOrderedListOfProfilesForCurrentUser(model);

            var domains = new ProfileMembers();
            var defaultProfile = listOfProfiles.FirstOrDefault(x => x.Selected) ?? listOfProfiles.FirstOrDefault();
            defaultProfile.Selected = true;

            model.ListOfDomains = CommonUtilities.GetOrderedListOfDomainsWithGroupId(domains, defaultProfile, _profileRepository);

            var userPermissions = CommonUtilities.GetUserGroupPermissionsByUserId(_reader.GetUserByUserName(_userName).Id);

            model.UserGroupPermissions = GetProfileUserPermissions(userPermissions, model.UrlKey);

            var indicatorSpecifiers = IndicatorSpecifierParser.Parse(jdata.ToArray());
            model.IndicatorsToTransfer = GetSpecifiedIndicatorNames(indicatorSpecifiers,
                model.Profile.IndicatorNames);

            ViewBag.SexList = _lookUpsRepository.GetSexes();

            ViewBag.AgeList = _lookUpsRepository.GetAges();


            if (Request.IsAjaxRequest())
            {
                return PartialView("_MoveSelectedIndicators", model);
            }

            return View(IndexView);
        }

        public ActionResult CopyIndicators(IEnumerable<string> jdata, string selectedProfileUrlkey,
            string selectedProfileName, int selectedDomainId, string selectedDomainName, int selectedAreaTypeId)
        {
            var model = new CopyIndicatorsModel
            {
                UrlKey = selectedProfileUrlkey,
                Profile = GetProfile(selectedProfileUrlkey, selectedDomainId, selectedAreaTypeId),
                DomainName = selectedDomainName,
                DomainIndex = selectedDomainId,
                ProfileName = selectedProfileName,
                AreaTypeId = selectedAreaTypeId,
                IndicatorsThatCantBeTransferred = null
            };

            // List of profiles
            var listOfProfiles = GetOrderedListOfProfilesForCurrentUser(model);
            var defaultProfile = listOfProfiles.FirstOrDefault(x => x.Selected) ?? listOfProfiles.FirstOrDefault();
            defaultProfile.Selected = true;

            // Selected group ID
            var profile = CommonUtilities.GetProfile(selectedProfileUrlkey, selectedDomainId, selectedAreaTypeId, _profileRepository);
            model.GroupId = profile.GroupingMetadatas.OrderBy(x => x.Sequence).ToList()[selectedDomainId - 1].GroupId;

            // Domain list
            var profileMembers = new ProfileMembers();
            model.ListOfDomains = CommonUtilities.GetOrderedListOfDomainsWithGroupId(profileMembers, defaultProfile,
                _profileRepository, model.GroupId);

            // User permissions
            var userPermissions = CommonUtilities.GetUserGroupPermissionsByUserId(_reader.GetUserByUserName(_userName).Id);
            model.UserGroupPermissions = GetProfileUserPermissions(userPermissions, model.UrlKey);

            var indicatorSpecifiers = IndicatorSpecifierParser.Parse(jdata.ToArray());
            model.IndicatorsToTransfer = GetSpecifiedIndicatorNames(indicatorSpecifiers,
                model.Profile.IndicatorNames);

            ViewBag.SexList = _lookUpsRepository.GetSexes();

            ViewBag.AgeList = _lookUpsRepository.GetAges();

            if (Request.IsAjaxRequest())
            {
                return PartialView("_CopySelectedIndicators", model);
            }

            return View(IndexView);
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
            var profileId = ProfileIds.Undefined;

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

            if (Request.UrlReferrer != null)
            {
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            }

            //return ListIndicatorsInProfile(profileId);
            return View(IndexView);
        }

        [HttpPost]
        public ActionResult SaveGridDomains(ProfileMembers pm, string newDomain)
        {
            if (ModelState.IsValid)
            {
                string allGroupIds = null;
                // Update the posted-back data
                if (pm.Profile.GroupingMetadatas == null)
                {
                    pm.Profile.GroupingMetadatas = new List<GroupingMetadata>();
                }
                foreach (var domain in pm.Profile.GroupingMetadatas)
                {
                    allGroupIds += domain.GroupId + ",";
                    _writer.UpdateGroupingMetadata(domain);
                }

                // Insert any newly added domains
                if (!String.IsNullOrEmpty(newDomain))
                {
                    var nextSequenceNumber = (pm.Profile.GroupingMetadatas.Count > 0)
                        ? pm.Profile.GroupingMetadatas.Last().Sequence + 1
                        : 1;
                    _writer.NewGroupingMetadata(newDomain, nextSequenceNumber, pm.Profile.Id);
                }
            }

            if (Request.UrlReferrer != null)
            {
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            }

            return View(IndexView);
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

            return View(IndexView);
        }

        public ActionResult ConfirmCopyIndicators(CopyIndicatorsModel cim,
            string indicatorTransferDetails, bool copyMetadataOption)
        {
            var modelCount = 0;

            var indicatorSpecifierStrings = indicatorTransferDetails.Split(',').ToList();
            var indicatorSpecifiers = IndicatorSpecifierParser.Parse(indicatorSpecifierStrings);

            var targetProfileUrlKey = cim.TargetProfileUrlKey;
            var targetGroupId = cim.TargetGroupId;
            var targetAreaTypeId = cim.TargetAreaTypeId;

            var srcGroupId = cim.GroupId;
            var srcProfileId = cim.Profile.Id;
            var srcAreaTypeId = cim.AreaTypeId;

            foreach (var indicatorSpecifier in indicatorSpecifiers)
            {
                var indicatorId = indicatorSpecifier.IndicatorId;
                var sexId = indicatorSpecifier.SexId;
                var ageId = indicatorSpecifier.AgeId;

                // Don't copy if identical indicator (including age and sex Id) already exists in destination.
                if (!_profileRepository.IndicatorGroupingsExist(indicatorId, targetGroupId,
                    targetAreaTypeId, ageId, sexId))
                {
                    var currentIndicator = cim.IndicatorsToTransfer[modelCount];

                    _profileRepository.CopyIndicatorToDomain(indicatorId,
                        srcGroupId, srcAreaTypeId, currentIndicator.SexId, currentIndicator.AgeId,
                        targetGroupId, targetAreaTypeId, sexId, ageId);

                    // Copy indicator metadata
                    if (copyMetadataOption)
                    {
                        var targetProfileId = _reader.GetProfileIdFromUrlKey(targetProfileUrlKey);

                        var srcMetadataTextValues = _reader.GetMetadataTextValueForAnIndicatorById(indicatorId, srcProfileId);
                        var targetMetadataTextValues = _reader.GetMetadataTextValueForAnIndicatorById(
                            indicatorId, targetProfileId);

                        if (srcMetadataTextValues != null && targetMetadataTextValues == null)
                        {
                            targetMetadataTextValues = new IndicatorMetadataTextValue();
                            AutoMapper.Mapper.Map(srcMetadataTextValues, targetMetadataTextValues);
                            targetMetadataTextValues.ProfileId = targetProfileId;
                            targetMetadataTextValues.Id = 0;
                            _writer.NewIndicatorMetadataTextValue(targetMetadataTextValues);
                        }
                    }

                    var targetDomainName = _reader.GetGroupingMetadataList(new List<int> { targetGroupId })[0].GroupName;
                    _profileRepository.LogAuditChange("Indicator " + indicatorId + " copied from [" +
                        cim.DomainName + " (Area: " + srcAreaTypeId + ", SexId:" + sexId +
                        ", AgeId:" + ageId + " )] Into " + "[" + targetDomainName +
                        " (Area: " + targetAreaTypeId + ", SexId:" + sexId + ", AgeId:" + ageId + " )]",
                        indicatorId, null, _userName, DateTime.Now, CommonUtilities.AuditType.Copy.ToString());
                }
                modelCount++;
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        public ActionResult ConfirmMoveIndicators(MoveIndicatorsModel mim, string selectedProfileId,
            List<int> selectedDomainId, int selectedAreaTypeId, string indicatorTransferDetails)
        {
            var groupId = GetSelectedGroupIdUsingProfileKeyDomainAndAreaTypeId(mim.UrlKey, mim.DomainIndex, mim.AreaTypeId);
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
            int rowsCount;
            var model = new BrowseDataViewModel()
            {
                IndicatorId = indicatorId,
                Results = new CoreDataSetViewModel()
                {
                    DataSet = _coreDataRepository.GetCoreDataSet(indicatorId, out rowsCount),
                    RowsFound = rowsCount,
                    CanDeleteDataSet = _coreDataRepository.CanDeleteDataSet(indicatorId, UserName)
                },
                DataChange = GetDataChangeAudit(indicatorId)
            };

            PopulateFilterDataDropDownLists(model);
            return PartialView("BrowseIndicatorData", model);
        }

        [HttpPost]
        [AjaxOnly]
        [Route("BrowseData")]
        public ActionResult BrowseIndicatorData(BrowseDataViewModel model)
        {
            var filters = GetFilters(model);
            int rowsCount;
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
            if (_profileRepository == null) _profileRepository = new ProfileRepository(NHibernateSessionFactory.GetSession());
            if (_coreDataRepository == null) _coreDataRepository = new CoreDataRepository(NHibernateSessionFactory.GetSession());
            if (_lookUpsRepository == null) _lookUpsRepository = new LookUpsRepository(NHibernateSessionFactory.GetSession());

            base.OnActionExecuting(filterContext);
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

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _profileRepository.Dispose();
            _lookUpsRepository.Dispose();
            _coreDataRepository.Dispose();

            base.OnActionExecuted(filterContext);
        }

        private Dictionary<string, object> GetFilters(BrowseDataViewModel model)
        {
            var filters = new Dictionary<string, object>();

            if (model.AgeId > UIHelper.ShowAll) filters.Add(CoreDataFilters.AgeId, model.AgeId);
            if (model.SexId > UIHelper.ShowAll) filters.Add(CoreDataFilters.SexId, model.SexId);
            if (model.AreaTypeId > 0) filters.Add(CoreDataFilters.AreaTypeId, model.AreaTypeId);
            if (model.CategoryTypeId > 0) filters.Add(CoreDataFilters.CategoryTypeId, model.CategoryTypeId);
            if (model.Month > 0) filters.Add(CoreDataFilters.Month, model.Month);
            if (model.Year > 0) filters.Add(CoreDataFilters.Year, model.Year);
            if (model.Quarter > 0) filters.Add(CoreDataFilters.Quarter, model.Quarter);
            if (model.YearRange > 0) filters.Add(CoreDataFilters.YearRange, model.YearRange);

            // Add area code
            var areaCode = model.AreaCode;
            if (string.IsNullOrWhiteSpace(areaCode) == false)
            {
                filters.Add(CoreDataFilters.AreaCode, model.AreaCode.Trim());
            }

            return filters;
        }

        private void AssignContactDetails(Profile profile, IndicatorGridModel model)
        {
            var profileContact = _reader.GetUserByUserId(profile.ContactUserId);
            model.ContactUserName = profileContact.DisplayName;
            model.EmailAddress = profileContact.EmailAddress;
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

        private IEnumerable<SelectListItem> GetOrderedListOfProfilesForCurrentUser(BaseDataModel model)
        {
            var listOfProfiles = CommonUtilities.GetOrderedListOfProfilesForCurrentUser(model);
            ViewBag.listOfProfiles = listOfProfiles;
            return listOfProfiles;
        }

        private UserGroupPermissions GetProfileUserPermissions(IEnumerable<UserGroupPermissions> userPermissions, string urlKey)
        {
            return userPermissions.FirstOrDefault(
                x => x.ProfileId == _reader.GetProfileIdFromUrlKey(urlKey));
        }

        private Profile GetProfile(string urlKey, int selectedDomainNumber = 0, int areaType = AreaTypeIds.Undefined)
        {
            return new ProfileBuilder(_profileRepository).Build(urlKey, selectedDomainNumber, areaType);
        }

        private static void AssignAreaTypeData(IndicatorGridModel model, Profile profile)
        {
            var builder = new AreaTypeSelectListBuilder(profile.AreaTypes, profile.SelectedAreaType);
            model.SelectedAreaTypeId = builder.SelectedAreaTypeId;
            model.AreaTypeList = builder.SelectListItems;
        }

        private IList<GroupingMetadata> ReloadGridDomains(string selectedProfile, ProfileRepository profileRepository)
        {
            var model = new ProfileMembers
            {
                UrlKey = selectedProfile,
                Profile = CommonUtilities.GetProfile(selectedProfile, 1, AreaTypeIds.Undefined, profileRepository)
            };

            return model.Profile.GroupingMetadatas;
        }

        private void AssignUserPermissionsToIndicatorGridModel(IndicatorGridModel model)
        {
            var userPermissions = GetUserGroupPermissions();
            model.UserHasAssignedPermissions = userPermissions.Any();
            model.UserGroupPermissions = GetProfileUserPermissions(userPermissions, model.UrlKey);
        }

        private IEnumerable<Grouping> GetGroupingsForIndicatorInProfile(int profileId, int indicatorId)
        {
            var groupingsForAllProfiles = _reader
                .GetGroupingByIndicatorId(new List<int> { indicatorId });

            var profileGroupIds = _reader.GetGroupingIds(profileId);

            var groupingsForCurrentProfile = groupingsForAllProfiles
                .Where(x => profileGroupIds.Contains(x.GroupId));

            return groupingsForCurrentProfile;
        }

        private void DeleteFromGrouping(int groupId, int indicatorId, int areaTypeId, int sexId, int ageId)
        {
            // Profile doesn't own the indicator so actually deleted it from the grouping table 
            _profileRepository.DeleteIndicatorFromGrouping(groupId, indicatorId, areaTypeId, sexId, ageId);

            // Also delete from the IndicatorMetadataTextValue table (where it has an overridden groupId)
            _profileRepository.DeleteOverriddenMetadataTextValues(indicatorId, groupId);

            _profileRepository.LogAuditChange("Indicator " + indicatorId + " (Area: " + areaTypeId +
                                       ", SexId:" + sexId + ", AgeId:" + ageId + " )  has been deleted.",
                indicatorId, null, _userName, DateTime.Now, CommonUtilities.AuditType.Delete.ToString());
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

        private DataChange GetDataChangeAudit(int indicatorId)
        {
            try
            {
                var url = ApplicationConfiguration.CoreWsUrl + "api/data_changes?indicator_id=" + indicatorId;
                string json;
                using (WebClient wc = new WebClient())
                {
                    json = wc.DownloadString(url);
                }

                var dataChange = JsonConvert.DeserializeObject<DataChange>(json);
                return dataChange;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}