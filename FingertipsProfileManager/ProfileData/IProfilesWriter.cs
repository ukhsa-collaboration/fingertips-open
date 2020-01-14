using Fpm.ProfileData.Entities.LookUps;
using Fpm.ProfileData.Entities.Profile;
using System.Collections.Generic;

namespace Fpm.ProfileData
{
    public interface IProfilesWriter : IProfilesReader
    {
        void ReorderIndicatorSequence(int groupId, int areaTypeId);
        void UpdateTargetConfig(TargetConfig target);
        TargetConfig NewTargetConfig(TargetConfig target);
        void UpdateGroupingMetadata(GroupingMetadata groupingMetadata);
        void UpdateGroupingList(IList<Grouping> groupings);
        GroupingMetadata NewGroupingMetadata(string name, int sequence, int profileId);
        void DeleteOverriddenIndicatorMetadataTextValue(IndicatorMetadataTextValue indicatorMetadataTextValue);
        int NewIndicatorMetadataTextValue(IndicatorMetadataTextValue indicatorMetadataTextValue);
        void DeleteTargetConfig(TargetConfig target);
        void NewTargetConfigAudit(TargetConfigAudit targetAudit);

        ContentItem NewContentItem(int profileId, string contentKey, string description, bool isPlainTextContent,
            string content);

        ContentAudit NewContentAudit(ContentAudit contentAudit);
        void UpdateContentItem(ContentItem contentItem);
        void DeleteContentItem(string contentKey, int profileId);
        int NewDocument(Document doc);
        void UpdateDocument(Document doc);
        void DeleteDocument(Document doc);
        void DeleteGroupingMetadata(int groupId);
        void ReorderDomainSequence(IList<int> groupIds);
        void DeleteIndicatorMetatdataById(IndicatorMetadata indicatorMetadata);
        void UpdateCategoryType(CategoryType categoryType);
    }
}
