using System.Collections.Generic;
using System.Linq;
using Fpm.MainUI.Models;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Entities.User;

namespace Fpm.MainUI.ViewModels.Indicator
{
    public class IndicatorViewModel : BaseDataModel
    {
        public IndicatorMetadataTextValue IndicatorMetadataTextValue { get; set; }
        public List<IndicatorMetadataTextProperty> IndicatorMetadataTextProperties { get; set; }
        public IndicatorMetadataReviewAudit IndicatorMetadataReviewAudit { get; set; }
        public List<IndicatorMetadataReviewAudit> IndicatorMetadataReviewAudits { get; set; }
        public Grouping Grouping { get; set; }
        public int DomainSequence { get; set; }
        public List<string> PartitionAgeIds { get; set; }
        public List<string> PartitionSexIds { get; set; }
        public List<string> PartitionAreaTypeIds { get; set; }
        public bool IsEditAction { get; set; }
        public string CopyToProfileUrlKey { get; set; }
        public int CopyToDomainId { get; set; }
        public bool IsCopyAction { get; set; }
        public List<FpmUser> FpmUsers { get; set; }
        public string DestinationProfileUrlKey { get; set; }
        public string DestinationProfileName { get; set; }
        public int AreaTypeId { get; set; }
        public int SexId { get; set; }
        public int AgeId { get; set; }

        public IndicatorMetadataTextProperty GetProperty(string columnName)
        {
            return IndicatorMetadataTextProperties.First(x => x.ColumnName == columnName);
        }
    }
}