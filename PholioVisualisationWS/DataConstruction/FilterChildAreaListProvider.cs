using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public interface IFilteredChildAreaListProvider
    {
        IList<IArea> ReadChildAreas(string parentAreaCode, int profileId, int childAreaTypeId);
    }

    public class FilteredChildAreaListProvider : IFilteredChildAreaListProvider
    {
        private readonly IAreasReader _areasReader;

        public FilteredChildAreaListProvider(IAreasReader areasReader)
        {
            _areasReader = areasReader;
        }

        public IList<IArea> ReadChildAreas(string parentAreaCode, int profileId, int childAreaTypeId)
        {
            IList<IArea> childAreas;
            if (NearestNeighbourArea.IsNearestNeighbourAreaCode(parentAreaCode))
            {
                var nearestNeighbourArea = (NearestNeighbourArea)AreaFactory.NewArea(_areasReader, parentAreaCode);
                childAreas = new NearestNeighbourAreaListBuilder(_areasReader, nearestNeighbourArea).Areas;
            }
            else
            {
                childAreas = new ChildAreaListBuilder(_areasReader, parentAreaCode, childAreaTypeId)
                    .ChildAreas;
            }

            IgnoredAreasFilter filter = IgnoredAreasFilterFactory.New(profileId);
            return filter.RemoveAreasIgnoredEverywhere(childAreas).ToList();
        }
    }
}