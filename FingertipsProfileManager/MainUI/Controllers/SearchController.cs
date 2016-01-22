using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class SearchController : Controller
    {
        private readonly ProfilesReader _reader = ReaderFactory.GetProfilesReader();
       
        private string _userName;

        private ProfileRepository _profileRepository;

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _userName = UserDetails.CurrentUser().Name;
        }

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
                    Debug.WriteLine(grouping.GroupId);
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
                            .GetGroupingPlusNames(grouping.IndicatorId, grouping.GroupId, grouping.AreaTypeId, profile.Id)
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

        private void LoadProfileAndDomainDropdowns()
        {
            List<SelectListItem> listOfProfiles =
                CommonUtilities.GetOrderedListOfProfilesForCurrentUser(new BaseDataModel())
                .ToList();
            ViewBag.listOfProfiles = listOfProfiles;

            var domains = new ProfileMembers();
            var defaultProfile = listOfProfiles.FirstOrDefault();
            ViewBag.ListOfDomains = CommonUtilities.GetOrderedListOfDomainsWithGroupId(domains, defaultProfile, _profileRepository);
        }

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

            return RedirectToAction("SortPageAndFilter", "ProfilesAndIndicators", new { ProfileKey = selectedProfileId });
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _profileRepository = new ProfileRepository();
            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _profileRepository.Dispose();
            base.OnActionExecuted(filterContext);
        }

    }
}