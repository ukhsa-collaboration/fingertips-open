using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.UserData;
using PholioVisualisation.UserData.Repositories;

namespace PholioVisualisation.DataConstruction
{
    public class AreaFactory
    {
        private IAreasReader _areasReader;

        public AreaFactory(IAreasReader areasReader)
        {
            _areasReader = areasReader;
        }

        public IArea NewArea(string areaCode)
        {
            return NewArea(_areasReader, areaCode);
        }

        public static IArea NewArea(IAreasReader areasReader, string areaCode)
        {
            if (areaCode == null)
            {
                throw new FingertipsException("Area code was null");
            }

            // AreaList areas
            if (Area.IsAreaListAreaCode(areaCode))
            {
                IAreaListRepository areaListRepository = new AreaListRepository(new fingertips_usersEntities());
                var areaList = areaListRepository.GetAreaListByPublicId(areaCode);
                var areaListArea = new Area
                {
                    Name = areaList.ListName,
                    ShortName = areaList.ListName,
                    Code = areaList.PublicId,
                    AreaTypeId = areaList.AreaTypeId
                };

                return areaListArea;
            }

            // Category areas
            if (CategoryArea.IsCategoryAreaCode(areaCode))
            {
                var categoryArea = new CategoryArea(areaCode);
                var category = areasReader.GetCategory(categoryArea.CategoryTypeId, categoryArea.CategoryId);
                categoryArea.SetNames(category);
                return categoryArea;
            }

            // Nearest neighbour
            if (NearestNeighbourArea.IsNearestNeighbourAreaCode(areaCode))
            {
                var nearestNeighboursArea = new NearestNeighbourArea(areaCode);
                nearestNeighboursArea.Neighbours = areasReader.GetNearestNeighbours(
                    nearestNeighboursArea.AreaCodeOfAreaWithNeighbours, nearestNeighboursArea.NeighbourTypeId);
                return nearestNeighboursArea;
            }

            // Standard area
            return areasReader.GetAreaFromCode(areaCode);
        }
    }
}