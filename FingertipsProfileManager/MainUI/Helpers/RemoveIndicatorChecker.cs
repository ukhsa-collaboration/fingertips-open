using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using System.Collections.Generic;
using System.Linq;

namespace Fpm.MainUI.Helpers
{
    /// <summary>
    /// See https://digitaltools.phe.org.uk/confluence/pages/viewpage.action?pageId=576684611
    /// for decision flow
    /// </summary>
    public class RemoveIndicatorChecker : IRemoveIndicatorChecker
    {
        private readonly IProfilesReader _profilesReader;

        private int _indicatorOwnerProfileId;
        private IList<Grouping> _groupings;
        private IList<GroupingMetadata> _groupingMetadataList;

        public RemoveIndicatorChecker(IProfilesReader profilesReader)
        {
            _profilesReader = profilesReader;
        }

        public bool CanIndicatorBeRemoved(int profileId, IndicatorMetadata indicatorMetadata, GroupingPlusName indicator)
        {
            _indicatorOwnerProfileId = indicatorMetadata.OwnerProfileId;

            if (profileId == _indicatorOwnerProfileId)
            {
                InitGroupingLists(indicator);

                if (IsLastAreaTypeGroupingForProfile() && IsIndicatorAssociatedWithOtherProfiles())
                {
                    // Cannot remove if indicator is last area type for owner profile and is used in other profiles
                    return false;
                }
            }

            // Can remove because indicator does not belong to owner profile 
            return true;
        }

        private bool IsIndicatorAssociatedWithOtherProfiles()
        {
            return _groupingMetadataList.Any(x => x.ProfileId != _indicatorOwnerProfileId);
        }

        private bool IsLastAreaTypeGroupingForProfile()
        {
            var ownerProfileGroupingMetadata =
                _groupingMetadataList.Where(x => x.ProfileId == _indicatorOwnerProfileId);

            // Is indicator only in one domain
            if (ownerProfileGroupingMetadata.Count() > 1)
            {
                return false;
            }

            var lastGroupingMetadataId = ownerProfileGroupingMetadata.First().GroupId;
            var ownerProfileGroupings = _groupings.Where(x => x.GroupId == lastGroupingMetadataId);

            // Is last usage of indicator in the profile
            var uniqueAreaTypeCount = ownerProfileGroupings.GroupBy(x => new {x.AreaTypeId}).Count();
            return uniqueAreaTypeCount == 1;
        }

        private void InitGroupingLists(GroupingPlusName indicator)
        {
            // Get the groupings associated to the indicator / sex / age
            _groupings = _profilesReader.GetGroupingByIndicatorIdAndSexIdAndAgeId(indicator.IndicatorId,
                indicator.SexId, indicator.AgeId);

            // Get the relevant grouping metadata
            _groupingMetadataList = GetGroupingMetadataList(_groupings);
        }

        private List<GroupingMetadata> GetGroupingMetadataList(IList<Grouping> groupings)
        {
            var groupIds = new List<int>();
            foreach (var grouping in groupings)
            {
                groupIds.Add(grouping.GroupId);
            }

            // Get the grouping metadata associated to the groupings
            var groupingMetadataList = _profilesReader.GetGroupingMetadataList(groupIds).ToList();
            return groupingMetadataList;
        }
    }
}