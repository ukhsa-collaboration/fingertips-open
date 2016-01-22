using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioObjectsTest
{
    [TestClass]
    public class CategoryAreaTest
    {
        [TestMethod]
        public void TestNew_FromIds()
        {
            var cateogryArea = CategoryArea.New(10, 22);
            Assert.AreEqual(10, cateogryArea.CategoryTypeId);
            Assert.AreEqual(22, cateogryArea.CategoryId);
        }

        [TestMethod]
        public void TestNew_FromCategory()
        {
            var categoryArea = CategoryArea.New(new Category
            {
                CategoryId = 3,
                CategoryTypeId = 2,
                ShortName = "short",
                Name = "name"
            });

            Assert.AreEqual("cat-2-3", categoryArea.Code);
            Assert.AreEqual(3, categoryArea.CategoryId);
            Assert.AreEqual(2, categoryArea.CategoryTypeId);
            Assert.AreEqual("short", categoryArea.ShortName);
            Assert.AreEqual("name", categoryArea.Name);
        }

        [TestMethod]
        public void TestNew_CodeIsCorrect()
        {
            var categoryArea = CategoryArea.New(1, 2);
            Assert.AreEqual("cat-1-2", categoryArea.Code);
        }

        [TestMethod]
        public void TestAreaTypeId()
        {
            var categoryArea = CategoryArea.New(1, 2);
            Assert.AreEqual(10001, categoryArea.AreaTypeId);
        }

        [TestMethod]
        public void TestSetNames()
        {
            var categoryArea = CategoryArea.New(1, 2);
            categoryArea.SetNames(new Category { Name = "a", ShortName = "b" });
            Assert.AreEqual("a", categoryArea.Name);
            Assert.AreEqual("b", categoryArea.ShortName);
        }

        [TestMethod]
        public void TestIsCategoryAreaCode()
        {
            Assert.IsTrue(CategoryArea.IsCategoryAreaCode(AreaCodes.DeprivationDecile_Utla3));
            Assert.IsFalse(CategoryArea.IsCategoryAreaCode(AreaCodes.Ccg_AireDaleWharfdaleAndCraven));
        }
    }
}
