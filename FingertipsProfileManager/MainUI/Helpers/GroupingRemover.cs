using Fpm.ProfileData;
using Fpm.ProfileData.Repositories;
using System;

namespace Fpm.MainUI.Helpers
{
    public class GroupingRemover
    {
        private readonly IProfilesReader _profilesReader;
        private readonly IProfileRepository _profileRepository;
        private readonly string _userName;

        public GroupingRemover(IProfilesReader profilesReader, IProfileRepository profileRepository)
        {
            _profilesReader = profilesReader;
            _profileRepository = profileRepository;
            _userName = UserDetails.CurrentUser().Name;
        }

        public void RemoveGroupings(int profileId, int groupId, int indicatorId, int areaTypeId, int sexId, int ageId)
        {
            var indicatorMetadata = _profilesReader.GetIndicatorMetadata(indicatorId);

            if (indicatorMetadata.OwnerProfileId == profileId)
            {
                // Indicator owned by profile
                var groupings = CommonUtilities.GetGroupingsForIndicatorInProfile(profileId, indicatorId);

                var distinctGroupingsForProfile = CommonUtilities.GetDistinctGroupingsByGroupIdAndAreaTypeId(groupings);

                if (distinctGroupingsForProfile.Count > 1)
                {
                    // This isn't the last occurence of this indicator in this profile
                    // so it doesn't need to be unassigned and can simply be deleted from the grouping table
                    DeleteFromGrouping(profileId, groupId, indicatorId, areaTypeId, sexId, ageId);
                }
                else
                {
                    //Indicator is owned by the profile so unassign it
                    _profileRepository.UnassignIndicatorFromGrouping(groupId, indicatorId,
                        areaTypeId, sexId, ageId);

                    //Set the indicator ownership to the unassigned profile
                    new IndicatorOwnerChanger(_profilesReader, _profileRepository)
                        .AssignIndicatorToProfile(indicatorId, ProfileIds.UnassignedIndicators);

                    // Log audit message
                    var auditMessage =
                        string.Format(
                            "Indicator {0} (ProfileId: {1}, GroupId: {2}, AreaTypeId: {3}, SexId: {4}, AgeId: {5} has been unassigned.",
                            indicatorId, profileId, groupId, areaTypeId, sexId, ageId);

                    _profileRepository.LogAuditChange(auditMessage, indicatorId, groupId, _userName, DateTime.Now,
                        CommonUtilities.AuditType.Delete.ToString());
                }
            }
            else
            {
                // Indicator not owned by profile
                DeleteFromGrouping(profileId, groupId, indicatorId, areaTypeId, sexId, ageId);
            }
        }

        private void DeleteFromGrouping(int profileId, int groupId, int indicatorId, int areaTypeId, int sexId, int ageId)
        {
            // Profile doesn't own the indicator so actually deleted it from the grouping table 
            _profileRepository.DeleteIndicatorFromGrouping(groupId, indicatorId, areaTypeId, sexId, ageId);

            // Also delete from the IndicatorMetadataTextValue table (where it has an overridden groupId)
            _profileRepository.DeleteOverriddenMetadataTextValues(indicatorId, profileId);

            // Log audit message
            var auditMessage =
                string.Format(
                    "Indicator {0} (ProfileId: {1}, GroupId: {2}, AreaTypeId: {3}, SexId: {4}, AgeId: {5} has been deleted.",
                    indicatorId, profileId, groupId, areaTypeId, sexId, ageId);

            _profileRepository.LogAuditChange(auditMessage, indicatorId, groupId, _userName, DateTime.Now,
                CommonUtilities.AuditType.Delete.ToString());
        }
    }
}