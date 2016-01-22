using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.DataAccess;

namespace PholioVisualisation.DataConstruction
{
    public class ParentChildAreaRelationshipBuilder
    {
        private IgnoredAreasFilter ignoredAreasFilter;
        private AreaListBuilder areaListBuilder;

        public ParentChildAreaRelationshipBuilder(IgnoredAreasFilter ignoredAreasFilter,
            AreaListBuilder areaListBuilder)
        {
            this.ignoredAreasFilter = ignoredAreasFilter;
            this.areaListBuilder = areaListBuilder;
        }

        public ParentAreaWithChildAreas GetParentAreaWithChildAreas(IArea parentArea, 
            int childAreaTypeId, bool retrieveIgnoredAreas)
        {
            areaListBuilder.CreateChildAreaList(parentArea.Code, childAreaTypeId);

            if (retrieveIgnoredAreas == false)
            {
                areaListBuilder.RemoveAreasIgnoredEverywhere(ignoredAreasFilter);
            }
            areaListBuilder.SortByOrderOrName();

            return new ParentAreaWithChildAreas(parentArea,  areaListBuilder.Areas, childAreaTypeId);
        }
    }
}
