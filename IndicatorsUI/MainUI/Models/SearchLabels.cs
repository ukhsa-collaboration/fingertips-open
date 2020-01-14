using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUI.Models
{
    public class SearchLabels
    {
        public string Heading { get; set; }
        public string Watermark { get; set; }

        private SearchLabels() { }

        public static SearchLabels New(int areaTypeId)
        {
            if (areaTypeId == AreaTypeIds.CountyAndUnitaryAuthorityPre2019)
            {
                return new SearchLabels
                {
                    Heading = "See how your local authority compares",
                    Watermark = "enter postcode, town or local authority"
                };
            }

            // CCG
            return new SearchLabels
            {
                Heading = "See how your GP practice or CCG compares",
                Watermark = "enter postcode, town or CCG"
            };
        }
    }
}