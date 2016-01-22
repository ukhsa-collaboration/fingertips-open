using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class ChildAreaListForCategoryAreaBuilder
    {
        public IList<IArea> ChildAreas { get; set; }

        public ChildAreaListForCategoryAreaBuilder(IAreasReader reader, CategoryArea parentArea, int childAreaType)
        {
            var areaCodes = reader
                .GetCategorisedAreasForOneCategory(AreaTypeIds.Country, childAreaType, parentArea.CategoryTypeId, parentArea.CategoryId)
                .Select(x => x.AreaCode);
            var areaListBuilder = new AreaListBuilder(reader);
            areaListBuilder.CreateAreaListFromAreaCodes(areaCodes);
            ChildAreas = areaListBuilder.Areas.OrderBy(x=> x.Name).ToList();
        }
    }
}