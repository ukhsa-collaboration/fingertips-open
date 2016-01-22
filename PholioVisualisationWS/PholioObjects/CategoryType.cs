
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NHibernate.Collection.Generic;

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
                    .OrderBy(x => x.CategoryId)
                    .ToList();
            }
        }
    }
}
