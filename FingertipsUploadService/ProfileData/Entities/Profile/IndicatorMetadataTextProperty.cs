namespace FingertipsUploadService.ProfileData.Entities.Profile
{
    public class IndicatorMetadataTextProperty
    {
        public int PropertyId { get; set; }

        public string ColumnName { get; set; }

        public string DisplayName { get; set; }

        public string Definition { get; set; }

        public bool IsHtml { get; set; }

        public bool IsMandatory { get; set; }

        public bool IsSearchable { get; set; }

        public int DisplayOrder { get; set; }

        public string GetSqlColumnName()
        {
            return PropertyId + "_" + ColumnName;
        }

        public string Text { get; set; }
    }
}
