using System;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public interface ICategoryArea : IArea
    {
        int CategoryTypeId { get; }
        int CategoryId { get; }
        string ParentAreaCode { get; }
        int ParentAreaTypeId { get; }
    }

    public class CategoryArea : ICategoryArea
    {
        /// <summary>
        /// For Mocking
        /// </summary>
        public CategoryArea()
        {
        }

        public CategoryArea(string areaCode)
        {
            string[] areaCodeCharArray = areaCode.Split('-');
            Code = areaCode;
            CategoryTypeId = int.Parse(areaCodeCharArray[1]);
            CategoryId = int.Parse(areaCodeCharArray[2]);
        }

        public static CategoryArea New(int categoryTypeId, int categoryId)
        {
            return new CategoryArea
            {
                Code = CreateAreaCode(categoryTypeId, categoryId),
                CategoryTypeId = categoryTypeId,
                CategoryId = categoryId
            };
        }

        public static CategoryArea New(Category category)
        {
            var categoryArea = New(category.CategoryTypeId, category.Id);
            categoryArea.Name = category.Name;
            categoryArea.ShortName = category.ShortName;
            return categoryArea;
        }

        public void SetNames(Category category)
        {
            Name = category.Name;
            ShortName = category.ShortName;
        }

        public static string CreateAreaCode(int categoryTypeId, int categoryId)
        {
            return "cat-" + categoryTypeId + "-" + categoryId;
        }

        [JsonIgnore]
        public virtual int CategoryTypeId { get; internal set; }

        [JsonIgnore]
        public virtual int CategoryId { get; internal set; }

        [JsonProperty]
        public virtual string Code { get; internal set; }

        [JsonIgnore]
        public virtual string ParentAreaCode
        {
            get { return AreaCodes.England; }
        }

        [JsonIgnore]
        public virtual int ParentAreaTypeId
        {
            get { return AreaTypeIds.Country; }
        }

        [JsonIgnore]
        public virtual int? Sequence {
            get { return 0; }
            set { ; }
        }

        [JsonProperty(PropertyName = "Short")]
        public string ShortName { get; set; }

        [JsonProperty]
        public virtual string Name { get; set; }

        [JsonIgnore]
        public virtual int AreaTypeId
        {
            get { return CategoryAreaType.GetAreaTypeIdFromCategoryTypeId(CategoryTypeId); }
        }

        /// <summary>
        /// Whether or not the area is a CCG.
        /// </summary>
        [JsonIgnore]
        public bool IsCcg
        {
            get { return false; }
        }

        /// <summary>
        /// Whether or not the area is a practice shape.
        /// </summary>
        [JsonIgnore]
        public bool IsShape
        {
            get { return false; }
        }

        [JsonIgnore]
        public bool IsCountry
        {
            get { return false; }
        }

        /// <summary>
        /// Whether or not the area is a deprivation decile.
        /// </summary>
        [JsonIgnore]
        public bool IsGpDeprivationDecile
        {
            get
            {
                return CategoryType.IsGpDeprivationDecile(CategoryTypeId);
            }
        }

        [JsonIgnore]
        public bool IsCountyAndUADeprivationDecile
        {
            get
            {
                return CategoryType.IsCountyAndUADeprivationDecile(CategoryTypeId);
            }
        }

        [JsonIgnore]
        public bool IsDistrictAndUADeprivationDecile
        {
            get
            {
                return CategoryType.IsDistrictAndUADeprivationDecile(CategoryTypeId);
            }
        }

        [JsonIgnore]
        public bool IsGpPractice
        {
            get { return false; }
        }

        public bool IsOnsClusterGroup
        {
            get { return false; }
        }

        public static bool IsCategoryAreaCode(string areaCode)
        {
            return areaCode.StartsWith("cat-", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}