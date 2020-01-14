using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Fpm.MainUI.Helpers;
using Fpm.MainUI.Models;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Repositories;
using Fpm.Search;

namespace Fpm.MainUI.Controllers
{
    [RoutePrefix("search")]
    public class SearchController : Controller
    {
        private readonly IProfilesReader _reader;
        private readonly IProfilesWriter _writer;

        private readonly IProfileRepository _profileRepository;

        private string _userName;

        public SearchController(IProfilesReader reader, IProfilesWriter writer, IProfileRepository profileRepository)
        {
            _reader = reader;
            _writer = writer;

            _profileRepository = profileRepository;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _userName = UserDetails.CurrentUser().Name;
        }

        [Route("search-all")]
        [HttpGet]
        public ActionResult SearchAll(string searchTerm, string indicatorId)
        {
            var model = new List<DomainIndicatorsSearchResult>();

            // Get a distinct list of  records that match the search
            var search = new IndicatorSearch();
            IEnumerable<IndicatorMetadataTextValue> indicatorMatch = string.IsNullOrEmpty(indicatorId)
                ? search.SearchByText(searchTerm)
                : search.SearchByIndicatorId(Convert.ToInt32(indicatorId));

            var allAreaTypes = CommonUtilities.GetAllAreaTypes();
            var profiles = _profileRepository.GetProfiles();

            foreach (var indicatorMetadataTextValue in indicatorMatch)
            {
                var groupings = CommonUtilities
                    .GetGroupingsByIndicatorIds(new List<int> { indicatorMetadataTextValue.IndicatorId })
                    .Distinct(new DistinctGroupComparer())
                    .ToList();

                foreach (var grouping in groupings)
                {
                    var domainIndicators = new DomainIndicatorsSearchResult();

                    domainIndicators.GroupId = grouping.GroupId;

                    // Assign grouping metadata
                    var groupingMetadata = CommonUtilities.GetGroupingMetadata(new List<int> { grouping.GroupId }).First();
                    domainIndicators.GroupName = groupingMetadata.GroupName;
                    domainIndicators.SequenceId = groupingMetadata.Sequence;

                    // Assign profile information                                                         
                    var profile = profiles.FirstOrDefault(x => x.Id == groupingMetadata.ProfileId);

                    if (profile != null)
                    {
                        domainIndicators.ProfileName = profile.Name;
                        domainIndicators.UrlKey = profile.UrlKey;

                        // Assign the rest
                        GroupingPlusName groupingPlusNames = _profileRepository
                            .GetGroupingPlusNames(grouping.IndicatorId, grouping.GroupId, grouping.AreaTypeId, profile.Id, profile.AreIndicatorNamesDisplayedWithNumbers)
                            .First(x => x.SexId == grouping.SexId &&
                                        x.AgeId == grouping.AgeId &&
                                        x.YearRange == grouping.YearRange);
                        groupingPlusNames.AreaType = allAreaTypes.First(x => x.Id == grouping.AreaTypeId).ShortName;
                        groupingPlusNames.ComparatorMethodId = grouping.ComparatorMethodId;
                        groupingPlusNames.ComparatorId = grouping.ComparatorId;

                        domainIndicators.Indicators.Add(groupingPlusNames);
                        model.Add(domainIndicators);
                    }
                }
            }

            model = model.Where(m => m.Indicators.Any()).ToList();
            LoadProfileAndDomainDropdowns();
            return View("SearchAll", model);
        }

        [Route("copy-multiple-indicators")]
        [HttpGet]
        public ActionResult CopyMultipleIndicators(string selectedIndicators, string selectedDomainId, string selectedProfileId)
        {
            var indicatorPropertiesList = selectedIndicators.Split(',').ToArray();

            foreach (var indicatorPropertiesString in indicatorPropertiesList)
            {
                var indicatorProperties = indicatorPropertiesString.Split('~');
                var groupId = indicatorProperties[1];
                var indicatorId = indicatorProperties[2];
                var areaTypeId = indicatorProperties[3];
                var sexId = indicatorProperties[4];
                var ageId = indicatorProperties[5];

                var groupMetaData = _reader.GetGroupingMetadataList(new List<int> { Convert.ToInt32(groupId) })[0];

                //Don't copy if identical indicator already exists in destination.
                if (!_profileRepository.IndicatorGroupingsExist(Convert.ToInt32(indicatorId), Convert.ToInt32(selectedDomainId), Convert.ToInt32(areaTypeId), Convert.ToInt32(ageId), Convert.ToInt32(sexId)))
                {
                    _profileRepository.CopyIndicatorToDomain(Convert.ToInt32(indicatorId), Convert.ToInt32(groupId), Convert.ToInt32(areaTypeId), Convert.ToInt32(sexId), Convert.ToInt32(ageId), Convert.ToInt32(selectedDomainId), Convert.ToInt32(areaTypeId), Convert.ToInt32(sexId), Convert.ToInt32(ageId));
                    _profileRepository.LogAuditChange("Indicator " + indicatorId + " copied from [" + groupMetaData.GroupName + " (Area: " + Convert.ToInt32(areaTypeId) + ")] Into " + "[" + _reader.GetGroupingMetadataList(new List<int> { Convert.ToInt32(selectedDomainId) })[0].GroupName + " (Area: " + Convert.ToInt32(areaTypeId) + ")]", Convert.ToInt32(indicatorId), null, _userName, DateTime.Now, CommonUtilities.AuditType.Copy.ToString());
                }
            }

            return RedirectToAction("ListIndicatorsInProfileSpecific", "ProfilesAndIndicators", new { ProfileKey = selectedProfileId });
        }

