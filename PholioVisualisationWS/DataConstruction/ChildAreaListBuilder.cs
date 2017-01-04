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

        public virtual List<IArea> GetChildAreas(string parentAreaCode, int childAreaTypeId)
        {
            IArea area = AreaFactory.NewArea(_areasReader, parentAreaCode);
            List<IArea> childAreas;

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
                // TODO: this should be replaced by CreateAreaListFromNearestNeighbourAreaCode
                var nearestNeighbourArea = new NearestNeighbourArea(parentAreaCode);
                var nearestNeighbours = _areasReader.GetNearestNeighbours(nearestNeighbourArea.AreaCodeOfAreaWithNeighbours,
                    nearestNeighbourArea.NeighbourTypeId);
                var nearestNeighboursAreas = nearestNeighbours.Select(x => x.NeighbourAreaCode).ToList();
                var areas = _areasReader.GetAreasFromCodes(nearestNeighboursAreas);
                childAreas = areas.Cast<IArea>().ToList();
            }
            else
            {
                var areas = area.IsCountry
                    ? _areasReader.GetAreasByAreaTypeId(childAreaTypeId)
                    : _areasReader.GetChildAreas(parentAreaCode, childAreaTypeId);

                childAreas = areas.Cast<IArea>().ToList();
            }

            return childAreas;
        }
    }
}