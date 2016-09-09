using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;

namespace PholioVisualisation.PholioObjectsTest
{
    [TestClass]
    public class CategoryTypeTest
    {
        [TestMethod]
        public void TestCategoriesAreOrderedByCategoryId()
        {
            var type = new CategoryType();

            var mockSessionImplementor = new Moq.Mock<ISessionImplementor>();

            var set = new HashSet<Category>(new List<Category>
            {
                new Category {Id = 3},
                new Category {Id = 1},
                new Category {Id = 2}
            });

            type.CategoriesFromDatabase = new PersistentGenericSet<Category>(mockSessionImplementor.Object, set);

            var categories = type.Categories;

            Assert.AreEqual(1, categories[0].Id);
            Assert.AreEqual(2, categories[1].Id);
            Assert.AreEqual(3, categories[2].Id);
        }

        [TestMethod]
        public void TestIsDeprivationDecile()
        {
            Assert.IsTrue(CategoryType.IsDeprivationDecile(CategoryTypeIds.DeprivationDecileDistrictAndUA2010));
            Assert.IsTrue(CategoryType.IsDeprivationDecile(CategoryTypeIds.DeprivationDecileDistrictAndUA2015));
            Assert.IsTrue(CategoryType.IsDeprivationDecile(CategoryTypeIds.DeprivationDecileCountyAndUA2010));
            Assert.IsTrue(CategoryType.IsDeprivationDecile(CategoryTypeIds.DeprivationDecileGp2010));
            Assert.IsTrue(CategoryType.IsDeprivationDecile(CategoryTypeIds.DeprivationDecileGp2015));
        }

        [TestMethod]
        public void TestIsGpDeprivationDecile()
        {
            Assert.IsTrue(CategoryType.IsGpDeprivationDecile(CategoryTypeIds.DeprivationDecileGp2010));
            Assert.IsTrue(CategoryType.IsGpDeprivationDecile(CategoryTypeIds.DeprivationDecileGp2015));
        }

        [TestMethod]
        public void TestIsDistrictAndUADeprivationDecile()
        {
            Assert.IsTrue(CategoryType.IsDistrictAndUADeprivationDecile(CategoryTypeIds.DeprivationDecileDistrictAndUA2010));
            Assert.IsTrue(CategoryType.IsDistrictAndUADeprivationDecile(CategoryTypeIds.DeprivationDecileDistrictAndUA2015));
        }

        [TestMethod]
        public void TestIsCountyAndUADeprivationDecile()
        {
            Assert.IsTrue(CategoryType.IsDeprivationDecile(CategoryTypeIds.DeprivationDecileCountyAndUA2010));
        }
    }
}
