using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UploadTest
{
    [TestClass]
    public class WhenUsingCategoryValuesValidator
    {
        private CategoryValuesValidator validator;
        private const int rowNumber = 3;
        private const int allowedCategoryTypeId = CategoryTypeIds.EthnicGroups7Categories;

        [TestInitialize]
        public void TestInitialize()
        {
            IList<Category> allowedCategories = new []
            {
                new Category
                {
                    CategoryId = CategoryIds.Mixed,
                    CategoryTypeId = allowedCategoryTypeId
                },
                new Category
                {
                    CategoryId = CategoryIds.Black,
                    CategoryTypeId = allowedCategoryTypeId
                } 
            };

            validator = new CategoryValuesValidator(allowedCategories);
        }

        [TestMethod]
        public void ReturnsNullIfBothValuesNull()
        {
            Assert.IsNull(validator.Validate(rowNumber, null, null));
        }

        [TestMethod]
        public void ReturnsNullIfBothValuesMinus1()
        {
            Assert.IsNull(validator.Validate(rowNumber, CategoryTypeIds.Undefined, CategoryIds.Undefined));
        }

        [TestMethod]
        public void ReturnsNullIfBothValuesAreAllowed()
        {
            Assert.IsNull(validator.Validate(rowNumber, allowedCategoryTypeId, CategoryIds.Black));
        }

        [TestMethod]
        public void ReturnsErrorIfCategoryDefinedButCategoryTypeIsNot()
        {
            Assert.AreEqual(CategoryValuesValidator.InvalidCategoryAndTypeMessage,
                validator.Validate(rowNumber, CategoryTypeIds.Undefined, CategoryIds.Mixed).ErrorMessage);
        }

        [TestMethod]
        public void ReturnsErrorIfCategoryTypeDefinedButCategoryIsNot()
        {
            Assert.AreEqual(CategoryValuesValidator.InvalidCategoryAndTypeMessage,
                validator.Validate(rowNumber, CategoryTypeIds.EthnicGroups7Categories, CategoryIds.Undefined).ErrorMessage);
        }
    }
}
