using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class AreaFactory
    {
        public static IArea NewArea(IAreasReader areasReader, string areaCode)
        {
            if (areaCode == null)
            {
                throw new FingertipsException("Area code was null");
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