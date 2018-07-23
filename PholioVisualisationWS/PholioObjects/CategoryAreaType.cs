using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class CategoryAreaType : IAreaType
    {
        public const int IdAddition = 10000;

        private CategoryAreaType() { }

        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Short")]
        public string ShortName { get; set; }

        public static CategoryAreaType New(CategoryType categoryType)
        {
            // Need to create a spoof area type id that will never be a genuine area type ID
            int spoofAreaTypeID = categoryType.Id + IdAddition;

            return new CategoryAreaType
            {
                Id = spoofAreaTypeID,
                Name = categoryType.ShortName,
                ShortName = categoryType.ShortName
            };
        }

        [JsonIgnore]
        public int CategoryTypeId
        {
            get
            {
                return GetCategoryTypeIdFromAreaTypeId(Id);
            }
        }

        public static int GetCategoryTypeIdFromAreaTypeId(int areaTypeId)
        {
            return areaTypeId - IdAddition;
        }

        public static int GetAreaTypeIdFromCategoryTypeId(int categoryTypeId)
        {
            return categoryTypeId + IdAddition;
        }

        public static bool IsCategoryAreaTypeId(int areaTypeId)
        {
            return areaTypeId > IdAddition && areaTypeId < NearestNeighbourAreaType.IdAddition;
        }
    }
}