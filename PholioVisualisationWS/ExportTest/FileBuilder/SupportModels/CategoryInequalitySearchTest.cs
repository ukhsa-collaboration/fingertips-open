using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Export.FileBuilder.SupportModels;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServicesWebTest.Helpers;

namespace PholioVisualisation.ExportTest.FileBuilder.SupportModels
{
    [TestClass]
    public class CategoryInequalitySearchTest
    {
        [TestMethod]
        public void ShouldGetUndefinedCategoryInequality()
        {
            var categoryInequalitySearchExpected = new CategoryInequalitySearch(CategoryTypeIds.Undefined, CategoryIds.Undefined);
            var categoryInequalitySearchResult = CategoryInequalitySearch.GetUndefinedCategoryInequality();

            Assert.IsNotNull(categoryInequalitySearchResult);
            AssertHelper.IsType(categoryInequalitySearchExpected.GetType(), categoryInequalitySearchResult);

            Assert.AreEqual(categoryInequalitySearchExpected.CategoryTypeId, categoryInequalitySearchResult.CategoryTypeId);
            Assert.AreEqual(categoryInequalitySearchExpected.CategoryId, categoryInequalitySearchResult.CategoryId);
        }
    }
}