        [Route("remove-multiple-indicators")]
        [HttpGet]
        public ActionResult RemoveMultipleIndicators(string selectedIndicators)
        {
            var model = new List<RemoveIndicatorModel>();
            var indicatorPropertiesList = selectedIndicators.Split(',').ToArray();

            var indicatorDeleteChecker = new RemoveIndicatorChecker(_reader);
            var allAreaTypes = CommonUtilities.GetAllAreaTypes();

            var userPermissions = CommonUtilities.GetUserGroupPermissionsByUserId(_reader.GetUserByUserName(_userName).Id);

            foreach (var indicatorPropertiesString in indicatorPropertiesList)
            {
                var indicatorProperties = indicatorPropertiesString.Split('~');
                var profileUrlKey = indicatorProperties[0];
                var groupId = int.Parse(indicatorProperties[1]);
                var indicatorId = int.Parse(indicatorProperties[2]);
                var areaTypeId = int.Parse(indicatorProperties[3]);
                var sexId = int.Parse(indicatorProperties[4]);
                var ageId = int.Parse(indicatorProperties[5]);

                var profile = GetProfile(profileUrlKey);
                var indicatorMetadata = _reader.GetIndicatorMetadata(indicatorId);

                var grouping = CommonUtilities
                    .GetGroupingsByIndicatorIds(new List<int> {indicatorId})
                    .Distinct(new DistinctGroupComparer()).First(x => x.GroupId == groupId);

                var groupingMetadata = _reader.GetGroupingMetadata(groupId);


                // Assign the rest
                GroupingPlusName groupingPlusName = _profileRepository
                    .GetGroupingPlusNames(indicatorId, groupId, areaTypeId, profile.Id, profile.AreIndicatorNamesDisplayedWithNumbers)
                    .First(x => x.SexId == sexId &&
                                x.AgeId == ageId &&
                                x.YearRange == grouping.YearRange);

                groupingPlusName.AreaType = allAreaTypes.First(x => x.Id == areaTypeId).ShortName;
                groupingPlusName.ComparatorMethodId = grouping.ComparatorMethodId;
                groupingPlusName.ComparatorId = grouping.ComparatorId;

                // Get the user group permissions and assign it to the model
                var userGroupPermissions = CommonUtilities.GetProfileUserPermissions(userPermissions, profileUrlKey);
                var userHasPermissionToProfile = userGroupPermissions != null;

                var indicatorCanBeRemoved = userHasPermissionToProfile && indicatorDeleteChecker.CanIndicatorBeRemoved(profile.Id, indicatorMetadata, groupingPlusName);

                var removeIndicatorModel = new RemoveIndicatorModel
                {
                    UrlKey = profileUrlKey,
                    Profile = GetProfile(profileUrlKey),
                    GroupName = groupingMetadata.GroupName,
                    Indicator = groupingPlusName,
                    IndicatorCanBeRemoved = indicatorCanBeRemoved,
                    UserHasPermissionToProfile = userHasPermissionToProfile
                };

                model.Add(removeIndicatorModel);
            }

            return PartialView("_RemoveMultipleIndicatorsConfirmation", model);
        }

        [Route("confirm-remove-multiple-indicators")]
        [HttpGet]
        public ActionResult ConfirmRemoveMultipleIndicators(string removeMultipleIndicatorDetails)
        {
            var indicatorPropertiesList = removeMultipleIndicatorDetails.Split(',').ToArray();

            foreach (var indicatorPropertiesString in indicatorPropertiesList)
            {
                var indicatorProperties = indicatorPropertiesString.Split('~');
                var profileId = int.Parse(indicatorProperties[0]);
                var groupId = int.Parse(indicatorProperties[1]);
                var indicatorId = int.Parse(indicatorProperties[2]);
                var areaTypeId = int.Parse(indicatorProperties[3]);
                var sexId = int.Parse(indicatorProperties[4]);
                var ageId = int.Parse(indicatorProperties[5]);

                GroupingRemover groupingRemover = new GroupingRemover(_reader, _profileRepository);
                groupingRemover.RemoveGroupings(profileId, groupId, indicatorId, areaTypeId, sexId, ageId);

                // Reorder sequence
                _writer.ReorderIndicatorSequence(groupId, areaTypeId);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        private void LoadProfileAndDomainDropdowns()
        {
            List<SelectListItem> listOfProfiles =
                CommonUtilities.GetOrderedListOfProfilesForCurrentUser(new BaseDataModel())
                    .Where(x => x.Value != "indicators-for-review").ToList();

            ViewBag.listOfProfiles = listOfProfiles;

            var domains = new ProfileMembers();
            var defaultProfile = listOfProfiles.FirstOrDefault();
            ViewBag.ListOfDomains = CommonUtilities.GetOrderedListOfDomainsWithGroupId(domains, defaultProfile, _profileRepository);
        }

        private Profile GetProfile(string urlKey, int selectedDomainNumber = 0, int areaType = AreaTypeIds.Undefined)
        {
            return new ProfileBuilder(_reader, _profileRepository).Build(urlKey, selectedDomainNumber, areaType);
        }
    }
}