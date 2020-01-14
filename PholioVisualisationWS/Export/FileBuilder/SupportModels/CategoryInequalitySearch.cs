using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export.FileBuilder.SupportModels
{
    public class CategoryInequalitySearch
    {
        public int CategoryTypeId { get; set; }
        public int CategoryId { get; set; }

        public CategoryInequalitySearch(int categoryTypeId, int categoryId)
        {
            CategoryTypeId = categoryTypeId;
            CategoryId = categoryId;
        }

        public CategoryInequalitySearch()
        {
        }

        public static CategoryInequalitySearch GetUndefinedCategoryInequality()
        {
            return new CategoryInequalitySearch(CategoryTypeIds.Undefined, CategoryIds.Undefined);
        }

        public string ConvertIntoCategoryAreaCode()
        {
            return "cat-" + CategoryTypeId + "-" + CategoryId;
        }
    }
}
