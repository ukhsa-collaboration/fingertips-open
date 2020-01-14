using System;
using System.Collections.Generic;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.ProfileData.Repositories
{
    public interface IProfileRepository
    {
        void RefreshObject(object o);
        IList<ProfileDetails> GetProfiles();
        int CreateProfile(ProfileDetails profileDetails);
        void UpdateProfile(ProfileDetails profileDetails);
        void UpdateProfileDetail(ProfileDetails profileDetails);
        void CreateIndicator(IEnumerable<IndicatorMetadataTextProperty> allPropertiesToAdd, int indicatorId);
        void CreateNewOverriddenIndicator(IndicatorMetadataTextProperty property, string text, int indicatorId, int? profileId);
        bool DoesOverriddenIndicatorMetadataRecordAlreadyExist(int indicatorId, int? profileId);
        IndicatorMetadata GetIndicatorMetadata(int indicatorId);
        int GetNextIndicatorId();
        int CreateGrouping(Grouping grouping);
        int CreateProfileCollection(ProfileCollection profileCollection, string assignedProfiles);
        bool UpdateProfileCollection(int profileCollectionId, string assignedProfilesToUpdate, string collectionNameToUpdate, string collectionSkinTitleToUpdate);

        bool ChangeOwner(int indicatorId, int newOwnerProfileId,
            IList<IndicatorText> newOwnerMetadataTextValues,
            IList<IndicatorText> currentOwnerMetadataTextValues);

        void UpdateProperty(IndicatorMetadataTextProperty property, string text, int indicatorId, int? profileId);

        int GetNextAvailableIndicatorSequenceNumber(int domainId);

        bool LogIndicatorMetadataTextPropertyChange(int propertyId, string oldText,
            int indicatorId, int? profileId, string userName, DateTime timestamp);

        bool LogAuditChange(string auditMessage, int indicatorId, int? groupId, string userName,
            DateTime timestamp, string auditType);

        bool DeleteChangeAudit(int indicatorId);

        void MoveIndicatorToDomain(int indicatorId, int fromGroupId, int fromAreaTypeId, int fromSexId,
            int fromAgeId, int toGroupId, int toAreaType, int toSexId, int toAgeId);

        void MoveIndicators(int indicatorId, int fromGroupId, int toGroupId, int areaTypeId, int sexId, int ageId, string status);

        bool IndicatorGroupingsExist(int indicatorId, int domainId, int areaTypeId, int ageId, int sexId);

        void CopyIndicatorToDomain(int indicatorId, int fromGroupId, int fromAreaTypeId, int fromSexId,
            int fromAgeId, int toGroupId, int toAreaType, int toSexId, int toAgeId);

        void DeleteIndicatorFromGrouping(int? groupId, int? indicatorId, int areaTypeId, int sexId, int ageId);
        void DeleteOverriddenMetadataTextValues(int? indicatorId, int profileId);
        void UnassignIndicatorFromGrouping(int groupId, int? indicatorId, int areaTypeId, int sexId, int ageId);

        string GetDomainName(int groupId, int domainSequence);
        IList<GroupingSubheading> GetGroupingSubheadingsByGroupIds(IList<int> groupIds);
        IList<GroupingSubheading> GetGroupingSubheadings(int areaTypeId, int groupId);

        IList<GroupingPlusName> GetGroupingPlusNames(int indicatorId, int? selectedDomainId,
            int areaTypeId, int profileId, bool areIndicatorNamesDisplayedWithNumbers);

        void SaveGroupingSubheading(GroupingSubheading groupingSubheading);
        void UpdateGroupingSubheading(GroupingSubheading groupingSubheading);
        void DeleteGroupingSubheading(int subheadingId);
        ProfileDetails GetProfileDetailsById(int Id);

        void LogIndicatorMetadataReviewAudit(IndicatorMetadataReviewAudit indicatorMetadataReviewAudit);
        void ChangeIndicatorProfile(int indicatorId, int profileId, string status);
        void DeleteIndicator(int indicatorId);

        bool CreateGroupingAndMetadata(IndicatorMetadata indicatorMetadata,
            IndicatorMetadataTextValue indicatorMetadataTextValue, Grouping grouping, string user);

        bool UpdateGroupingAndMetadata(IndicatorMetadata indicatorMetadata,
            IndicatorMetadataTextValue indicatorMetadataTextValue, Grouping grouping, int oldAreaTypeId, int oldSexId,
            int oldAgeId, string user);
    }
}