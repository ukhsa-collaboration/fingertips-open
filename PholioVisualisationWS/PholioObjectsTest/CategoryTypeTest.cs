using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using PholioVisualisation.PholioObjects;

namespace PholioObjectsTest
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
                new Category {CategoryId = 3},
                new Category {CategoryId = 1},
                new Category {CategoryId = 2}
            });

            type.CategoriesFromDatabase = new PersistentGenericSet<Category>(mockSessionImplementor.Object, set);

            var categories = type.Categories;

            Assert.AreEqual(1, categories[0].CategoryId);
            Assert.AreEqual(2, categories[1].CategoryId);
            Assert.AreEqual(3, categories[2].CategoryId);
        }
    }
}
