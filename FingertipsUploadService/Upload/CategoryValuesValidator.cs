using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Profile;
using System.Collections.Generic;
using System.Linq;

namespace Fpm.Upload
{
    public class CategoryValuesValidator
    {
        public const string InvalidCategoryAndTypeMessage = "Invalid combination of CategoryTypeId and CategoryId";

        private IList<Category> _allowedCategories;

        public CategoryValuesValidator(IList<Category> allowedCategories)
        {
            _allowedCategories = allowedCategories;
        }

        public UploadValidationFailure Validate(int rowNumber, int? categoryTypeId, int? categoryId)
        {
            if (categoryTypeId.HasValue == false && categoryId.HasValue == false)
            {
                // Allowed
            }
            else if (categoryTypeId == CategoryTypeIds.Undefined && categoryId == CategoryIds.Undefined)
            {
                // Allowed
            }
            else
            {
                var category = _allowedCategories.FirstOrDefault(x =>
                    (x.CategoryId == categoryId && x.CategoryTypeId == categoryTypeId));

                if (category == null)
                {
                    return new UploadValidationFailure(rowNumber, "Category", InvalidCategoryAndTypeMessage, null);
                }
            }

            return null;
        }
    }
}
