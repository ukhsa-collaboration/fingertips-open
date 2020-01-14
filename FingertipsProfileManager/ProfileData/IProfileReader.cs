using Fpm.ProfileData.Entities.Core;
using Fpm.ProfileData.Entities.LookUps;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Entities.User;
using System.Collections;
using System.Collections.Generic;

namespace Fpm.ProfileData
{
    public interface IProfilesReader
    {
        IList<ProfileDetails> GetProfiles();
        IEnumerable<ProfileCollection> GetProfileCollections();
        ProfileCollection GetProfileCollection(int collectionId);
        int GetProfileIdFromUrlKey(string urlKey);
        string GetProfileUrlKeyFromId(int profileId);
        ProfileDetails GetProfileDetails(string urlKey);
        ProfileDetails GetProfileDetailsByProfileId(int profileId);
        ProfileDetails GetOwnerProfilesByIndicatorIds(int indicatorId);
        IList<UserGroupPermissions> GetUserGroupPermissionsByUserId(int userId);
        IList<UserGroupPermissions> GetUserGroupPermissionsByProfileId(int profileId);
        IList<ProfileDetails> GetProfilesEditableByUser(int userId);
        IList<Grouping> GetGroupings(int groupId);
        IList<int> GetGroupingIndicatorIds(IList<int> groupIds);
        IList<int> GetGroupingIds(int profileId);
        IList<Grouping> GetGroupingsByGroupIdAndAreaTypeId(int groupId, int areaTypeId);

        IList<Grouping> GetGroupingsByGroupIdAreaTypeIdIndicatorIdAndSexIdAndAgeId(int groupId, int areaTypeId,
            int indicatorId, int sexId, int ageId);

        IList<Grouping> GetGroupings(IList<int> groupIds, int areaTypeId, int indicatorId, int sexId, int ageId,
            int yearRange);

        IList<GroupingMetadata> GetGroupingMetadataList(IList<int> groupIds);

        IList<int> GetGroupingMetadataGroupIdListByProfileId(int profileId);
        GroupingMetadata GetGroupingMetadata(int groupId);
        IList<Grouping> GetGroupingByIndicatorId(List<int> indicatorIds);
        IList<Grouping> GetGroupingByIndicatorIdAndSexIdAndAgeId(int indicatorId, int sexId, int ageId);
        IList<IndicatorMetadataTextProperty> GetIndicatorMetadataTextProperties();
        IList<IndicatorMetadataTextValue> SearchIndicatorMetadataTextValuesByText(string searchText);
        IList<IndicatorMetadataTextValue> SearchIndicatorMetadataTextValuesByIndicatorId(int indicatorId);
        IList<IndicatorMetadataTextValue> GetIndicatorMetadataTextValuesByProfileId(int profileId);

        IList<IndicatorMetadataTextValue> GetIndicatorMetadataTextValuesByIndicatorIdsAndProfileId(
            IList<int> indicatorIds, int profileId);

        IList GetIndicatorDefaultTextValues(int indicatorId);

        IList<IndicatorText> GetIndicatorTextValues(int indicatorId, IList<IndicatorMetadataTextProperty> properties,
            int? profileId);

        IList<IndicatorText> GetOverriddenIndicatorTextValuesForSpecificProfileId(int indicatorId,
            IList<IndicatorMetadataTextProperty> properties, int? profileId);

        IndicatorMetadata GetIndicatorMetadata(int indicatorId);
        IList<IndicatorMetadata> GetIndicatorMetadata(List<int> indicatorIds);
        IEnumerable<Grouping> DoesIndicatorExistInMoreThanOneGroup(int indicatorId, int ageId, int sexId);
        IEnumerable<Area> GetAreas(string searchText, int? areaTypeId);
        IList<ComparatorMethod> GetAllComparatorMethods();
        IList<Polarity> GetAllPolarities();
        IList<AreaType> GetAllAreaTypes();
        IList<AreaType> GetSpecificAreaTypes(List<int> areaTypeIds);
        IList<AreaType> GetSupportedAreaTypes();
        IEnumerable<IndicatorAudit> GetIndicatorAudit(List<int> indicatorIds);
        IEnumerable<ContentAudit> GetContentAuditList(List<int> contentIds);
        ContentAudit GetContentAuditFromId(int contentAuditId);
        IList<CoreDataSet> GetCoreDataForIndicatorIds(List<int> indicatorId);
        IList<CoreDataSetArchive> GetCoreDataArchiveForIndicatorIds(List<int> indicatorId);
        IList<TimePeriod> GetCoreDataSetTimePeriods(GroupingPlusName groupingPlusNames);
        IList<ContentItem> GetProfileContentItems(int profileId);
        ContentItem GetContentItem(int contentId);
        IList<ContentItem> GetContentItems(IList<int> contentIds);
        ContentItem GetContentItem(string contentKey, int profileId);
        IEnumerable<UserGroupPermissions> GetUserPermissions();
        FpmUser GetUserByUserName(string userName);
        FpmUser GetUserByUserId(int userId);
        IList<FpmUser> GetUserListByUserIds(List<string> userIds);
        IList<UserGroupPermissions> GetUserGroupPermissionsByUserAndProfile(int userId, int profileId);
        IList<ProfileCollectionItem> GetProfileCollectionItems(int profileCollectionId);
        IndicatorMetadataTextValue GetMetadataTextValueForAnIndicatorById(int indicatorId, int profileId);
        IList<Document> GetDocumentsWithoutFileData(int profileId);
        Document GetDocumentWithoutFileData(int id);
        Document GetDocumentWithFileData(int id);
        IList<Document> GetDocumentsWithoutFileData(string fileName);
        IEnumerable<SpineChartMinMaxLabel> GetSpineChartMinMaxLabelOptions();
        IList<AreaType> GetAreaTypes(int profileId);
        IList<AreaType> GetAreaTypesWhichContainsPdf(int profileId);
        IList<UserGroupPermissions> GetProfileUsers(int profileId);
        IList<IndicatorMetadataReviewAudit> GetIndicatorMetadataReviewAudits(int indicatorId);
        IList<Sex> GetAllSexes();
        IList<Age> GetAllAges();
        IList<TargetConfig> GetAllTargets();
        IList<ValueNote> GetAllValueNotes();
        IList<Category> GetCategoriesByCategoryTypeId(int categoryTypeId);
        TargetConfig GetTargetById(int targetId);
    }
}
