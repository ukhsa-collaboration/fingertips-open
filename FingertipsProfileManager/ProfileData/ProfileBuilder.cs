using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Fpm.ProfileData
{
    public class ProfileBuilder
    {
        private readonly IProfilesReader _reader;
    
        private IProfileRepository _profileRepository;

        public ProfileBuilder(IProfilesReader reader, IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
            _reader = reader;
        }

        public Profile Build(string urlKey, int selectedDomainNumber = 0, int selectedAreaTypeId = AreaTypeIds.Undefined)
        {
            var profile = new Profile();

            ProfileDetails profileDetails = GetProfileDetails(urlKey);
            profile.Name = profileDetails.Name;
            profile.Id = profileDetails.Id;
            profile.ContactUserIds = profileDetails.ContactUserIds.Split(',').ToList();
            profile.IsProfileViewable = profileDetails.IsProfileViewable;

            profileDetails.GroupIds = _reader.GetGroupingIds(profile.Id).ToList();
            profile.GroupingMetadatas = _reader.GetGroupingMetadataList(profileDetails.GroupIds);
            profile.AreIndicatorsToBeListed = profile.GroupingMetadatas.Any();
            profile.AreIndicatorNamesDisplayedWithNumbers = profileDetails.AreIndicatorNamesDisplayedWithNumbers;

            // Groupings
            profile.SelectedDomain = selectedDomainNumber > 0 ? selectedDomainNumber : 1;
            var orderedGroupIds = profile.GroupingMetadatas.Select(x => x.GroupId).ToList();
            var allGroupings = new List<Grouping>();
            List<Grouping> domainGroupings = null;
            int? selectedDomainId = null;
            for (int i = 0; i < orderedGroupIds.Count; i++)
            {
                int groupId = orderedGroupIds[i];
                IList<Grouping> groupings = _reader.GetGroupings(groupId);
                if (i + 1 == profile.SelectedDomain)
                {
                    selectedDomainId = groupId;
                    domainGroupings = groupings.ToList();
                }

                allGroupings.AddRange(groupings);
            }

            // Area type ID
            if (allGroupings.Any())
            {
                // There are groupings so get the available Area types
                var areaTypeIds = allGroupings.Select(x => x.AreaTypeId).Distinct().ToList();
                profile.AreaTypes = _reader.GetSpecificAreaTypes(areaTypeIds);

                selectedAreaTypeId = selectedAreaTypeId > 0 ? selectedAreaTypeId : areaTypeIds.First();
                profile.SelectedAreaType = selectedAreaTypeId;
            }

            // Indicators

            if (profile.SelectedDomain > 0 && domainGroupings != null && profile.GroupingMetadatas.Count > 0)
            {
                // Only include indicators from the selected domain and area type
                IEnumerable<int> indicatorIds;
                if (selectedAreaTypeId > 0)
                {
                    indicatorIds = domainGroupings
                        .Where(x => x.AreaTypeId == selectedAreaTypeId)
                        .OrderBy(v => v.Sequence)
                        .Select(x => x.IndicatorId).Distinct();

                    profile.IndicatorNames = GetIndicatorsForGrid(indicatorIds, selectedDomainId,
                        selectedAreaTypeId, profile.Id, profile.AreIndicatorNamesDisplayedWithNumbers);
                }
                else
                {
                    indicatorIds = domainGroupings
                        .OrderBy(v => v.Sequence)
                        .Select(x => x.IndicatorId)
                        .Distinct();

                    profile.IndicatorNames = GetIndicatorsForGrid(indicatorIds, selectedDomainId,
                        selectedAreaTypeId, profile.Id, profile.AreIndicatorNamesDisplayedWithNumbers);
                }
            }
            else if (profile.GroupingMetadatas.Count == 0)
            {
                // No domains for this profile so use all groupings
                var indicatorIds =
                    allGroupings.Where(x => x.AreaTypeId == selectedAreaTypeId).Select(x => x.IndicatorId).Distinct();

                profile.IndicatorNames = GetIndicatorsForGrid(indicatorIds, selectedDomainId, selectedAreaTypeId,
                    profile.Id, profile.AreIndicatorNamesDisplayedWithNumbers);
            }

            //Handle IndicatorNames null condition if there is not domain present
            return profile;
        }

        private ProfileDetails GetProfileDetails(string urlKey)
        {
            return _reader.GetProfileDetails(urlKey);
        }

        private List<GroupingPlusName> GetIndicatorsForGrid(IEnumerable<int> indicatorIds, 
            int? selectedDomainId, int areaTypeId, int profileId, bool areIndicatorNamesDisplayedWithNumbers)
        {
            var allGroupingPlusNames = new List<GroupingPlusName>();

            foreach (var indicatorId in indicatorIds)
            {
                var groupingPlusNamesForIndicator = _profileRepository.GetGroupingPlusNames(indicatorId,
                    selectedDomainId, areaTypeId, profileId, areIndicatorNamesDisplayedWithNumbers);
                allGroupingPlusNames.AddRange(groupingPlusNamesForIndicator);
            }

            return allGroupingPlusNames;
        }
    }
}
