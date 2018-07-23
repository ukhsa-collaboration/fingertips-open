using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.DataConstruction
{
    public class ChildAreaListBuilder
    {
        private IAreasReader _areasReader;

        public ChildAreaListBuilder(IAreasReader areasReader)
        {
            _areasReader = areasReader;
        }

        public virtual IList<IArea> GetChildAreas(string parentAreaCode, int childAreaTypeId)
        {
            IArea area = AreaFactory.NewArea(_areasReader, parentAreaCode);
            IList<IArea> childAreas;

            var categoryArea = area as CategoryArea;
            if (categoryArea != null)
            {
                childAreas =
                    new ChildAreaListForCategoryAreaBuilder(_areasReader, categoryArea, childAreaTypeId)
                    .ChildAreas
                    .ToList();
            }
            else if (Area.IsNearestNeighbour(parentAreaCode))
            {
                var areaListProvider = new AreaListProvider(_areasReader);
                areaListProvider.CreateAreaListFromNearestNeighbourAreaCode(parentAreaCode);
                childAreas = areaListProvider.Areas;
            }
            else
            {
                childAreas = area.IsCountry
                    ? _areasReader.GetAreasByAreaTypeId(childAreaTypeId)
                    : _areasReader.GetChildAreas(parentAreaCode, childAreaTypeId);
            }

            return childAreas;
        }
    }
}