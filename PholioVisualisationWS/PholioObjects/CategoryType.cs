
using Newtonsoft.Json;
using NHibernate.Collection.Generic;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.PholioObjects
{
    public class CategoryType
    {
        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Short")]
        public string ShortName { get; set; }

        /// <summary>
        /// Only used for retrieval from database by NHibernate. Use Categories to access data.
        /// </summary>
        [JsonIgnore]
        public PersistentGenericSet<Category> CategoriesFromDatabase { get; set; }

        [JsonProperty]
        public IList<Category> Categories
        {
            get
            {
                return CategoriesFromDatabase
                    .ToList()
                    .OrderBy(x => x.Id)
                    .ToList();
            }
        }

        public static bool IsDeprivationDecile(int categoryTypeId)
        {
            return
                IsCountyAndUADeprivationDecile(categoryTypeId) ||
                IsDistrictAndUADeprivationDecile(categoryTypeId) ||
                IsGpDeprivationDecile(categoryTypeId);
        }

        public static bool IsCountyAndUADeprivationDecile(int categoryTypeId)
        {
            return categoryTypeId == CategoryTypeIds.DeprivationDecileCountyAndUA2010;
        }

        public static bool IsDistrictAndUADeprivationDecile(int categoryTypeId)
        {
            return
                categoryTypeId == CategoryTypeIds.DeprivationDecileDistrictAndUA2010 ||
                categoryTypeId == CategoryTypeIds.DeprivationDecileDistrictAndUA2015;
        }

        public static bool IsGpDeprivationDecile(int categoryTypeId)
        {
            return
                categoryTypeId == CategoryTypeIds.DeprivationDecileGp2010 ||
                categoryTypeId == CategoryTypeIds.DeprivationDecileGp2015;
        }
    }
}
