using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Repositories;

namespace Fpm.ProfileData
{
    public class ProfileBuilder
    {
        private readonly ProfilesReader _reader = ReaderFactory.GetProfilesReader();
    
        private ProfileRepository _profileRepository;

        public ProfileBuilder(ProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public Profile Build(string urlKey, int selectedDomainNumber = 0, int selectedAreaType = 0)
        {
            var profile = new Profile();

            ProfileDetails profileDetails = GetProfileDetails(urlKey);
            profile.Name = profileDetails.Name;
            profile.Id = profileDetails.Id;
            profile.ContactUserId = profileDetails.ContactUserId;
            profile.IsProfileViewable = profileDetails.IsProfileViewable;

            profileDetails.GroupIds = _reader.GetGroupingIds(profile.Id).ToList();
            profile.GroupingMetadatas = _reader.GetGroupingMetadataList(profileDetails.GroupIds);

            profile.AreIndicatorsToBeListed = profile.GroupingMetadatas.Count == 1 || (selectedDomainNumber > 0);

            // Select default domain if none
            profile.SelectedDomain = selectedDomainNumber <= 0 ? 1 : selectedDomainNumber;

            var orderedGroupIds = profile.GroupingMetadatas.Select(x => x.GroupId).ToList();

            // Groupings
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

            if (allGroupings.Count > 0)
            {
                // There are groupings so get the available Area types
                var areaTypeIds = allGroupings.Select(x => x.AreaTypeId).Distinct().ToList();
                profile.AreaTypes = _reader.GetSpecificAreaTypes(areaTypeIds);

                if (selectedAreaType != 0)
                {
                    selectedAreaType = selectedAreaType != -1 ? selectedAreaType : areaTypeIds.First();
                    profile.SelectedAreaType = selectedAreaType;
                }
            }

            // Indicators
            if (profile.AreIndicatorsToBeListed)
            {
                if (profile.SelectedDomain != 0 && domainGroupings != null)
                {
                    // Only include indicators from the selected domain and area type
                    IEnumerable<int> indicatorIds;
                    if (selectedAreaType==0)
                    {
                        indicatorIds = domainGroupings
                            .OrderBy(v => v.Sequence)
                            .Select(x => x.IndicatorId)
                            .Distinct();

                        profile.IndicatorNames = GetIndicatorsForGrid(indicatorIds, selectedDomainId, selectedAreaType, profile.Id);
                    }
                    else
                    {
                        indicatorIds =
                            domainGroupings.Where(x => x.AreaTypeId == selectedAreaType).OrderBy(v => v.Sequence).Select(x => x.IndicatorId).Distinct();
                        profile.IndicatorNames = GetIndicatorsForGrid(indicatorIds, selectedDomainId, selectedAreaType, profile.Id);
                    }
                }
                else if (profile.GroupingMetadatas.Count == 0)
                {
                    // No domains for this profile so use all groupings
                    var indicatorIds =
                        allGroupings.Where(x => x.AreaTypeId == selectedAreaType).Select(x => x.IndicatorId).Distinct();

                    profile.IndicatorNames = GetIndicatorsForGrid(indicatorIds, selectedDomainId, selectedAreaType, profile.Id);
                }
            }

            return profile;
        }

        private ProfileDetails GetProfileDetails(string urlKey)
        {
            return _reader.GetProfileDetails(urlKey);
        }

        private List<GroupingPlusName> GetIndicatorsForGrid(IEnumerable<int> indicatorIds, 
            int? selectedDomainId, int areaTypeId, int profileId)
        {
            var allGroupingPlusNames = new List<GroupingPlusName>();

            foreach (var indicatorId in indicatorIds)
            {
                var groupingPlusNamesForIndicator = _profileRepository.GetGroupingPlusNames(indicatorId,
                    selectedDomainId, areaTypeId, profileId);
                allGroupingPlusNames.AddRange(groupingPlusNamesForIndicator);
            }

            return allGroupingPlusNames;
        }
    }
}
