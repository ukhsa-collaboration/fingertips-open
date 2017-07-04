using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataSorting;

namespace PholioVisualisation.DataConstruction
{
    public class ParentChildAreaRelationshipBuilder
    {
        private IgnoredAreasFilter ignoredAreasFilter;
        private AreaListProvider _areaListProvider;

        public ParentChildAreaRelationshipBuilder(IgnoredAreasFilter ignoredAreasFilter,
            AreaListProvider _areaListProvider)
        {
            this.ignoredAreasFilter = ignoredAreasFilter;
            this._areaListProvider = _areaListProvider;
        }

        public ParentAreaWithChildAreas GetParentAreaWithChildAreas(IArea parentArea, 
            int childAreaTypeId, bool retrieveIgnoredAreas)
        {
            _areaListProvider.CreateChildAreaList(parentArea.Code, childAreaTypeId);

            if (retrieveIgnoredAreas == false)
            {
                _areaListProvider.RemoveAreasIgnoredEverywhere(ignoredAreasFilter);
            }
            _areaListProvider.SortByOrderOrName();

            return new ParentAreaWithChildAreas(parentArea,  _areaListProvider.Areas, childAreaTypeId);
        }
    }
}
