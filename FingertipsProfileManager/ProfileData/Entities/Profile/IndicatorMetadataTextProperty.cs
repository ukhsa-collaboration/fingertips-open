using System;
using System.Collections.Generic;

namespace Fpm.ProfileData.Entities.Profile
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

        public bool IsOverridable { get; set; }

        public string GetSqlColumnName()
        {
            return PropertyId + "_" + ColumnName;
        }

        public string Text { get; set; }

        public bool IsInternalMetadata { get; set; }

        public static IList<IndicatorMetadataTextProperty> GetIndicatorMetadataTextPropertyInternalMetadata()
        {
            var internalMetadataPropertiesList = new List<IndicatorMetadataTextProperty>();

            for (int i = 28; i <= 36; i++)
            {
                var internalMetadataProperty = new IndicatorMetadataTextProperty
                {
                    PropertyId = i,
                    ColumnName = GetColumnName(i),
                    Text = "Test"
                };
                internalMetadataPropertiesList.Add(internalMetadataProperty);
            }
            return internalMetadataPropertiesList;
        }

        private static string GetColumnName(int propertyId)
        {
            switch (propertyId)
            {
                case 28:
                    return IndicatorTextMetadataColumnNames.Keywords;
                case 29:
                    return IndicatorTextMetadataColumnNames.EvidOfVariability;
                case 30:
                    return IndicatorTextMetadataColumnNames.JustifConfIntMeth;
                case 31:
                    return IndicatorTextMetadataColumnNames.QualityAssurance;
                case 32:
                    return IndicatorTextMetadataColumnNames.QualityImprPlan;
                case 33:
                    return IndicatorTextMetadataColumnNames.JustiOfExclusions;
                case 34:
                    return IndicatorTextMetadataColumnNames.JustifOfDataSources;
                case 35:
                    return IndicatorTextMetadataColumnNames.SponsorStakeholders;
                case 36:
                    return IndicatorTextMetadataColumnNames.IndOwnerContDet;
                case 37:
                    return IndicatorTextMetadataColumnNames.Comments;
            }
            throw new Exception("Property Id invalid");
        }
    }
}