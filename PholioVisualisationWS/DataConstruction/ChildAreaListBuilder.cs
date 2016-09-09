using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.DataConstruction
{
    public class ChildAreaListBuilder
    {
        public ChildAreaListBuilder(IAreasReader areasReader, string parentAreaCode, int childAreaTypeId)
        {
            IArea area = AreaFactory.NewArea(areasReader, parentAreaCode);

            var categoryArea = area as CategoryArea;
            if (categoryArea != null)
            {
                ChildAreas =
                    new ChildAreaListForCategoryAreaBuilder(areasReader, categoryArea, childAreaTypeId)
                    .ChildAreas
                    .ToList();
            }
            else if (Area.IsNearestNeighbour(parentAreaCode))
            {
                // TODO: this should be replaced by CreateAreaListFromNearestNeighbourAreaCode

                var nearestNeighbourArea = new NearestNeighbourArea(parentAreaCode);
                var nearestNeighbours = areasReader.GetNearestNeighbours(nearestNeighbourArea.AreaCodeOfAreaWithNeighbours, nearestNeighbourArea.NeighbourTypeId);
                var nearestNeighboursAreas = nearestNeighbours.Select(x => x.NeighbourAreaCode).ToList();
                var areas = areasReader.GetAreasFromCodes(nearestNeighboursAreas);
                ChildAreas = areas.Cast<IArea>().ToList();
            }
            else
            {
                var areas = area.IsCountry
                    ? areasReader.GetAreasByAreaTypeId(childAreaTypeId)
                    : areasReader.GetChildAreas(parentAreaCode, childAreaTypeId);

                ChildAreas = areas.Cast<IArea>().ToList();
            }
        }

        public List<IArea> ChildAreas { get; set; }
    }
}