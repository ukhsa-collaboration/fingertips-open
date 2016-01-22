using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class AreaCodeToCategoryIdMapBuilder
    {
        public int CategoryTypeId { get; set; }
        public int ChildAreaTypeId { get; set; }

        /// <summary>
        /// Key is child area code, Value is category ID
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> Build()
        {
            IAreasReader areasReader = ReaderFactory.GetAreasReader();
            IList<CategorisedArea> categorisedAreas = areasReader.
                GetCategorisedAreasForAllCategories(AreaTypeIds.Country, ChildAreaTypeId, CategoryTypeId);
            return categorisedAreas.ToDictionary(x => x.AreaCode, x => x.CategoryId);
        }
    }
}