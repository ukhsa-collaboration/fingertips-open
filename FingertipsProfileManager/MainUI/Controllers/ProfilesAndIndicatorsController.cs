using Fpm.MainUI.Helpers;
using Fpm.MainUI.Models;
using Fpm.MainUI.ViewModels.ProfilesAndIndicators;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Fpm.MainUI.Controllers
{
    [RoutePrefix("profiles-and-indicators")]
    public class ProfilesAndIndicatorsController : Controller
    {
        public const string IndexView = "ListIndicatorsInProfile";

        private readonly IProfilesReader _reader;
        private readonly IProfilesWriter _writer;

        private readonly IProfileRepository _profileRepository;
        private readonly ILookUpsRepository _lookUpsRepository;
        private readonly ICoreDataRepository _coreDataRepository;
        private readonly IEmailRepository _emailRepository;

        private string _userName;
        private readonly int _sequenceFistElementNegative = -2;
        private readonly ProfilesAndIndicatorsHelper _helper;

        /// <summary>
        /// Accessor that ensures username is defined.
        /// </summary>
        public string UserName
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

        public ProfilesAndIndicatorsController(IProfilesReader reader, IProfilesWriter writer,
            IProfileRepository profileRepository, ILookUpsRepository lookUpsRepository,
            ICoreDataRepository coreDataRepository, IEmailRepository emailRepository)
        {
            _reader = reader;
            _writer = writer;

            _profileRepository = profileRepository;
            _lookUpsRepository = lookUpsRepository;
            _coreDataRepository = coreDataRepository;
            _emailRepository = emailRepository;

            _helper = new ProfilesAndIndicatorsHelper(reader, writer, profileRepository);
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _userName = UserName;
        }

        [Route("test-page")]
        public ActionResult Test()
        {
            return Content("Test");
        }

        [Route("~/")]
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

                model.GroupingSubheadings = _profileRepository.GetGroupingSubheadings(profile.SelectedAreaType, model.SelectedGroupId);
            }

            return View(IndexView, model);
        }

        [Route("profile/indicators/specific")]
        public ActionResult ListIndicatorsInProfileSpecific(int? domainSequence, int? indicatorId, string resetArea,
            int selectedAreaTypeId = -1, string search_text = "", string sortBy = "Sequence",
            bool ascending = true, int page = 1, int pageSize = 200, string profileKey = null,
            int newIndicatorId = -1, bool isNewIndicator = false)
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

                    model.GroupingSubheadings = _profileRepository.GetGroupingSubheadings(profile.SelectedAreaType, model.SelectedGroupId);
                }
            }

            ViewBag.IsNewIndicator = isNewIndicator;
            ViewBag.NewIndicatorId = newIndicatorId;

            return View(IndexView, model);
        }

        /// <summary>
        /// Remove indicators from a profile
        /// </summary>
        [HttpGet]
        [Route("remove-indicators")]
        public ActionResult RemoveIndicators(IEnumerable<string> jdata, string selectedProfileUrlkey,
            string selectedProfileName, int selectedDomainId, string selectedDomainName, int selectedAreaTypeId)
        {
            var model = new RemoveIndicatorsModel
            {
                UrlKey = selectedProfileUrlkey,
                Profile = GetProfile(selectedProfileUrlkey, selectedDomainId, selectedAreaTypeId),
                DomainName = selectedDomainName,
                DomainId = selectedDomainId,
                ProfileName = selectedProfileName,
                SelectedAreaTypeId = selectedAreaTypeId,
                IndicatorsThatCantBeRemoved = null,
            };

            // Get the user group permissions and assign it to the model
            var userPermissions = CommonUtilities.GetUserGroupPermissionsByUserId(_reader.GetUserByUserName(_userName).Id);
            model.UserGroupPermissions = CommonUtilities.GetProfileUserPermissions(userPermissions, model.UrlKey);

            // Get the list of indicators considered for deletion
            var indicatorSpecifiers = IndicatorSpecifierParser.Parse(jdata.ToArray());
            var indicatorList = CommonUtilities.GetSpecifiedIndicatorNames(indicatorSpecifiers, model.Profile.IndicatorNames);

            // Categorise the indicators into two separate lists
            // first with the indicators that cannot be removed
            // second with the indicators that can be removed
            var cannotRemove = new List<GroupingPlusName>();
            var canRemove = new List<GroupingPlusName>();

            // Loop through the indicator list
            var indicatorDeleteChecker = new RemoveIndicatorChecker(_reader);
            foreach (var indicator in indicatorList)
            {
                // Find out the profile that owns the indicator
                var indicatorMetadata = _reader.GetIndicatorMetadata(indicator.IndicatorId);

                // Assign the indicator metadata to the model
                model.IndicatorMetadata = indicatorMetadata;

                if (indicatorDeleteChecker.CanIndicatorBeRemoved(model.Profile.Id, indicatorMetadata, indicator))
                {
                    canRemove.Add(indicator);
                }
                else
                {
                    cannotRemove.Add(indicator);
                }
            }

            // Assign the list to the models
            model.IndicatorsThatCantBeRemoved = cannotRemove;
            model.IndicatorsToRemove = canRemove;

            if (Request.IsAjaxRequest())
            {
                return PartialView("_RemoveSelectedIndicators", model);
            }

            return View(IndexView);
        }

        /// <summary>
        /// Delete indicators permanently from the system
        /// </summary>
        [Route("delete-indicators")]
        [HttpGet]
        public ActionResult DeleteIndicators(IEnumerable<string> jdata, string selectedProfileUrlkey,
            string selectedProfileName, int selectedDomainId, string selectedDomainName, int selectedAreaTypeId)
        {
            var model = new DeleteIndicatorsModel
            {
                Profile = GetProfile(selectedProfileUrlkey, selectedDomainId, selectedAreaTypeId)
            };

            var indicatorSpecifiers = IndicatorSpecifierParser.Parse(jdata.ToArray());
            model.IndicatorsToDelete = CommonUtilities.GetSpecifiedIndicatorNames(indicatorSpecifiers, model.Profile.IndicatorNames);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_DeleteSelectedIndicators", model);
            }

            return View(IndexView);
        }

        [Route("download-metadata")]
        [HttpGet]
        public FileContentResult DownloadMetadata(string indicatorIds)
        {
            var indicatorIdList = indicatorIds.Split(',').Select(int.Parse).ToList();
            var indicatorMetadataList = _reader.GetIndicatorMetadata(indicatorIdList).ToList();

            var data = CsvStreamHelper.GetCsvDataForMetadataDownload(indicatorMetadataList);

            return new FileContentResult(data, "text/csv");
        }

        [Route("download-metadata1")]
        [HttpGet]
        public FileContentResult DownloadMetadata1(string indicatorIds)
        {
            StringBuilder sb = new StringBuilder();

            var isHeaderRow = true;
            var indicatorIdStringList = indicatorIds.Split(',').ToList();
            foreach (var indicatorIdString in indicatorIdStringList)
            {
                var indicatorMetadata = _reader.GetIndicatorMetadata(int.Parse(indicatorIdString));

                var indicatorMetadataTextProperties = _reader.GetIndicatorMetadataTextProperties();
                var indicatorMetadataTextValues = (List<IndicatorText>) _reader.GetIndicatorTextValues(int.Parse(indicatorIdString),
                    indicatorMetadataTextProperties, indicatorMetadata.OwnerProfileId);

                // Header row
                string values;
                if (isHeaderRow)
                {
                    values = string.Empty;

                    foreach (var indicatorMetadataTextValue in indicatorMetadataTextValues)
                    {
                        values += indicatorMetadataTextValue.IndicatorMetadataTextProperty.DisplayName + ",";
                    }

                    sb.AppendLine(values);

                    isHeaderRow = false;
                }

                // Data row
                values = string.Empty;
                foreach (var indicatorMetadataTextValue in indicatorMetadataTextValues)
                {
                    values += indicatorMetadataTextValue.ValueGeneric + ",";
                }
                sb.AppendLine(values);
            }

            return new FileContentResult(new System.Text.UTF8Encoding().GetBytes(sb.ToString()), "text/csv");
        }

        [Route("indicator-audit")]
        [HttpGet]
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

        [Route("move-indicators")]
        [HttpGet]
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

            var listOfProfiles = GetOrderedListOfProfilesForCurrentUser(model)
                .Where(x => x.Value != "indicators-for-review").ToList();

            var domains = new ProfileMembers();
            var defaultProfile = listOfProfiles.FirstOrDefault(x => x.Selected) ?? listOfProfiles.FirstOrDefault();
            defaultProfile.Selected = true;

            model.ListOfDomains = CommonUtilities.GetOrderedListOfDomainsWithGroupId(domains, defaultProfile, _profileRepository);

            var userPermissions = CommonUtilities.GetUserGroupPermissionsByUserId(_reader.GetUserByUserName(_userName).Id);

            model.UserGroupPermissions = CommonUtilities.GetProfileUserPermissions(userPermissions, model.UrlKey);

            var indicatorSpecifiers = IndicatorSpecifierParser.Parse(jdata.ToArray());
            model.IndicatorsToTransfer = CommonUtilities.GetSpecifiedIndicatorNames(indicatorSpecifiers,
                model.Profile.IndicatorNames);

            ViewBag.SexList = _lookUpsRepository.GetSexes();

            ViewBag.AgeList = _lookUpsRepository.GetAges();


            if (Request.IsAjaxRequest())
            {
                return PartialView("_MoveSelectedIndicators", model);
            }

            return View(IndexView);
        }

        [Route("copy-indicators")]
        [HttpGet]
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
            var listOfProfiles = GetOrderedListOfProfilesForCurrentUser(model)
                .Where(x => x.Value != "indicators-for-review").ToList();

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
            model.UserGroupPermissions = CommonUtilities.GetProfileUserPermissions(userPermissions, model.UrlKey);

            var indicatorSpecifiers = IndicatorSpecifierParser.Parse(jdata.ToArray());
            model.IndicatorsToTransfer = CommonUtilities.GetSpecifiedIndicatorNames(indicatorSpecifiers,
                model.Profile.IndicatorNames);

            ViewBag.Profiles = listOfProfiles;
            ViewBag.SexList = _lookUpsRepository.GetSexes();
            ViewBag.AgeList = _lookUpsRepository.GetAges();

            if (Request.IsAjaxRequest())
            {
                return PartialView("_CopySelectedIndicators", model);
            }

            return View(IndexView);
        }

        [Route("submit-indicators-for-review")]
        [HttpGet]
        public ActionResult SubmitIndicatorsForReview(IEnumerable<string> jdata, string selectedProfileUrlkey,
            int selectedDomainId, int selectedAreaTypeId)
        {
            var model = new SubmitIndicatorsForReviewModel
            {
                UrlKey = selectedProfileUrlkey,
                Profile = GetProfile(selectedProfileUrlkey, selectedDomainId, selectedAreaTypeId),
                FromGroupId = selectedDomainId == 1 ? GroupIds.InDevelopment : GroupIds.AwaitingRevision,
                ToGroupId = GroupIds.UnderReview,
                AreaTypeId = selectedAreaTypeId
            };

            var indicatorSpecifiers = IndicatorSpecifierParser.Parse(jdata.ToArray());
            model.IndicatorsToReview = CommonUtilities.GetSpecifiedIndicatorNames(indicatorSpecifiers,
                model.Profile.IndicatorNames);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_SubmitIndicatorsForReview", model);
            }

            return View(IndexView);
        }

        [Route("indicators-awaiting-revision")]
        [HttpGet]
        public ActionResult IndicatorsAwaitingRevision(IEnumerable<string> jdata, string selectedProfileUrlkey,
            int selectedDomainId, int selectedAreaTypeId)
        {
            var model = new IndicatorsAwaitingRevisionModel
            {
                UrlKey = selectedProfileUrlkey,
                Profile = GetProfile(selectedProfileUrlkey, selectedDomainId, selectedAreaTypeId),
                FromGroupId = GroupIds.UnderReview,
                ToGroupId = GroupIds.AwaitingRevision,
                AreaTypeId = selectedAreaTypeId
            };

            var indicatorSpecifiers = IndicatorSpecifierParser.Parse(jdata.ToArray());
            model.IndicatorsAwaitingRevision = CommonUtilities.GetSpecifiedIndicatorNames(indicatorSpecifiers,
                model.Profile.IndicatorNames);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_IndicatorsAwaitingRevision", model);
            }

            return View(IndexView);
        }

        [Route("approve-indicators")]
        [HttpGet]
        public ActionResult ApproveIndicators(IEnumerable<string> jdata, string selectedProfileUrlkey,
            int selectedDomainId, int selectedAreaTypeId)
        {
            var model = new ApproveIndicatorsModel
            {
                UrlKey = selectedProfileUrlkey,
                Profile = GetProfile(selectedProfileUrlkey, selectedDomainId, selectedAreaTypeId),
                AreaTypeId = selectedAreaTypeId
            };

            var indicatorSpecifiers = IndicatorSpecifierParser.Parse(jdata.ToArray());
            model.IndicatorsToApprove = CommonUtilities.GetSpecifiedIndicatorNames(indicatorSpecifiers,
                model.Profile.IndicatorNames);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_ApproveIndicators", model);
            }

            return View(IndexView);
        }

        [Route("reload-domains")]
        [HttpPost]
        public JsonResult ReloadDomains(string selectedProfile)
        {
            var model = new ProfileMembers
            {
                UrlKey = selectedProfile,
                Profile = CommonUtilities.GetProfile(selectedProfile, 1, AreaTypeIds.CountyAndUnitaryAuthorityPre2019, _profileRepository)
            };

            IList<GroupingMetadata> groupingMetadatas = model.Profile.GroupingMetadatas;

            var domains = new SelectList(groupingMetadatas.OrderBy(g => g.Sequence), "GroupId", "GroupName");
            return Json(domains);
        }

        [Route("delete-domain")]
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

        [Route("save-grid-domains")]
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

        [Route("confirm-copy-indicators")]
        [HttpGet]
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

        [Route("confirm-move-indicators")]
        [HttpGet]
        public ActionResult ConfirmMoveIndicators(MoveIndicatorsModel mim, string selectedProfileId,
            List<int> selectedDomainId, int selectedAreaTypeId, string indicatorTransferDetails)
        {
            var groupId = _helper.GetSelectedGroupIdUsingProfileKeyDomainAndAreaTypeId(mim.UrlKey, mim.DomainIndex, mim.AreaTypeId);
            var modelCount = 0;

            //There are indicators to remove (move to the unassigned domain)
            var indicatorSpecifierStrings = indicatorTransferDetails.Split(',').ToList();
            var indicatorSpecifiers = IndicatorSpecifierParser.Parse(indicatorSpecifierStrings);

            foreach (var indicatorSpecifier in indicatorSpecifiers)
            {
                var indicatorId = indicatorSpecifier.IndicatorId;
                var sexId = indicatorSpecifier.SexId;
                var ageId = indicatorSpecifier.AgeId;

                //Don't move if identical indicator already exists in destination.
                if (!_profileRepository.IndicatorGroupingsExist(indicatorId,
                    Convert.ToInt32(selectedDomainId[0]), selectedAreaTypeId, ageId,
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

            _writer.ReorderIndicatorSequence(groupId, mim.AreaTypeId);

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [Route("confirm-remove-indicators")]
        [HttpGet]
        public ActionResult ConfirmRemoveIndicators(RemoveIndicatorsModel indicatorsModel, string indicatorRemoveDetails)
        {
            if (indicatorRemoveDetails != string.Empty)
            {
                var areaTypeId = indicatorsModel.SelectedAreaTypeId;
                var groupId = _helper.GetSelectedGroupIdUsingProfileKeyDomainAndAreaTypeId(indicatorsModel.UrlKey,
                    indicatorsModel.DomainId, areaTypeId);

                var indicatorSpecifierStrings = indicatorRemoveDetails.Split(',').ToList();
                var indicatorSpecifiers = IndicatorSpecifierParser.Parse(indicatorSpecifierStrings);

                foreach (var indicatorSpecifier in indicatorSpecifiers)
                {
                    var indicatorId = indicatorSpecifier.IndicatorId;
                    var sexId = indicatorSpecifier.SexId;
                    var ageId = indicatorSpecifier.AgeId;
                    var profileId = indicatorsModel.Profile.Id;

                    GroupingRemover groupingRemover = new GroupingRemover(_reader, _profileRepository);
                    groupingRemover.RemoveGroupings(profileId, groupId, indicatorId, areaTypeId, sexId, ageId);
                }

                _writer.ReorderIndicatorSequence(groupId, areaTypeId);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [Route("confirm-delete-indicators")]
        [HttpGet]
        public ActionResult ConfirmDeleteIndicators(DeleteIndicatorsModel model, string indicatorDeleteDetails)
        {
            if (indicatorDeleteDetails != string.Empty)
            {
                var indicatorSpecifierStrings = indicatorDeleteDetails.Split(',').ToList();
                var indicatorSpecifiers = IndicatorSpecifierParser.Parse(indicatorSpecifierStrings);

                foreach (var indicatorSpecifier in indicatorSpecifiers)
                {
                    var indicatorId = indicatorSpecifier.IndicatorId;

                    _profileRepository.DeleteIndicator(indicatorId);

                    var auditMessage = string.Format("Indicator {0} deleted permanently from the system.", indicatorId);
                    _profileRepository.LogAuditChange(auditMessage, indicatorId, null, _userName, DateTime.Now,
                        CommonUtilities.AuditType.Delete.ToString());
                }
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [Route("confirm-submit-indicators-for-review")]
        [HttpPost]
        public ActionResult ConfirmSubmitIndicatorsForReview(SubmitIndicatorsForReviewModel model, string indicatorsToReviewDetails)
        {
            var fromGroupId = model.FromGroupId;
            var toGroupId = model.ToGroupId;
            var areaTypeId = model.AreaTypeId;
            var domainNumber = fromGroupId == GroupIds.InDevelopment ? 1 : 3;

            var profile = GetProfile(UrlKeys.IndicatorsForReview, domainNumber, areaTypeId);

            var indicatorSpecifierStrings = indicatorsToReviewDetails.Split(',').ToList();
            var indicatorSpecifiers = IndicatorSpecifierParser.Parse(indicatorSpecifierStrings);
            var indicators = CommonUtilities.GetSpecifiedIndicatorNames(indicatorSpecifiers, profile.IndicatorNames);
            var urlReferrer = Request.UrlReferrer;

            foreach (var indicator in indicators)
            {
                _helper.MoveIndicatorInDb(fromGroupId, toGroupId, areaTypeId, indicator.IndicatorId, indicator.SexId, indicator.AgeId, IndicatorStatus.UnderReview);

                var indicatorNameWithSexAndAgeLabel = CommonUtilities.GetIndicatorNameWithSexAndAgeLabel(indicator);
                var indicatorUrl = CommonUtilities.GetIndicatorUrl(urlReferrer.Scheme, urlReferrer.Authority,
                    urlReferrer.AbsolutePath, UrlKeys.IndicatorsForReview, 2, areaTypeId);

                var notifyEmailTemplate = fromGroupId == GroupIds.InDevelopment
                    ? NotifyEmailTemplates.IndicatorSubmittedForReview
                    : NotifyEmailTemplates.IndicatorSubmittedForReviewAfterRevision;

                EmailHelper emailHelper = new EmailHelper(_emailRepository, notifyEmailTemplate, indicator.IndicatorId, indicatorNameWithSexAndAgeLabel, indicatorUrl);
                emailHelper.SendEmail();
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [Route("confirm-indicators-awaiting-revision")]
        [HttpPost]
        public ActionResult ConfirmIndicatorsAwaitingRevision(IndicatorsAwaitingRevisionModel model, string indicatorsAwaitingRevisionDetails)
        {
            var fromGroupId = model.FromGroupId;
            var toGroupId = model.ToGroupId;
            var areaTypeId = model.AreaTypeId;

            var profile = GetProfile(UrlKeys.IndicatorsForReview, 2, areaTypeId);

            var indicatorSpecifierStrings = indicatorsAwaitingRevisionDetails.Split(',').ToList();
            var indicatorSpecifiers = IndicatorSpecifierParser.Parse(indicatorSpecifierStrings);
            var indicators = CommonUtilities.GetSpecifiedIndicatorNames(indicatorSpecifiers, profile.IndicatorNames);
            var urlReferrer = Request.UrlReferrer;

            foreach (var indicator in indicators)
            {
                _helper.MoveIndicatorInDb(fromGroupId, toGroupId, areaTypeId, indicator.IndicatorId, indicator.SexId, indicator.AgeId, IndicatorStatus.ChangesRequested);

                var indicatorNameWithSexAndAgeLabel = CommonUtilities.GetIndicatorNameWithSexAndAgeLabel(indicator);
                var indicatorUrl = CommonUtilities.GetIndicatorUrl(urlReferrer.Scheme, urlReferrer.Authority,
                    urlReferrer.AbsolutePath, UrlKeys.IndicatorsForReview, 3, areaTypeId);

                EmailHelper emailHelper = new EmailHelper(_emailRepository, NotifyEmailTemplates.IndicatorAwaitingRevision, indicator.IndicatorId, indicatorNameWithSexAndAgeLabel, indicatorUrl);
                emailHelper.SendEmail();
            }

            return Redirect(urlReferrer.AbsoluteUri);
        }

        [Route("confirm-approve-indicators")]
        [HttpPost]
        public ActionResult ConfirmApproveIndicators(ApproveIndicatorsModel model, string indicatorsToApproveDetails)
        {
            var areaTypeId = model.AreaTypeId;
            var indicatorSpecifierStrings = indicatorsToApproveDetails.Split(',').ToList();
            var indicatorSpecifiers = IndicatorSpecifierParser.Parse(indicatorSpecifierStrings);
            var profile = GetProfile(UrlKeys.IndicatorsForReview, 2, areaTypeId);
            var indicators = CommonUtilities.GetSpecifiedIndicatorNames(indicatorSpecifiers, profile.IndicatorNames);
            var urlReferrer = Request.UrlReferrer;

            foreach (var indicator in indicators)
            {
                var indicatorMetadata = _reader.GetIndicatorMetadata(indicator.IndicatorId);
                _profileRepository.ChangeIndicatorProfile(indicator.IndicatorId, indicatorMetadata.DestinationProfileId, IndicatorStatus.Approved);

                var destinationProfile = _profileRepository.GetProfileDetailsById(indicatorMetadata.DestinationProfileId);
                var indicatorNameWithSexAndAgeLabel = CommonUtilities.GetIndicatorNameWithSexAndAgeLabel(indicator);
                var indicatorUrl = CommonUtilities.GetIndicatorUrl(urlReferrer.Scheme, urlReferrer.Authority,
                    urlReferrer.AbsolutePath, destinationProfile.UrlKey, 1, areaTypeId);

                EmailHelper emailHelper = new EmailHelper(_emailRepository, NotifyEmailTemplates.IndicatorApprovedNotificationToCreatedUser, indicator.IndicatorId, indicatorNameWithSexAndAgeLabel, indicatorUrl);
                emailHelper.SendEmail();

                emailHelper = new EmailHelper(_emailRepository, NotifyEmailTemplates.IndicatorApprovedNotificationToIMRG, indicator.IndicatorId, indicatorNameWithSexAndAgeLabel, indicatorUrl);
                emailHelper.SendEmail();
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [HttpGet]
        [Route("browse-data/full-page/{indicatorId}")]
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
        [Route("browse-data/{indicatorId}")]
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
                DataChange = _helper.GetDataChangeAudit(indicatorId)
            };

            PopulateFilterDataDropDownLists(model);
            return PartialView("BrowseIndicatorData", model);
        }

        [HttpPost]
        [AjaxOnly]
        [Route("browse-data")]
        public ActionResult BrowseIndicatorData(BrowseDataViewModel model)
        {
            var filters = _helper.GetFilters(model);
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
        [Route("delete-data")]
        public ActionResult DeleteIndicatorData(BrowseDataViewModel model)
        {
            var filters = _helper.GetFilters(model);

            var coreDataSetViewModel = new CoreDataSetViewModel();
            coreDataSetViewModel.CanDeleteDataSet = _coreDataRepository.CanDeleteDataSet(model.IndicatorId, _userName);

            try
            {
                var rowsDeleted = _coreDataRepository.DeleteCoreDataSet(model.IndicatorId, filters, _userName);
                coreDataSetViewModel.RowsFound = rowsDeleted;
                coreDataSetViewModel.DeleteStatus = true;
                coreDataSetViewModel.Message = string.Format("{0} rows have been deleted", rowsDeleted);
            }
            catch (Exception exception)
            {
                ExceptionLogger.LogException(exception, "Global.asax");
                coreDataSetViewModel.DeleteStatus = false;
                coreDataSetViewModel.Message = "Error occurred while deleting data - " + exception.Message;
            }

            return PartialView("_CoreData", coreDataSetViewModel);
        }

        /// <summary>
        /// Reorder indicators
        /// </summary>
        /// <param name="profileId">Profile id</param>
        /// <param name="profileUrlKey">Profile url key</param>
        /// <param name="domainSequence">Sequence number of the domain associated with the profile</param>
        /// <param name="areaTypeId">Area type id</param>
        /// <param name="groupId">Group id</param>
        /// <returns>Reorder indicators view model</returns>
        [Route("reorder-indicators")]
        [HttpGet]
        public ActionResult ReorderIndicators(int profileId, string profileUrlKey, int domainSequence, int areaTypeId, int groupId)
        {
            var reorderIndicatorsViewModel = new ReorderIndicatorsViewModel
            {
                ProfileId = profileId,
                ProfileUrlKey = profileUrlKey,
                DomainSequence = domainSequence,
                AreaTypeId = areaTypeId,
                GroupId = groupId
            };

            return View("ReorderIndicators", reorderIndicatorsViewModel);
        }

        /// <summary>
        /// Save reordered indicators
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("save-reordered-indicators")]
        public ActionResult SaveReorderedIndicators(FormCollection formCollection)
        {
            var profileId = Convert.ToInt32(formCollection["profileId"]);
            var profileUrlKey = formCollection["profileUrlKey"];
            var domainSequence = Convert.ToInt32(formCollection["domainSequence"]);
            var areaTypeId = Convert.ToInt32(formCollection["areaTypeId"]);
            var groupId = Convert.ToInt32(formCollection["groupId"]);
            var groupingPlusNames = JsonConvert.DeserializeObject<IList<GroupingPlusName>>(formCollection["groupingPlusNames"]);
            var groupingSubheadings = JsonConvert.DeserializeObject<IList<GroupingSubheading>>(formCollection["groupingSubheadings"]);

            try
            {
                _helper.UpdateGroupings(groupingPlusNames, groupId);

                // Delete grouping subheadings
                DeleteMissingSubheadings(areaTypeId, groupId, groupingSubheadings);

                // Update grouping subheadings
                UpdateNewSubheadingData(groupingSubheadings);

                return new HttpStatusCodeResult(HttpStatusCode.OK, "Saved reordered indicators");
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, exception.Message);
            }
        }

        /// <summary>
        /// Get the indicators with names for profile, area type and grouping sequence
        /// </summary>
        /// <param name="profileUrlKey">Profile url key</param>
        /// <param name="sequenceNumber">Sequence number of the group associated with the profile</param>
        /// <param name="areaTypeId">Area type id</param>
        /// <returns>List of grouping plus name objects</returns>
        [Route("profile-grouping-indicators")]
        [HttpGet]
        public ActionResult GetGroupingPlusNames(string profileUrlKey, int sequenceNumber, int areaTypeId)
        {
            Profile profile = new ProfileBuilder(_reader, _profileRepository).Build(profileUrlKey, sequenceNumber, areaTypeId);
            IList<GroupingPlusName> groupingPlusNames = profile.IndicatorNames;

            return Json(groupingPlusNames, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get group name by group id and sequence
        /// </summary>
        /// <param name="groupId">Group id</param>
        /// <param name="domainSequence">Sequence number of the domain</param>
        /// <returns>Domain name</returns>
        [Route("domain-name")]
        [HttpGet]
        public ActionResult GetDomainName(int groupId, int domainSequence)
        {
            string domainName = _profileRepository.GetDomainName(groupId, domainSequence);
            return Json(domainName, JsonRequestBehavior.AllowGet);
        }

        public void UpdateNewSubheadingData(IList<GroupingSubheading> groupingSubheadings)
        {
            foreach (var groupingSubheading in groupingSubheadings)
            {
                if (groupingSubheading.SubheadingId <= _sequenceFistElementNegative)
                {
                    _profileRepository.SaveGroupingSubheading(groupingSubheading);
                }
                else
                {
                    _profileRepository.UpdateGroupingSubheading(groupingSubheading);
                }
            }
        }

        public void DeleteMissingSubheadings(int areaTypeId, int groupId, IList<GroupingSubheading> groupingSubheadings)
        {
            var groupingSubheadingsInDb = _profileRepository.GetGroupingSubheadings(areaTypeId, groupId);
            foreach (var groupingSubheadingInDb in groupingSubheadingsInDb)
            {
                if (groupingSubheadings.FirstOrDefault(x => x.SubheadingId > _sequenceFistElementNegative && x.SubheadingId == groupingSubheadingInDb.SubheadingId) == null)
                {
                    _profileRepository.DeleteGroupingSubheading(groupingSubheadingInDb.SubheadingId);
                }
            }
        }

        private Profile GetProfile(string urlKey, int selectedDomainNumber = 0, int areaType = AreaTypeIds.Undefined)
        {
            return new ProfileBuilder(_reader, _profileRepository).Build(urlKey, selectedDomainNumber, areaType);
        }

        public void GetProfiles(IndicatorGridModel model)
        {
            var profiles = _reader.GetProfiles().OrderBy(x => x.Name);
            IList<ProfileDetails> profilesWithAssignedIndicators = profiles.ToList();

            var listOfProfiles = CommonUtilities.GetOrderedListOfProfiles(profilesWithAssignedIndicators);
            model.ProfileList = listOfProfiles;

            var firstOrDefault = model.ProfileList.FirstOrDefault();
            if (firstOrDefault != null)
            {
                model.ProfileKey = firstOrDefault.Value;
            }
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

        private IList<GroupingMetadata> ReloadGridDomains(string selectedProfile, IProfileRepository profileRepository)
        {
            var model = new ProfileMembers
            {
                UrlKey = selectedProfile,
                Profile = CommonUtilities.GetProfile(selectedProfile, 1, AreaTypeIds.Undefined, profileRepository)
            };

            return model.Profile.GroupingMetadatas;
        }

        private void AssignContactDetails(Profile profile, IndicatorGridModel model)
        {
            var profileContacts = _reader.GetUserListByUserIds(profile.ContactUserIds);
            var profileContactNames = new List<string>();
            var profileContactEmailAddresses = new List<string>();

            foreach (var fpmUser in profileContacts)
            {
                profileContactNames.Add(fpmUser.DisplayName);
                profileContactEmailAddresses.Add(fpmUser.EmailAddress);
            }

            model.ContactUserNames = String.Join(", ", profileContactNames);
            model.EmailAddresses = String.Join(", ", profileContactEmailAddresses);
        }

        private static void AssignAreaTypeData(IndicatorGridModel model, Profile profile)
        {
            var builder = new AreaTypeSelectListBuilder(profile.AreaTypes, profile.SelectedAreaType);
            model.SelectedAreaTypeId = builder.SelectedAreaTypeId;
            model.AreaTypeList = builder.SelectListItems;
        }

        private void AssignUserPermissionsToIndicatorGridModel(IndicatorGridModel model)
        {
            var userPermissions = _helper.GetUserGroupPermissions();
            model.UserHasAssignedPermissions = userPermissions.Any();
            model.UserGroupPermissions = CommonUtilities.GetProfileUserPermissions(userPermissions, model.UrlKey);
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

            ViewBag.YearRange = _coreDataRepository.GetYearRanges(model.IndicatorId).ConvertToSelectListWithAnyOption();

            ViewBag.Year = _coreDataRepository.GetYears(model.IndicatorId).ConvertToSelectListWithAnyOption();

            ViewBag.Month = _coreDataRepository.GetMonths(model.IndicatorId).ConvertToSelectListWithAnyOption();

            ViewBag.Quarter = _coreDataRepository.GetQuarters(model.IndicatorId).ConvertToSelectListWithAnyOption();
        }

        private IEnumerable<SelectListItem> GetOrderedListOfProfilesForCurrentUser(BaseDataModel model)
        {
            var listOfProfiles = CommonUtilities.GetOrderedListOfProfilesForCurrentUser(model);
            ViewBag.listOfProfiles = listOfProfiles;
            return listOfProfiles;
        }
    }
}