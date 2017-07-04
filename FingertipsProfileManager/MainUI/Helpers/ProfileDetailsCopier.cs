using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Repositories;

namespace Fpm.MainUI.Helpers
{
    public class ProfileDetailsCopier
    {
        private ProfileRepository _profileRepository;
        private ProfilesWriter _profilesWriter;

        public ProfileDetailsCopier(ProfileRepository profileRepository, ProfilesWriter profilesWriter)
        {
            _profileRepository = profileRepository;
            _profilesWriter = profilesWriter;
        }

        public int CreateCopy(ProfileDetails sourceProfile)
        {
            int sourceProfileId = sourceProfile.Id;
            int newProfileId = _profileRepository.CreateProfile(sourceProfile);

            CopyGroupingMetadatas(sourceProfileId, newProfileId);

            return newProfileId;
        }

        public void CopyContentItems(int sourceProfileId, int newProfileId)
        {
            var srcContentItems = _profilesWriter.GetProfileContentItems(sourceProfileId);
            foreach (var srcContentItem in srcContentItems)
            {
                _profilesWriter.NewContentItem(newProfileId,
                    srcContentItem.ContentKey,
                    srcContentItem.Description,
                    srcContentItem.IsPlainText,
                    srcContentItem.Content);
            }
        }

        private void CopyGroupingMetadatas(int sourceProfileId, int newProfileId)
        {
            var groupIds = _profilesWriter.GetGroupingIds(sourceProfileId).ToList();
            if (groupIds.Any())
            {
                var groupingMetadataList = _profilesWriter.GetGroupingMetadataList(groupIds);
                foreach (var groupingMetadata in groupingMetadataList)
                {
                    // Copy the metadat
                    var groupingMetadataCopy = _profilesWriter.NewGroupingMetadata(groupingMetadata.GroupName,
                        groupingMetadata.Sequence, newProfileId);

                    // Copy the groupings
                    var groupings = _profilesWriter.GetGroupings(groupingMetadata.GroupId);
                    foreach (var grouping in groupings)
                    {
                        var newGrouping = new Grouping();
                        AutoMapper.Mapper.Map(grouping, newGrouping);
                        newGrouping.GroupId = groupingMetadataCopy.GroupId;
                        _profileRepository.CreateGrouping(newGrouping);
                    }
                }
            }
        }
    }
}