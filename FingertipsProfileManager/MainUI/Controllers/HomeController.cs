using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using System.Web.Routing;
using Fpm.MainUI.Helpers;
using Fpm.MainUI.Models;
using Fpm.ProfileData;

namespace Fpm.MainUI.Controllers
{
    public class HomeController : Controller
    {
        public const string ProfilesAndIndicators = "ProfilesAndIndicators";
        
        private readonly ProfilesReader _reader = ReaderFactory.GetProfilesReader();
        private readonly ProfileDataAccess _dataAccess = new ProfileDataAccess();
        private readonly ProfilesWriter _writer = ReaderFactory.GetProfilesWriter();

        private string _userName;

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _userName = new CurrentUser().Name;
        }

        public ActionResult Index()
        {
            var model = new IndicatorGridModel
                                           {
                                               SortBy = "Sequence",
                                               SortAscending = true,
                                               CurrentPageIndex = 1,
                                               PageSize = 100
                                           };

            GetProfiles(model);
            GetDomains(model, null);

            var firstOrDefault = model.DomainList.FirstOrDefault();
            if (firstOrDefault != null)
            {
                var profile = GetProfile(model.ProfileKey, Convert.ToInt32(firstOrDefault.Value), -1);
                IQueryable<GroupingPlusNames> indicators = profile.IndicatorNames.AsQueryable();

                model.Profile = profile;
                model.UrlKey = model.ProfileKey;

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
            var reloadedDomains = ReloadGridDomains(model.ProfileKey);

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
            bool ascending = true, int page = 1, int pageSize = 100, string profileKey = null)
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
                IQueryable<GroupingPlusNames> indicators = profile.IndicatorNames.AsQueryable();

                AssignAreaTypeData(model, profile);

                model.Profile = profile;
                model.UrlKey = model.ProfileKey;

                if (indicatorId != null)
                    indicators = indicators.Where(i => i.IndicatorId.ToString().Contains(indicatorId.ToString()));

                if (search_text != null)
                    indicators = indicators.Where(i => i.IndicatorName.ToLower().Contains(search_text.ToLower()));

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

        private static Profile GetProfile(string urlKey, int selectedDomainNumber, int areaType)
        {
            return new ProfileBuilder().Build(urlKey, selectedDomainNumber, areaType);
        }

        private IList<GroupingMetadata> ReloadGridDomains(string selectedProfile)
        {
            var model = new ProfileMembers
                {
                    UrlKey = selectedProfile,
                    Profile = CommonUtilities.GetProfile(selectedProfile, 1, -1)
                };

            return model.Profile.GroupingMetadatas;
        }

        public int GetSelectedGroupIdUsingProfileKeyDomainAndAreaTypeId(string selectedProfile, int domainNumber, int areaTypeId)
        {
            var model = new ProfileMembers
            {
                UrlKey = selectedProfile,
                Profile = CommonUtilities.GetProfile(selectedProfile, domainNumber, areaTypeId)
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

            var cannotRemove = new List<GroupingPlusNames>();
            var canRemove = new List<GroupingPlusNames>();
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
                        var groupIds = _reader.GetProfileDetailsByProfileId(model.Profile.Id).GroupIds;
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

        private static List<GroupingPlusNames> GetSpecifiedIndicatorNames(List<IndicatorSpecifier> indicatorSpecifiers, IList<GroupingPlusNames> allIndicators)
        {
            List<GroupingPlusNames> indicatorList = new List<GroupingPlusNames>();
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
            var defaultProfile = listOfProfiles.FirstOrDefault();
            model.ListOfDomains = CommonUtilities.GetOrderedListOfDomainsWithGroupId(domains, defaultProfile, selectedDomainId);

            var userPermissions = CommonUtilities.GetUserGroupPermissionsByUserId(_reader.GetUserByUserName(_userName).Id);
            model.UserGroupPermissions = userPermissions.FirstOrDefault(x => x.ProfileId == _reader.GetProfileDetails(model.UrlKey).Id);

            var indicatorSpecifiers = IndicatorSpecifierParser.Parse(jdata.ToArray());
            model.IndicatorsToTransfer = GetSpecifiedIndicatorNames(indicatorSpecifiers,
                model.Profile.IndicatorNames);

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

            model.ListOfDomains = CommonUtilities.GetOrderedListOfDomainsWithGroupId(domains, defaultProfile, selectedDomainId);
            model.GroupId = Convert.ToInt32(model.ListOfDomains.FirstOrDefault(x => x.Selected).Value);

            var userPermissions = CommonUtilities.GetUserGroupPermissionsByUserId(_reader.GetUserByUserName(_userName).Id);
            model.UserGroupPermissions = userPermissions.FirstOrDefault(x => x.ProfileId == _reader.GetProfileDetails(model.UrlKey).Id);

            var indicatorSpecifiers = IndicatorSpecifierParser.Parse(jdata.ToArray());
            model.IndicatorsToTransfer = GetSpecifiedIndicatorNames(indicatorSpecifiers,
                model.Profile.IndicatorNames);

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
                Profile = CommonUtilities.GetProfile(selectedProfile, 1, AreaTypeIds.CountyAndUnitaryAuthority)
            };

            IList<GroupingMetadata> groupingMetadatas = model.Profile.GroupingMetadatas;

            var domains = new SelectList(groupingMetadatas.OrderBy(g => g.Sequence), "GroupId", "GroupName");
            return Json(domains);
        }

        [HttpPost]
        public ActionResult DelGridDomains(ProfileMembers pm, int domainDeleteId)
        {
            if (ModelState.IsValid) 
            { 
            List<int> groups = new List<int>();

            foreach (var domain in pm.Profile.GroupingMetadatas)
            {
                if (domain.GroupId != domainDeleteId)
                {
                     groups.Add(domain.GroupId);
                }
               
               
            }

            _writer.UpdateProfileDetailsGroupIds(pm.Profile.Id, groups);
            _writer.DeleteGroupingMetadata(domainDeleteId);

          
            }

            if (Request.UrlReferrer != null) return Redirect(Request.UrlReferrer.AbsoluteUri);

            return View("ProfilesAndIndicators");
        }

        [HttpPost]
        public ActionResult SaveGridDomains(ProfileMembers pm, string newDomain)
        {
            if (ModelState.IsValid)
            {
                string allGroupIds = null;
                //OK, now update the posted-back data...
                foreach (var domain in pm.Profile.GroupingMetadatas)
                {
                    allGroupIds += domain.GroupId + ",";
                    _writer.Update(domain);
                }

                //insert any newly added domains
                if (!String.IsNullOrEmpty(newDomain))
                {
                    var groupingMetadata = _writer.NewGroupingMetadata(newDomain,
                        pm.Profile.GroupingMetadatas.Last().Sequence + 1);

                    _dataAccess.UpdateProfileGroupIds(allGroupIds + groupingMetadata.GroupId.ToString(), pm.Profile.Id);
                }
            }

            if (Request.UrlReferrer != null) return Redirect(Request.UrlReferrer.AbsoluteUri);

            return View("ProfilesAndIndicators");
        }

       

        [HttpPost]
        public ActionResult SaveReorderedIndicators(FormCollection fc, int? currentDomainId, string indicatorOrder)
        {
            var indicatorSpecifierStrings = indicatorOrder.Split('¬').ToList();
            indicatorSpecifierStrings.RemoveAt(indicatorSpecifierStrings.Count - 1);
            var sequenceNumber = 1;

            var indicatorSpecifiers = IndicatorSpecifierParser.Parse(indicatorSpecifierStrings);
            foreach (var indicatorSpecifier in indicatorSpecifiers)
            {
                _dataAccess.UpdateIndicatorGroupingSequence(indicatorSpecifier.IndicatorId, sequenceNumber,
                    currentDomainId, Convert.ToInt32(fc["AreaType"]), indicatorSpecifier.SexId);
                sequenceNumber++;
            }

            if (Request.UrlReferrer != null) return Redirect(Request.UrlReferrer.AbsoluteUri);

            return View("ProfilesAndIndicators");
        }

        public ActionResult ConfirmCopyIndicators(CopyIndicatorsModel cim, List<int> selectedDomainId,
            int selectedAreaTypeId, string indicatorTransferDetails)
        {
            var groupId = GetSelectedGroupIdUsingProfileKeyDomainAndAreaTypeId(cim.UrlKey, cim.DomainId, cim.AreaTypeId);
            var modelCount = 0;

            var indicatorSpecifierStrings = indicatorTransferDetails.Split(',').ToList();
            var indicatorSpecifiers = IndicatorSpecifierParser.Parse(indicatorSpecifierStrings);

            foreach (var indicatorSpecifier in indicatorSpecifiers)
            {
                var indicatorId = indicatorSpecifier.IndicatorId;
                var sexId = indicatorSpecifier.SexId;
                var ageId = indicatorSpecifier.AgeId;

                // Don't copy if identical indicator (including age and sex Id) already exists in destination.
                if (!_dataAccess.IndicatorGroupingsExist(indicatorId, Convert.ToInt32(selectedDomainId[0]),
                    selectedAreaTypeId, Convert.ToInt32(ageId), Convert.ToInt32(sexId)))
                {
                    var currentIndicator = cim.IndicatorsToTransfer[modelCount];

                    _dataAccess.CopyIndicatorToDomain(indicatorId, groupId, cim.AreaTypeId,
                        currentIndicator.SexId, currentIndicator.AgeId,
                        Convert.ToInt32(selectedDomainId[0]), selectedAreaTypeId, sexId,
                        ageId);

                    _dataAccess.LogAuditChange("Indicator " + indicatorId + " copied from [" +
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
                if (!_dataAccess.IndicatorGroupingsExist(indicatorId,
                    Convert.ToInt32(selectedDomainId[0]), mim.AreaTypeId, ageId,
                    sexId))
                {
                    var currentIndicator = mim.IndicatorsToTransfer[modelCount];

                    _dataAccess.MoveIndicatorToDomain(indicatorId, groupId, mim.AreaTypeId,
                        currentIndicator.SexId, currentIndicator.AgeId,
                        selectedDomainId[0], selectedAreaTypeId, sexId, ageId);

                    _dataAccess.LogAuditChange("Indicator " + indicatorId + " moved from [" + mim.DomainName +
                        " (Area: " + mim.AreaTypeId + ", SexId:" + currentIndicator.SexId +
                        ", AgeId:" + currentIndicator.AgeId + " )] Into [" +
                        _reader.GetGroupingMetadataList(selectedDomainId)[0].GroupName +
                        " (Area: " + selectedAreaTypeId + ", SexId:" + sexId + ", AgeId:" + ageId + " )]",
                        indicatorId, null, _userName, DateTime.Now, CommonUtilities.AuditType.Move.ToString());

                    var indicatorMetadata = _reader.GetIndicatorMetadata(indicatorId);
                    if (indicatorMetadata.OwnerProfileId == mim.Profile.Id)
                    {
                        _dataAccess.UpdateIndicatorOwnership(_reader.GetProfileDetails(selectedProfileId).Id,
                                                                indicatorId);
                    }
                }
                modelCount++;
            }

            _writer.ResetSequences(groupId, mim.AreaTypeId);

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        public ActionResult ConfirmDeleteIndicators(DeleteIndicatorsModel dim, string indicatorDeleteDetails)
        {
            if (indicatorDeleteDetails != string.Empty)
            {
                var areaTypeId = dim.SelectedAreaTypeId;
                var groupId = GetSelectedGroupIdUsingProfileKeyDomainAndAreaTypeId(dim.UrlKey, dim.DomainId, areaTypeId);

                var indicatorSpecifierStrings = indicatorDeleteDetails.Split(',').ToList();
                var indicatorSpecifiers = IndicatorSpecifierParser.Parse(indicatorSpecifierStrings);

                foreach (var indicatorSpecifier in indicatorSpecifiers)
                {
                    var indicatorId = indicatorSpecifier.IndicatorId;
                    var sexId = indicatorSpecifier.SexId;
                    var ageId = indicatorSpecifier.AgeId;

                    var indicatorMetadata = _reader.GetIndicatorMetadata(indicatorId);

                    if (indicatorMetadata.OwnerProfileId == dim.Profile.Id)
                    {
                        //If the profile owns the indicator, check to see if this is the last occurance within this profile that is being deleted
                        var groupIds = _reader.GetProfileDetailsByProfileId(dim.Profile.Id).GroupIds;
                        var indicatorList = _reader.GetGroupingByIndicatorId(new List<int> { indicatorId });

                        var indicatorGroups = new List<Grouping>();
                        foreach (Grouping @group in indicatorList.Where(group => groupIds.Contains(@group.GroupId)).Where(@group => indicatorGroups.All(x => x.AreaTypeId != @group.AreaTypeId)))
                        {
                            indicatorGroups.Add(@group);
                        }

                        var lastIndicatorOccuranceInProfile = indicatorGroups.Count == 1;

                        if (!lastIndicatorOccuranceInProfile)
                        {
                            //This isn't the last occurance of this indicator in this profile so it doesn't need to be archived and can simply be deleted from the grouping table
                            DeleteFromGrouping(groupId, indicatorId, areaTypeId, sexId, ageId);
                        }
                        else
                        {
                            //Indicator is owned by the profile so archive it
                            _dataAccess.ArchiveIndicatorFromGrouping(groupId, indicatorId,
                                areaTypeId, sexId, ageId);

                            //Set the indicator ownership to the archive profile
                            _dataAccess.UpdateIndicatorOwnership(CommonUtilities.ArchiveProfileId,
                                indicatorId);

                            _dataAccess.LogAuditChange("Indicator " + indicatorId +
                                " (Area: " + areaTypeId + ", SexId:" + sexId +
                                ", AgeId:" + ageId + " )  has been archived.",
                                indicatorId, null, _userName, DateTime.Now,
                                CommonUtilities.AuditType.Delete.ToString());
                        }
                    }
                    else
                    {
                        DeleteFromGrouping(groupId, indicatorId, areaTypeId, sexId, ageId);
                    }
                }

                _writer.ResetSequences(groupId, areaTypeId);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        private void DeleteFromGrouping(int groupId, int indicatorId, int areaTypeId, int sexId, int ageId)
        {
            // Profile doesn't own the indicator so actually deleted it from the grouping table 
            _dataAccess.DeleteIndicatorFromGrouping(groupId, indicatorId, areaTypeId, sexId, ageId);

            // Also delete from the IndicatorMetaDataTextValue table (where it has an overridden groupId)
            _dataAccess.DeleteOverridenMetaDataTextValues(indicatorId, groupId);

            _dataAccess.LogAuditChange("Indicator " + indicatorId + " (Area: " + areaTypeId +
                                       ", SexId:" + sexId + ", AgeId:" + ageId + " )  has been deleted.",
                indicatorId, null, _userName, DateTime.Now, CommonUtilities.AuditType.Delete.ToString());
        }
    }
}
