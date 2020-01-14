using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.UserData;
using PholioVisualisation.UserData.Repositories;

namespace PholioVisualisation.DataConstruction
{
    public class ChildAreaCounter
    {
        private readonly IAreasReader _areasReader;

        public ChildAreaCounter(IAreasReader areasReader)
        {
            _areasReader = areasReader;
        }

        public int GetChildAreasCount(IArea parentArea, int childAreaTypeId)
        {
            // Category
            var categoryArea = parentArea as CategoryArea;
            if (categoryArea != null)
            {
                return _areasReader.GetChildAreaCount(categoryArea, childAreaTypeId);
            }

            // Nearest neighbour
            if (Area.IsNearestNeighbour(parentArea.Code))
            {
                var areaListProvider = new AreaListProvider(_areasReader);
                areaListProvider.CreateAreaListFromNearestNeighbourAreaCode(parentArea.Code);
                return areaListProvider.Areas.Count;
            }

            // Area list
            if (Area.IsAreaListAreaCode(parentArea.Code))
            {
                IAreaListRepository areaListRepository = new AreaListRepository(new fingertips_usersEntities());
                var areaList = areaListRepository.GetAreaListByPublicId(parentArea.Code);

                var areaListAreaCodes = areaList.AreaListAreaCodes.Select(x => x.AreaCode);
                var areaListProvider = new AreaListProvider(_areasReader);
                areaListProvider.CreateAreaListFromAreaCodes(areaListAreaCodes);

                return areaListProvider.Areas.Count;
            }

            return parentArea.IsCountry
                    ? _areasReader.GetAreaCountForAreaType(childAreaTypeId) // NOTE ignored areas not considered here
                    : _areasReader.GetChildAreaCount(parentArea.Code, childAreaTypeId);
        }
    }
}