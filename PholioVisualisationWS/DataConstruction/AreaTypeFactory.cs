using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class AreaTypeFactory
    {
        public static IAreaType New(IAreasReader areasReader, ParentAreaGroup parentAreaGroup)
        {
            if (parentAreaGroup.ParentAreaTypeId.HasValue)
            {
                return areasReader.GetAreaType(parentAreaGroup.ParentAreaTypeId.Value);
            }

            if (parentAreaGroup.CategoryTypeId.HasValue)
            {
                var categoryType = areasReader.GetCategoryType(parentAreaGroup.CategoryTypeId.Value);
                return CategoryAreaType.New(categoryType);
            }

            throw new FingertipsException("Could not create an area type for Id=" + parentAreaGroup.Id);
        }

        public static IAreaType New(IAreasReader areasReader, int parentAreaTypeId)
        {

            if (CategoryAreaType.IsCategoryAreaTypeId(parentAreaTypeId))
            {
                var categoryTypeId = CategoryAreaType.GetCategoryTypeIdFromAreaTypeId(parentAreaTypeId);
                var categoryType = areasReader.GetCategoryType(categoryTypeId);
                return CategoryAreaType.New(categoryType);
            }

            if (AreaListAreaType.IsAreaListAreaType(parentAreaTypeId))
            {
                return new AreaListAreaType();
            }

            return areasReader.GetAreaType(parentAreaTypeId);
        }
    }
}